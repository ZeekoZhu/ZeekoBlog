module ZeekoBlog.Fun.Manage.Handlers.LoginHandler
open Giraffe
open ZeekoBlog.Fun.Manage.Pages
open Microsoft.AspNetCore.Authentication
open FSharp.Control.Tasks

let handler : HttpHandler =
    fun next ctx ->
        task {
            let! result = ctx.AuthenticateAsync()
            if not result.Succeeded then
                return! htmlView Login.view next ctx
            else
                let returnUrl =
                    match ctx.TryGetQueryStringValue "ReturnUrl" with
                    | Some x -> x
                    | None -> "/sudo"
                return! redirectTo false returnUrl next ctx
        }
