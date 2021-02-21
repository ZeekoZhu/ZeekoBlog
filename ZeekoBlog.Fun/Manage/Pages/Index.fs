module ZeekoBlog.Fun.Manage.Pages.Index
open Giraffe.GiraffeViewEngine
open LayoutPage
open ZeekoBlog.Application.DTO

type IndexModel =
    { Articles: ArticleListDto list
      CurrentIndex: int
      TotalPages: int
    }


let scripts =
    [ script [_src "/dist/zeeko.js"] []
      script [] [ rawText "window.__pageModule = 'list'" ]
    ]

let header =
    [ h1 [] [ rawText "今天要写点什么呢？" ]
    ]

let layoutData: LayoutModel =
    { Title = "管理" }

let renderArticle (dto: ArticleListDto) =
    div [ _class "row" ]
        [ a [ _class "title"; _href (sprintf "/a/%d" dto.Id) ] [ rawText dto.Title ]
          span [ _class "actions" ]
               [ a [ _href (sprintf "/sudo/edit/%d" dto.Id) ] [ rawText "编辑" ]
                 a [ _data "id" (dto.Id.ToString()); _class "renderAct" ] [ rawText "重新渲染" ]
                 a [ _data "id" (dto.Id.ToString()); _class "deleteAct" ] [ rawText "删除" ]
               ]
        ]

let view (model: IndexModel) =
    let viewBody =
        div []
            [ div [ _class "actions" ]
                  [ a [ _href "/sudo/edit" ] [ rawText "新随笔" ] ]
              div [ _class "list" ]
                  ( model.Articles
                    |> List.map renderArticle
                  )
            ]
    { Scripts = scripts
      Header = header
      ModuleName = "sudo-module"
      Styles = [ emptyText ]
      Body = [ viewBody ]
      Sidebar = []
      Meta = []
    }
    |> LayoutPage.view layoutData
