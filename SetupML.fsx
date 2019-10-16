open System
let path = Environment.GetEnvironmentVariable("path")
let current = __SOURCE_DIRECTORY__
let combine a b = IO.Path.GetFullPath(IO.Path.Combine(a,b))

let path' = 
    path 
    + ";" + combine current @".\packages\LightGBM\runtimes\win-x64\native" 
    + ";" + combine current @".\packages\Microsoft.ML\runtimes\win-x64\native" 
    + ";" + combine current @".\packages\Microsoft.ML.CpuMath\runtimes\win-x64\nativeassets\netstandard2.0" 
    + ";" + combine current @".\packages\Microsoft.ML.FastTree\runtimes\win-x64\native" 
    + ";" + combine current @".\packages\Microsoft.ML.Mkl.Components\runtimes\win-x64\native" 
    + ";" + combine current @".\packages\Microsoft.ML.Mkl.Redist\runtimes\win-x64\native" 
    + ";" + combine current @".\packages\Microsoft.ML.OnnxRuntime\runtimes\win-x64\native" 
    + ";" + combine current @".\packages\Microsoft.ML.Recommender\runtimes\win-x64\native" 
Environment.SetEnvironmentVariable("path",path')

#load @".paket\load\netstandard2.0\main.group.fsx"
