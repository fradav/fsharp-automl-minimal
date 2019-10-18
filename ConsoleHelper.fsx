// #load "SetupML.fsx"

namespace MLCommon

// type TableOutput = 
//     {
//         Columns: array<string>;
//         Rows: array<array<string>>;
//     }

module ConsoleHelper =
    open System
    open Microsoft.ML
    open Microsoft.ML.Data
    //open Microsoft.ML.Api
    open System.Reflection
    open IfSharp.Kernel

    let peekDataViewInConsole<'TObservation when 'TObservation : (new : unit -> 'TObservation) and 'TObservation : not struct> (mlContext : MLContext) (dataView : IDataView) (pipeline : IEstimator<ITransformer>) numberOfRows =
        
        //https://github.com/dotnet/machinelearning/blob/master/docs/code/MlNetCookBook.md#how-do-i-look-at-the-intermediate-data
        let transformer = pipeline.Fit dataView
        let transformedData = transformer.Transform dataView

        // 'transformedData' is a 'promise' of data, lazy-loading. call Preview  
        //and iterate through the returned collection from preview.

        let columns =
            transformedData.Preview(1).RowView
            |> Seq.head 
            |> (fun row ->
                    row.Values
                    |> Array.map (function KeyValue(k,v) -> k))
        let rows =
            transformedData.Preview(numberOfRows).RowView
            |> Seq.map
                (fun row -> 
                    row.Values
                    |> Array.map (function KeyValue(k,v) -> v.ToString()))
            |> Seq.toArray
        
        { Columns = columns; Rows = rows }

    let peekVectorColumnDataInConsole (mlContext : MLContext) (columnName: string) (dataView : IDataView) (pipeline : IEstimator<ITransformer>) numberOfRows =

        let transformer = pipeline.Fit dataView
        let transformedData = transformer.Transform dataView

        // Extract the 'Features' column.
        let someColumnData = 
            transformedData.GetColumn<float32[]>(columnName)
            |> Seq.take numberOfRows
            |> Seq.toList

        // print to console the peeked rows
        someColumnData
        |> List.map(fun row -> 
            let concatColumn = 
                row
                |> Array.map string
                |> String.concat ""
            sprintf "%s" concatColumn
        )
                        
        // someColumnData

    let printBinaryClassificationMetrics name (metrics : CalibratedBinaryClassificationMetrics) =
        printfn"************************************************************"
        printfn"*       Metrics for %s binary classification model      " name
        printfn"*-----------------------------------------------------------"
        printfn"*       Accuracy: %.2f%%" (metrics.Accuracy * 100.)
        printfn"*       Area Under Curve:      %.2f%%" (metrics.AreaUnderRocCurve * 100.)
        printfn"*       Area under Precision recall Curve:    %.2f%%" (metrics.AreaUnderPrecisionRecallCurve * 100.)
        printfn"*       F1Score:  %.2f%%" (metrics.F1Score * 100.)

        printfn"*       LogLogg:  %.2f%%" (metrics.LogLoss)
        printfn"*       LogLossreduction:  %.2f%%" (metrics.LogLossReduction)
        printfn"*       PositivePrecision:      %.2f" (metrics.PositivePrecision)
        printfn"*       PositiveRecall:      %.2f" (metrics.PositiveRecall)
        printfn"*       NegativePrecision:      %.2f" (metrics.NegativePrecision)
        printfn"*       NegativeRecall:      %.2f" (metrics.NegativeRecall)
        printfn"************************************************************"