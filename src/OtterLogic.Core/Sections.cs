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
    /// Building the structure once it is designed: the order its pieces go up in,
    /// and seeing that order happen.
    /// <para>
    /// Separate from <see cref="Fabrication"/> because that section makes the pieces
    /// and this one puts them together on site — a different person, with the model
    /// finished and the pieces on the lorry. What earns a place here is a tool that
    /// reasons about construction in general — what must stand before what can be
    /// lifted onto it — without knowing what kind of structure it is or what any
    /// piece is called. The reading of the structure it works from lives with the
    /// structural tools, so the sequence and the analysis can never disagree about
    /// what rests on what.
    /// </para>
    /// </summary>
    public const string Construction = "Construction";

    /// <summary>
    /// Questions asked of a network — which way is cheapest, how much passes through
    /// here, what falls off if this goes, what has to come first. Nothing in it is
    /// trained and nothing in it clusters.
    /// <para>
    /// Named for a technique, like the learning sections below, and for the same
    /// reason: it holds raw methods with every setting exposed and no opinion about
    /// what the nodes are. It is separate from them because its user is not doing
    /// machine learning. Routes, cut vertices and betweenness began under
    /// <see cref="UnsupervisedLearning"/>, where somebody ordering a toolpath or
    /// tracing a circulation route had no reason to look; they moved here when the
    /// Graphs repo moved out from under MachineLearning, and the panel mirrors that
    /// repo the way the learning panels mirror theirs.
    /// </para>
    /// <para>
    /// What stays under <see cref="UnsupervisedLearning"/> is what builds a graph
    /// from samples — Neighbour Graph, Gaussian Affinity — since measuring how alike
    /// two samples are is a learning question. Order within this section comes from
    /// <c>GH_Exposure</c>: building a graph and taking one apart, then its
    /// structure, then routes and flow, then importance.
    /// </para>
    /// </summary>
    public const string Graphs = "Graphs";

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
    /// clustering, building the graphs the graph methods run on, and refinement of a
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

    /// <summary>
    /// The order the sections are read in, left to right: the techniques first,
    /// then the jobs in the order a project meets them — form, design, making,
    /// building — and the document last. Grasshopper's own tabs run the same way,
    /// from Params through Maths and Sets to Display, and a user who has learned
    /// that habit should find it here.
    /// <para>
    /// Domain vocabulary rather than UI, like the names: the Rhino panel lists
    /// its headings in this order too. Form Finding sits after Structural Form
    /// because it shapes what that section generates; it shows nowhere until it
    /// has a tool.
    /// </para>
    /// </summary>
    public static readonly string[] Order =
    {
        Graphs,
        MachineLearning,
        UnsupervisedLearning,
        SupervisedLearning,
        StructuralForm,
        FormFinding,
        StructuralDesign,
        Fabrication,
        Construction,
        Document,
    };
}
