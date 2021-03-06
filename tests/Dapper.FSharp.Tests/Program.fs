﻿module Dapper.FSharp.Tests.Program

open Expecto
open Expecto.Logging
open Microsoft.Data.SqlClient
open Microsoft.Extensions.Configuration
open MySql.Data.MySqlClient
open Npgsql

let testConfig = 
    { Expecto.Tests.defaultConfig with 
        parallelWorkers = 4
        verbosity = LogLevel.Debug }

let mssqlTests connString =
    let mssql = new SqlConnection(connString)
    mssql |> Dapper.FSharp.Tests.MSSQL.Database.init
    [
        MSSQL.InsertTests.tests mssql
        MSSQL.UpdateTests.tests mssql
        MSSQL.DeleteTests.tests mssql
        MSSQL.SelectTests.tests mssql
        MSSQL.IssuesTests.tests mssql
    ]
    |> Tests.testList "MSSQL"
    |> Tests.testSequenced

let mysqlTests connString =
    let mysql = new MySqlConnection(connString)
    mysql |> Dapper.FSharp.Tests.MySQL.Database.init
    [
        MySQL.InsertTests.tests mysql
        MySQL.UpdateTests.tests mysql
        MySQL.DeleteTests.tests mysql
        MySQL.SelectTests.tests mysql
        MySQL.IssuesTests.tests mysql
    ]
    |> Tests.testList "MySQL"
    |> Tests.testSequenced

let postgresTests connString =
    let postgres = new NpgsqlConnection(connString)
    postgres |> Dapper.FSharp.Tests.PostgreSQL.Database.init
    [
        PostgreSQL.InsertTests.tests postgres
        PostgreSQL.UpdateTests.tests postgres
        PostgreSQL.DeleteTests.tests postgres
        PostgreSQL.SelectTests.tests postgres
        PostgreSQL.IssuesTests.tests postgres
    ]
    |> Tests.testList "PostgreSQL"
    |> Tests.testSequenced

[<EntryPoint>]
let main _ =
    let conf = (ConfigurationBuilder()).AddJsonFile("local.settings.json").Build()
    
    Dapper.FSharp.OptionTypes.register()
    
    [
        conf.["mssqlConnectionString"] |> mssqlTests
        conf.["mysqlConnectionString"] |> mysqlTests
        conf.["postgresConnectionString"] |> postgresTests
    ]
    |> Tests.testList ""
    |> Tests.runTests testConfig