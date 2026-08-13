using ace.domain.entities.agents;
using ace.domain.entities.exchange;
using ace.domain.interfaces.application;

namespace ace.application;

public sealed class EconomicCycle : IEconomicCycle
{
    /// <summary>
    /// Runs one period of the economic cycle for the given agents.
    /// Mandatory order: Production → Needs → Trades → Consumption → Survival.
    /// </summary>
    public static void RunPeriod(IReadOnlyList<Agent> agents)
    {
        // Phase 1: Production
        foreach (Agent agent in agents)
            agent.Produce();

        // Phase 2: Needs (query — no action needed; unfulfilled needs are
        // computed on-demand by the trade discovery)

        // Phase 3: Trades — discover and execute bilateral exchange opportunities
        DiscoverAndExecuteTrades(agents);

        // Phase 4: Consumption
        foreach (Agent agent in agents)
            agent.Consume();

        // Phase 5: Survival — observe IsAlive (no action; Consume already
        // sets IsAlive = false for agents that couldn't fulfill their needs)
    }

    /// <summary>
    /// Discovers and executes bilateral exchange opportunities over all alive
    /// agents using an O(n²) pairwise comparison with complete information.
    /// </summary>
    private static void DiscoverAndExecuteTrades(IReadOnlyList<Agent> agents)
    {
        for (int i = 0; i < agents.Count; i++)
        {
            for (int j = 0; j < agents.Count; j++)
            {
                if (i == j) continue;

                Agent a = agents[i];
                Agent b = agents[j];

                if (!a.IsAlive || !b.IsAlive) continue;

                // Re-check unfulfilled needs each time (inventories change during trading)
                IReadOnlyList<domain.entities.economy.Need> unfulfilledA = a.UnfulfilledNeeds();
                IReadOnlyList<domain.entities.economy.Need> unfulfilledB = b.UnfulfilledNeeds();

                if (unfulfilledA.Count == 0 || unfulfilledB.Count == 0) continue;

                // Find a reciprocal opportunity:
                // A needs X (B has X) + B needs Y (A has Y)
                foreach (domain.entities.economy.Need needA in unfulfilledA)
                {
                    if (!b.Inventory.Has(needA.Food, 1)) continue;

                    bool exchanged = false;

                    foreach (domain.entities.economy.Need needB in unfulfilledB)
                    {
                        if (!a.Inventory.Has(needB.Food, 1)) continue;

                        // Reciprocal match: A needs needA.Food (B has it),
                        // B needs needB.Food (A has it)
                        int qtyFromB = Math.Min(needA.QuantityPerPeriod, b.Inventory.Get(needA.Food));
                        int qtyFromA = Math.Min(needB.QuantityPerPeriod, a.Inventory.Get(needB.Food));

                        if (qtyFromB <= 0 || qtyFromA <= 0) continue;

                        var exchange = new Exchange(
                            buyer: a,
                            seller: b,
                            foodFromBuyer: needB.Food,
                            quantityFromBuyer: qtyFromA,
                            foodFromSeller: needA.Food,
                            quantityFromSeller: qtyFromB);

                        _ = exchange.Execute();
                        exchanged = true;
                        break;
                    }

                    if (exchanged)
                        break;
                }
            }
        }
    }
}
