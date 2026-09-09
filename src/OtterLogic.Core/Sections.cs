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

    /// <summary>
    /// Tools that act on analysis results rather than producing geometry:
    /// grouping members by behaviour, sizing, predicting demand.
    /// <para>
    /// Separate from <see cref="StructuralForm"/> because that section generates a
    /// structure and this one answers questions about one that already exists.
    /// What earns a place here is a tool carrying a structural opinion — it knows
    /// what a bending moment is. Whatever method it uses lives under
    /// <see cref="MachineLearning"/>, and a user here should never need to go
    /// looking for it.
    /// </para>
    /// </summary>
    public const string StructuralDesign = "Structural Design";

    /// <summary>Relaxation and equilibrium.</summary>
    public const string FormFinding = "Form Finding";

    /// <summary>Unrolling, nesting, toolpaths.</summary>
    public const string Fabrication = "Fabrication";

    /// <summary>
    /// The raw methods, for somebody assembling their own pipeline: dataset
    /// capture, feature preparation, the learning algorithms themselves, and the
    /// scores that judge them.
    /// <para>
    /// Named for the technique because that is what it holds. The sections above
    /// are named for jobs, and a user who has a job to do should find it there
    /// without ever opening this one. This is the section for the other kind of
    /// user — the one who wants to drive a method directly, or reproduce what a
    /// finished tool did with their own choices.
    /// </para>
    /// <para>
    /// It is one section, not one per paradigm, and stays that way while it fits.
    /// Order within it comes from <c>GH_Exposure</c>, which groups components by
    /// pipeline stage — data, features, learning, evaluation — and draws a divider
    /// between them. Split it only when it genuinely overflows.
    /// </para>
    /// </summary>
    public const string MachineLearning = "Machine Learning";
}
