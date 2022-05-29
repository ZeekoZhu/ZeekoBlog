module ArticlePage

open ZeekoBlog.Core.Models
open Giraffe.ViewEngine
open LayoutPage
open Common

let header =
    [ a [ _class "font-bold mt-4 inline-block" ; _href "/" ] [ rawText "目录：网上冲浪指南" ] ]

type ArticleModel =
    { Article: Article
      Languages: string list
      TOCList: TOCItem list
    }

let og prop value =
    meta [ attr "property" ("og:" + prop); attr "content" value ]

let view (model: ArticleModel) =
    let layoutModel: LayoutModel =
        { Title = model.Article.Title }
    let pageUrl = "https://gianthard.rocks/a/" + model.Article.Id.ToString()
    let meta = [
        og "title" model.Article.Title
        og "type" "article"
        og "url" pageUrl
        og "description" model.Article.Summary
        og "site_name" "网上冲浪指南"
    ]
    let sidebar =
        if model.TOCList |> List.isEmpty then []
        else sideGroup "内容导航"
                  [ ul [ _class "toc-list text-sm space-y-1.5" ]
                       ( model.TOCList
                         |> List.ofSeq
                         |> List.map
                            ( fun item ->
                                li [ _class (sprintf "toc-H%d" item.Level) ]
                                   [ a [ _class "link" ; _href (sprintf "#%s" item.AnchorName) ]
                                       [ rawText item.Name ]
                                   ]
                            )
                       )
                  ]
    let pageId = "zeeko-blog/a/" + model.Article.Id.ToString()
    let title = model.Article.Title
    let scripts =
        [ script [] [rawText "window.__pageModule = 'article-ro'"]
          script []
                 [ rawText """
$('input[type=checkbox][disabled][checked]').replaceWith('<b class="mdl2" aria-hidden="true" style="font-size: .8rem;">&#xF16C;</b>')
$('input[type=checkbox][disabled]').replaceWith('<b class="mdl2" aria-hidden="true" style="font-size: .8rem;">&#xF16B;</b>');"""
                 ]
          script []
                 [ rawText (sprintf """
var disqus_config = function () {
this.page.title = '%s'
this.page.url = '%s';  // Replace PAGE_URL with your page's canonical URL variable
this.page.identifier = '%s'; // Replace PAGE_IDENTIFIER with your page's unique identifier variable
};
(function() { // DON'T EDIT BELOW THIS LINE
var d = document, s = d.createElement('script');
s.src = 'https://zeeko-blog.disqus.com/embed.js';
s.setAttribute('data-timestamp', +new Date());
(d.head || d.body).appendChild(s);
})();
""" title pageUrl pageId) ]
        ]
    // view
    let viewBody =
        div []
            [ h1 [ _class "text-4xl mt-10 mb-8" ] [ rawText model.Article.Title ]
              div []
                  [ span [] [ rawText (model.Article.Created.ToString("yyyy/MM/dd")) ] ]
              article [ "mt-7 process_math" |> renderedClass model.Article.DocType |> _class ]
                  [ rawText model.Article.RenderedContent
                  ]
              div [ _id "disqus_thread" ] []
            ]

    { Scripts = scripts
      Styles = [ link [ _href "https://cdnjs.cloudflare.com/ajax/libs/highlight.js/9.12.0/styles/github-gist.min.css"; _rel "stylesheet"]
                 link [ _href "https://cdnjs.cloudflare.com/ajax/libs/font-awesome/4.7.0/css/font-awesome.min.css"; _rel "stylesheet" ]
               ] @ katexResource
      Header = header
      ModuleName = "article-module"
      Body = [ viewBody ]
      Sidebar = sidebar
      Meta = meta
    }
    |> view layoutModel
