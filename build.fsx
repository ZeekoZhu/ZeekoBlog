#load ".fake/build.fsx/intellisense.fsx"
open Fake.Core
open Fake.DotNet
open Fake.IO
open Fake.IO.FileSystemOperators
open Fake.IO.Globbing.Operators
open Fake.JavaScript

let yarnInstall workDir =
    Yarn.install (fun o -> { o with WorkingDirectory = workDir })

let yarnExec workDir command =
    Yarn.exec command (fun o -> { o with WorkingDirectory=workDir })

// Define general properties across various commands (with arguments)
let inline withWorkDir wd =
    DotNet.Options.withWorkingDirectory wd

// Default target
Target.create "Default" (fun _ ->
  Trace.trace "Hello World from FAKE"
)

Target.create "restore:yarn" (fun _ ->
    yarnInstall "./ZeekoBlog"
    yarnInstall "./ZeekoBlog.CodeHighlight/scripts"
)

Target.create "restore:dotnet" (fun _ ->
    !! "**/*.*proj"
    |> Seq.iter (DotNet.restore (fun o -> { o with ConfigFile = Some "./Nuget.Config" }))
)

Target.create "build:node" (fun _ ->
    yarnExec "./ZeekoBlog" "run build"
)

Target.create "build:dotnet" (fun _ ->
    [
        "./ZeekoBlog/ZeekoBlog.csproj"
        "./Tests/Tests.csproj"
    ]
    |> Seq.iter (DotNet.build id)
)

Target.create "test" (fun _ ->
    DotNet.test id "./Tests/Tests.csproj"
)

Target.create "publish" (fun _ ->
    DotNet.publish
        (fun o ->
            { o with
                OutputPath = Some "/app";
                Configuration = DotNet.BuildConfiguration.Release
            } |> withWorkDir "./ZeekoBlog"
        )
        "./ZeekoBlog.csproj"
)

open Fake.Core.TargetOperators

Target.create "restore" ignore

"restore:yarn" ==> "restore"
"restore:dotnet" ==> "restore"

"restore"
    ==> "build:node"
    ==> "build:dotnet"
    ==> "test"
    ==> "publish"


// start build
Target.runOrDefault "publish"
