module ZeekoBlog.Fun.Manage.Handlers.EditHandler
open Giraffe
open FSharp.Control.Tasks.V2
open ZeekoBlog.Application.Services
open ZeekoBlog.Fun.Manage.Pages

let hander (id: int option) : HttpHandler =
    fun next ctx ->
        match id with
        | None ->
            htmlView (Edit.view { Article = None }) next ctx
        | Some articleId ->
            let articleSvc = ctx.GetService<ArticleService>()
            task {
                let! article = articleSvc.GetById(articleId)
                return! htmlView (Edit.view { Article = Some article }) next ctx
            }
