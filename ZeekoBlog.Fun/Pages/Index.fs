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
    katexResource

let friendLinks =
    [ "https://rocka.me", "Rocket1184"
      "https://jolyne.club", "Neatline"
      "https://www.jijiwuming.cn", "jijiwuming"
      "https://meowv.com", "阿星Plus"
      "https://codeporter.dev", "Tim's Blog"
    ]
    |> List.map
        ( fun (link, text) -> a [ _class "side-item"; _href link; _target "_blank" ] [ rawText text ])

let sidebar =
    [ sideGroup
        "友情链接"
        friendLinks
    ]

let header =
    [
      div [ _class "h-80" ] [
          h1 [ _class "text-6xl font-bold absolute bottom-16" ] [ rawText "网上冲浪指南" ]
      ]
    ]

module Index =
    let view model =
        let articleList =
            model.Articles
            |> List.groupBy (fun x -> sprintf "%d / %02d" x.Created.Year x.Created.Month)
            |> List.map
                ( fun (key, articles) ->
                    seq {
                        yield span [ _class "z-section" ] [ rawText key ]
                        for article in articles do
                            yield div [ _class "mt-6" ]
                                      [ span [ _class "text-xl underline" ]
                                             [ a [ _href (sprintf "a/%d" article.Id) ] [ rawText article.Title ] ]
                                        div [ _class ("mt-2 process_math" |> renderedClass article.DocType) ] [ rawText article.RenderedSummary ]
                                      ]
                    }
                )
            |> List.collect List.ofSeq

        let viewBody =
            div [ _class "index" ]
                [
                  div [ _class "articles" ]
                      articleList
                  div [ _class "w-full flex justify-center mt-12" ]
                      [ a [ _class (hideWhen (model.CurrentIndex = 1) |> sprintf "prev paging-btn %s")
                            _href (model.CurrentIndex - 1 |> sprintf "/?p=%d")
                          ] [ rawText "上一页" ]
                        span [ _class "paging black" ] [ rawText (model.CurrentIndex |> string) ]
                        a [ _class (hideWhen (model.CurrentIndex = model.TotalPages) |> sprintf "next paging-btn %s")
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
        |> view layoutData
