module internal Common

open Giraffe.GiraffeViewEngine
let mathJaxScript = [
    script [ _src "https://cdnjs.cloudflare.com/ajax/libs/mathjax/2.7.4/MathJax.js" ] []
    script [] [
                rawText 
                    """
MathJax.Hub.Config({
    showProcessingMessages: false,
    messageStyle: 'none',
    extensions: ["tex2jax.js"],
    jax: ["input/TeX", "output/HTML-CSS"],
    tex2jax: {
        inlineMath: [['$', '$'], ['\\(', '\\)']],
        displayMath: [['$$', '$$'], ['\\[', '\\]']],
        processEscapes: true,
        processClass: 'process_math'
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

let gtag = [
    script [ _src "https://www.googletagmanager.com/gtag/js?id=UA-133364348-1"; _async ] []
    script [] [ rawText """
window.dataLayer = window.dataLayer || [];
function gtag(){dataLayer.push(arguments);}
gtag('js', new Date());
gtag('config', 'UA-133364348-1');
""" ]
]
