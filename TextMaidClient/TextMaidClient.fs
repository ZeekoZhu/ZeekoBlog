namespace ZeekoBlog

open System
open FSharpx
open System.Threading.Tasks
open HttpRequestMessage
open Newtonsoft.Json.Serialization

module TextMaid =
    open Newtonsoft.Json
    open System.Net.Http
    type ToStringConverter() =
        inherit JsonConverter() with
        override __.CanConvert obj = true
        override __.WriteJson (writer, value, serializer) =
            writer.WriteValue(value.ToString())
        override __.CanRead with get() = false
        override __.ReadJson (_, _, _, _) =
            raise (NotImplementedException())
    let jsonSetting =
        let settings = JsonSerializerSettings()
        settings.ContractResolver <- CamelCasePropertyNamesContractResolver()
        settings
    let toJson (obj: 'a) =
        JsonConvert.SerializeObject(obj, jsonSetting)
    let decodeRespAsync<'a> (resp: HttpResponseMessage) =
        if resp.IsSuccessStatusCode then
            Task.task {
                let! content = resp.Content.ReadAsStringAsync()
                return JsonConvert.DeserializeObject<'a>(content) |> Some
            }
        else Task.FromResult None

    type TextDoc =
        { Source: string; Html: string }
    type TocItem =
        { Name: string; Level: int; Id: string }
    type RenderedDocument =
        { Doc: TextDoc; Toc: TocItem list; Language: string list }

    [<JsonConverter(typeof<ToStringConverter>)>]
    type TextType =
        | Markdown
        | AsciiDoc
        with override x.ToString() =
                match x with
                | Markdown -> "md"
                | AsciiDoc -> "adoc"

    type RenderOption =
        { CodeHighlight: bool; Math: bool }
    type RenderTextInput =
        { Text: string
          Type: TextType
          ExtraDocData: bool
          RenderOption: RenderOption
        }

    type TextMaidClient(client: HttpClient) =
        let renderText (input: RenderTextInput) =
            let data = input |> toJson
            requestBuilder {
                post "/api/text/render"
                json data
            }
            |> client.SendAsync
            |> Task.bind decodeRespAsync<RenderedDocument>

        member __.RenderText x = renderText x

