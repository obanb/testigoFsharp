﻿namespace helloworld.Effects

open MongoDB.Bson
open MongoDB.Driver

module database =
   open MongoDB.Driver
   open helloworld.Issue
   let getInternalMongoClient () =
       let conn = MongoClient ("mongodb+srv://admin:clovek789@cluster0.6tq4i.mongodb.net/cloud?retryWrites=true&w=majority")
       conn.GetDatabase "cloud"

   let getIssues (dbClient : IMongoDatabase) =
       let col = dbClient.GetCollection<Issue> "issues"
       col.Find(Builders.Filter.Empty).ToEnumerable() |> List.ofSeq
              
   let getIssueById (dbClient: IMongoDatabase) (id:string) =
       let col = dbClient.GetCollection<Issue> "issues"
       let filter = Builders.Filter.Eq((fun issue -> issue.Id), id)
       let find = col.Find(filter).ToEnumerable() |> List.ofSeq
       let res = match find with
                 | [] -> None
                 | _ -> Some find.Head 
       res
       
   type CreateIssueInput = { name: string; content: string; desc: string }
   
   let prepareIssueDocument createIssueInput =
      let doc = { Id = ObjectId.GenerateNewId().ToString() ; name = createIssueInput.name; content = createIssueInput.content; desc = createIssueInput.desc }
        
      doc

   let createIssue (dbClient: IMongoDatabase) (input:CreateIssueInput) =
       let col = dbClient.GetCollection<Issue> "issues"
       let doc = input |> prepareIssueDocument
       col.InsertOne doc
    
       input
       
   let createIssue2 (dbClient: IMongoDatabase) (input: CreateIssueInput): Async<Result<CreateIssueInput,string>> =
       async {
         try
           let col = dbClient.GetCollection<Issue> "issues"
           let result = input
                        |> prepareIssueDocument
                        |> col.InsertOne
                         
           return Ok input
         with
           | e -> return Error (e.ToString())
    }
     