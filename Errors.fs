module helloworld.Errors

type ApplicationError =
    | NetworkError
    | Non200Response
    | DatabaseError of string
    | ParseError of string