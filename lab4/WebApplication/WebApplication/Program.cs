using GenAlgorithm_Kasumov;
using Microsoft.AspNetCore.Http;
using System.Diagnostics;
using static System.Runtime.InteropServices.JavaScript.JSType;

var builder = WebApplication.CreateBuilder();
var app = builder.Build();
app.UseStaticFiles();

app.MapPut("/experiments", (Parameters parameters) =>
{
    var id = Guid.NewGuid();
    List<List<int>> distance = new List<List<int>>();
    MatrixGenerator.InitDistance(parameters.MatrixSize, distance);
    GenAlg Alg = new GenAlg(distance, parameters.IndividNums, parameters.TournamentsShare, parameters.CrossingShare, parameters.MutationShare);
    ExperimentBank.AddExperiment(id, new Experiment(Alg, parameters));

    return Results.Ok(new { expid = id, matrix = JSONConverter.MatrixToJson(distance) });
});

app.MapPost("/experiments/{id:guid}", (Guid id) =>
{
    Experiment experiment;
    if(!ExperimentBank.Get(id, out experiment))
    {
        return Results.NotFound("Experiment not found");
    }
    experiment.oprimizer.LifeCycle();
    return Results.Ok(new { result = experiment.oprimizer.best_score, epoch = experiment.oprimizer.iternum });
    

});

app.MapPost("/experiments/{id:guid}/start", async (HttpContext context, Guid id) =>
{
    Experiment experiment;
    double prev = Double.MaxValue;
    if (!ExperimentBank.Get(id, out experiment))
        return Results.NotFound("Not found");

    if (ExperimentBank.GetEvolutionTokens(id))
    {
        return Results.BadRequest("Оптимизация уже запущена");
    }
    var cnctkn = new CancellationTokenSource();
    ExperimentBank.EvolutionTokens[id] = cnctkn;

    _ = Task.Run(async () =>
    {
        
            try
            {
                while (experiment.oprimizer.population.Count > 2)
                {
                    if (cnctkn.Token.IsCancellationRequested)
                        break;
                    experiment.oprimizer.LifeCycle();
                }
            }
            catch (Exception)
            {
                Debug.WriteLine($"Experiment {id} evolution stopped.");
            }
            finally
            {
                ExperimentBank.EvolutionTokens.Remove(id);
            }

        
    }); //Разделение потока
    return Results.Ok("Оптимизация началась");
});

app.MapPost("/experiments/{id:guid}/stop", (Guid id) =>
{
    Experiment experiment;
    if (!ExperimentBank.EvolutionTokens.TryGetValue(id, out var cnctkn))
        return Results.BadRequest("Нечего останавливать");

    if (!ExperimentBank.Get(id, out experiment))
        return Results.NotFound("Not found");

    cnctkn.Cancel(); // Остановка эволюции
    return Results.Ok(new { result = experiment.oprimizer.best_score, epoch = experiment.oprimizer.iternum });

});

app.MapDelete("/experiments/{id:guid}", (Guid id) =>
{
    if (ExperimentBank.Experiments.Remove(id))
        return Results.Ok("Experiment deleted");

    return Results.NotFound("Experiment not found");
    
});

app.MapGet("/experiments/{id:guid}/stream", async (HttpContext context, Guid id) =>
{
    Experiment experiment;
    if (!ExperimentBank.Get(id, out experiment))
    {
        await context.Response.WriteAsync("Experiment not found");
        return;
    }
    context.Response.ContentType = "text/event-stream";
    var data = $"data: {{" +
            $" \"epochs\": {experiment.oprimizer.iternum}," +
            $" \"best\": \"{experiment.oprimizer.best_score}\"}}\n\n";

    await context.Response.WriteAsJsonAsync(new {epochs = experiment.oprimizer.iternum , best= experiment.oprimizer.best_score });
    await context.Response.Body.FlushAsync();
    await Task.Delay(500); 


});
app.MapFallbackToFile("index.html");

app.Run();

public record Parameters(double CrossingShare, double TournamentsShare, double MutationShare, int IndividNums, int MatrixSize);

public partial class Program { }