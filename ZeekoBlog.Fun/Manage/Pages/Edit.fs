module ZeekoBlog.Fun.Manage.Pages.Edit
open ZeekoBlog.Core.Models
open Giraffe.GiraffeViewEngine
open LayoutPage

type EditModel =
    { Article: Article option
    }

let scripts =
    [ script [_src "/dist/zeeko.js"] []
      script [] [ rawText "window.__pageModule = 'edit'" ]
    ]

let header =
    [ h1 [] [ rawText "今天要写点什么呢？" ]
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
            [ div [ _class "actions" ]
                  [ a [ _href "/sudo/" ] [ rawText "返回列表" ] ]
              input [ _hidden; _id "id"; _data "id" (model.Id.ToString()) ]
              div [ _class "form" ]
                  [ div [ _class "field" ]
                        [
                            label [ _for "title" ] [ rawText "标题" ]
                            input [ _class "control"; _id "title"; _value model.Title ]
                        ]
                    div [ _class "field" ]
                        [
                            label [ _for "summary" ] [ rawText "摘要" ]
                            textarea [ _class "control"; _id "summary" ] [ rawText model.Summary ]
                        ]
                    div [ _class "field" ]
                        [
                            label [ _for "docType" ] [ rawText "类型" ]
                            select [ _class "control"; _id "docType"; _value (model.DocType |> int |> string) ]
                                   [ selectedDocType ArticleDocType.Markdown
                                     selectedDocType ArticleDocType.AsciiDoc
                                     selectedDocType ArticleDocType.Raw
                                   ]
                        ]
                    div [ _class "field" ]
                        [
                            label [ _for "content" ] [ rawText "正文" ]
                            textarea [ _class "control"; _id "content" ] [ rawText model.Content ]
                        ]
                    div [ _class "field" ]
                        [
                            label [ _id "tip" ] []
                            button [ _class "control"; _id "save" ] [ rawText "保存" ]
                        ]
                  ]
            ]
    { Scripts = scripts
      Header = header 
      ModuleName = "edit-module"
      Styles = [ emptyText ]
      Body = [ viewBody ]
      Sidebar = []
    }
    |> LayoutPage.view layoutData
