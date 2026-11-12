# Lift Pass Pricing C# API

Lift Pass Pricing Refactoring Kata.

Original: https://github.com/martinsson/Refactoring-Kata-Lift-Pass-Pricing

This is a .NET 10 project. All you need is the [.NET 10 SDK](https://dotnet.microsoft.com/download) — no database server.

The data lives in a SQLite file, `lift_pass.db`, which is copied fresh into the build output on every build
(so prices changed through `PUT /prices` are reset by the next build). Its schema and seed data are in
`initDatabase.sql`; rebuild the file with `sqlite3 lift_pass.db < initDatabase.sql` if you ever need to.

To install dependencies:

    dotnet restore

The API is built with [ASP.NET Core minimal APIs](https://learn.microsoft.com/aspnet/core/fundamentals/minimal-apis).
The project uses [xUnit](https://xunit.net/) and [WebApplicationFactory](https://learn.microsoft.com/aspnet/core/test/integration-tests) for testing.

To run the tests:

    dotnet test

To start the application:

    dotnet run --project LiftPassPricing

Then open http://localhost:5000/prices?type=1jour&age=23&date=2019-02-18
