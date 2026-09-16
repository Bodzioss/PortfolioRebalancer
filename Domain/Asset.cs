namespace PortfolioRebalancer.Domain;   

public record Asset(
    string Ticker,
    decimal CurrentPrice,
    decimal TargetWeight,
    int CurrentQuantity);
