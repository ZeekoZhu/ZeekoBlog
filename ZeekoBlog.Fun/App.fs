module App
open Giraffe




let webApp: HttpHandler =
    choose [
        GET >=>
            choose [
                route "/" >=> IndexHandler.handler
                routeCif "/a/%i" ArticleHandler.handler
                routeCif "/oops/%i" ErrorHandler.errorCodeHandler
            ]
    ]
