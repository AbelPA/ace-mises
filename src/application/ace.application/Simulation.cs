using ace.domain.entities.agents;

namespace ace.application;

public sealed class Simulation
{
    private readonly EconomicCycle _cycle = new();

    public void RunPeriods(IReadOnlyList<Agent> agents, int periods)
    {
        for (int p = 0; p < periods; p++)
            _cycle.RunPeriod(agents);
    }

    public void RunPeriod(IReadOnlyList<Agent> agents)
        => _cycle.RunPeriod(agents);
}