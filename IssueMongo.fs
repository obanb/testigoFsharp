module helloworld.IssueMongo

open System.Collections.Generic
open MongoDB.Driver
open Microsoft.Extensions.DependencyInjection
open Issue


let find (collection : IMongoCollection<Issue>) (criteria : IssueFilter) : IEnumerable<Issue> =
  match criteria with
  | IssueFilter.All -> collection.Find(Builders.Filter.Empty).ToEnumerable()