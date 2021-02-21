module IndexHandler

open Microsoft.AspNetCore.Http
open Giraffe
open ZeekoBlog.Application.Services
open Utils.TryParse
open FSharp.Control.Tasks
open IndexPage
open System.Linq

let handler (user: string): HttpHandler =
    fun (next: HttpFunc) (ctx: HttpContext) ->
        let articleSvc = ctx.GetService<ArticleService>()
        let accountSvc = ctx.GetService<AccountService>()

        let page =
            ctx.TryGetQueryStringValue("p")
            |> Option.bind parseInt
            |> Option.map (max 1)
            |> Option.defaultValue 1

        task {
            let! blogUser = accountSvc.GetUserByNameAsync(user)

            let blogUserId =
                if blogUser |> isNull then 1 else blogUser.Id

            let! pagedList = articleSvc.GetPaged(page - 1, 20, blogUserId)
            let articles = pagedList.List
            let totalPages = pagedList.TotalPage

            if page > totalPages then
                return! ZeekoBlog.ErrorHandler.errorCodeHandler 404 next ctx
            else
                let model =
                    { CurrentIndex = page
                      TotalPages = totalPages
                      Articles = List.ofSeq (articles.AsEnumerable()) }

                return! htmlView (Index.view model) next ctx
        }
