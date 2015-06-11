// include Fake lib
#r @"packages\FAKE\tools\FakeLib.dll"

open Fake
open Fake.AssemblyInfoFile

// Directories
let buildDir  = @".\build\"
let packagesDir = @".\deploy\"

let authors = ["Andrea Magnorsky"; "Andrew O'Connor"; "Dean Ellis";]

// project name and description
let projectName = "Android.Duality.Farseer"
let projectDescription = "Farseer for Duality on Android"
let projectSummary = projectDescription // TODO: write a summary

let version = "0.1."+ buildVersion  


// Targets
Target "Clean" (fun _ ->
    CleanDirs [buildDir; packagesDir] 
)

Target "SetVersions" (fun _ ->
    CreateCSharpAssemblyInfo "./Properties/AssemblyInfo.cs"
        [Attribute.Title projectName
         Attribute.Description projectDescription
         Attribute.Product projectSummary
         Attribute.Version version
         Attribute.FileVersion version]
)

Target "RestorePackages" (fun _ ->
    !! "./**/packages.config"
        |> Seq.iter (RestorePackage (fun p ->
            { p with Sources = ["https://www.myget.org/F/6416d9912a7c4d46bc983870fb440d25/"]}))
)

Target "Build" (fun _ ->          
    let buildMode = getBuildParamOrDefault "buildMode" "Release"
    let setParams defaults =
        { defaults with
            Verbosity = Some(Normal)            
            Properties =
                [            
                    "Configuration", buildMode                    
                ]
        }
    build setParams "./FarseerDuality.sln"    
    |> DoNothing  
)

Target "AndroidPack" (fun _ ->         
    NuGet (fun p -> 
        {p with 
            Authors = authors
            Project = projectName
            Description = projectDescription      
            Version = "0.1."+ buildVersion                                            
            PublishUrl = getBuildParamOrDefault "nugetrepo" ""
            AccessKey = getBuildParamOrDefault "keyfornuget" ""
            Publish = hasBuildParam "nugetrepo"
            WorkingDir = @".\"
            OutputPath = packagesDir
        }) "nuget/farseerduality.Android.nuspec"
)

// Dependencies
"Clean"    
  ==> "SetVersions"
  ==> "RestorePackages"
  ==> "Build"    
  ==> "AndroidPack"
  


RunTargetOrDefault "AndroidPack"