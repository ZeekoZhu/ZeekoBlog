module IndexPage
open Giraffe
open GiraffeViewEngine
open LayoutPage
open Common
open ZeekoBlog.Application.DTO



type IndexModel =
    { Articles: ArticleListDto list
      CurrentIndex: int
      TotalPages: int }


let hideWhen condition =
    if condition then "hide" else ""

let layoutData =
    { Title = "首页" }

let scripts =
    katexResource @
        [ script [] [ rawText "window.__pageModule = 'article-list'" ]
          script [ _src "/dist/article.js" ] []
        ]

let friendLinks =
    [ "https://rocka.me", "Rocket1184"
      "https://jolyne.club", "Neatline"
      "https://www.jijiwuming.cn", "jijiwuming"
      "https://meowv.com", "阿星Plus"
    ]
    |> List.map
        ( fun (link, text) -> a [ _class "side-item"; _href link; _target "_blank" ] [ rawText text ])

let sidebar =
    [ sideGroup
        "友情链接"
        friendLinks
    ]

let header =
    [ h1 [] [ rawText "网上冲浪指南" ]
    ]

module Index =
    let view model =
        let articleList =
            model.Articles
            |> List.groupBy (fun x -> sprintf "%d / %02d" x.Created.Year x.Created.Month)
            |> List.map
                ( fun (key, articles) ->
                    seq {
                        yield span [ _class "date section" ] [ rawText key ]
                        for article in articles do
                            yield div [ _class "article-item" ]
                                      [ span [ _class "title" ]
                                             [ a [ _href (sprintf "a/%d" article.Id) ] [ rawText article.Title ] ]
                                        div [ _class ("summary process_math" |> renderedClass article.DocType) ] [ rawText article.RenderedSummary ]
                                      ]
                    }
                )
            |> List.collect List.ofSeq

        let viewBody =
            div [ _class "index" ]
                [
                  div [ _class "articles" ]
                      articleList
                  div [ _class "pagination" ]
                      [ a [ _class (hideWhen (model.CurrentIndex = 1) |> sprintf "prev %s")
                            _href (model.CurrentIndex - 1 |> sprintf "/?p=%d")
                          ] [ rawText "上一页" ]
                        span [ _class "current" ] [ rawText (model.CurrentIndex |> string) ]
                        a [ _class (hideWhen (model.CurrentIndex = model.TotalPages) |> sprintf "next %s")
                            _href (model.CurrentIndex + 1 |> sprintf "/?p=%d")
                          ] [ rawText "下一页" ]
                      ]
                ]
        { Scripts = scripts
          Header = header
          ModuleName = "index-module"
          Styles = [ emptyText ]
          Body = [ viewBody ]
          Sidebar = sidebar
        }
        |> LayoutPage.view layoutData
