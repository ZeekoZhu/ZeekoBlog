module ErrorHandler

open Giraffe
open Microsoft.Extensions.Logging
open ErrorPage
open System

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
