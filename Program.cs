using Microsoft.EntityFrameworkCore;
using PortfolioRebalancer.Infrastructure;
using PortfolioRebalancer.Domain; 

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql("Host=localhost;Port=5432;Database=PortfolioDb;Username=admin;Password=Password123!"));

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

app.MapPost("/api/portfolio/{id}/rebalance", async (Guid id, AppDbContext db) =>
{
    var portfolio = await db.Portfolios
            .Include(p => p.Assets)
            .FirstOrDefaultAsync(p => p.Id == id);

    if (portfolio is null)
        return Results.NotFound("Nie znaleziono portfela o takim ID.");

    var assetsToFix = portfolio.GetAssetsToRebalance(0.05m);
    return Results.Ok(assetsToFix);
});

app.Run();
