module helloworld.tests.config

open Expecto

let runExpecto args test =
    runTestsWithCLIArgs [] args test