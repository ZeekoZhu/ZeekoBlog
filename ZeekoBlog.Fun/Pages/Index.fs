module IndexPage
open ZeekoBlog.Core.Models
open ZeekoBlog.Markdown
open Giraffe
open GiraffeViewEngine
open LayoutPage
open Common

type IndexModel =
    { Articles: Article list
      CurrentIndex: int
      TotalPages: int
      Markdown: MarkdownService }


let hideWhen condition =
    if condition then "hide" else ""

let layoutData =
    { Title = "首页" }

let scripts =
    mathJaxScript @
        [ script [] [ rawText "window.__pageModule = 'article-list'" ]
          script [ _src "/dist/article.js" ] []
        ]

let sidebar =
    [ sideGroup
        "友情链接"
        [ a [ _class "side-item"
              _href "https://rocka.me"
              _target "_blank" ] [ rawText "Rocket1184" ]
          a [ _class "side-item"
              _href "https://blog.neatline.cn"
              _target "_blank" ] [ rawText "Neatline" ]
        ]
    ]

module Index =
    let view model =
        let markdown = createMd model.Markdown

        let viewBody =
            div [ _class "index" ]
                [ h1 []
                     [ rawText "网上冲浪指南 "
                       code [] [ rawText " |> λ" ]
                     ]
                  div [ _class "divide wide-divide" ] []
                  div [ _class "articles" ]
                      ( model.Articles
                          |> List.map
                            ( fun article ->
                              div [ _class "article-short" ]
                                [ h2 []
                                     [ a [ _href (sprintf "/a/%d" article.Id) ] [ rawText article.Title ]
                                     ]
                                  span [ _class "weak" ] [ rawText (article.LastEdited.ToString("yyyy/MM/dd")) ]
                                  div [ _class "weak summary" ] [ markdown article.Summary ]
                                ]
                            )
                      )
                  div [ _class "pager" ]
                      [ span [ _class "current" ] [ rawText (model.CurrentIndex |> string) ]
                        a [ _class (hideWhen (model.CurrentIndex = 1) |> sprintf "prev mdl2 %s")
                            _href (model.CurrentIndex - 1 |> sprintf "/?p=%d")
                          ] []
                        a [ _class (hideWhen (model.CurrentIndex = model.TotalPages) |> sprintf "next mdl2 %s")
                            _href (model.CurrentIndex - 1 |> sprintf "/?p=%d")
                          ] []
                      ]
            ]
        { Scripts = scripts
          Styles = [ emptyText ]
          Body = [ viewBody ]
          Sidebar = sidebar
        }
        |> LayoutPage.view layoutData
