open Fake.MyFakeTools

#load ".fake/build.fsx/intellisense.fsx"
open Fake.Core
open Fake.DotNet
open Fake.IO
open Fake.JavaScript
open Fake.MyFakeTools

module Docker =
    open System.IO
    let login () =
        let dockerPwd = Environment.environVar "DOCKER_PASSWORD"
        let dockerUsr = Environment.environVar "DOCKER_USERNAME"
        let input = StreamRef.Empty
        
        let proc =
            CreateProcess.fromRawCommand
                "docker" [ "login"; "-u"; dockerUsr; "--password-stdin" ]
            |> CreateProcess.withStandardInput (CreatePipe input)
            |> Utils.showOutput
            |> Proc.startRawSync
        use inputWriter = new StreamWriter(input.Value)
        inputWriter.WriteLine dockerPwd
        inputWriter.Close()
        proc.Result.Wait()

    let build () =
        Utils.dockerCmd "build" ["-t"; "hkccr.ccs.tencentyun.com/zeeko/blog-server:tmp"; "--build-arg"; "APPENV=Production"; "."]

module Cli =
    open CommandLine
    type DockerPublishOptions =
        { [<Option('t')>] Tag: string
          [<Option('n')>] ImageName: string
          [<Option('l')>] Latest: bool
          [<Option("suffix")>] Suffix: string
        }

    let handleDockerPublish (opt: DockerPublishOptions) =
        Docker.login ()
        let localImg = sprintf "%s:tmp" opt.ImageName
        let version = SemVer.parse opt.Tag
        let major = string version.Major
        let minor = major + "." + string version.Minor
        let patch = minor + "." + string version.Patch
        seq {
            yield major
            yield minor
            yield patch
            if opt.Latest then yield "latest"
        }
        |> Seq.map (fun t ->
             let tag = opt.ImageName + ":" + t
             if String.isNullOrEmpty opt.Suffix then tag
             else tag + "-" + opt.Suffix
           )
        |> Seq.iter (fun t ->
            Trace.tracefn "Pushing %s" t
            Utils.dockerCmd "tag" [ localImg; t ]
            Utils.dockerCmd "push" [ t ]
           )
        

let yarnInstall workDir =
    Trace.trace (sprintf "Yarn restoring: %s" workDir)
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
    yarnInstall "./ZeekoBlog.CodeHighlight/codehighlight-scripts"
    yarnInstall "./ZeekoBlog.Asciidoc/asciidoc-scripts"
)

Target.create "restore:dotnet" (fun _ ->
    Shell.Exec ("paket", "restore") |> ignore
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
                OutputPath = Some "./publish";
                Configuration = DotNet.BuildConfiguration.Release
            } |> withWorkDir "./ZeekoBlog"
        )
        "./ZeekoBlog.csproj"
)

open Fake.Core.TargetOperators

Target.useTriggerCI ()

Target.create "docker:publish"
    ( fun p -> Utils.handleCli p.Context.Arguments Cli.handleDockerPublish)

Target.create "docker:build"
    ( fun _ -> Docker.build ())

Target.create "restore" ignore

"restore:yarn" ==> "restore"
"restore:dotnet" ==> "restore"

"restore"
    ==> "build:node"
    ==> "build:dotnet"
    ==> "test"
    ==> "publish"

"publish"
    ==> "docker:build"
    ==> "docker:publish"



// start build
Target.runOrDefaultWithArguments "publish"
