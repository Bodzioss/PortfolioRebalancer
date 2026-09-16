using PortfolioRebalancer.Domain;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapPost("/api/portfolio/rebalance", (Portfolio portfolio) =>
{
    var assetToFix = portfolio.GetAssetsToRebalance(0.05m);

    return Results.Ok(assetToFix);
});

app.Run();
