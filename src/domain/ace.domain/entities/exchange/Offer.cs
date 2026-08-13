using ace.domain.entities.agents;
using ace.domain.entities.economy;

namespace ace.domain.entities.exchange;

public sealed record Offer(
    Agent Seller,
    FoodType OfferedFood,
    int OfferedQuantity,
    FoodType RequestedFood,
    int RequestedQuantity);
