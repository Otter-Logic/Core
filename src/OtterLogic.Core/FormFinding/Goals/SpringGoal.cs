using Rhino.Geometry;

namespace OtterLogic.Core.FormFinding.Goals;

/// <summary>
/// Pulls two particles toward a rest length. A rest length shorter than the
/// current one contracts the network — that is what drives minimal-surface and
/// hanging-chain form finding.
/// </summary>
public sealed class SpringGoal : IGoal
{
    public SpringGoal(int startParticle, int endParticle, double restLength, double strength = 1.0)
    {
        if (restLength < 0.0) throw new ArgumentOutOfRangeException(nameof(restLength));
        Indices = new[] { startParticle, endParticle };
        RestLength = restLength;
        Strength = strength;
    }

    public int[] Indices { get; }
    public double RestLength { get; }
    public double Strength { get; }

    public void Calculate(IReadOnlyList<Point3d> positions, Point3d[] targets, double[] weights)
    {
        Point3d a = positions[Indices[0]];
        Point3d b = positions[Indices[1]];

        Vector3d span = b - a;
        double length = span.Length;

        // Coincident particles have no direction to correct along; skip this iteration.
        if (length < 1e-12)
        {
            targets[0] = a;
            targets[1] = b;
            weights[0] = weights[1] = 0.0;
            return;
        }

        // Each end travels half the error, so the goal is momentum-neutral.
        Vector3d correction = span / length * (0.5 * (length - RestLength));

        targets[0] = a + correction;
        targets[1] = b - correction;
        weights[0] = weights[1] = Strength;
    }
}
