module IndexHandler
open Microsoft.AspNetCore.Http
open Giraffe
open ZeekoBlog.Core.Services
open ZeekoBlog.Markdown
open Utils.TryParse
open IndexPage
open System.Linq

let handler (user: string): HttpHandler =
    fun (next: HttpFunc) (ctx: HttpContext) ->
        let articleSvc = ctx.GetService<ArticleService> ()
        let accountSvc = ctx.GetService<AccountService>()
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
            let! blogUser = accountSvc.GetUserByNameAsync(user)
            let blogUserId = if blogUser |> isNull then 1 else blogUser.Id
            let! struct (articles, totalPages) = articleSvc.GetPaged(page - 1, 20, blogUserId);
            if articles.Any()
            then
                let model =
                    { CurrentIndex = page
                      TotalPages = totalPages
                      Articles = List.ofSeq articles
                      Markdown = mdService }
                return! htmlView (Index.view model) next ctx
            else
                return! setStatusCode 404 next ctx
        }
