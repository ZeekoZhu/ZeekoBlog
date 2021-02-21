module ZeekoBlog.Fun.Manage.Pages.Login

open Giraffe.GiraffeViewEngine
open LayoutPage

let scripts =
    [ script [ _src "/dist/zeeko.js" ] []
      script [] [
          rawText "window.__pageModule = 'login'"
      ] ]

let header =
    [ h1 [ _class "font-bold mt-4 inline-block" ] [
        rawText "sudo rm -rf /"
      ] ]

let layoutData = { Title = "Sudo" }

let viewBody =
    div [] [
        div [ _class "mt-4" ] [
            label [ _for "userName"
                    _class "w-24 inline-block" ] [
                rawText "UserName"
            ]
            input [ _id "userName"
                    _class "border-2 border-blue-800 rounded-sm focus:ring outline-none" ]
        ]
        div [ _class "mt-4" ] [
            label [ _for "password"
                    _class "w-24 inline-block" ] [
                rawText "Password"
            ]
            input [ _id "password"
                    _class "border-2 border-blue-800 rounded-sm focus:ring outline-none"
                    _type "password" ]
        ]
        div [ _class "mt-4" ] [
            button [ _id "loginBtn"
                     _class "bg-blue-600 text-gray-100 rounded-sm py-1 px-4 hover:bg-blue-700 text-base uppercase" ] [
                rawText "sudo"
            ]
        ]
    ]

let view =
    { Scripts = scripts
      Header = header
      ModuleName = "login-module"
      Styles = [ emptyText ]
      Body = [ viewBody ]
      Meta = []
      Sidebar = [] }
    |> LayoutPage.view layoutData
