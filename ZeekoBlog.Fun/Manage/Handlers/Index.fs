module ZeekoBlog.Fun.Manage.Handlers.IndexHandler
open Giraffe
open ZeekoBlog.Application.Services
open ZeekoBlog.Fun.Manage.Pages
open FSharp.Control.Tasks.V2
open Utils.TryParse
open ZeekoBlog.Fun.Manage.Pages.Index
open System.Linq


let handler user : HttpHandler =
    fun next ctx ->
        let articleSvc = ctx.GetService<ArticleService> ()
        let accountSvc = ctx.GetService<AccountService>()
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
            let! pagedList = articleSvc.GetPaged(page - 1, 20, blogUserId);
            let articles = pagedList.List
            let totalPages = pagedList.TotalPage
            let model: IndexModel =
                { CurrentIndex = page
                  TotalPages = totalPages
                  Articles = List.ofSeq (articles.AsEnumerable()) }
            return! htmlView (Index.view model) next ctx
        }

