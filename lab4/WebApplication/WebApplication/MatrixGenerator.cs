internal class MatrixGenerator
{
    public static void InitDistance(int N, List<List<int>> distance)
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
}

