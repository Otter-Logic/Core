using Rhino.Geometry;

namespace OtterLogic.Core;

/// <summary>
/// Every piece of geometry described by the same row of numbers, so that a mixed
/// bag of curves, surfaces, solids and meshes can be wired into anything that
/// learns from a table: how big it is each way, how long, how much area and
/// volume, how it stands, how straight or flat, whether it closes, and how many
/// corners, faces and holes it has.
/// <para>
/// In Core because it is geometry with no model in it — nothing here knows a
/// beam from a duct from a panel — and because both front-ends want the same row:
/// a Grasshopper component feeds it to OtterCluster, and a Rhino command can write
/// it onto the objects as user text. The reading of a <em>structural</em> model,
/// which knows what a joint or a support is, lives in StructuralEngine and is a
/// different thing.
/// </para>
/// <para>
/// The columns are the same whatever the geometry, with zero where a measure does
/// not apply — a curve has no area, an open surface no volume — rather than a
/// missing cell, because a table with ragged rows is a table nothing downstream can
/// read. What differs between kinds is said by the first column, so a clustering
/// can keep curves and surfaces apart or not, as its user chooses. Position is
/// optional and off by default: two identical brackets on opposite sides of a
/// building are the same bracket, and a column that can tell them apart can only
/// harm a grouping by kind. Turn it on for a grouping by place.
/// </para>
/// <para>
/// This is the one place in Core that reads RhinoCommon classes — Curve, Brep,
/// Mesh — rather than structs, because measuring a length or an area <em>is</em>
/// calling them. It therefore runs only with Rhino booted, which is every place it
/// is called from.
/// </para>
/// </summary>
public static class GeometryDescription
{
    /// <summary>How finely a curve is read when its flatness is measured.</summary>
    private const int CurveSamples = 32;

    /// <summary>The columns every row has, in order.</summary>
    public static readonly string[] Names =
    {
        "Dimension", "Size X", "Size Y", "Size Z", "Length", "Area", "Volume",
        "Upright", "Straightness", "Closed", "Corners", "Faces", "Holes",
    };

    /// <summary>The columns added when position is asked for, after the others.</summary>
    public static readonly string[] PositionNames = { "Centre X", "Centre Y", "Centre Z" };

    /// <summary>The columns of a row, with or without position.</summary>
    public static string[] ColumnNames(bool position)
        => position ? Names.Concat(PositionNames).ToArray() : (string[])Names.Clone();

    /// <summary>
    /// What each column holds, in the words a user reads: what it is for a curve,
    /// for a surface or solid, and for a mesh.
    /// </summary>
    public static readonly IReadOnlyList<string> ColumnNotes = new[]
    {
        "Dimension: 0 for a point, 1 for a curve, 2 for a surface or open mesh, 3 for a solid or closed mesh.",
        "Size X, Y, Z: the extents of the bounding box in the frame given.",
        "Length: a curve's length; the total length of a surface's or solid's edges; a mesh's naked edges.",
        "Area: a closed planar curve's area; a surface's, solid's or mesh's area. Zero for an open curve.",
        "Volume: a solid's or closed mesh's volume. Zero for anything open.",
        "Upright: how much the thing stands up, 0 lying level to 1 standing plumb — a curve by its ends, a surface "
        + "or mesh by its normal, a solid by its height against its plan.",
        "Straightness: for an open curve, end to end over length, 1 for a straight line; for anything else, how "
        + "nearly flat it is, 1 for planar.",
        "Closed: 1 for a closed curve, a solid or a closed mesh.",
        "Corners: a curve's kinks and ends; a surface's, solid's or mesh's vertices.",
        "Faces: a surface's or solid's faces; a mesh's faces. Zero for a curve.",
        "Holes: inner loops of a surface's faces; a mesh's naked-edge loops beyond its outline.",
        "Centre X, Y, Z: the centroid in the frame given, when position is asked for.",
    };

    /// <summary>
    /// What a piece of geometry is, in one word: point, curve, surface, solid or mesh.
    /// Null for anything this cannot describe.
    /// </summary>
    public static string? Kind(GeometryBase geometry) => geometry switch
    {
        Point => "point",
        Curve => "curve",
        Brep brep => brep.IsSolid ? "solid" : "surface",
        // An extrusion is a Surface to the type system and a solid when capped, so it is tested first.
        Extrusion extrusion => extrusion.IsSolid ? "solid" : "surface",
        Surface => "surface",
        SubD subd => subd.IsSolid ? "solid" : "surface",
        Mesh => "mesh",
        _ => null,
    };

