using Rhino.Geometry;

namespace OtterLogic.Core.FormFinding;

/// <summary>
/// A steppable, inspectable relaxation solver.
/// <para>
/// Deliberately *not* a "solve()" that returns an answer. Grasshopper drives it
/// to convergence inside SolveInstance; the Rhino command drives it one step at
/// a time from the idle loop so it can draw each iteration and honour Esc.
/// Both front-ends share this one interface.
/// </para>
/// </summary>
public interface IRelaxationSolver
{
    /// <summary>Current particle positions. Live view — changes as you step.</summary>
    IReadOnlyList<Point3d> Positions { get; }

    /// <summary>Largest distance any single particle moved on the last iteration.</summary>
    double Residual { get; }

    /// <summary>Iterations run since construction.</summary>
    int Iterations { get; }

    /// <summary>True once <see cref="Residual"/> drops below the solver's tolerance.</summary>
    bool HasConverged { get; }

    /// <summary>Advance the simulation. Returns early once converged.</summary>
    void Step(int iterations = 1);
}
