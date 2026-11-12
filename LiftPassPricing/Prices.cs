using System;
using System.Collections;
using System.Data;
using System.Globalization;
using System.IO;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Data.Sqlite;

namespace LiftPassPricing
{
    public static class Prices
    {

        private static readonly string ConnectionString =
            "Data Source=" + Path.Combine(AppContext.BaseDirectory, "lift_pass.db") + ";Mode=ReadWrite";

        public static void MapPrices(this IEndpointRouteBuilder app)
        {
            var prices = app.MapGroup("/prices");

            prices.MapPut("", (int cost, string type) =>
            {
                using var connection = new SqliteConnection(ConnectionString);
                connection.Open();

                int liftPassCost = cost;
                string liftPassType = type;

                using (var command = new SqliteCommand( //
                       "INSERT INTO base_price (type, cost) VALUES (@type, @cost) " + //
                       "ON CONFLICT(type) DO UPDATE SET cost = @cost;", connection))
                {
                    command.Parameters.AddWithValue("@type", liftPassType);
                    command.Parameters.AddWithValue("@cost", liftPassCost);
                    command.Prepare();
                    command.ExecuteNonQuery();
                }

                return "";
            });

            prices.MapGet("", (string type, int? age, string date) =>
            {
                using var connection = new SqliteConnection(ConnectionString);
                connection.Open();

                double basePrice;
                using (var costCmd = new SqliteCommand( //
                    "SELECT cost FROM base_price " + //
                    "WHERE type = @type", connection))
                {
                    costCmd.Parameters.AddWithValue("@type", type);
                    costCmd.Prepare();
                    basePrice = (long)costCmd.ExecuteScalar();
                }

                if (age != null && age < 6)
                {
                    return "{ \"cost\": 0}";
                }
                else
                {

                    if (!"night".Equals(type))
                    {
                        var holidaysDates = new ArrayList();
                        using (var holidayCmd = new SqliteCommand( //
                            "SELECT * FROM holidays", connection))
                        {
                            holidayCmd.Prepare();
                            using (var holidays = holidayCmd.ExecuteReader())
                            {
                                while (holidays.Read())
                                {
                                    holidaysDates.Add(holidays.GetDateTime("holiday"));
                                }
                            }
                        }

                        int reduction = 0;
                        var isHoliday = false;
                        foreach (DateTime holiday in holidaysDates)
                        {
                            if (date != null)
                            {
                                DateTime d = DateTime.ParseExact(date, "yyyy-MM-dd", CultureInfo.InvariantCulture);
                                if (d.Year == holiday.Year &&
                                    d.Month == holiday.Month &&
                                    d.Date == holiday.Date)
                                {
                                    isHoliday = true;
                                }
                            }
                        }

                        if (date != null)
                        {
                            DateTime d = DateTime.ParseExact(date, "yyyy-MM-dd", CultureInfo.InvariantCulture);
                            if (!isHoliday && (int)d.DayOfWeek == 1)
                            {
                                reduction = 35;
                            }
                        }

                        // TODO apply reduction for others
                        if (age != null && age < 15)
                        {
                            return "{ \"cost\": " + (int)Math.Ceiling(basePrice * .7) + "}";
                        }
                        else
                        {
                            if (age == null)
                            {
                                double cost = basePrice * (1 - reduction / 100.0);
                                return "{ \"cost\": " + (int)Math.Ceiling(cost) + "}";
                            }
                            else
                            {
                                if (age > 64)
                                {
                                    double cost = basePrice * .75 * (1 - reduction / 100.0);
                                    return "{ \"cost\": " + (int)Math.Ceiling(cost) + "}";
                                }
                                else
                                {
                                    double cost = basePrice * (1 - reduction / 100.0);
                                    return "{ \"cost\": " + (int)Math.Ceiling(cost) + "}";
                                }
                            }
                        }
                    }
                    else
                    {
                        if (age != null && age >= 6)
                        {
                            if (age > 64)
                            {
                                return "{ \"cost\": " + (int)Math.Ceiling(basePrice * .4) + "}";
                            }
                            else
                            {
                                return "{ \"cost\": " + basePrice + "}";
                            }
                        }
                        else
                        {
                            return "{ \"cost\": 0}";
                        }
                    }
                }
            });

            prices.AddEndpointFilter(async (context, next) =>
            {
                var result = await next(context);
                return result is string json ? Results.Text(json, "application/json") : result;
            });

        }

    }
}
