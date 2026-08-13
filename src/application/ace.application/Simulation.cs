using ace.domain.entities.agents;

namespace ace.application;

public sealed class Simulation
{
    private readonly EconomicCycle _cycle = new();

    public static void RunPeriods(IReadOnlyList<Agent> agents, int periods)
    {
        for (int p = 0; p < periods; p++)
            EconomicCycle.RunPeriod(agents);
    }

    public static void RunPeriod(IReadOnlyList<Agent> agents)
        => EconomicCycle.RunPeriod(agents);
}
