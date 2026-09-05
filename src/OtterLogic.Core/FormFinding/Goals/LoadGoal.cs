using Rhino.Geometry;

namespace OtterLogic.Core.FormFinding.Goals;

/// <summary>
/// A constant per-particle force, such as gravity or wind.
/// <para>
/// Expressed as a displacement target at unit weight rather than as an
/// acceleration, so it composes with the other goals in the same weighted
/// average. Scale <see cref="Force"/> relative to spring strength to change how
/// far the form sags.
/// </para>
/// </summary>
public sealed class LoadGoal : IGoal
{
    public LoadGoal(int particle, Vector3d force, double strength = 1.0)
    {
        Indices = new[] { particle };
        Force = force;
        Strength = strength;
    }

    public int[] Indices { get; }
    public Vector3d Force { get; }
    public double Strength { get; }

    public void Calculate(IReadOnlyList<Point3d> positions, Point3d[] targets, double[] weights)
    {
        targets[0] = positions[Indices[0]] + Force;
        weights[0] = Strength;
    }
}
