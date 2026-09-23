namespace OtterLogic.Core;

/// <summary>
/// How much a <see cref="Note"/> should worry the reader.
/// </summary>
public enum NoteLevel
{
    /// <summary>Worth knowing; nothing to fix. A column was dropped, a count was derived.</summary>
    Remark = 0,

    /// <summary>The answer stands but is weaker than it looks. Check before acting on it.</summary>
    Warning = 1,
}

/// <summary>
/// Something a user would otherwise have to read a report to notice — and would
/// not, because the tool looks like it worked.
/// <para>
/// A domain type rather than a string so that the level travels with the text.
/// Both front-ends say the same sentence about the same thing, so the sentence is
/// a claim about the domain and is written once here; each adaptor maps the level
/// onto what it has — a Grasshopper bubble, a command-line line.
/// </para>
/// </summary>
public readonly record struct Note(NoteLevel Level, string Text)
{
    public static Note Remark(string text) => new(NoteLevel.Remark, text);

    public static Note Warning(string text) => new(NoteLevel.Warning, text);

    public override string ToString() => Text;
}
