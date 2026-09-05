using Rhino.Geometry;

namespace OtterLogic.Core.FormFinding.Solvers;

/// <summary>
/// Projective dynamic relaxation with velocity damping.
/// <para>
/// Each iteration: every goal proposes target positions for its particles, the
/// solver takes the weighted mean per particle, and integrates the resulting
/// move into a damped velocity. Damping below 1.0 bleeds off kinetic energy so
/// the system settles instead of oscillating.
/// </para>
/// </summary>
public sealed class DynamicRelaxationSolver : IRelaxationSolver
{
    private readonly IGoal[] _goals;
    private readonly Point3d[] _positions;
    private readonly Vector3d[] _velocities;

    // Per-particle accumulators, reused every iteration to keep the hot loop allocation-free.
    private readonly Vector3d[] _weightedTargets;
    private readonly double[] _weightSums;

    // Scratch buffers handed to goals, sized to the largest goal in the system.
    private readonly Point3d[] _goalTargets;
    private readonly double[] _goalWeights;

    public DynamicRelaxationSolver(IEnumerable<Point3d> startPositions, IEnumerable<IGoal> goals)
    {
        _positions = startPositions?.ToArray() ?? throw new ArgumentNullException(nameof(startPositions));
        _goals = goals?.ToArray() ?? throw new ArgumentNullException(nameof(goals));

        if (_positions.Length == 0)
            throw new ArgumentException("Solver needs at least one particle.", nameof(startPositions));

        foreach (var goal in _goals)
            foreach (var index in goal.Indices)
                if (index < 0 || index >= _positions.Length)
                    throw new ArgumentOutOfRangeException(nameof(goals),
                        $"Goal {goal.GetType().Name} references particle {index}, but only {_positions.Length} exist.");

        _velocities = new Vector3d[_positions.Length];
        _weightedTargets = new Vector3d[_positions.Length];
        _weightSums = new double[_positions.Length];

        int widest = _goals.Length == 0 ? 0 : _goals.Max(g => g.Indices.Length);
        _goalTargets = new Point3d[widest];
        _goalWeights = new double[widest];
    }

    /// <summary>Velocity retained between iterations. 1.0 is undamped, ~0.9 settles quickly.</summary>
    public double Damping { get; set; } = 0.9;

    /// <summary>Per-iteration max movement below which the system is considered settled.</summary>
    public double Tolerance { get; set; } = 1e-6;

    public IReadOnlyList<Point3d> Positions => _positions;
    public double Residual { get; private set; } = double.MaxValue;
    public int Iterations { get; private set; }
    public bool HasConverged => Residual < Tolerance;

    public void Step(int iterations = 1)
    {
        for (int i = 0; i < iterations && !HasConverged; i++)
            StepOnce();
    }

    private void StepOnce()
    {
        Array.Clear(_weightedTargets, 0, _weightedTargets.Length);
        Array.Clear(_weightSums, 0, _weightSums.Length);

        foreach (var goal in _goals)
        {
            goal.Calculate(_positions, _goalTargets, _goalWeights);

            var indices = goal.Indices;
            for (int k = 0; k < indices.Length; k++)
            {
                double weight = _goalWeights[k];
                if (weight <= 0.0) continue;

                int particle = indices[k];
                _weightedTargets[particle] += new Vector3d(_goalTargets[k]) * weight;
                _weightSums[particle] += weight;
            }
        }

        double largestMove = 0.0;

        for (int i = 0; i < _positions.Length; i++)
        {
            // A particle no goal cares about this iteration simply stops.
            if (_weightSums[i] <= 0.0)
            {
                _velocities[i] = Vector3d.Zero;
                continue;
            }

            var target = new Point3d(_weightedTargets[i] / _weightSums[i]);
            Vector3d move = target - _positions[i];

            _velocities[i] = (_velocities[i] + move) * Damping;
            _positions[i] += _velocities[i];

            double travelled = _velocities[i].Length;
            if (travelled > largestMove) largestMove = travelled;
        }

        Residual = largestMove;
        Iterations++;
    }
}
