module DatabaseManager.Tables.Course

open ApplicationLibrary.Config
open ApplicationLibrary.Data.Database
open ApplicationLibrary.Data.WebScraping
open Microsoft.Extensions.DependencyInjection
open Microsoft.FSharp.Collections

open Spectre.Console

open ApplicationLibrary.Data.Entities


let private addCourse () : string =
    "addCourse"


let private removeCourse () : string =
   "removeCourse"
   

let private dropAll () : string =
    "dropAll"


let private replaceAllWith () : string = 
    let bulkWebScrapeIntoInsertion () : unit =
        try
            let webScraper = ConcordiaWebScraper("Config/files/concordia_urls.json")
            let webScrapedCourses = Seq.map (fun (rawHtml: string) -> webScraper.TransformToCourse(rawHtml)) (webScraper.ScrapeAll(52, 2)) 
            let courseTableManager = new CourseTableManager(AppSettings.DbConnectionString)
            courseTableManager.ReplaceAllWith(webScrapedCourses)
        with
            _ as ex ->AnsiConsole.WriteLine(ex.StackTrace)
        ()
    
    let options = ["1. Bulk web-scrape into insertion"]
    let selectionPrompt = SelectionPrompt<string>(Title = "Select where to fetch course data from:")
                              .AddChoices(options)
    let selectedOption = AnsiConsole.Prompt(selectionPrompt)
    match selectedOption with
    | "1. Bulk web-scrape into insertion" -> bulkWebScrapeIntoInsertion ()
    | _ -> ()
    "replaceAllWith"
    

let private getAllEntries () : string =
    "getAllEntries"
    
    
let private executeRawSql () : string =
    "executeRawSql"
    
    
let private actionFunctionMap = Map.ofList [
    ("[bold]Add[/] a course", addCourse);
    ("[bold]Remove[/] a course", removeCourse);
    ("[bold]Drop[/] all courses", dropAll);
    ("[bold]Replace all[/] with courses", replaceAllWith);
    ("[bold][/]Get all courses", getAllEntries)
    ("[bold][/][bold underline]Return[/]", (fun () -> "~END"))
]


/// <summary>
/// </summary>
/// <returns> False indicates exit, true otherwise. </returns>
let courseTableActions (tableName: string) (serviceProvider: ServiceProvider) : unit =
    
    let rec actionLoop () : unit =
        let prompt = "Select what action to perform on the table:"
        let selectionPrompt = SelectionPrompt<string>(Title = prompt)
                                  .AddChoices(Map.keys actionFunctionMap)
        let selectedAction = AnsiConsole.Prompt(selectionPrompt)
        
        match selectedAction with
        | "[bold][/][bold underline]Return[/]" ->
            ()
        | _ ->
            actionFunctionMap[selectedAction] () |> ignore
            actionLoop ()
        
    actionLoop ()