module ZeekoBlog.Fun.Manage.Pages.Edit
open ZeekoBlog.Core.Models
open Giraffe.ViewEngine
open LayoutPage

type EditModel =
    { Article: Article option
    }

let scripts =
    [ script [_src "/dist/zeeko.js"] []
      script [] [ rawText "window.__pageModule = 'edit'" ]
    ]

let header =
    [ h1 [ _class "my-8" ] [ rawText "今天要写点什么呢？" ]
    ]

let layoutData: LayoutModel =
    { Title = "管理" }

let view (model: EditModel) =
    let model =
        match model.Article with
        | Some x -> x
        | None ->
            Article(Id = 0, Title = "", Summary = "", Content = "", DocType = ArticleDocType.AsciiDoc )
    let selectedDocType (docType: ArticleDocType) =
        let text = rawText (docType.ToString())
        let value = _value (docType |> int |> string)
        if model.DocType = docType then
            option [ value; _selected ] [ text ]
        else option [ value; ] [ text ]
    let viewBody =
        div []
            [ div [ _class "actions mb-4" ]
                  [ a [ _href "/sudo/"; _class "link" ] [ rawText "返回列表" ] ]
              input [ _hidden; _id "id"; _data "id" (model.Id.ToString()) ]
              let fieldCls = "flex flex-col gap-2"
              let inputCls = "px-2 py-1 border border-slate-200 rounded focus:border-gray-600 focus:outline-none"
              div [ _class "form flex flex-col gap-4" ]
                  [ div [ _class $"field {fieldCls}" ]
                        [
                            label [ _for "title" ] [ rawText "标题" ]
                            input [ _class $"{inputCls} control"; _id "title"; _value model.Title ]
                        ]
                    div [ _class $"field {fieldCls}" ]
                        [
                            label [ _for "summary" ] [ rawText "摘要" ]
                            textarea [ _class $"{inputCls} control"; _id "summary" ] [ rawText model.Summary ]
                        ]
                    div [ _class $"field {fieldCls}" ]
                        [
                            label [ _for "docType" ] [ rawText "类型" ]
                            select [ _class $"{inputCls} control"; _id "docType"; _value (model.DocType |> int |> string) ]
                                   [ selectedDocType ArticleDocType.Markdown
                                     selectedDocType ArticleDocType.AsciiDoc
                                     selectedDocType ArticleDocType.Raw
                                   ]
                        ]
                    div [ _class $"field {fieldCls}" ]
                        [
                            label [ _for "content" ] [ rawText "正文" ]
                            textarea [ _class $"{inputCls} control min-h-[200px]"; _id "content" ] [ rawText model.Content ]
                        ]
                    div [ _class $"field {fieldCls}" ]
                        [
                            label [ _id "tip" ] []
                            button [ _class $"control {Style.btnClass}"; _id "save" ] [ rawText "保存" ]
                        ]
                  ]
            ]
    { Scripts = scripts
      Header = header
      ModuleName = "edit-module"
      Styles = [ emptyText ]
      Body = [ viewBody ]
      Sidebar = []
      Meta = []
    }
    |> LayoutPage.view layoutData
