using System.Text.RegularExpressions;

namespace Planner.Models.HtmlGeneration;

public static partial class MathJaxEpilogueProvider
{
    public static bool MightHaveMath(string code) => MathRegex().IsMatch(code);

    [GeneratedRegex(@"\$.+\$", RegexOptions.Compiled)]
    public static partial Regex MathRegex();

    public const string Epilogue = """
        <script src=\"https://cdn.jsdelivr.net/npm/mathjax@3/es5/tex-mml-chtml.js\"></script>
        """;
        
}