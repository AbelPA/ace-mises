using ace.application;
using ace.domain.entities.agents;
using ace.domain.entities.economy;

using Xunit;

namespace ace.tests;

public class AbScenarioTests
{
    [Fact]
    public void AbScenario_BothProducersWithReciprocalNeeds_BothSurvive()
    {
        // Arrange — A/B scenario from spec 004 / EXP-001
        var agentA = new Agent(
            "A",
            AgentType.Producer,
            [new Need(FoodType.Meat, 1)],
            [new Production(FoodType.Wheat, 5)]);

        var agentB = new Agent(
            "B",
            AgentType.Producer,
            [new Need(FoodType.Wheat, 1)],
            [new Production(FoodType.Meat, 2)]);

        var agents = new List<Agent> { agentA, agentB };
        var cycle = new EconomicCycle();

        // Act — run one period
        cycle.RunPeriod(agents);

        // Assert — final inventories and survival
        // After production + exchange + consumption:
        //   A: Wheat=4, Meat=0, alive
        //   B: Wheat=0, Meat=1, alive
        Assert.True(agentA.IsAlive, "A should be alive");
        Assert.True(agentB.IsAlive, "B should be alive");

        Assert.Equal(4, agentA.Inventory.Get(FoodType.Wheat));
        Assert.Equal(0, agentA.Inventory.Get(FoodType.Meat));

        Assert.Equal(0, agentB.Inventory.Get(FoodType.Wheat));
        Assert.Equal(1, agentB.Inventory.Get(FoodType.Meat));
    }
}