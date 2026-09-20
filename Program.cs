using MediatR;
using Microsoft.EntityFrameworkCore;
using PortfolioRebalancer.Application.Queries;
using PortfolioRebalancer.Domain; 
using PortfolioRebalancer.Infrastructure;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql("Host=localhost;Port=5432;Database=PortfolioDb;Username=admin;Password=Password123!"));

builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblyContaining<Program>());

builder.Services.AddHostedService<PriceUpdaterBackgroundService>();

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

app.MapPost("/api/portfolios", async (Portfolio portfolio, AppDbContext db) =>
{
    db.Portfolios.Add(portfolio);
    await db.SaveChangesAsync(); 

    return Results.Ok(portfolio.Id); 
});

app.MapPost("/api/portfolio/{id}/rebalance", async (Guid id, IMediator mediator) =>
{
    var result = await mediator.Send(new GetPortfolioRebalanceQuery(id));

    if(!result.Any())
    {
        return Results.NotFound("Nie znaleziono portfela o takim ID (lub portfel jest pusty).");
    }

    return Results.Ok(result);
});
});

app.Run();
