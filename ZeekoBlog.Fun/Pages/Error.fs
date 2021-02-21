module ErrorPage

open Giraffe.GiraffeViewEngine
open LayoutPage

type ErrorModel =
    { Code: int }


let view (model: ErrorModel) =
    let layoutModel: LayoutModel =
        { Title = sprintf "%d" model.Code  }
    let viewBody =
        html []
             [ head []
                    [  meta [ _charset "utf-8" ]
                       meta [ _name "viewport"; _content "width=device-width, initial-scale=1.0" ]
                       meta [ _lang "zh" ]
                       title [] [ rawText (sprintf "Oops! - %d" model.Code) ]
                    ]
               body []
                    [ h1 []
                         [ rawText (sprintf "Oops，遭遇 %d ！" model.Code)
                           a [ _href "/" ] [ rawText "带我回去" ]
                         ]
                    ]
             ]
    let layoutSlot: LayoutSlot =
        { Scripts = []
          Styles = []
          Header = []
          ModuleName = "error-module"
          Body = [ viewBody ]
          Sidebar = []
          Meta = []
        }
    layoutSlot |> LayoutPage.view layoutModel
