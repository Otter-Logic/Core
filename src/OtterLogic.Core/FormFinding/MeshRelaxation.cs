using OtterLogic.Core.FormFinding.Goals;
using OtterLogic.Core.FormFinding.Solvers;
using Rhino;
using Rhino.Geometry;

namespace OtterLogic.Core.FormFinding;

/// <summary>
/// Turns a mesh into a relaxation system and back again.
/// <para>
/// This is the seam both front-ends sit on: the Rhino command and the
/// Grasshopper component each call <see cref="CreateSolver"/> then
/// <see cref="ApplyPositions"/>, and share every line of logic in between.
/// </para>
/// </summary>
public static class MeshRelaxation
{
    /// <summary>
    /// Build a solver from a mesh: one particle per topology vertex, one spring
    /// per topology edge, gravity on every free particle.
    /// </summary>
    /// <param name="mesh">Source mesh. Not modified.</param>
    /// <param name="anchorPoints">
    /// Points to pin. Each snaps to its nearest topology vertex within
    /// <paramref name="anchorTolerance"/>. Pass null or empty to pin the naked
    /// boundary instead — the usual choice for a minimal surface.
    /// </param>
    /// <param name="restLengthFactor">
    /// Rest length as a fraction of each edge's starting length. Below 1.0 the
    /// network contracts (minimal surface); above 1.0 it slackens and sags.
    /// </param>
    /// <param name="springStrength">Weight of every spring goal.</param>
    /// <param name="gravity">Per-particle load. Vector3d.Zero for pure form finding.</param>
    /// <param name="anchorTolerance">Snapping radius for <paramref name="anchorPoints"/>.</param>
    public static DynamicRelaxationSolver CreateSolver(
        Mesh mesh,
        IEnumerable<Point3d>? anchorPoints = null,
        double restLengthFactor = 1.0,
        double springStrength = 1.0,
        Vector3d gravity = default,
        double anchorTolerance = 0.01)
    {
        if (mesh is null) throw new ArgumentNullException(nameof(mesh));
        if (!mesh.IsValid) throw new ArgumentException("Mesh is not valid.", nameof(mesh));

        var topology = mesh.TopologyVertices;
        topology.SortEdges();

        int count = topology.Count;
        var positions = new Point3d[count];
        for (int i = 0; i < count; i++)
            positions[i] = topology[i];   // Point3f widens to Point3d

        var goals = new List<IGoal>();

        var edges = mesh.TopologyEdges;
        for (int i = 0; i < edges.Count; i++)
        {
            IndexPair pair = edges.GetTopologyVertices(i);
            double length = positions[pair.I].DistanceTo(positions[pair.J]);
            goals.Add(new SpringGoal(pair.I, pair.J, length * restLengthFactor, springStrength));
        }

        var anchored = ResolveAnchors(mesh, positions, anchorPoints, anchorTolerance);
        foreach (int index in anchored)
            goals.Add(new AnchorGoal(index, positions[index]));

        if (!gravity.IsZero)
            for (int i = 0; i < count; i++)
                if (!anchored.Contains(i))
                    goals.Add(new LoadGoal(i, gravity));

        return new DynamicRelaxationSolver(positions, goals);
    }

    /// <summary>
    /// Copy solved particle positions back onto a duplicate of the source mesh.
    /// Positions are indexed by topology vertex, so welded duplicates all follow.
    /// </summary>
    public static Mesh ApplyPositions(Mesh source, IReadOnlyList<Point3d> positions)
    {
        if (source is null) throw new ArgumentNullException(nameof(source));
        if (positions is null) throw new ArgumentNullException(nameof(positions));

        Mesh result = source.DuplicateMesh();
        var topology = result.TopologyVertices;

        if (positions.Count != topology.Count)
            throw new ArgumentException(
                $"Expected {topology.Count} positions to match the mesh topology, got {positions.Count}.",
                nameof(positions));

        for (int i = 0; i < topology.Count; i++)
            foreach (int vertex in topology.MeshVertexIndices(i))
                result.Vertices.SetVertex(vertex, positions[i]);

        result.Normals.ComputeNormals();
        result.FaceNormals.ComputeFaceNormals();
        return result;
    }

    /// <summary>
    /// Topology-vertex indices to pin: the supplied points snapped to nearest
    /// vertices, or the naked boundary when no points are given.
    /// </summary>
    private static HashSet<int> ResolveAnchors(
        Mesh mesh,
        IReadOnlyList<Point3d> positions,
        IEnumerable<Point3d>? anchorPoints,
        double tolerance)
    {
        var anchored = new HashSet<int>();
        var requested = anchorPoints?.ToArray() ?? Array.Empty<Point3d>();

        if (requested.Length == 0)
        {
            bool[] naked = mesh.GetNakedEdgePointStatus();
            var topology = mesh.TopologyVertices;
            for (int vertex = 0; vertex < naked.Length; vertex++)
                if (naked[vertex])
                    anchored.Add(topology.TopologyVertexIndex(vertex));

            return anchored;
        }

        double toleranceSquared = tolerance * tolerance;

        foreach (Point3d point in requested)
        {
            int nearest = -1;
            double best = double.MaxValue;

            for (int i = 0; i < positions.Count; i++)
            {
                double distanceSquared = positions[i].DistanceToSquared(point);
                if (distanceSquared < best)
                {
                    best = distanceSquared;
                    nearest = i;
                }
            }

            if (nearest >= 0 && best <= toleranceSquared)
                anchored.Add(nearest);
        }

        return anchored;
    }
}
