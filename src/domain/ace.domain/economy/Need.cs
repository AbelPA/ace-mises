namespace ace.domain.economy;

public sealed record Need
{
    public FoodType Food { get; init; }
    public int QuantityPerPeriod { get; init; }

    public Need(FoodType food, int quantityPerPeriod)
    {
        if (quantityPerPeriod <= 0)
            throw new ArgumentOutOfRangeException(nameof(quantityPerPeriod), "QuantityPerPeriod must be > 0");
        Food = food;
        QuantityPerPeriod = quantityPerPeriod;
    }
}