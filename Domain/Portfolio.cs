namespace PortfolioRebalancer.Domain;

public class Portfolio
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public List<Asset> Assets { get; set; } = new List<Asset>();

    public decimal GetTotalValue()
    {
        return Assets.Sum(asset => asset.CurrentPrice * asset.CurrentQuantity);
    }

    public IEnumerable<Asset> GetAssetsToRebalance(decimal tolerance)
    {
        var totalValue = GetTotalValue();
        if (totalValue == 0 ) 
            return Enumerable.Empty<Asset>();

        var AssetsToRebalance = Assets.Where(asset => (Math.Abs((asset.CurrentPrice * asset.CurrentQuantity / totalValue) - asset.TargetWeight)) > tolerance);

        return AssetsToRebalance; 
    }
}
