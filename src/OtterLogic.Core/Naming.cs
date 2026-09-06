using System.Text;

namespace OtterLogic.Core;

/// <summary>
/// Turning code names into text a person reads.
/// <para>
/// Lives in Core because every domain ends up with option enums, and both
/// front-ends have to show them: Grasshopper in a right-click menu, Rhino as
/// command-line options. Two copies of this drift into two different spellings
/// of the same option, which is exactly the confusion a shared vocabulary is
/// supposed to prevent.
/// </para>
/// </summary>
public static class Naming
{
    /// <summary>
    /// "WarrenWithVerticals" becomes "Warren with verticals".
    /// </summary>
    public static string Humanise(Enum value) => Humanise(value.ToString());

    /// <summary>
    /// Splits a PascalCase name into sentence case: a space at each word
    /// boundary, the first word left capitalised and the rest lowered.
    /// <para>
    /// Runs of capitals are read as acronyms and kept intact, so "UDLCase"
    /// gives "UDL case" rather than being broken up letter by letter.
    /// </para>
    /// </summary>
    public static string Humanise(string name)
    {
        if (string.IsNullOrEmpty(name)) return string.Empty;

        var text = new StringBuilder(name.Length + 8);
        text.Append(name[0]);

        for (int i = 1; i < name.Length; i++)
        {
            char current = name[i];

            // A word starts where lower gives way to upper, and at the last
            // capital of a run of them — the point where the acronym ends and
            // the next word begins.
            bool startsWord = char.IsUpper(current)
                && (!char.IsUpper(name[i - 1]) || (i + 1 < name.Length && char.IsLower(name[i + 1])));

            if (!startsWord)
            {
                text.Append(current);
                continue;
            }

            text.Append(' ');

            // One capital opening a word is sentence case mid-sentence, so it
            // drops; more than one is an acronym and keeps its case.
            bool acronym = i + 1 < name.Length && char.IsUpper(name[i + 1]);
            text.Append(acronym ? current : char.ToLowerInvariant(current));
        }

        return text.ToString();
    }
}
