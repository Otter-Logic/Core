using Rhino.Geometry;

namespace OtterLogic.Core.FormFinding.Goals;

/// <summary>
/// Holds a particle at a fixed point. The default strength is deliberately huge
/// so anchors win against everything else in the weighted average — lower it to
/// get a soft, spring-like support.
/// </summary>
public sealed class AnchorGoal : IGoal
{
    public AnchorGoal(int particle, Point3d target, double strength = 1e4)
    {
        Indices = new[] { particle };
        Target = target;
        Strength = strength;
    }

    public int[] Indices { get; }
    public Point3d Target { get; }
    public double Strength { get; }

    public void Calculate(IReadOnlyList<Point3d> positions, Point3d[] targets, double[] weights)
    {
        targets[0] = Target;
        weights[0] = Strength;
    }
}
