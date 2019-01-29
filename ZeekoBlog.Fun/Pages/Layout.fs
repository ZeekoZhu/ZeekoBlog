module LayoutPage

open Common
open Giraffe
open GiraffeViewEngine
open System

type LayoutModel =
    { Title: string }

type LayoutSlot =
    { Styles: XmlNode list
      Body: XmlNode list
      Sidebar: XmlNode list
      Scripts: XmlNode list
    }

let linkStyle (href: string) =
    link [ _href href; _rel "stylesheet"]
    
let favicon icon =
    link [ _href icon
           _rel "icon"
           _type "image/png" ]

let favicons =
    [ 16; 32; 96; 160; 192 ]
    |> List.map
        ( (fun d -> sprintf "/assets/favicon-%dx%d.png" d d) 
            >> favicon
        )

let view (data: LayoutModel) (slots: LayoutSlot) =
        html []
            [ head []
                   (
                    [ meta [ _charset "utf-8" ]
                      meta [ _lang "zh" ]
                      meta [ _name "viewport"
                             _content "width=device-width, initial-scale=1.0" ]
                      title []  [ rawText (sprintf "%s - 网上冲浪指南" data.Title) ]
                      yandexTag
                      linkStyle "https://cdnjs.cloudflare.com/ajax/libs/normalize/8.0.0/normalize.min.css"
                      linkStyle "/dist/theme.css"
                      script [ _src "/dist/commons.js" ] []
                    ]
                    @ favicons
                    @ slots.Styles
                   )
              body []
                   ([ yandexNoScript
                      div [ _class "layout" ]
                          [ div [ _class "module" ] slots.Body 
                            div [ _class "side-container" ] slots.Sidebar
                          ]
                      div [ _class "footer" ]
                          [ span [ _class "copyright" ] [ rawText (sprintf "&copy; %d Zeeko" DateTime.Now.Year) ]
                            span [ _class "powered" ] [ rawText "Powered by ASP.NET Core 🐳" ]
                          ]
                      div [ _class "action-btn" ]
                          [ b [ _class "mdl2" ] [ rawText "&#xE700;" ] ]
                      script [ _src "/lib/zepto.min.js" ] []
                    ]
                    @ slots.Scripts
              )
        ]
