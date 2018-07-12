module IndexHandler
open Microsoft.AspNetCore.Http
open Giraffe
open ZeekoBlog.Core.Services
open ZeekoBlog.Markdown
open Utils.TryParse
open IndexPage

let handler: HttpHandler =
    fun (next: HttpFunc) (ctx: HttpContext) ->
        let articleSvc = ctx.GetService<ArticleService> ()
        let mdService = ctx.GetService<MarkdownService> ()
        let page =
            match ctx.TryGetQueryStringValue("p") with
            | Some p ->
                parseInt >> function
                    | Some n -> n
                    | None -> 1
                <| p
            | None -> 1
            |> max 1
        task {
            let! struct (articles, totalPages) = articleSvc.GetPaged(page - 1, 20);
            let model =
                { CurrentIndex = page
                  TotalPages = totalPages
                  Articles = List.ofSeq articles
                  Markdown = mdService }
            return! htmlView (Index.view model) next ctx
        }
