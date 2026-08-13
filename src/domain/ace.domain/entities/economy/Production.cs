namespace ace.domain.entities.economy;

public sealed record Production
{
    public FoodType Food { get; init; }
    public int QuantityPerPeriod { get; init; }

    public Production(FoodType food, int quantityPerPeriod)
    {
        if (quantityPerPeriod <= 0)
            ArgumentOutOfRangeException.ThrowIfNegativeOrZero(quantityPerPeriod, nameof(quantityPerPeriod));
        Food = food;
        QuantityPerPeriod = quantityPerPeriod;
    }
}