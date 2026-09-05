using Rhino.Geometry;

namespace OtterLogic.Core.FormFinding;

/// <summary>
/// A single constraint in a relaxation system.
/// <para>
/// A goal does not move anything itself. Asked where its particles *would like*
/// to be, it writes one target position and one weight per particle it owns.
/// The solver averages every goal's opinion, weighted, and moves from there.
/// This is the projective / position-based formulation (as used by ShapeOp and
/// Kangaroo) rather than a force-accumulating one: it stays stable at large
/// step sizes and mixes stiff and soft constraints without tuning a timestep.
/// </para>
/// </summary>
public interface IGoal
{
    /// <summary>Indices into the solver's particle array that this goal acts on.</summary>
    int[] Indices { get; }

    /// <summary>
    /// Write desired positions and weights for this goal's particles.
    /// <paramref name="targets"/> and <paramref name="weights"/> are scratch buffers
    /// owned by the solver and are at least <see cref="Indices"/>.Length long;
    /// write to slots 0..Indices.Length-1 only, and do not retain them.
    /// </summary>
    void Calculate(IReadOnlyList<Point3d> positions, Point3d[] targets, double[] weights);
}
