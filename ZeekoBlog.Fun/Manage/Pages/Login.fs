module ZeekoBlog.Fun.Manage.Pages.Login
open Giraffe.GiraffeViewEngine
open LayoutPage

let scripts =
    [ script [_src "/dist/zeeko.js"] []
      script [] [ rawText "window.__pageModule = 'login'" ]
    ]

let header =
    [ h1 [] [ rawText "sudo rm -rf /" ]
    ]

let layoutData =
    { Title = "Sudo" }

let viewBody =
    div []
        [ div [ _class "form" ]
              [ div [ _class "control" ]
                    [ label [ _for "userName" ] [ rawText "UserName" ]
                      input [ _id "userName" ]
                    ]
                div [ _class "control" ]
                    [ label [ _for "password" ] [ rawText "Password" ]
                      input [ _id "password"; _type "password" ]
                    ]
                div [ _class "control" ]
                    [ button [ _id "loginBtn" ] [ rawText "sudo" ] ]
              ]
        ]

let view =
    { Scripts = scripts
      Header = header 
      ModuleName = "login-module"
      Styles = [ emptyText ]
      Body = [ viewBody ]
      Sidebar = []
    }
    |> LayoutPage.view layoutData
    
