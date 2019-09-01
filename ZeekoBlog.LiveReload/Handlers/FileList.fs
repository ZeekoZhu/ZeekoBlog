module ZeekoBlog.LiveReload.Handlers.FileList
open System
open System.IO
open System.Net
open Giraffe
open ZeekoBlog.LiveReload
open ZeekoBlog.LiveReload.Handlers

let parseDirResult (root: string) (dirResult: string list) : FileList_View.Model =
    let root = root.Replace("\\", "/")
    let urlPath (absPath: string) =
        "/files/" + absPath.Substring(root.Length).Replace("\\", "/").TrimStart('/')
    let children =
        match dirResult with
        | [] -> []
        | current :: children ->
            seq {
                if current = root then
                    yield "/", urlPath current
                else
                    yield "..", urlPath (Path.GetDirectoryName current)
                for child in children do
                    yield (Path.GetFileName child), urlPath child
            } |> List.ofSeq
    let current = dirResult.Head
    let path =
        current.Substring(root.Length).Split('/')
        |> List.ofArray
        |> List.fold (fun (state: (string * string) list * string) segment ->
                let arr, path = state
                let segment = if segment = "" then "/" else segment
                let path = path + (if segment = "/" then segment else segment + "/")
                [ yield! arr; yield (segment, path) ], path
            ) ([], "/files")
        |> fst
    { Path = path; Children = children }


let handler: HttpHandler =
    fun next ctx ->
        let fileProvider = ctx.GetService<FileProvider>()
        let fileListResult =
            fileProvider.GetFileList(ctx.Request.Path.ToString().Substring(6) |> WebUtility.UrlDecode)
        let fn =
            match fileListResult with
            | Error e -> RequestErrors.badRequest (text e)
            | Ok x ->
                match x with
                | Dir x -> Successful.ok ( x |> parseDirResult (Environment.CurrentDirectory) |> FileList_View.page |> htmlView)
                | File f -> Successful.ok (text f)
        fn next ctx
