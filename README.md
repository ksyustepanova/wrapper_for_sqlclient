# Query

An ADO.NET wrapper for convenient access of SQL server database using c#

* [Description](#description)
* [Connection](#сonnection)
* [Stored procedures or query string](#stored-procedures-or-query-string)
* [Input parameters](#input-parameters)
* [Output](#output)

## Description

The solution contains two projects. there are "wrapper_for_sqlclient" and "wrapper_for_sqlclient.data". The first is the main thing in the decision. 
From there the solution starts. This is a webAPI project in which a connection to a database is configured, services, repositories and the wrapper itself are registered. 
The second project organizes all work with the database.

## Connection

The appsettings.json file in the main project specifies the database connection string.

```cs
"ConnectionStrings": {
    "DefaultConnection": "Data Source=server; Initial Catalog=database; User Id=user; Password=password"
  }
```

## Stored procedures or query string

Stored procedures are specified as follows:

```cs
await Sql.New("spSelectNews")
    .AddIn("id", id)
    .Select(NewsModelAdapter);
```

But you can also specify the query string in the code.

```cs
await Sql.New(@"SELECT id, title, body, date_create FROM News WHERE @id is null or id = @id ORDER BY id DESC", System.Data.CommandType.Text)
    .AddIn("id", id)
    .Select(NewsModelAdapter);
```

## Input parameters

Input parameters to stored procedures or queries are specified through the AddIn parameter.

```cs
await Sql.New("spSelectNews")
    .AddIn("id", id)
    .Select(NewsModelAdapter);
```

You can also pass the model as User-Defined Table Types described in the SqlEnumerableConverterFactory.

## Output

There are three types of output from a query such as:
* `Execute`: execute only, no data is returned;
* `Get`: get a string or value;
* `Select`: get multiple strings or values.
