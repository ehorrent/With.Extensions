// include Fake lib
#r @"packages/FAKE/tools/FakeLib.dll"
open Fake

let packageVersion = "0.6.0"
let workingDir = "./.nuget"

// Build solution
Target "Build" (fun _ ->
    let setParams defaults =
      { defaults with
          Verbosity = Some(Minimal)
          Targets = ["Clean"; "Rebuild"]
          Properties = [("Configuration", "Release")]
      }

    build setParams "./With.Extensions.sln"
)

// Create nuget package
Target "CreatePackage" (fun _ ->
  CreateDir workingDir
  let setParams defaults =
    { defaults with
      OutputPath = workingDir
      Properties = [("Configuration", "Release")]
      Version = packageVersion
      WorkingDir = workingDir
    }

  NuGet setParams "./src/With.Extensions/With.Extensions.csproj"
)

// Dependencies
"Build"
  ==> "CreatePackage"

// start build
RunTargetOrDefault "Build"
