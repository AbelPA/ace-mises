using ace.domain.entities.agents;
using ace.domain.entities.economy;
using ace.domain.entities.exchange;

using Xunit;

namespace ace.tests;

public class DomainUnitTests
{
    // ─────────────────────────────────────────────────────────────────────
    // Inventory tests (spec 001)
    // ─────────────────────────────────────────────────────────────────────

    [Fact]
    public void InventoryAddIncrementsQuantity()
    {
        var inventory = new Inventory();

        inventory.Add(FoodType.Wheat, 5);

        Assert.Equal(5, inventory.Get(FoodType.Wheat));
        Assert.True(inventory.Has(FoodType.Wheat, 5));
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(-5)]
    public void InventoryAddInvalidQuantityThrows(int quantity)
    {
        var inventory = new Inventory();

        _ = Assert.Throws<ArgumentOutOfRangeException>(() => inventory.Add(FoodType.Wheat, quantity));
    }

    [Fact]
    public void InventoryHasAndGetQueryInventory()
    {
        var inventory = new Inventory();
        inventory.Add(FoodType.Wheat, 3);

        Assert.Equal(3, inventory.Get(FoodType.Wheat));
        Assert.True(inventory.Has(FoodType.Wheat, 2));
        Assert.False(inventory.Has(FoodType.Wheat, 4));
        Assert.Equal(0, inventory.Get(FoodType.Meat));
    }

    [Fact]
    public void InventoryRemoveAvailableReturnsTrue()
    {
        var inventory = new Inventory();
        inventory.Add(FoodType.Wheat, 4);

        var result = inventory.Remove(FoodType.Wheat, 3);

        Assert.True(result);
        Assert.Equal(1, inventory.Get(FoodType.Wheat));
    }

    [Fact]
    public void InventoryRemoveInsufficientReturnsFalse()
    {
        var inventory = new Inventory();
        inventory.Add(FoodType.Wheat, 2);

        var result = inventory.Remove(FoodType.Wheat, 3);

        Assert.False(result);
        Assert.Equal(2, inventory.Get(FoodType.Wheat));
    }

    // ─────────────────────────────────────────────────────────────────────
    // Production tests (spec 002)
    // ─────────────────────────────────────────────────────────────────────

    [Fact]
    public void ProduceLiveProducerInventoryIncreases()
    {
        var agent = new Agent(
            "P",
            AgentType.Producer,
            [new Need(FoodType.Meat, 1)],
            [new Production(FoodType.Wheat, 5)]);

        agent.Produce();

        Assert.Equal(5, agent.Inventory.Get(FoodType.Wheat));
    }

    [Fact]
    public void ProduceMultipleProductionsAccumulate()
    {
        var agent = new Agent(
            "P",
            AgentType.Producer,
            [new Need(FoodType.Meat, 1)],
            [new Production(FoodType.Wheat, 5), new Production(FoodType.Corn, 2)]);

        agent.Produce();

        Assert.Equal(5, agent.Inventory.Get(FoodType.Wheat));
        Assert.Equal(2, agent.Inventory.Get(FoodType.Corn));
    }

    [Fact]
    public void ProduceRepeatedProductionAccumulatesAcrossPeriods()
    {
        var agent = new Agent(
            "P",
            AgentType.Producer,
            [new Need(FoodType.Meat, 1)],
            [new Production(FoodType.Wheat, 5)]);

        agent.Produce();
        agent.Produce();

        Assert.Equal(10, agent.Inventory.Get(FoodType.Wheat));
    }

    [Fact]
    public void ProduceDeadAgentDoesNotProduce()
    {
        var agent = new Agent(
            "X",
            AgentType.Producer,
            [new Need(FoodType.Meat, 1)],
            [new Production(FoodType.Wheat, 5)]);

        agent.Consume(); // dies because no Meat in inventory
        Assert.False(agent.IsAlive);

        agent.Produce(); // should do nothing

        Assert.Equal(0, agent.Inventory.Get(FoodType.Wheat));
    }

    // ─────────────────────────────────────────────────────────────────────
    // Exchange tests (spec 003)
    // ─────────────────────────────────────────────────────────────────────

    [Fact]
    public void ExecuteValidExchangeTransfersBilaterally()
    {
        var a = new Agent(
            "A",
            AgentType.Producer,
            [new Need(FoodType.Meat, 1)],
            [new Production(FoodType.Wheat, 1)]);
        var b = new Agent(
            "B",
            AgentType.Producer,
            [new Need(FoodType.Wheat, 1)],
            [new Production(FoodType.Meat, 1)]);

        a.Produce(); // A: Wheat=1
        b.Produce(); // B: Meat=1

        var exchange = new Exchange(
            buyer: a,
            seller: b,
            foodFromBuyer: FoodType.Wheat,
            quantityFromBuyer: 1,
            foodFromSeller: FoodType.Meat,
            quantityFromSeller: 1);

        var result = exchange.Execute();

        Assert.True(result);
        Assert.Equal(0, a.Inventory.Get(FoodType.Wheat));
        Assert.Equal(1, a.Inventory.Get(FoodType.Meat));
        Assert.Equal(1, b.Inventory.Get(FoodType.Wheat));
        Assert.Equal(0, b.Inventory.Get(FoodType.Meat));
    }

