module internal Common

open Giraffe.GiraffeViewEngine
let mathJaxScript = [
    script [ _src "https://cdn.bootcss.com/mathjax/2.7.2/MathJax.js" ] []
    script [] [
                rawText 
                    """
messageStyle: 'none',
extensions: ["tex2jax.js"],
jax: ["input/TeX", "output/HTML-CSS"],
tex2jax: {
    inlineMath: [['$', '$'], ['\\(', '\\)']],
    displayMath: [['$$', '$$'], ['\[', '\]']],
    processEscapes: true
},
'HTML-CSS': { availableFonts: ['TeX'] }
        });""" ]
]

let sideGroup title content =
    div [ _class "side-card" ] [
        div [ _class "side-title" ] [
            h3 [] [ rawText title ]
        ]
        div [ _class "divide" ] []
        div [ _class "side-body" ] content
    ]
