namespace ZeekoBlog.Fun
open Microsoft.Extensions.DependencyInjection
open Giraffe;
open Microsoft.AspNetCore.Builder
open App

[<System.Runtime.CompilerServices.Extension>]
module AppExtensions =
    [<System.Runtime.CompilerServices.Extension>]
    let AddZeekoBlogFun (services: IServiceCollection) =
        services.AddGiraffe()

    [<System.Runtime.CompilerServices.Extension>]
    let UseZeekoBlogFun (app : IApplicationBuilder) =
        app.UseGiraffe webApp
        app.UseGiraffeErrorHandler ErrorHandler.handler