    [Fact]
    public void ExecuteBuyerLacksGoodReturnsFalseNoAlteration()
    {
        var a = new Agent(
            "A",
            AgentType.Producer,
            [new Need(FoodType.Meat, 1)],
            [new Production(FoodType.Wheat, 1)]);
        var b = new Agent(
            "B",
            AgentType.Producer,
            [new Need(FoodType.Wheat, 1)],
            [new Production(FoodType.Meat, 1)]);

        b.Produce(); // only B produces: B has Meat=1, A has Wheat=0

        var exchange = new Exchange(
            buyer: a,
            seller: b,
            foodFromBuyer: FoodType.Wheat,
            quantityFromBuyer: 1,
            foodFromSeller: FoodType.Meat,
            quantityFromSeller: 1);

        var result = exchange.Execute();

        Assert.False(result);
        Assert.Equal(0, a.Inventory.Get(FoodType.Wheat));
        Assert.Equal(0, a.Inventory.Get(FoodType.Meat));
        Assert.Equal(0, b.Inventory.Get(FoodType.Wheat));
        Assert.Equal(1, b.Inventory.Get(FoodType.Meat));
    }

    [Fact]
    public void ExecuteSellerLacksGoodReturnsFalseNoAlteration()
    {
        var a = new Agent(
            "A",
            AgentType.Producer,
            [new Need(FoodType.Meat, 1)],
            [new Production(FoodType.Wheat, 1)]);
        var b = new Agent(
            "B",
            AgentType.Producer,
            [new Need(FoodType.Wheat, 1)],
            [new Production(FoodType.Meat, 1)]);

        a.Produce(); // only A produces: A has Wheat=1, B has Meat=0

        var exchange = new Exchange(
            buyer: a,
            seller: b,
            foodFromBuyer: FoodType.Wheat,
            quantityFromBuyer: 1,
            foodFromSeller: FoodType.Meat,
            quantityFromSeller: 1);

        var result = exchange.Execute();

        Assert.False(result);
        Assert.Equal(1, a.Inventory.Get(FoodType.Wheat));
        Assert.Equal(0, a.Inventory.Get(FoodType.Meat));
        Assert.Equal(0, b.Inventory.Get(FoodType.Wheat));
        Assert.Equal(0, b.Inventory.Get(FoodType.Meat));
    }

    // ─────────────────────────────────────────────────────────────────────
    // Survival tests (spec 004)
    // ─────────────────────────────────────────────────────────────────────

    [Fact]
    public void UnfulfilledNeedsFulfilledNeedNotReturned()
    {
        var agent = new Agent(
            "P",
            AgentType.Producer,
            [new Need(FoodType.Meat, 1)],
            [new Production(FoodType.Wheat, 1)]);

        agent.Inventory.Add(FoodType.Meat, 1);

        IReadOnlyList<Need> unfulfilled = agent.UnfulfilledNeeds();

        Assert.Empty(unfulfilled);
        Assert.False(agent.NeedsFood());
    }

    [Fact]
    public void UnfulfilledNeedsUnfulfilledNeedReturned()
    {
        var agent = new Agent(
            "P",
            AgentType.Producer,
            [new Need(FoodType.Meat, 1)],
            [new Production(FoodType.Wheat, 1)]);

        IReadOnlyList<Need> unfulfilled = agent.UnfulfilledNeeds();

        Assert.Contains(unfulfilled, n => n.Food == FoodType.Meat);
        Assert.True(agent.NeedsFood());
    }

    [Fact]
    public void ConsumeAllNeedsFulfilledStaysAlive()
    {
        var agent = new Agent(
            "P",
            AgentType.Producer,
            [new Need(FoodType.Meat, 1)],
            [new Production(FoodType.Wheat, 1)]);

        agent.Inventory.Add(FoodType.Meat, 1);

        agent.Consume();

        Assert.Equal(0, agent.Inventory.Get(FoodType.Meat));
        Assert.True(agent.IsAlive);
    }

    [Fact]
    public void ConsumeUnfulfilledNeedAgentDies()
    {
        var agent = new Agent(
            "P",
            AgentType.Producer,
            [new Need(FoodType.Meat, 1)],
            [new Production(FoodType.Wheat, 1)]);

        agent.Consume();

        Assert.False(agent.IsAlive);
    }

    [Fact]
    public void ConsumeDeadAgentNoChange()
    {
        var agent = new Agent(
            "X",
            AgentType.Producer,
            [new Need(FoodType.Meat, 1)],
            [new Production(FoodType.Wheat, 5)]);

        agent.Consume(); // dies because no Meat
        Assert.False(agent.IsAlive);

        agent.Inventory.Add(FoodType.Wheat, 3); // pre-existing inventory state to observe

        agent.Consume(); // dead — should be a no-op

        Assert.False(agent.IsAlive);
        Assert.Equal(3, agent.Inventory.Get(FoodType.Wheat));
        Assert.Equal(0, agent.Inventory.Get(FoodType.Meat));
    }
}
