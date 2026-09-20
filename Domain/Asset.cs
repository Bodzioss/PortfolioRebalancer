namespace PortfolioRebalancer.Domain;   

public class Asset
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Ticker { get; set; }
    public decimal CurrentPrice { get; set; }
    public decimal TargetWeight { get; set; }
    public int CurrentQuantity { get; set; }
}