open System.IO
#load ".fake/build.fsx/intellisense.fsx"

open Fake.MyFakeTools
open Fake.Core
open Fake.DotNet
open Fake.IO
open Fake.JavaScript
open Fake.BuildServer

BuildServer.install [ GitHubActions.Installer ]

module GitHubActions =
    let getTag () =
        let tag = (Environment.environVar "GITHUB_REF").Substring(10)
        seq {
            tag
            "latest"
        }
        |> Seq.map (sprintf "blog-server/%s")
        |> String.concat ","
    let setupEnv () =
        let githubEnv = Environment.environVar "GITHUB_ENV"
        Trace.tracefn "DOCKER_TAGS=%s" (getTag())
        File.append githubEnv [ sprintf "DOCKER_TAGS=%s" (getTag()) ]

let npmInstall workDir =
    Trace.trace (sprintf "Npm restoring: %s" workDir)
    Npm.install (fun o -> { o with WorkingDirectory = workDir })

let npmExec workDir command =
    Npm.exec command (fun o -> { o with WorkingDirectory=workDir })

// Define general properties across various commands (with arguments)
let inline withWorkDir wd =
    DotNet.Options.withWorkingDirectory wd

// Default target
Target.create "Default" (fun _ ->
  Trace.trace "Hello World from FAKE"
)

Target.create "restore:npm" (fun _ ->
    Shell.pushd "./ZeekoBlog"
    if Shell.Exec ("npm", "install") <> 0 then failwith "npm install failed"
    Shell.Exec ("npm","-v") |> ignore
    Shell.Exec ("node","-v") |> ignore
    Shell.Exec ("ls"," -al node_modules/.bin") |> ignore
    Shell.Exec ("ls"," -al node_modules/") |> ignore
    Shell.popd ()
)

Target.create "build:node" (fun _ ->
    Shell.pushd "./ZeekoBlog"
    let webpack = "./node_modules/.bin/webpack" |> Path.getFullName
    webpack |> Trace.tracefn "webpack cli path: %A"
    Shell.Exec ("ls"," -al node_modules/.bin") |> ignore
    Shell.Exec ("ls"," -al node_modules/") |> ignore
    if Shell.Exec webpack <> 0 then failwith "npm build failed"
    Shell.popd ()
)

Target.create "build:dotnet" (fun _ ->
    [
        "./ZeekoBlog/ZeekoBlog.csproj"
    ]
    |> Seq.iter (DotNet.build id)
)

Target.create "test" (fun _ ->
//    DotNet.test id "./Tests/Tests.csproj"
    ()
)

Target.create "publish" (fun _ ->
    DotNet.publish
        (fun o ->
            { o with
                OutputPath = Some "./artifacts";
                Configuration = DotNet.BuildConfiguration.Release
            } |> withWorkDir "./ZeekoBlog"
        )
        "./ZeekoBlog.csproj"
)

open Fake.Core.TargetOperators

Target.useTriggerCI ()

Target.create "restore" ignore

"restore:npm" ==> "restore"

"restore"
    ==> "build:node"
    ==> "build:dotnet"
    ==> "test"
    ==> "publish"

Target.create "github-actions" (fun _ ->
        GitHubActions.setupEnv()
    )

Environment.setEnvironVar "NODE_ENV" "production"
Environment.setEnvironVar "ASPNETCORE_ENVIRONMENT" "production"

// start build
Target.runOrDefaultWithArguments "publish"
