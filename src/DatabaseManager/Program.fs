module DatabaseManager.Program

open System
open System.Collections.Generic
open System.ComponentModel.DataAnnotations.Schema
open System.Reflection

open ApplicationLibrary.Config
open ApplicationLibrary.Data.Repositories
open ApplicationLibrary.Data.Repositories.Database
open Microsoft.Extensions.DependencyInjection
open Microsoft.FSharp.Collections

open Microsoft.FSharp.Core
open Spectre.Console

open ApplicationLibrary.Data.Entities

let applicationDependencies =
    ServiceCollection()
        .AddSingleton<IRepository<Course>>(fun _ -> new CourseRepositoryDatabase(AppSettings.DbConnectionString) : IRepository<Course>)
        .BuildServiceProvider()

/// <summary> A dictionary containing entity class types and their respective action function to be executed. </summary>
let private actionToFunctionMap =
    Map.ofList [
        ("Course", (fun () -> Tables.Course.courseTableActions "Course" applicationDependencies))
        ("[bold]Quit[/]", (fun () -> ()))
    ]
    

[<EntryPoint>]
let main argv =
    let rec mainMenuLoop () : unit =
        let prompt = "Select what table in the database:"
        let selectionPrompt = SelectionPrompt<string>(Title = prompt)
                                  .AddChoices(Map.keys actionToFunctionMap)
                                  
        let selectedAction = AnsiConsole.Prompt(selectionPrompt)
        match selectedAction with
        | "[bold]Quit[/]" ->
            ()  
        | _ ->
            actionToFunctionMap[selectedAction] ()
            mainMenuLoop ()
        
    mainMenuLoop ()
    0