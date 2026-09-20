using MediatR;
using Microsoft.EntityFrameworkCore;
using PortfolioRebalancer.Domain;
using PortfolioRebalancer.Infrastructure;

namespace PortfolioRebalancer.Application.Queries;

// 1. KARTECZKA (Wiadomość). Zauważ, że mówimy MediatR'owi, jakiego wyniku oczekujemy w < >
public record GetPortfolioRebalanceQuery(Guid portfolioId) : IRequest<IEnumerable<Asset>>;

public class GetPortfolioRebalanceHandler : IRequestHandler<GetPortfolioRebalanceQuery, IEnumerable<Asset>>
{
    private readonly AppDbContext _db;
    public GetPortfolioRebalanceHandler(AppDbContext db)
    {
        _db = db;
    }

    // Ta metoda zostanie wywołana automatycznie przez MediatR
    public async Task<IEnumerable<Asset>> Handle(GetPortfolioRebalanceQuery request, CancellationToken cancellationToken)
    {
        var portfolio = await _db.Portfolios
                                .Include(p => p.Assets)
                                .FirstOrDefaultAsync(p => p.Id == request.portfolioId);

        if (portfolio == null)
        {
            // Na razie zwrócimy pustą listę. W kolejnych krokach zrobimy lepszą obsługę błędów.
            return Enumerable.Empty<Asset>();
        }

        return portfolio.GetAssetsToRebalance(0.05m);
    }
}
