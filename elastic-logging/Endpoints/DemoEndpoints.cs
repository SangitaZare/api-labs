namespace ElasticLoggingApi.Endpoints;

public static class DemoEndpoints
{
    public static void MapDemoEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api").WithTags("Demo");

        // 1. INFO log, or WARNING log if the product does not exist
        group.MapGet("/products/{id:int}", (int id, ILogger<Program> logger) =>
        {
            logger.LogInformation("Fetching product {ProductId}", id);

            if (id > 100)
            {
                logger.LogWarning("Product {ProductId} was not found", id);
                return Results.NotFound(new { message = "Product not found" });
            }

            return Results.Ok(new { id, name = $"Product {id}", price = id * 100 });
        });

        // 2. INFO log with business values (a GET, so you can test it in the browser)
        group.MapGet("/orders/place", (string customer, decimal amount, ILogger<Program> logger) =>
        {
            if (amount <= 0)
            {
                logger.LogWarning("Order rejected for {Customer}: invalid amount {Amount}", customer, amount);
                return Results.BadRequest(new { message = "Amount must be greater than 0" });
            }

            var orderId = Random.Shared.Next(1000, 9999);
            logger.LogInformation("Order {OrderId} placed by {Customer} for amount {Amount}", orderId, customer, amount);
            return Results.Ok(new { orderId, customer, amount });
        });

        // 3. ERROR log with the full exception
        group.MapGet("/crash", (ILogger<Program> logger) =>
        {
            var orderId = Random.Shared.Next(1000, 9999);

            try
            {
                throw new InvalidOperationException("Payment gateway timeout (test error)");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Payment failed for order {OrderId}", orderId);
                return Results.Problem("Something went wrong. Please try again.", statusCode: 500);
            }
        });

        // 4. A slow request, so response times differ
        group.MapGet("/slow", async (ILogger<Program> logger) =>
        {
            var delay = Random.Shared.Next(200, 2000);
            await Task.Delay(delay);
            logger.LogInformation("Slow operation finished after {DelayMs} ms", delay);
            return Results.Ok(new { delayMs = delay });
        });

        // 5. Creates many logs at once, so the Kibana dashboard has data
        group.MapGet("/generate", (int count, ILogger<Program> logger) =>
        {
            count = Math.Clamp(count, 1, 200);
            string[] customers = { "Ravi", "Sneha", "Amit", "Priya", "Karan" };

            for (var i = 0; i < count; i++)
            {
                var roll = Random.Shared.Next(100);
                var customer = customers[Random.Shared.Next(customers.Length)];

                if (roll < 70)
                    logger.LogInformation("Order {OrderId} placed by {Customer} for amount {Amount}",
                        Random.Shared.Next(1000, 9999), customer, Random.Shared.Next(100, 5000));
                else if (roll < 90)
                    logger.LogWarning("Low stock for product {ProductId}", Random.Shared.Next(1, 50));
                else
                    logger.LogError(new TimeoutException("Database timeout (test)"),
                        "Failed to save order for {Customer}", customer);
            }

            return Results.Ok(new { generated = count });
        });
    }
}