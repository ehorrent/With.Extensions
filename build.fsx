// include Fake lib
#r @"packages/FAKE/tools/FakeLib.dll"
open Fake

// NuGet settings
let nugetPackageVersion = "0.6.0"
let nugetToolPath = "./.nuget"
let nugetPackageDir = "./.nuget/nupkg"

// NUnit settings
let nunitToolPath = "./packages/NUnit.Runners/tools"
let nunitOutputPath =  "./reports/TestResults.xml"

// Build solution
Target "Build" (fun _ ->
    let setParams defaults =
      { defaults with
          Verbosity = Some(Minimal)
          Targets = ["Clean"; "Rebuild"]
          Properties =
          [
            "Optimize", "True"
            "DebugSymbols", "False"
            "Configuration", "Release"
          ]
      }

    build setParams "./With.Extensions.sln"
)

// Unit tests
Target "NUnitTest" (fun _ ->
  let setParams defaults =
    { defaults with
      ToolPath = nunitToolPath
      Framework = "4.5"
      DisableShadowCopy = false
      OutputFile = nunitOutputPath
    }

  !!("./src/**/bin/release/*.Tests.dll")
    |> NUnit setParams
)

// Create nuget package
Target "CreatePackage" (fun _ ->
  CreateDir nugetPackageDir
  let setParams defaults =
    { defaults with
      OutputPath = nugetPackageDir
      Properties = [("Configuration", "Release")]
      Version = nugetPackageVersion
      WorkingDir = nugetPackageDir
    }

  NuGetPack setParams "./src/With.Extensions/With.Extensions.csproj"
)

// Dependencies
"Build"
  ==> "NUnitTest"
  ==> "CreatePackage"

// start build
RunTargetOrDefault "Build"
