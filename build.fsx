#r "paket:
nuget Fake.DotNet.Cli
nuget Fake.DotNet.Paket
nuget Fake.Core.Target
nuget Fake.Core.Process
nuget Fake.Core.String
nuget Fake.Core.ReleaseNotes
nuget Fake.IO.FileSystem
nuget Fake.Tools.Git
nuget Fake.JavaScript.Npm
//"
#load ".fake/build.fsx/intellisense.fsx"

#nowarn "52"

open Fake.Core
open Fake.Core.TargetOperators
open Fake.DotNet
open Fake.IO
open Fake.IO.Globbing.Operators
open Fake.JavaScript

let buildDir = "bin"
let destDir = "output"

let run cmd dir args =
    let result =
        CreateProcess.fromRawCommandLine cmd args
        |> CreateProcess.withWorkingDirectory dir
        |> Proc.run
    if result.ExitCode <> 0 then
        failwithf "Error while running '%s' with args: %s " cmd args

let platformTool tool =
    ProcessUtils.tryFindFileOnPath tool
    |> function Some t -> t | _ -> failwithf "%s not found" tool

let npxTool = platformTool "npx"
let npx args = run npxTool "./" args

Target.create "Clean" (fun _ ->
    !! "src/bin"
    ++ "src/obj"
    ++ "output"
    ++ "src/.fable"
    |> Seq.iter Shell.cleanDir

    !! "src/**/*fs.js"
    ++ "src/**/*fs.js.map"
    |> Seq.iter Shell.rm
)

Target.create "Install" (fun _ ->
    DotNet.restore
        (DotNet.Options.withWorkingDirectory __SOURCE_DIRECTORY__)
        "app.sln"
)

Target.create "JSInstall" (fun _ ->
    Npm.install id
)

Target.create "Build" (fun _ ->
    DotNet.exec id "fable" (sprintf "src --outDir %s" buildDir) |> ignore
)

Target.create "Watch" (fun _ ->
    DotNet.exec id "fable" (sprintf "watch src --outDir %s --define DEBUG" buildDir) |> ignore
)

Target.create "Bundle" (fun _ ->
   npx <| sprintf "rollup --file %s/app.js --format umd --name app %s/App.js" destDir buildDir
)

// Build order
"Clean"
    ==> "Install"
    ==> "JSInstall"
    ==> "Build"

"Build"
    ==> "Bundle"

"Watch"
    <== [ "JSInstall" ]

// start build
Target.runOrDefault "Build"
