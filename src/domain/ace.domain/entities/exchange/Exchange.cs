using ace.domain.entities.agents;
using ace.domain.entities.economy;

namespace ace.domain.entities.exchange;

public sealed class Exchange
{
    public Agent Buyer { get; }
    public Agent Seller { get; }
    public FoodType FoodFromBuyer { get; }
    public int QuantityFromBuyer { get; }
    public FoodType FoodFromSeller { get; }
    public int QuantityFromSeller { get; }

    public Exchange(
        Agent buyer,
        Agent seller,
        FoodType foodFromBuyer,
        int quantityFromBuyer,
        FoodType foodFromSeller,
        int quantityFromSeller)
    {
        Buyer = buyer;
        Seller = seller;
        FoodFromBuyer = foodFromBuyer;
        QuantityFromBuyer = quantityFromBuyer;
        FoodFromSeller = foodFromSeller;
        QuantityFromSeller = quantityFromSeller;
    }

    public bool Execute()
    {
        if (!Buyer.Inventory.Has(FoodFromBuyer, QuantityFromBuyer))
            return false;

        if (!Seller.Inventory.Has(FoodFromSeller, QuantityFromSeller))
            return false;

        _ = Buyer.Inventory.Remove(FoodFromBuyer, QuantityFromBuyer);
        _ = Seller.Inventory.Remove(FoodFromSeller, QuantityFromSeller);

        Buyer.Inventory.Add(FoodFromSeller, QuantityFromSeller);
        Seller.Inventory.Add(FoodFromBuyer, QuantityFromBuyer);

        return true;
    }
}
