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

            thus:
            `````
            $$ \left\{   \begin{array}{c|c} 1 & 2 \\ 3 & 4 \\ a/b & 10^2 \end{array} \right\}  $$
            `````
            displays as
            $$ \left\{   \begin{array}{c|c} 1 & 2 \\ 3 & 4 \\ a/b & 10^2 \end{array} \right\}  $$

            ### Type Styles
            Code | Example
            ---|---
            \mathcal\{ABC} | $\mathcal{ABC}$
            \mathbb\{ABC} | $\mathbb{ABC}$
            \mathfrak\{ABC} | $\mathfrak{ABC}$
            \mathbf\{ABC} | $\mathbf{ABC}$

            ### Text Size Prefixes
            Code |Example
            ---|---
            \\tiny tiny | $\tiny tiny$
            \scriptsize scripsize|$\scriptsize scripsize$
            \small Small |$ \small Small $
            \normalsize Normal | $\normalsize Normal$
            \large large| $\large large$
            \Large Large| $\Large Large$
            \LARGE LARGE | $\LARGE LARGE$
            \huge huge| $\huge huge$
            \Huge Huge | $\Huge Huge$

            ### Misc Symbols
            Code | Char | Code | Char | Code | Char 
            ---|---|---|---|---|--- 
            \\ast | $\ast$ |\\pm | $\pm$ |\\cap | $\cap$ 
            \\lhd | $\lhd$ |\\star | $\star$ |\\mp | $\mp$ 
            \\cup | $\cup$ |\\rhd | $\rhd$ |\\cdot | $\cdot$ 
            \\amalg | $\amalg$ |\\uplus | $\uplus$ |\\triangleleft | $\triangleleft$ 
            \\circ | $\circ$ |\\odot | $\odot$ |\\sqcap | $\sqcap$ 
            \\triangleright | $\triangleright$ |\\bullet | $\bullet$ |\\ominus | $\ominus$ 
            \\sqcup | $\sqcup$ |\\unlhd | $\unlhd$ |\\bigcirc | $\bigcirc$ 
            \\oplus | $\oplus$ |\\wedge | $\wedge$ |\\unrhd | $\unrhd$ 
            \\diamond | $\diamond$ |\\oslash | $\oslash$ |\\vee | $\vee$ 
            \\bigtriangledown | $\bigtriangledown$ |\\times | $\times$ |\\otimes | $\otimes$ 
            \\dagger | $\dagger$ |\\bigtriangleup | $\bigtriangleup$ |\\div | $\div$ 
            \\wr | $\wr$ |\\ddagger | $\ddagger$ |\\setminus | $\setminus$ 
            \\centerdot | $\centerdot$ |\\Box | $\Box$ |\\barwedge | $\barwedge$ 
            \\veebar | $\veebar$ |\\circledast | $\circledast$ |\\boxplus | $\boxplus$ 
            \\curlywedge | $\curlywedge$ |\\curlyvee | $\curlyvee$ |\\circledcirc | $\circledcirc$ 
            \\boxminus | $\boxminus$ |\\Cap | $\Cap$ |\\Cup | $\Cup$ 
            \\circleddash | $\circleddash$ |\\boxtimes | $\boxtimes$ |\\bot | $\bot$ 
            \\top | $\top$ |\\dotplus | $\dotplus$ |\\boxdot | $\boxdot$ 
            \\intercal | $\intercal$ |\\rightthreetimes | $\rightthreetimes$ |\\divideontimes | $\divideontimes$ 
            \\square | $\square$ |\\doublebarwedge | $\doublebarwedge$ |\\leftthreetimes | $\leftthreetimes$ 
            \\equiv | $\equiv$ |\\leq | $\leq$ |\\geq | $\geq$ 
            \\perp | $\perp$ |\\cong | $\cong$ |\\prec | $\prec$ 
            \\succ | $\succ$ |\\mid | $\mid$ |\\neq | $\neq$ 
            \\preceq | $\preceq$ |\\succeq | $\succeq$ |\\parallel | $\parallel$ 
            \\sim | $\sim$ |\\ll | $\ll$ |\\gg | $\gg$ 
            \\bowtie | $\bowtie$ |\\simeq | $\simeq$ |\\subset | $\subset$ 
            \\supset | $\supset$ |\\Join | $\Join$ |\\approx | $\approx$ 
            \\subseteq | $\subseteq$ |\\supseteq | $\supseteq$ |\\ltimes | $\ltimes$ 
            \\asymp | $\asymp$ |\\sqsubset | $\sqsubset$ |\\sqsupset | $\sqsupset$ 
            \\rtimes | $\rtimes$ |\\doteq | $\doteq$ |\\sqsubseteq | $\sqsubseteq$ 
            \\sqsupseteq | $\sqsupseteq$ |\\smile | $\smile$ |\\propto | $\propto$ 
            \\dashv | $\dashv$ |\\vdash | $\vdash$ |\\frown | $\frown$ 
            \\models | $\models$ |\\in | $\in$ |\\ni | $\ni$ 
            \\notin | $\notin$ |\\approxeq | $\approxeq$ |\\leqq | $\leqq$ 
            \\geqq | $\geqq$ |\\lessgtr | $\lessgtr$ |\\thicksim | $\thicksim$ 
            \\leqslant | $\leqslant$ |\\geqslant | $\geqslant$ |\\lesseqgtr | $\lesseqgtr$ 
            \\backsim | $\backsim$ |\\lessapprox | $\lessapprox$ |\\gtrapprox | $\gtrapprox$ 
            \\lesseqqgtr | $\lesseqqgtr$ |\\backsimeq | $\backsimeq$ |\\lll | $\lll$ 
            \\ggg | $\ggg$ |\\gtreqqless | $\gtreqqless$ |\\triangleq | $\triangleq$ 
            \\lessdot | $\lessdot$ |\\gtrdot | $\gtrdot$ |\\gtreqless | $\gtreqless$ 
            \\circeq | $\circeq$ |\\lesssim | $\lesssim$ |\\gtrsim | $\gtrsim$ 
            \\gtrless | $\gtrless$ |\\bumpeq | $\bumpeq$ |\\eqslantless | $\eqslantless$ 
            \\eqslantgtr | $\eqslantgtr$ |\\backepsilon | $\backepsilon$ |\\Bumpeq | $\Bumpeq$ 
            \\precsim | $\precsim$ |\\succsim | $\succsim$ |\\between | $\between$ 
            \\doteqdot | $\doteqdot$ |\\precapprox | $\precapprox$ |\\succapprox | $\succapprox$ 
            \\pitchfork | $\pitchfork$ |\\thickapprox | $\thickapprox$ |\\Subset | $\Subset$ 
            \\Supset | $\Supset$ |\\shortmid | $\shortmid$ |\\fallingdotseq | $\fallingdotseq$ 
            \\subseteqq | $\subseteqq$ |\\supseteqq | $\supseteqq$ |\\smallfrown | $\smallfrown$ 
            \\risingdotseq | $\risingdotseq$ |\\sqsubset | $\sqsubset$ |\\sqsupset | $\sqsupset$ 
            \\smallsmile | $\smallsmile$ |\\varpropto | $\varpropto$ |\\preccurlyeq | $\preccurlyeq$ 
            \\succcurlyeq | $\succcurlyeq$ |\\Vdash | $\Vdash$ |\\therefore | $\therefore$ 
            \\curlyeqprec | $\curlyeqprec$ |\\curlyeqsucc | $\curlyeqsucc$ |\\vDash | $\vDash$ 
            \\because | $\because$ |\\blacktriangleleft | $\blacktriangleleft$ |\\blacktriangleright | $\blacktriangleright$ 
            \\Vvdash | $\Vvdash$ |\\eqcirc | $\eqcirc$ |\\trianglelefteq | $\trianglelefteq$ 
            \\trianglerighteq | $\trianglerighteq$ |\\shortparallel | $\shortparallel$ |\\neq | $\neq$ 
            \\vartriangleleft | $\vartriangleleft$ |\\vartriangleright | $\vartriangleright$ |\\nshortparallel | $\nshortparallel$ 
            \\ncong | $\ncong$ |\\nleq | $\nleq$ |\\ngeq | $\ngeq$ 
            \\nsubseteq | $\nsubseteq$ |\\nmid | $\nmid$ |\\nleqq | $\nleqq$ 
            \\ngeqq | $\ngeqq$ |\\nsupseteq | $\nsupseteq$ |\\nparallel | $\nparallel$ 
            \\nleqslant | $\nleqslant$ |\\ngeqslant | $\ngeqslant$ |\\nsubseteqq | $\nsubseteqq$ 
            \\nshortmid | $\nshortmid$ |\\nless | $\nless$ |\\ngtr | $\ngtr$ 
            \\nsupseteqq | $\nsupseteqq$ |\\nshortparallel | $\nshortparallel$ |\\nprec | $\nprec$ 
            \\nsucc | $\nsucc$ |\\subsetneq | $\subsetneq$ |\\nsim | $\nsim$ 
            \\npreceq | $\npreceq$ |\\nsucceq | $\nsucceq$ |\\supsetneq | $\supsetneq$ 
            \\nVDash | $\nVDash$ |\\precnapprox | $\precnapprox$ |\\succnapprox | $\succnapprox$ 
            \\subsetneqq | $\subsetneqq$ |\\nvDash | $\nvDash$ |\\precnsim | $\precnsim$ 
            \\succnsim | $\succnsim$ |\\supsetneqq | $\supsetneqq$ |\\nvdash | $\nvdash$ 
            \\lnapprox | $\lnapprox$ |\\gnapprox | $\gnapprox$ |\\varsubsetneq | $\varsubsetneq$ 
            \\ntriangleleft | $\ntriangleleft$ |\\lneq | $\lneq$ |\\gneq | $\gneq$ 
            \\varsupsetneq | $\varsupsetneq$ |\\ntrianglelefteq | $\ntrianglelefteq$ |\\lneqq | $\lneqq$ 
            \\gneqq | $\gneqq$ |\\varsubsetneqq | $\varsubsetneqq$ |\\ntriangleright | $\ntriangleright$ 
            \\lnsim | $\lnsim$ |\\gnsim | $\gnsim$ |\\varsupsetneqq | $\varsupsetneqq$ 
            \\ntrianglerighteq | $\ntrianglerighteq$ |\\lvertneqq | $\lvertneqq$ |\\gvertneqq | $\gvertneqq$ 
            \\leftarrow | $\leftarrow$ |\\longleftarrow | $\longleftarrow$ |\\uparrow | $\uparrow$ 
            \\Leftarrow | $\Leftarrow$ |\\Longleftarrow | $\Longleftarrow$ |\\Uparrow | $\Uparrow$ 
            \\rightarrow | $\rightarrow$ |\\longrightarrow | $\longrightarrow$ |\\downarrow | $\downarrow$ 
            \\Rightarrow | $\Rightarrow$ |\\Longrightarrow | $\Longrightarrow$ |\\Downarrow | $\Downarrow$ 
            \\leftrightarrow | $\leftrightarrow$ |\\longleftrightarrow | $\longleftrightarrow$ |\\updownarrow | $\updownarrow$ 
            \\Leftrightarrow | $\Leftrightarrow$ |\\Longleftrightarrow | $\Longleftrightarrow$ |\\Updownarrow | $\Updownarrow$ 
            \\mapsto | $\mapsto$ |\\longmapsto | $\longmapsto$ |\\nearrow | $\nearrow$ 
            \\hookleftarrow | $\hookleftarrow$ |\\hookrightarrow | $\hookrightarrow$ |\\searrow | $\searrow$ 
            \\leftharpoonup | $\leftharpoonup$ |\\rightharpoonup | $\rightharpoonup$ |\\swarrow | $\swarrow$ 
            \\leftharpoondown | $\leftharpoondown$ |\\rightharpoondown | $\rightharpoondown$ |\\nwarrow | $\nwarrow$ 
            \\rightleftharpoons | $\rightleftharpoons$ |\\leadsto | $\leadsto$ |\\dashrightarrow | $\dashrightarrow$ 
            \\dashleftarrow | $\dashleftarrow$ |\\leftleftarrows | $\leftleftarrows$ |\\leftrightarrows | $\leftrightarrows$ 
            \\Lleftarrow | $\Lleftarrow$ |\\twoheadleftarrow | $\twoheadleftarrow$ |\\leftarrowtail | $\leftarrowtail$ 
            \\looparrowleft | $\looparrowleft$ |\\leftrightharpoons | $\leftrightharpoons$ |\\curvearrowleft | $\curvearrowleft$ 
            \\circlearrowleft | $\circlearrowleft$ |\\Lsh | $\Lsh$ |\\upuparrows | $\upuparrows$ 
            \\upharpoonleft | $\upharpoonleft$ |\\downharpoonleft | $\downharpoonleft$ |\\multimap | $\multimap$ 
            \\leftrightsquigarrow | $\leftrightsquigarrow$ |\\rightrightarrows | $\rightrightarrows$ |\\rightleftarrows | $\rightleftarrows$ 
            \\rightrightarrows | $\rightrightarrows$ |\\rightleftarrows | $\rightleftarrows$ |\\twoheadrightarrow | $\twoheadrightarrow$ 
            \\rightarrowtail | $\rightarrowtail$ |\\looparrowright | $\looparrowright$ |\\rightleftharpoons | $\rightleftharpoons$ 
            \\curvearrowright | $\curvearrowright$ |\\circlearrowright | $\circlearrowright$ |\\Rsh | $\Rsh$ 
            \\downdownarrows | $\downdownarrows$ |\\upharpoonright | $\upharpoonright$ |\\downharpoonright | $\downharpoonright$ 
            \\rightsquigarrow | $\rightsquigarrow$ |\\nleftarrow | $\nleftarrow$ |\\nrightarrow | $\nrightarrow$ 
            \\nLeftarrow | $\nLeftarrow$ |\\nRightarrow | $\nRightarrow$ |\\nleftrightarrow | $\nleftrightarrow$ 
            \\nLeftrightarrow | $\nLeftrightarrow$ |\\infty | $\infty$ |\\forall | $\forall$ 
            \\Bbbk | $\Bbbk$ |\\wp | $\wp$ |\\nabla | $\nabla$ 
            \\exists | $\exists$ |\\bigstar | $\bigstar$ |\\angle | $\angle$ 
            \\partial | $\partial$ |\\nexists | $\nexists$ |\\diagdown | $\diagdown$ 
            \\measuredangle | $\measuredangle$ |\\eth | $\eth$ |\\emptyset | $\emptyset$ 
            \\diagup | $\diagup$ |\\sphericalangle | $\sphericalangle$ |\\clubsuit | $\clubsuit$ 
            \\varnothing | $\varnothing$ |\\Diamond | $\Diamond$ |\\complement | $\complement$ 
            \\diamondsuit | $\diamondsuit$ |\\imath | $\imath$ |\\Finv | $\Finv$ 
            \\triangledown | $\triangledown$ |\\heartsuit | $\heartsuit$ |\\jmath | $\jmath$ 
            \\Game | $\Game$ |\\triangle | $\triangle$ |\\spadesuit | $\spadesuit$ 
            \\ell | $\ell$ |\\hbar | $\hbar$ |\\vartriangle | $\vartriangle$ 
            \\cdots | $\cdots$ |\\iiiint | $\iiiint$ |\\hslash | $\hslash$ 
            \\blacklozenge | $\blacklozenge$ |\\vdots | $\vdots$ |\\iiint | $\iiint$ 
            \\lozenge | $\lozenge$ |\\blacksquare | $\blacksquare$ |\\ldots | $\ldots$ 
            \\iint | $\iint$ |\\mho | $\mho$ |\\blacktriangle | $\blacktriangle$ 
            \\ddots | $\ddots$ |\\sharp | $\sharp$ |\\prime | $\prime$ 
            \\blacktrinagledown | $\blacktrinagledown$ |\\Im | $\Im$ |\\flat | $\flat$ 
            \\square | $\square$ |\\backprime | $\backprime$ |\\Re | $\Re$ 
            \\natural | $\natural$ |\\surd | $\surd$ |\\circledS | $\circledS$ 
            \\acute a| $\acute a$ |\\bar a| $\bar a$  | \\breve a | $\breve a$ 
            \\check a| $\check a$ | \\ddot a| $\ddot a$ |\\dot a| $\dot a$ 
            \\grave a| $\grave a$ |\\hat a| $\hat a$
            \\tilde a| $\tilde a$ |\\vec a| $\vec a$ 

            """
        },
        new Note() {
            Title = "Mermaid Class Diagrams",
            Text = """
                        Mermaid class diagram full syntax is at:
            https://mermaid.ai/open-source/syntax/classDiagram.html

            ### Example:
            `````
            ---
            title: Animal example
            ---
            classDiagram
                note "From Duck till Zebra"
                Animal <|-- Duck
                note for Duck "can fly<br>can swim<br>can dive<br>can help in debugging"
                Animal <|-- Fish
                Animal <|-- Zebra
                Animal : +int age
                Animal : +String gender
                Animal: +isMammal()
                Animal: +mate()
                class Duck{
                    +String beakColor
                    +swim()
                    +quack()
                }
                class Fish{
                    -int sizeInFeet
                    -canEat()
                }
                class Zebra{
                    +bool is_wild
                    +run()
                }
            `````
            renders as
            `````mermaid
            ---
            title: Animal example
            ---
            classDiagram
                note "From Duck till Zebra"
                Animal <|-- Duck
                note for Duck "can fly<br>can swim<br>can dive<br>can help in debugging"
                Animal <|-- Fish
                Animal <|-- Zebra
                Animal : +int age
                Animal : +String gender
                Animal: +isMammal()
                Animal: +mate()
                class Duck{
                    +String beakColor
                    +swim()
                    +quack()
                }
                class Fish{
                    -int sizeInFeet
                    -canEat()
                }
                class Zebra{
                    +bool is_wild
                    +run()
                }
            `````

            ### Associations can exist in multiple types
            ````
            classDiagram
            classA --|> classB : Inheritance
            classC --* classD : Composition
            classE --o classF : Aggregation
            classG --> classH : Association
            classI -- classJ : Link(Solid)
            classK ..> classL : Dependency
            classM ..|> classN : Realization
            classO .. classP : Link(Dashed)
            ````

            ````mermaid
            classDiagram
            classA --|> classB : Inheritance
            classC --* classD : Composition
            classE --o classF : Aggregation
            classG --> classH : Association
            classI -- classJ : Link(Solid)
            classK ..> classL : Dependency
            classM ..|> classN : Realization
            classO .. classP : Link(Dashed)
            ````

            ### Cardinality on Relations
            `````
            classDiagram
                Customer "1" --> "*" Ticket
                Student "1" --> "1..*" Course
                Galaxy --> "many" Star : Contains
            `````

            ````` mermaid
            classDiagram
                Customer "1" --> "*" Ticket
                Student "1" --> "1..*" Course
                Galaxy --> "many" Star : Contains
            `````
            """
        },
        new Note(){
            Title = "Mermaid State Diagram",
            Text = """
                        State diagram specification:
            https://mermaid.ai/open-source/syntax/stateDiagram.html

            ### Example
            `````
            stateDiagram
                [*] --> Still
                Still --> [*]

                Still --> Moving
                Moving --> Still
                Moving --> Crash
                Crash -> [*]
            `````
            `````mermaid
            stateDiagram
                [*] --> Still
                Still --> [*]

                Still --> Moving
                Moving --> Still
                Moving --> Crash 
                Crash --> [*]
            `````

            ### Labeled Transitions

            `````
            stateDiagram-v2
                s1 --> s2: A transition
            `````

            `````mermaid
            stateDiagram-v2
                s1 --> s2: A transition
            ``````
            """
        },
        new Note() {
            Title = "Mermaid Sequence Diagram",
            Text = """
                        Sequence Diagram spec is at
            https://mermaid.ai/open-source/syntax/sequenceDiagram.html

            ### Example
            `````
            sequenceDiagram
                Alice->>John: Hello John, how are you?
                John-->>Alice: Great!
                Alice-)John: See you later!
            `````
            `````mermaid
            sequenceDiagram
                Alice->>John: Hello John, how are you?
                John-->>Alice: Great!
                Alice-)John: See you later!
            `````

            ### A more complicated example
            `````
            sequenceDiagram
                participant API@{ "type": "boundary", "alias": "Public API" }
                participant Auth@{ "type": "control", "alias": "Auth Service" }
                participant DB@{ "type": "database", "alias": "User Database" }
                API->>Auth: Login request
                Auth->>DB: Query user
                DB-->>Auth: User data
                Auth-->>API: Access token
            `````

            `````mermaid
            sequenceDiagram
                participant API@{ "type": "boundary", "alias": "Public API" }
                participant Auth@{ "type": "control", "alias": "Auth Service" }
                participant DB@{ "type": "database", "alias": "User Database" }
                API->>Auth: Login request
                Auth->>DB: Query user
                DB-->>Auth: User data
                Auth-->>API: Access token
            `````

            ### Participant types 
            actor, boundary,  control, entity, database, collections, queue

            ### Activation
            `````
            sequenceDiagram
                Alice->>John: Hello John, how are you?
                activate John
                John-->>Alice: Great!
                deactivate John
            `````
            `````mermaid
            sequenceDiagram
                Alice->>John: Hello John, how are you?
                activate John
                John-->>Alice: Great!
                deactivate John
            `````
            """
        }
    ];

    internal static Note NoteForIndex(int index) =>
        Notes[Math.Clamp(index, 0, Notes.Length - 1)];
}