    /// <summary>
    /// The row for one piece of geometry, or null when it is a kind this cannot describe.
    /// </summary>
    /// <param name="geometry">A point, curve, surface, Brep, extrusion, SubD or mesh.</param>
    /// <param name="frame">The plane sizes and positions are read in, and whose z is up. World XY for the usual reading.</param>
    /// <param name="position">Whether to add the centroid as three more columns.</param>
    public static double[]? Describe(GeometryBase geometry, Plane frame, bool position)
    {
        ArgumentNullException.ThrowIfNull(geometry);

        var row = geometry switch
        {
            Point point => DescribePoint(point, frame),
            Curve curve => DescribeCurve(curve, frame),
            Brep brep => DescribeBrep(brep, frame),
            Extrusion extrusion => DescribeBrep(extrusion.ToBrep(), frame),
            Surface surface => DescribeBrep(surface.ToBrep(), frame),
            SubD subd => DescribeBrep(subd.ToBrep(), frame),
            Mesh mesh => DescribeMesh(mesh, frame),
            _ => null,
        };

        if (row is null)
            return null;

        if (!position)
            return row.Values;

        frame.RemapToPlaneSpace(row.Centre, out var local);
        return row.Values.Concat(new[] { local.X, local.Y, local.Z }).ToArray();
    }

    private sealed record Row(double[] Values, Point3d Centre);

    private static Row DescribePoint(Point point, Plane frame)
    {
        var values = new double[Names.Length];
        return new Row(values, point.Location);
    }

    private static Row DescribeCurve(Curve curve, Plane frame)
    {
        var values = new double[Names.Length];
        var box = curve.GetBoundingBox(frame);
        double length = curve.GetLength();
        double size = Math.Max(box.Diagonal.Length, 1e-12);

        values[0] = 1.0;
        (values[1], values[2], values[3]) = Sizes(box);
        values[4] = length;

        // A closed planar curve encloses an area, which is usually what a closed
        // curve is drawn to say; an open one, or a closed one not in a plane, has none.
        if (curve.IsClosed && curve.IsPlanar())
            values[5] = AreaMassProperties.Compute(curve)?.Area ?? 0.0;

        var chord = curve.PointAtEnd - curve.PointAtStart;
        if (curve.IsClosed)
        {
            // Closed, the curve is an outline: it stands as the plane it lies in does,
            // and its straightness is how flat it is.
            var points = Sample(curve);
            values[7] = UprightOfPoints(points, frame);
            values[8] = Flatness(points, size);
        }
        else
        {
            values[7] = chord.Length > 0.0 ? Math.Abs(Vector3d.Multiply(chord, frame.ZAxis)) / chord.Length : 0.0;
            values[8] = length > 0.0 ? Math.Min(1.0, chord.Length / length) : 1.0;
        }

        values[9] = curve.IsClosed ? 1.0 : 0.0;
        values[10] = Corners(curve);

        var centroid = LengthMassProperties.Compute(curve)?.Centroid ?? box.Center;
        return new Row(values, centroid);
    }

    private static Row DescribeBrep(Brep brep, Plane frame)
    {
        var values = new double[Names.Length];
        var box = brep.GetBoundingBox(frame);
        double size = Math.Max(box.Diagonal.Length, 1e-12);

        values[0] = brep.IsSolid ? 3.0 : 2.0;
        (values[1], values[2], values[3]) = Sizes(box);
        values[4] = brep.Edges.Sum(edge => edge.GetLength());

        var area = AreaMassProperties.Compute(brep);
        values[5] = area?.Area ?? 0.0;

        Point3d centroid = area?.Centroid ?? box.Center;
        if (brep.IsSolid)
        {
            var volume = VolumeMassProperties.Compute(brep);
            values[6] = volume?.Volume ?? 0.0;
            centroid = volume?.Centroid ?? centroid;

            // A solid stands up as its height stands against its plan.
            values[7] = Math.Min(1.0, values[3] / Math.Max(Math.Max(values[1], values[2]), 1e-12));
        }
        else
        {
            // Area-weighted, so a big flat face outweighs a small return.
            double weighted = 0.0, total = 0.0;
            foreach (var face in brep.Faces)
            {
                double faceArea = AreaMassProperties.Compute(face)?.Area ?? 0.0;
                var domainU = face.Domain(0);
                var domainV = face.Domain(1);
                var normal = face.NormalAt(domainU.Mid, domainV.Mid);
                weighted += faceArea * (1.0 - Math.Abs(Vector3d.Multiply(normal, frame.ZAxis)));
                total += faceArea;
            }

            values[7] = total > 0.0 ? weighted / total : 0.0;
        }

        values[8] = brep.Faces.All(face => face.IsPlanar())
            ? 1.0
            : Flatness(brep.Vertices.Select(v => v.Location).Concat(SampleEdges(brep)), size);
        values[9] = brep.IsSolid ? 1.0 : 0.0;
        values[10] = brep.Vertices.Count;
        values[11] = brep.Faces.Count;
        values[12] = brep.Faces.Sum(face => Math.Max(0, face.Loops.Count - 1));

        return new Row(values, centroid);
    }

