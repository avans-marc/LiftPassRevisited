using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using LiftPassPricing;

var builder = WebApplication.CreateBuilder(args);
builder.WebHost.UseUrls("http://localhost:5000");

var app = builder.Build();
app.MapPrices();

Console.WriteLine(@"LiftPassPricing Api started on 5000,
you can open http://localhost:5000/prices?type=night&age=23&date=2019-02-18 in a navigator
and you'll get the price of the list pass for the day.");
app.Run();
