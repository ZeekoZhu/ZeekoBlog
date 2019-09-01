module ZeekoBlog.LiveReload.Handlers.FileList_View
open Giraffe
open GiraffeViewEngine

type Model =
    { Path: (string * string) list
      Children: (string * string) list
    }

let private fileList (model: Model) =
    let path =
        model.Path |> List.map (fun (label, p) -> a [ _href p ] [ str label ])

    let fileLink (label: string, path: string) =
        li [] [ a [ _href path ] [ str label ] ]

    [ h1 [] ((span [] [ str "Path: " ]) :: path)
      ul [] (model.Children |> List.map fileLink)
    ]

let page x = Layout.View.layout {
    Title = "Live Reload"
    Body = fileList x
}
