module ZeekoBlog.Fun.Manage.Pages.Index

open Giraffe.ViewEngine
open LayoutPage
open ZeekoBlog.Application.DTO
open ZeekoBlog.Fun.Manage.Pages

type IndexModel =
  { Articles: ArticleListDto list
    CurrentIndex: int
    TotalPages: int }


let scripts =
  [ script [ _src "/dist/zeeko.js" ] []
    script [] [
      rawText "window.__pageModule = 'list'"
    ] ]

let header =
  [ h1 [ _class "my-8" ] [
      rawText "今天要写点什么呢？"
    ] ]

let layoutData: LayoutModel =
  { Title = "管理" }

let renderArticle (dto: ArticleListDto) =
  div [ _class "row mb-3 flex gap-2 items-baseline" ] [
    a [ _class "title w-[60%] text-ellipsis"
        _title dto.Title
        _href $"/a/%d{dto.Id}" ] [
      rawText dto.Title
    ]
    span [ _class "actions flex gap-4 items-baseline" ] [
      a [ _class "link text-sm"
          _href $"/sudo/edit/%d{dto.Id}" ] [
        rawText "编辑"
      ]

      button [ _data "id" (dto.Id.ToString())
               _class $"renderAct {Style.btnClass}" ] [
        rawText "重新渲染"
      ]

      button [ _data "id" (dto.Id.ToString())
               _class $"deleteAct {Style.btnClass}" ] [
        rawText "删除"
      ]
    ]
  ]

let view (model: IndexModel) =
  let viewBody =
    div [ _class "w-full" ] [
      div [ _class "actions mb-4" ] [
        a [ _href "/sudo/edit"; _class "link" ] [
          rawText "新随笔"
        ]
      ]
      div [ _class "list" ] (model.Articles |> List.map renderArticle)
    ]

  { Scripts = scripts
    Header = header
    ModuleName = "sudo-module"
    Styles = [ emptyText ]
    Body = [ viewBody ]
    Sidebar = []
    Meta = [] }
  |> LayoutPage.view layoutData
