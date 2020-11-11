module ArticleHandler
open Microsoft.AspNetCore.Http
open Giraffe
open ZeekoBlog.Application.Services
open FSharp.Control.Tasks
open ArticlePage

let handler (id: int): HttpHandler =
    fun (next: HttpFunc) (ctx: HttpContext) ->
        let articleSvc = ctx.GetService<ArticleService> ()
        task {
            let! article = articleSvc.GetById(id)
            if article |> isNull
            then
                return! setStatusCode 404 next ctx
            else
                let model: ArticleModel =
                    { Languages = article.Languages.Split(',') |> List.ofSeq
                      Article = article
                      TOCList = article.TOCList |> List.ofSeq
                    }
                return! htmlView (ArticlePage.view model) next ctx
        }