    private static Row DescribeMesh(Mesh mesh, Plane frame)
    {
        var values = new double[Names.Length];
        var box = mesh.GetBoundingBox(frame);
        double size = Math.Max(box.Diagonal.Length, 1e-12);
        bool closed = mesh.IsClosed;

        values[0] = closed ? 3.0 : 2.0;
        (values[1], values[2], values[3]) = Sizes(box);

        var naked = mesh.GetNakedEdges() ?? Array.Empty<Polyline>();
        values[4] = naked.Sum(loop => loop.Length);

        var area = AreaMassProperties.Compute(mesh);
        values[5] = area?.Area ?? 0.0;

        Point3d centroid = area?.Centroid ?? box.Center;
        if (closed)
        {
            var volume = VolumeMassProperties.Compute(mesh);
            values[6] = volume?.Volume ?? 0.0;
            centroid = volume?.Centroid ?? centroid;
            values[7] = Math.Min(1.0, values[3] / Math.Max(Math.Max(values[1], values[2]), 1e-12));
        }
        else
        {
            double weighted = 0.0, total = 0.0;
            for (int f = 0; f < mesh.Faces.Count; f++)
            {
                var face = mesh.Faces[f];
                Point3d a = mesh.Vertices[face.A], b = mesh.Vertices[face.B], c = mesh.Vertices[face.C];
                var normal = Vector3d.CrossProduct(b - a, c - a);
                if (face.IsQuad)
                    normal += Vector3d.CrossProduct(c - a, (Point3d)mesh.Vertices[face.D] - a);

                double faceArea = 0.5 * normal.Length;
                if (faceArea <= 0.0)
                    continue;

                weighted += faceArea * (1.0 - Math.Abs(Vector3d.Multiply(normal, frame.ZAxis)) / normal.Length);
                total += faceArea;
            }

            values[7] = total > 0.0 ? weighted / total : 0.0;
        }

        values[8] = Flatness(mesh.Vertices.ToPoint3dArray(), size);
        values[9] = closed ? 1.0 : 0.0;
        values[10] = mesh.Vertices.Count;
        values[11] = mesh.Faces.Count;
        values[12] = closed ? 0.0 : Math.Max(0, naked.Length - 1);

        return new Row(values, centroid);
    }

    private static (double X, double Y, double Z) Sizes(BoundingBox box)
        => box.IsValid ? (box.Max.X - box.Min.X, box.Max.Y - box.Min.Y, box.Max.Z - box.Min.Z) : (0.0, 0.0, 0.0);

    /// <summary>
    /// How nearly a set of points lies in one plane: one when it does, falling towards
    /// zero as the worst point strays by the size of the whole thing.
    /// </summary>
    private static double Flatness(IEnumerable<Point3d> points, double size)
    {
        var list = points.ToList();
        if (list.Count < 4)
            return 1.0;

        if (Plane.FitPlaneToPoints(list, out _, out double deviation) != PlaneFitResult.Success)
            return 1.0;

        return Math.Max(0.0, 1.0 - deviation / size);
    }

    /// <summary>How an outline stands, by the plane that fits it best: 0 lying level, 1 standing plumb.</summary>
    private static double UprightOfPoints(IReadOnlyList<Point3d> points, Plane frame)
    {
        if (points.Count < 3 || Plane.FitPlaneToPoints(points, out var plane) != PlaneFitResult.Success)
            return 0.0;

        return 1.0 - Math.Abs(Vector3d.Multiply(plane.Normal, frame.ZAxis));
    }

    /// <summary>A curve's kinks and ends: a polyline's own corners, otherwise one per smooth segment plus the free end.</summary>
    private static int Corners(Curve curve)
    {
        if (curve.TryGetPolyline(out var polyline))
            return curve.IsClosed ? Math.Max(0, polyline.Count - 1) : polyline.Count;

        var segments = curve.DuplicateSegments();
        int pieces = segments is { Length: > 0 } ? segments.Length : 1;
        return curve.IsClosed ? pieces : pieces + 1;
    }

    private static List<Point3d> Sample(Curve curve)
    {
        var parameters = curve.DivideByCount(CurveSamples, true) ?? Array.Empty<double>();
        return parameters.Select(curve.PointAt).ToList();
    }

    private static IEnumerable<Point3d> SampleEdges(Brep brep)
    {
        foreach (var edge in brep.Edges)
        {
            if (edge.IsLinear())
                continue;

            var parameters = edge.DivideByCount(4, false) ?? Array.Empty<double>();
            foreach (double t in parameters)
                yield return edge.PointAt(t);
        }
    }
}
