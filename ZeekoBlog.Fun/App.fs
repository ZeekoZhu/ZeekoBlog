module App
open Giraffe
open ZeekoBlog.Fun
open ZeekoBlog
open ZeekoBlog.Fun.Handlers



let sudoArea: HttpHandler =
    subRouteCi "/sudo"
        ( requiresAuthentication (ErrorHandler.authFailedHander)
          >=>
           ( choose
               [ routex "/?" >=> (Manage.Handlers.IndexHandler.handler "zeeko")
                 routeCif "/edit/%i" (Some >> Manage.Handlers.EditHandler.hander)
                 routeCi "/edit" >=> (Manage.Handlers.EditHandler.hander None)
               ]
           )
        )

let webApp: HttpHandler =
    choose [
        GET >=>
            choose [
                route "/" >=> (IndexHandler.handler "zeeko")
                route "/feed" >=> (Feed.handler "zeeko")
                routeCif "/u/%s" IndexHandler.handler
                routeCif "/a/%i" ArticleHandler.handler
                routeCif "/oops/%i" ErrorHandler.errorCodeHandler
                routeCi "/sudo/login" >=> (Manage.Handlers.LoginHandler.handler)
                sudoArea
            ]
    ]
