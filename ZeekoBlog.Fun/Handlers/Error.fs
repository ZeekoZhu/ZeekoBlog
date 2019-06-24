module ErrorHandler

open Giraffe
open Microsoft.Extensions.Logging
open ErrorPage
open System
open System.Net

let handler (ex: Exception) (logger: ILogger) =
    logger.LogError(EventId(), ex, "An unhandled exception has occurred while executing the request.")
    let errorView =
        { Code = 500 }
        |> ErrorPage.view
        |> htmlView
    clearResponse >=> errorView

let errorCodeHandler (code: int) =
    let errorView =
        { Code = code }
        |> ErrorPage.view
        |> htmlView
    clearResponse >=> setStatusCode code >=> errorView

let redirectToLogin returnUrl =
    setStatusCode 401 >=> redirectTo false (sprintf "/sudo/login?ReutrnUrl=%s" (WebUtility.UrlEncode returnUrl))

let authFailedHander: HttpHandler =
    fun next ctx ->
        let path = ctx.Request.Path.ToString()
        redirectToLogin path next ctx
