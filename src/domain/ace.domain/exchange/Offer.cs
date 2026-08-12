using ace.domain.agents;
using ace.domain.economy;

namespace ace.domain.exchange;

public sealed record Offer(
    Agent Seller,
    FoodType OfferedFood,
    int OfferedQuantity,
    FoodType RequestedFood,
    int RequestedQuantity);