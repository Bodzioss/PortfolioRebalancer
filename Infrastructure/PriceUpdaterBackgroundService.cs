
using Microsoft.EntityFrameworkCore;

namespace PortfolioRebalancer.Infrastructure;

public class PriceUpdaterBackgroundService : BackgroundService
{
    private readonly ILogger<PriceUpdaterBackgroundService> _logger;
    private readonly IServiceScopeFactory _scopeFactory;
    public PriceUpdaterBackgroundService(
        ILogger<PriceUpdaterBackgroundService> logger,
        IServiceScopeFactory scopeFactory)
    {
        _logger = logger;
        _scopeFactory = scopeFactory;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Uruchomiono pracownika w tle do aktualizacji cen.");
        var random = new Random();

        // Pętla kręci się w nieskończoność, dopóki serwer działa
        while (!stoppingToken.IsCancellationRequested)
        {
            // 1. Otwieramy nowy, bezpieczny cykl życia (Scope)
            using (var scope = _scopeFactory.CreateScope())
            {
                // 2. Wyciągamy świeżą instancję bazy danych tylko na tę jedną pętlę
                var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

                // 3. Pobieramy wszystkie aktywa z bazy
                var assets = await db.Assets.ToListAsync(stoppingToken);

                if (assets.Any())
                {
                    // 4. Symulujemy losowe wahania cen na giełdzie (od -2% do +2%)
                    foreach (var asset in assets)
                    {
                        var fluctuation = (decimal)(random.NextDouble() * 0.04 - 0.02);
                        asset.CurrentPrice = Math.Round(asset.CurrentPrice + (asset.CurrentPrice * fluctuation), 2);

                        // Zabezpieczenie przed ujemną ceną akcji
                        if (asset.CurrentPrice <= 0) asset.CurrentPrice = 0.01m;
                    }

                    // 5. Zapisujemy zaktualizowane ceny do bazy danych
                    await db.SaveChangesAsync(stoppingToken);
                    _logger.LogInformation("✅ Zaktualizowano ceny dla {count} aktywów.", assets.Count);
                }
            }
            // Usypiamy wątek na 10 sekund (nie blokując aplikacji)
            await Task.Delay(10000, stoppingToken);
        }
    }
}
