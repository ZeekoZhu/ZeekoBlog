module ArticlePage

open ZeekoBlog.Core.Models
open Giraffe
open GiraffeViewEngine
open LayoutPage
open Common

let header =
    [ a [ _href "/" ] [ rawText "目录：网上冲浪指南" ] ]

type ArticleModel =
    { Article: Article
      Languages: string list
      TOCList: TOCItem list
    }

let view (model: ArticleModel) =
    let layoutModel: LayoutModel =
        { Title = model.Article.Title }
    let sidebar =
        sideGroup "内容导航"
                  [ ul [ _class "toc-list" ]
                       ( model.TOCList
                         |> List.ofSeq
                         |> List.map
                            ( fun item ->
                                li [ _class (sprintf "toc-H%d" item.Level) ]
                                   [ a [ _href (sprintf "#%s" item.AnchorName) ]
                                       [ rawText item.Name ]
                                   ]
                            )
                       )
                  ]
    let scripts =
        [ script [] [rawText "window.__pageModule = 'article-ro'"]
          script [ _src "/dist/article.js" ] []
          script []
                 [ rawText """
$('input[type=checkbox][disabled][checked]').replaceWith('<b class="mdl2" aria-hidden="true" style="font-size: .8rem;">&#xF16C;</b>')
$('input[type=checkbox][disabled]').replaceWith('<b class="mdl2" aria-hidden="true" style="font-size: .8rem;">&#xF16B;</b>');"""
                 ]
        ]
        @ mathJaxScript
    // view
    let viewBody =
        div []
            [ h1 [ _class "title" ] [ rawText model.Article.Title ]
              div []
                  [ span [ _class "weak" ] [ rawText (model.Article.Created.ToString("yyyy/MM/dd")) ] ]
              article [ "process_math" |> renderedClass model.Article.DocType |> _class ]
                  [ rawText model.Article.RenderedContent
                  ]
            ]

    { Scripts = scripts
      Styles = [ link [ _href "https://cdnjs.cloudflare.com/ajax/libs/highlight.js/9.12.0/styles/vs2015.min.css"; _rel "stylesheet"]
                 link [ _href "https://cdnjs.cloudflare.com/ajax/libs/font-awesome/4.7.0/css/font-awesome.min.css"; _rel "stylesheet" ]
               ]
      Header = header
      ModuleName = "article-module"
      Body = [ viewBody ]
      Sidebar = [ sidebar ]
    }
    |> LayoutPage.view layoutModel
