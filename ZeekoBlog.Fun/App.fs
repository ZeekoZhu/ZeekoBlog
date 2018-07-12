module App
open Giraffe




let webApp: HttpHandler =
    choose [
        route "/" >=> GET >=> IndexHandler.handler
        GET >=>  routef "/a/%i" ArticleHandler.handler
    ]
