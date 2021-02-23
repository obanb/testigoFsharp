module helloworld.Issue
open System.Collections.Generic
open MongoDB.Bson
open MongoDB.Bson.Serialization.Attributes
open Newtonsoft.Json

type Issue = {
  [<BsonId>]
  [<BsonRepresentation(BsonType.ObjectId)>]
  Id : string;
  desc: string;
  name: string;
  content: string;
}

 type IssueFilter =
     | All

 type IssueFind = IssueFilter -> IEnumerable<Issue>