module LayoutPage

open Common
open Giraffe.ViewEngine

type LayoutModel = { Title: string }

type LayoutSlot =
    { Styles: XmlNode list
      Header: XmlNode list
      Meta: XmlNode list
      Body: XmlNode list
      Sidebar: XmlNode list
      Scripts: XmlNode list
      ModuleName: string }

let linkStyle (href: string) = link [ _href href; _rel "stylesheet" ]

let favicon icon =
    link [ _href icon
           _rel "icon"
           _type "image/png" ]

let favicons =
    [ 16; 32; 96; 160; 192 ]
    |> List.map
        ((fun d -> sprintf "/assets/favicon-%dx%d.png" d d)
         >> favicon)

let view (data: LayoutModel) (slots: LayoutSlot) =
    html [ _lang "zh" ] [
        head
            []
            ([ meta [ _charset "utf-8" ]
               meta [ _lang "zh" ]
               meta [ _name "viewport"
                      _content "width=device-width, initial-scale=1.0" ]
               yield! slots.Meta
               title [] [
                   rawText (sprintf "%s - 网上冲浪指南" data.Title)
               ]
               yandexTag
               linkStyle "/dist/styles.css"
               linkStyle "/dist/rendered-content.css"
               yield! favicons
               yield! slots.Styles ])
        body
            [ _class "flex flex-col h-screen" ]
            ([ yandexNoScript
               div [ _class "z-container relative" ] slots.Header
               div [ _class "z-container flex-grow flex sm:flex-row flex-col flex-nowrap" ] [
                   div [ _class "module min-w-0" ] slots.Body
                   div [ _class "z-side-items" ] slots.Sidebar
               ]
               div [ _class "z-container font-mono text-sm mt-20 mb-10" ] [
                   div [] [
                       p [] [
                           rawText "本网站所展示的文章由 Zeeko Zhu 采用知识共享署名-相同方式共享 4.0 国际许可协议进行许可"
                       ]
                       p [] [
                           rawText "Zeeko's blog, Powered by ASP.NET Core 🐳"
                       ]
                   ]
               ]
               script [ _src "/lib/zepto.min.js" ] []
               yield! slots.Scripts ])
    ]
