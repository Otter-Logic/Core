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
    /// <see cref="UnsupervisedLearning"/> or its sibling paradigm sections, and a
    /// user here should never need to go looking for it.
    /// </para>
    /// </summary>
    public const string StructuralDesign = "Structural Design";

    /// <summary>Relaxation and equilibrium.</summary>
    public const string FormFinding = "Form Finding";

    /// <summary>Unrolling, nesting, toolpaths.</summary>
    public const string Fabrication = "Fabrication";

    /// <summary>
    /// The steps every learning paradigm shares: dataset capture, feature
    /// preparation, decomposition, inference. Reserved — nothing ships here yet.
    /// <para>
    /// The methods themselves live in a section per paradigm, starting with
    /// <see cref="UnsupervisedLearning"/>. This one mirrors the MachineLearning
    /// repo the way those mirror their paradigm repos: what more than one
    /// paradigm uses sits here rather than being filed under any one of them.
    /// </para>
    /// </summary>
    public const string MachineLearning = "Machine Learning";

    /// <summary>
    /// The raw unsupervised methods, for somebody assembling their own pipeline:
    /// clustering, the graphs the graph methods run on, and refinement of a
    /// labelling. Every setting exposed, no opinion about the data.
    /// <para>
    /// Named for the technique because that is what it holds. The sections above
    /// are named for jobs, and a user who has a job to do should find it there
    /// without ever opening this one. This is the section for the other kind of
    /// user — the one who wants to drive a method directly, or reproduce what a
    /// finished tool did with their own choices.
    /// </para>
    /// <para>
    /// A section per paradigm rather than one for all of machine learning, because
    /// that is how somebody who wants the raw methods already thinks about them:
    /// "a clustering" is an unsupervised question before it is anything else, and
    /// the panel should be where that person looks first. It also mirrors the
    /// Unsupervised repo exactly, so a component's panel says where its algorithm
    /// lives. Order within it comes from <c>GH_Exposure</c> — graphs, then
    /// methods, then refinement, then enum dropdowns — with a divider between each.
    /// </para>
    /// </summary>
    public const string UnsupervisedLearning = "Unsupervised Learning";
}
