using System.Text.Json;

internal class JSONConverter
{
    public static string MatrixToJson(List<List<int>> matrix)
    {
        int rws = matrix.Count, cls = matrix[0].Count;
        double[][] arr = new double[rws][];
        for (int i = 0; i < rws; i++)
        {
            arr[i] = new double[cls];
            for (int j = 0; j < cls; j++)
            {
                arr[i][j] = matrix[i][j];
            }
        }
        return JsonSerializer.Serialize(arr);
    }
}

