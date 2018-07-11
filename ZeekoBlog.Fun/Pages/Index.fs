namespace ZeekoBlog.Fun.Pages

module Index =
    open ZeekoBlog.Core.Models
    open Giraffe
    open GiraffeViewEngine
    open Layout
    open Common

    type IndexModel =
        { Articles: Article list
          CurrentIndex: int
          TotalPages: int }

    let hideWhen condition =
        if condition then "hide" else ""

    let layoutData =
        { Title = "首页" }
            
    let scripts =
        mathJaxScript @ [
            script [] [ rawText "window.__pageModule = 'article-list'" ]
            script [ _src "/dist/article.js" ] []
        ]

    let sidebar = [
        // todo: sidebar
        div [] []
    ]

    let view model =
        let viewBody =
            div [ _class "index" ] [
                h1 [] [ rawText "网上冲浪指南 (Beta)" ]
                div [ _class "divide wide-divide" ] []
                div [ _class "articles" ] (
                    model.Articles
                    |> List.map (fun article ->
                        div [ _class "article-short" ] [
                            h2 [] [
                                a [ _href (sprintf "/a/%d" article.Id) ] [ rawText article.Title ]
                            ]
                            span [ _class "weak" ] [ rawText (article.LastEdited.ToString("yyyy/MM/dd")) ]
                            div [ _class "weak summary" ] [ (* todo markdown *) ]
                        ]
                    )
                )
                div [ _class "pager" ] [
                    span [ _class "current" ] [ rawText (model.CurrentIndex |> string) ]
                    a [
                        _class (hideWhen (model.CurrentIndex = 1) |> sprintf "prev mdl2 %s")
                        _href (model.CurrentIndex - 1 |> sprintf "/?p=%d")
                    ] []
                    a [
                        _class (hideWhen (model.CurrentIndex = model.TotalPages) |> sprintf "next mdl2 %s")
                        _href (model.CurrentIndex - 1 |> sprintf "/?p=%d")
                    ] []
                ]
            ]
        { Scripts = scripts
          Styles = [ emptyText ]
          Body = [ viewBody ]
          Sidebar = sidebar }
        |> Layout.view layoutData 
