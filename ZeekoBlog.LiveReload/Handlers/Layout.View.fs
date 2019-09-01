module ZeekoBlog.LiveReload.Handlers.Layout.View
open Giraffe
open GiraffeViewEngine

type LayoutModel =
    { Title: string
      Body: XmlNode list
    }
let layout (model: LayoutModel) =
    html [] [
        head [] [
            title [] [ str model.Title ]
        ]
        body [] model.Body
    ]
