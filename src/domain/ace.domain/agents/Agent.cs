using System.Collections.Immutable;

using ace.domain.economy;

namespace ace.domain.agents;

public sealed class Agent
{
    public Guid Id { get; }
    public string Name { get; }
    public AgentType Type { get; }
    public Inventory Inventory { get; }
    public ImmutableArray<Need> Needs { get; }
    public ImmutableArray<Production> Productions { get; }
    public bool IsAlive { get; private set; }

    public Agent(
        string name,
        AgentType type,
        IEnumerable<Need> needs,
        IEnumerable<Production> productions)
    {
        Id = Guid.NewGuid();
        Name = name;
        Type = type;

        // Evita conversões desnecessárias se já for uma coleção imutável, 
        // ou converte de forma eficiente.
        Needs = needs is ImmutableArray<Need> immNeeds
            ? immNeeds
            : [.. needs]; // Sintaxe moderna de collection expressions (.NET 8+)

        Productions = productions is ImmutableArray<Production> immProd
            ? immProd
            : [.. productions];

        Inventory = new Inventory();
        IsAlive = true;
    }

    public void Produce()
    {
        if (!IsAlive)
            return;

        foreach (var production in Productions)
            Inventory.Add(production.Food, production.QuantityPerPeriod);
    }

    public IReadOnlyList<Need> UnfulfilledNeeds()
        => Needs
            .Where(n => !Inventory.Has(n.Food, n.QuantityPerPeriod))
            .ToList();

    public bool NeedsFood()
        => UnfulfilledNeeds().Count > 0;

    public void Consume()
    {
        if (!IsAlive)
            return;

        foreach (var need in Needs)
        {
            if (!Inventory.Has(need.Food, need.QuantityPerPeriod))
            {
                IsAlive = false;
                return;
            }
        }

        foreach (var need in Needs)
            Inventory.Remove(need.Food, need.QuantityPerPeriod);
    }
}