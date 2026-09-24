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
    /// <see cref="MachineLearning"/>, and a user here should never need to go
    /// looking for it.
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
    /// Named for a technique, like <see cref="MachineLearning"/>, and cut the same
    /// way: one core, OtterPath, takes a graph, a method on a wire, and the sources
    /// and targets the question is about; each algorithm is a small component that
    /// outputs nothing but its wire, and with nothing wired the core reads the
    /// question off what it was given. It is separate from that section because its
    /// user is not doing machine learning. Routes, cut vertices and betweenness began
    /// in the learning panels, where somebody ordering a toolpath or tracing a
    /// circulation route had no reason to look; they moved here when the Graphs repo
    /// moved out from under MachineLearning. Order within it comes from
    /// <c>GH_Exposure</c>: the core, then the methods, then building a graph and
    /// taking one apart.
    /// </para>
    /// </summary>
    public const string Graphs = "Graphs";

    /// <summary>
    /// Getting data ready before anything learns from it: a table typed or pasted
    /// straight onto the canvas, datasets gathered on disk one model at a time and
    /// read back, and features taken from geometry.
    /// <para>
    /// Separate from <see cref="MachineLearning"/> because preparing data is a
    /// different job from fitting a model to it, done by a different person on a
    /// different day: whoever collects results from a run of analyses is not
    /// choosing a clustering method. It began as the data tier at the bottom of the
    /// Machine Learning panel and moved out in 2026-09 when the Dataset repo arrived
    /// to hold it. What earns a place here is a tool that knows what a table is —
    /// rows, named columns, numbers beside names — without knowing what the numbers
    /// mean or what will be trained on them. Order within it comes from
    /// <c>GH_Exposure</c>: the table itself, then datasets on disk, then features
    /// from geometry.
    /// </para>
    /// </summary>
    public const string Dataset = "Dataset";

    /// <summary>
    /// One panel for all of machine learning, whatever the paradigm: four cores
    /// that take data and a method on a wire, the methods that go on that wire, and
    /// the data steps around them.
    /// <para>
    /// Order within it comes from <c>GH_Exposure</c>, which draws a divider between
    /// tiers: the four cores (cluster, train, predict, embed), then the clustering
    /// methods, then the learners, then the embedding methods, then enum dropdowns.
    /// Getting the data ready is the
    /// <see cref="Dataset"/> section's job, one panel to the left. A method
    /// component has no data input and outputs nothing but its wire, so the core a person reaches for first is the one at the top, and it
    /// answers with nothing else wired.
    /// </para>
    /// <para>
    /// Named for a technique because its user is assembling a pipeline — the one
    /// kind of user who wants to choose a method. Every job-named section above
    /// hides the methods behind a tool, and a user who has a job to do should find
    /// it there without ever opening this one. It used to be three sections, one per
    /// paradigm, mirroring the repos; that mirrored a distinction the person writing
    /// the code cares about and the person using it does not, and left someone who
    /// wanted to cluster something choosing between panels before they had chosen
    /// a method.
    /// </para>
    /// </summary>
    public const string MachineLearning = "Machine Learning";

    /// <summary>
    /// The order the sections are read in, left to right: the techniques first,
    /// with the data before the learning that consumes it, then the jobs in the
    /// order a project meets them — form, design, making,
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
        Dataset,
        MachineLearning,
        StructuralForm,
        FormFinding,
        StructuralDesign,
        Fabrication,
        Construction,
        Document,
    };
}
