namespace ZeekoBlog.LiveReload.Hub
open System
open System.Collections.Concurrent
open System.Threading.Channels
open Microsoft.AspNetCore.SignalR;
open FSharp.Control.Tasks.V2.ContextInsensitive
open ZeekoBlog.LiveReload
open FSharp.Control.Reactive

type FileWatcherHub() =
    inherit Hub()
    let subs = ConcurrentDictionary<Guid, IDisposable>()
    member public this.WatchFile(fileName: string) =
        use watcher = new FileWatcher(fileName)
        let channel = Channel.CreateUnbounded<string> ()
        let writeContent content =
            task {
                do! channel.Writer.WriteAsync content
            }
        let sub =
            watcher.Observe()
            |> Observable.flatmapTask writeContent
            |> Observable.subscribe ignore
        let id = Guid.NewGuid()
        subs.AddOrUpdate(id, sub, fun _ _ -> sub)
