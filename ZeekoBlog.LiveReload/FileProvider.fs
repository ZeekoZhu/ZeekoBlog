namespace ZeekoBlog.LiveReload
open System
open System.IO
open System.Reactive.Linq

module internal Utils =
    let (</>) (a: string) (b: string) =
        let a = a.TrimEnd (Path.DirectorySeparatorChar)
        let b = b.TrimStart (Path.DirectorySeparatorChar)
        Path.Combine(a, b)
    let cwd = Environment.CurrentDirectory

open Utils

type FileListResult =
    | Dir of string list
    | File of string

type FileWatcher(path: string) =
    let path = cwd </> path
    let dirPath = Path.GetDirectoryName path
    let fileName = Path.GetFileName path
    let watcher = new FileSystemWatcher(dirPath)
    do watcher.Filter <- fileName
    do watcher.NotifyFilter <- NotifyFilters.LastWrite

    let observeFile (watcher: FileSystemWatcher) =
        Observable.FromEventPattern<FileSystemEventHandler, FileSystemEventArgs>
            (watcher.Changed.AddHandler, watcher.Changed.RemoveHandler)
        |> Observable.map (fun x -> x.EventArgs)
        |> Observable.map (fun _ -> File.ReadAllText path)

    let event = observeFile watcher
    interface IDisposable with
        member this.Dispose() =
            watcher.Dispose()
    member __.Observe () =
        watcher.EnableRaisingEvents <- true
        event


type FileProvider() =

    let getDirContent path =
        let subDirs = Directory.GetDirectories path |> List.ofArray
        let files = Directory.GetFiles path |> List.ofArray
        path :: subDirs @ files

    let getFileContent path =
        File.ReadAllText path

    let getFileListResult relativePath =
        let path = (cwd </> relativePath) |> Path.GetFullPath
        try
            match path with
            | x when (Directory.Exists(path)) -> x |> getDirContent |> Dir |> Ok
            | x when (File.Exists(path)) -> x |> getFileContent |> File |> Ok
            | _ -> Error(sprintf "Can not open: %s" path)
        with
            | _ -> Error(sprintf "Can not open: %s" path)

    member public this.GetFileList(relativePath: string) =
        getFileListResult (relativePath.TrimEnd('/'))
