using Planner.Models.Notes;
using System;

namespace Planner.Models.Markdown;

public static class MarkdownGuideText {

    public static Note[] Notes { get; }  = [
        new Note() {
            Title = "LaTeX Mathematics",
            Text = """
                        ### Math Constructs
            Code|Example
            ---|---
            x^{y+1} | $x^{y+1}$
            A_b|$A_b$
            \frac\{numerator}\{denominator}|$$\frac{numerator}{denominator}$$
            F'|$F'$
            \sqrt\{a}|$\sqrt{a}$
            \sqrt\[4]\{a}|$\sqrt[4]{a}$
            \overline\{ab}|$\overline{ab}$
            Also works for the following|\underline \widehat \widetilde \overrightarrow \overleftarrow \overbrace \underbrace

            ### Delimiters
            Code|Example
            ---|---
            \vert | $\vert$
            \Vert | $\Vert$
            \\\{ abc \\\} | $\{abc\}$
            \(abc)| $(abc)$
            \[abc]|$[abc]$
            \\lfloor abc \\rfloor |$\lfloor abc \rfloor$
            Can replace floor above with | ceil, angle, uparrow, or downarrow
            \\left\{x} ... \\right\{x} where \{x} is any of the above scales to content | $$\left[ \sum\limits^{20}_{i = 1} i^2 + 1 \right]$$

            ### Variable sized symbols
            Formula|Example
            ---|---
            \sum_\{i=1}^{25} i | $\sum_{i=1}^{25} i$
            \sum\limits_\{i=1}^{25} i | $\sum\limits_{i=1}^{25} i$
            sum in the above 2 lines can be|int, iint, iiint, iiiint, prod, coprod,biguplus,bigcap,bigcuip,bigoplus, bigotimes, bigodot,bigvee,bigwedge, or bigsqcup

            ### Standard function Names
            most standard functions (\sin \cos \tan \log \ln etc) print in roman instead of italic.

            ### Arrays
            `````
                \begin{array}{cols} row1 \\ row2 \\ . . . rowm \end{array}
            `````
            where cols includes one character [lrc] for each column (with optional characters | inserted for vertical lines)
            and rowj includes character & a total of (n -1) times to separate the n elements in the row. 

            thus
            `````
            $$ \left\{   \begin{array}{c|c} 1 & 2 \\ 3 & 4 \\ a/b & 10^2 \end{array} \right\}  $$
            `````
            displays as
            $$ \left\{   \begin{array}{c|c} 1 & 2 \\ 3 & 4 \\ a/b & 10^2 \end{array} \right\}  $$
            """
        }
    ];

    internal static Note NoteForIndex(int index) =>
        Notes[Math.Clamp(index, 0, Notes.Length - 1)];
}
