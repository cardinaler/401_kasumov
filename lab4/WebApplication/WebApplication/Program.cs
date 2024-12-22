using GenAlgorithm_Kasumov;

var builder = WebApplication.CreateBuilder();
var app = builder.Build();
GenAlg? Alg;
void InitDistance(int N, List<List<int>> distance)
{
    
    int L = 10;
    int R = 100;
    Random rnd = new Random();
    List<int> tmp = new List<int>();
//    distance = new List<List<int>>();
    for (int i = 0; i < N; ++i)
    {
        List<int> ints = new List<int>();
        for (int j = 0; j < N; ++j)
        {
            ints.Add(0);
        }
        distance.Add(ints);
    }
    for (int i = 0; i < N; ++i)
    {
        tmp.Add(R);
    }
    for (int i = 0; i < N; ++i)
    {
        for (int j = i + 1; j < N; ++j)
        {
            if (i == 0)
            {
                distance[i][j] = rnd.Next(L, R);
            }
            else
            {
                distance[i][j] = rnd.Next(L, tmp[i] + tmp[j]);
            }
        }
        distance[i][i] = 0;
        for (int j = 0; j < N; ++j)
        {
            distance[j][i] = distance[i][j];
        }

        for (int j = i + 1; j < N; ++j)
        {
            tmp[j] = Math.Min(tmp[j], distance[i][j]);
        }
    }
}
app.Run(async (context) =>
{
var response = context.Response;
var request = context.Request;
if (request.Path == "/api/user")
{
    var message = "Некорректные данные";   // содержание сообщения по умолчанию
    List<List<int>> distance = new List<List<int>>();

    try
    {
        var person = await request.ReadFromJsonAsync<Person>();
        if (person != null)
        {
            InitDistance(person.MatrixSize, distance);
            message = "hello";
            Alg = new GenAlg(distance, person.IndividNums, person.TournamentsShare, person.CrossingShare, person.MutationShare);
        }
    }
        catch { }
        // отправляем пользователю данные
        await response.WriteAsJsonAsync(new {text=message, matrix = JSONConverter.MatrixToJson(distance) });
    }
    else
    {
        response.ContentType = "text/html; charset=utf-8";
        await response.SendFileAsync("html/index.html");
    }
});

app.Run();

public record Person(double CrossingShare, double TournamentsShare, double MutationShare, int IndividNums, int MatrixSize);
