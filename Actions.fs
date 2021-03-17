namespace helloworld.Effects

open System.ComponentModel.DataAnnotations
open FSharpPlus.Control
open helloworld

module actions =
    open helloworld.Effects.database
    open Suave
    open MongoDB.Bson
    open MongoDB.Bson.Serialization.Attributes
    open MongoDB.Driver
    open Issue
    open Newtonsoft.Json
    open System

    let getIssueList dbClient httpContext =
        let issues = getIssues dbClient
                    
        let res = match issues with
                  | [] -> "Empty issue list"
                  | _ -> JsonConvert.SerializeObject issues
                  
        Successful.OK res httpContext
        
    let getIssue id dbClient httpContext =
        let issue = getIssueById dbClient id
        
        let res = match issue with
                  | Some iss -> JsonConvert.SerializeObject iss
                  | None -> "Issue not fond"
            
        Successful.OK res httpContext    

    let userRequestToUser (issue: CreateIssueInput): Issue = 
        {
            name = issue.name;
            content = issue.content;
            desc = issue.desc
            Id = ""
         }
           
    let createIssue dbClient  =
        request (fun req ->
            Newtonsoft.Json.JsonConvert.DeserializeObject<CreateIssueInput>(req.rawForm |> System.Text.ASCIIEncoding.UTF8.GetString)
            |> createIssue dbClient
            |> userRequestToUser
            |> JsonConvert.SerializeObject
            |> Successful.OK
        )
        
    let deserializeRequest request =
        Newtonsoft.Json.JsonConvert.DeserializeObject<CreateIssueInput>(request.rawForm |> System.Text.ASCIIEncoding.UTF8.GetString)
    let validateRequired prop propName =
        match prop with
              | "" -> Error ("Property " + propName + " is required.")
              | _ -> Ok prop
    
    let partial f x y = f(x, y)
    let f = ["a";"b"] |> partial String.Join "fe"
    
    let validateCreateIssueInput (input: CreateIssueInput) =
         let validationResults = [validateRequired input.name "name"; validateRequired input.desc "desc"] |> List.choose (fun res ->
             match res with
                   | Error msg -> Some msg
                   | Ok _ -> None)
                                |> (fun filtered -> match filtered with
                                        | [] -> Ok input
                                        | _ ->  (Environment.NewLine, filtered) |> String.Join |> Error)
         
         validationResults
 
    let createIssue2 dbClient httpContext =
        async {
             let result = deserializeRequest httpContext.request
                          |> validateCreateIssueInput
                          |> Result.bind (fun res -> createIssue2 dbClient res |> Async.RunSynchronously)
                          
             return match result with
                  | Error e -> Successful.OK e httpContext
                  | Ok issue -> issue
                                |> userRequestToUser
                                |> JsonConvert.SerializeObject
                                |> (fun x -> Successful.OK x httpContext)    
        }
        
   
