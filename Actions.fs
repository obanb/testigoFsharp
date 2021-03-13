namespace helloworld.Effects

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
        
    let createIssue2 dbClient httpContext =
        async {
             let! result = Newtonsoft.Json.JsonConvert.DeserializeObject<CreateIssueInput>(httpContext.request.rawForm |> System.Text.ASCIIEncoding.UTF8.GetString)
                          |> createIssue2 dbClient
                          
             return match result with
                  | Error _ -> Successful.NO_CONTENT httpContext
                  | Ok issue -> issue
                                |> userRequestToUser
                                |> JsonConvert.SerializeObject
                                |> (fun x -> Successful.OK x httpContext)    
        }
        
   
