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
    /// Multipurpose tools for structural engineering, before an analysis and after
    /// one: reading the structure a model's geometry describes, and grouping
    /// elements by the six degrees of freedom of data on them.
    /// <para>
    /// Separate from <see cref="StructuralForm"/> because that section generates a
    /// structure and this one answers questions about one that already exists.
    /// What earns a place here is a tool that knows what structural data is — a
    /// stick model, supports, forces beside moments — without hard-coding what any
    /// one structure or job makes of it. Frames, shells, bridges and gridshells go
    /// through the same tools, and the user prepares the data for their own purpose
    /// in their own definition. Whatever method a tool uses lives under
    /// <see cref="UnsupervisedLearning"/> or its sibling paradigm sections, and a
    /// user here should never need to go looking for it.
    /// </para>
    /// </summary>
    public const string StructuralDesign = "Structural Design";

    /// <summary>Relaxation and equilibrium.</summary>
    public const string FormFinding = "Form Finding";

    /// <summary>
    /// Making the structure: what its connections are and how many kinds there are,
    /// then unrolling, nesting and toolpaths.
    /// <para>
    /// A panel is cut by who reaches for a tool, not by where its code lives. Joint
    /// Signature and Connection Typology are here because connection detailing is the
    /// fabricator's question, though their code reads joints with the same machinery
    /// as the structural tools and lives beside it for now.
    /// </para>
    /// </summary>
    public const string Fabrication = "Fabrication";

    /// <summary>
    /// The steps every learning paradigm shares: dataset capture, feature
    /// preparation, decomposition, inference. Shape Signature is the first of
    /// them — turning outlines into features is a thing to do before a method,
    /// not a method.
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

    /// <summary>
    /// The raw supervised methods: fit a model to samples whose answer is known,
    /// predict the answer for samples where it is not, and score how well that went.
    /// <para>
    /// A section of its own for the same reason <see cref="UnsupervisedLearning"/>
    /// has one, and it mirrors the Supervised repo the same way. What is <em>not</em>
    /// here is the dataset — writing a table, reading it back, splitting it by group
    /// — which sits under <see cref="MachineLearning"/> because every paradigm
    /// gathers data before it does anything else. Order within it comes from
    /// <c>GH_Exposure</c>: methods, then evaluation, then enum dropdowns.
    /// </para>
    /// </summary>
    public const string SupervisedLearning = "Supervised Learning";
}
