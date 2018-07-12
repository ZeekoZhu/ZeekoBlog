module ArticleHandler
open Microsoft.AspNetCore.Http
open Giraffe
open ZeekoBlog.Core.Services
open ZeekoBlog.Markdown
open ZeekoBlog.Markdown.Plugins.TOCItemsPlugin
open ZeekoBlog.Markdown.Plugins.CodeLangDetectionPlugin
open ArticlePage

let handler (id: int): HttpHandler =
    fun (next: HttpFunc) (ctx: HttpContext) ->
        let articleSvc = ctx.GetService<ArticleService> ()
        let mdSvc = ctx.GetService<MarkdownService> ()
        task {
            let! article = articleSvc.GetById(id)
            if article |> isNull
            then
                return! RequestErrors.NOT_FOUND "" next ctx
            else
                let mdResult = mdSvc.Process(article.Content)
                let langResult = CodeLangDetectionPlugin.TryGet<CodeLangDetectionPlugin>(mdResult.Storage)
                let struct (_, languages) = langResult
                let tocResult = TOCItemsPlugin.TryGet<TOCItemsPlugin>(mdResult.Storage)
                let struct (_, tocList) = tocResult
                let model: ArticleModel =
                    { Languages = languages |> List.ofSeq
                      Markdown = mdSvc
                      Article = article
                      TOCList = tocList |> List.ofSeq
                    }
                return! htmlView (ArticlePage.view model) next ctx
        }
