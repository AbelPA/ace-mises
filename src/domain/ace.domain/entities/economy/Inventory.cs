namespace ace.domain.entities.economy;

public sealed class Inventory
{
    private readonly Dictionary<FoodType, int> _items = [];

    public int Get(FoodType food)
        => _items.GetValueOrDefault(food);

    public bool Has(FoodType food, int quantity)
        => Get(food) >= quantity;

    public void Add(FoodType food, int quantity)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(quantity);

        _items[food] = Get(food) + quantity;
    }

    public bool Remove(FoodType food, int quantity)
    {
        if (quantity <= 0)
            ArgumentOutOfRangeException.ThrowIfNegativeOrZero(quantity, nameof(quantity));

        if (!Has(food, quantity))
            return false;

        _items[food] -= quantity;

        return true;
    }

    public IReadOnlyDictionary<FoodType, int> Items
    {
        get => _items;
    }
}
