using GenAlgorithm_Kasumov;
using System.Runtime.CompilerServices;

public class Experiment
{
    public GenAlg oprimizer;
    public Parameters parameters;
    public Experiment(GenAlg oprimizer, Parameters parameters)
    {
        this.oprimizer = oprimizer;
        this.parameters = parameters;  
    }
}

public static class ExperimentBank
{
    public static Dictionary<Guid, Experiment> Experiments = new();
    public static readonly Dictionary<Guid, CancellationTokenSource> EvolutionTokens = new();


    public static bool Get(Guid id, out Experiment experiment)
    {
        return Experiments.TryGetValue(id, out experiment);
    }
    public static bool GetEvolutionTokens(Guid id)
    {
        return EvolutionTokens.ContainsKey(id); 
    }
    public static void AddExperiment(Guid id,  Experiment experiment)
    {
        Experiments[id] = experiment;
    }
}