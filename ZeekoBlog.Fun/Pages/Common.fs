module internal Common

open Giraffe.GiraffeViewEngine
open ZeekoBlog.Core.Models

let katexResource = [
    link [ _rel "stylesheet"; _href "https://cdn.jsdelivr.net/npm/katex@0.11.1/dist/katex.min.css" ]
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

let yandexTag = script [] [ rawText """
(function(m,e,t,r,i,k,a){m[i]=m[i]||function(){(m[i].a=m[i].a||[]).push(arguments)};
m[i].l=1*new Date();k=e.createElement(t),a=e.getElementsByTagName(t)[0],k.async=1,k.src=r,a.parentNode.insertBefore(k,a)})
(window, document, "script", "https://mc.yandex.ru/metrika/tag.js", "ym");

ym(52117861, "init", {
     id:52117861,
     clickmap:true,
     trackLinks:true,
     accurateTrackBounce:true
});
""" ]

let yandexNoScript = noscript [] [
    div [] [
        img [ _src "https://mc.yandex.ru/watch/52117861"; _style "position:absolute; left:-9999px;"; _alt "" ]
    ]
]

let renderedClass (article: ArticleDocType) x =
    let className =
        match article with
        | ArticleDocType.Raw -> "adoc"
        | ArticleDocType.AsciiDoc -> "adoc"
        | ArticleDocType.Markdown -> "md"
        | _ -> "adoc"
    sprintf "%s rendered %s" x className
