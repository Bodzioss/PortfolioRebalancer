namespace PortfolioRebalancer.Domain;   

public record Asset(
    Guid Id,
    string Ticker,
    decimal CurrentPrice,
    decimal TargetWeight,
    int CurrentQuantity);
