namespace OtterLogic.Core;

/// <summary>
/// The names of the tool groups, shared by both front-ends.
/// <para>
/// These are domain vocabulary, not UI: the Grasshopper ribbon builds its tab
/// and panels from them, and the Rhino panel builds its headings from the same
/// strings. Keeping one copy is the only thing stopping the two lists drifting
/// apart as tools get added.
/// </para>
/// </summary>
public static class Sections
{
    /// <summary>The Grasshopper tab name, and the Rhino panel caption.</summary>
    public const string Root = "OtterLogic";

    /// <summary>
    /// Getting at what is already in the Rhino document — layers, objects,
    /// references. Nothing here builds geometry; it points at geometry the
    /// other sections built.
    /// </summary>
    public const string Document = "Document";

    /// <summary>Trusses, frames, and other discrete structural layouts.</summary>
    public const string StructuralForm = "Structural Form";

    /// <summary>Relaxation and equilibrium.</summary>
    public const string FormFinding = "Form Finding";

    /// <summary>Unrolling, nesting, toolpaths.</summary>
    public const string Fabrication = "Fabrication";

    /// <summary>Dataset capture, clustering, and inference.</summary>
    public const string MachineLearning = "Machine Learning";

    /// <summary>
    /// Behaviour classification of structural analysis results.
    /// <para>
    /// Its own section rather than a corner of Machine Learning, because the
    /// split is what a user is here to do. Machine Learning holds the raw
    /// methods, each exposing every setting, for somebody who wants to drive
    /// them or reproduce a result their own way. This holds finished tools that
    /// take analysis output and answer a structural question, with the settings
    /// already decided.
    /// </para>
    /// </summary>
    public const string SixDofBehaviour = "6DOF Behaviour";
}
