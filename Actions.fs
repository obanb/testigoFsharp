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
