using Microsoft.EntityFrameworkCore;

namespace GenAlgorithm_Kasumov
{
    public class GenAlg
    {
        public List<List<int>> distance; //Матрица расстояний между городами Сохранить
        public List<List<int>> population; //Текущая популяция Сохранить
        public List<int> best_indi; //Сохранить

        public double best_score; //Сохранить

        public int IndividNums;   //Число особей при старте
        public double crossing_share; //Сохранить
        public double turnaments_share; //Сохранить
        public double mutation_share; //Сохранить

        Random rnd; //Ядро рандома

        public bool proceed;
        public int iternum = 0;
        public GenAlg()
        {
            this.distance = new List<List<int>>();
            this.population = new List<List<int>>();
            this.best_indi = new List<int>();
            this.rnd = new Random(DateTime.Now.Ticks.GetHashCode());

        }
        public GenAlg(List<List<int>> distance, int IndividNums, double turnaments_share, double crossing_share, double mutation_share, bool proceed = false, string db_name = "")
        {
            this.distance = distance;
            this.population = new List<List<int>>();

            this.best_indi = new List<int>();

            this.best_score = Double.MaxValue;

            this.IndividNums = IndividNums;
            this.crossing_share = crossing_share;
            this.turnaments_share = turnaments_share;
            this.mutation_share = mutation_share;

            this.rnd = new Random(DateTime.Now.Ticks.GetHashCode());

            generate_population();
        }

        
        public void LifeCycle()
        {
            crossover_stage();
            mutation_stage();
            selection_stage();
            iternum++;
        }
        void generate_population()
        {
            Random rnd = new Random();
            int[] indi = new int[distance.Count];
            Parallel.For(0, distance.Count, (i) => indi[i] = i);
            for (int i = 0; i < this.IndividNums; ++i)
            {
                rnd.Shuffle(indi);
                this.population.Add(new List<int>(indi));
                evaluate_new_indi(new List<int>(indi));
            }
        }

        void crossover_stage()
        {
            Random rnd = new Random();
            int crossing_num = (int)(this.population.Count * this.crossing_share);
            Parallel.For(0, crossing_num, (i) =>
            {
                int id1 = rnd.Next(0, this.population.Count - 1);
                int id2 = rnd.Next(0, this.population.Count - 1);
                List<int> child = new List<int>();
                List<int> a;
                List<int> b;
                while (id1 == id2)
                {
                    id2 = rnd.Next(0, this.population.Count - 1);
                }
                try
                {
                    a = new List<int>(this.population[id1]);
                    b = new List<int>(this.population[id2]);
                    if(a is null || b is null)
                    {
                        throw new Exception();
                    }
                    child = cross(a, b);
                    this.population.Add(child);

                    evaluate_new_indi(child);
                }
                catch (Exception ex)
                {
                    Console.WriteLine();
                }
                               
            });
            
        }
        public List<int> cross(List<int> indi1, List<int> indi2)
        {
            try
            {
                Random rnd = new Random();
                List<int> child = new List<int>(indi2);
                int N = rnd.Next((int)(indi1.Count * 0.25), (int)(indi1.Count * 0.75));

                for (int i = 0; i < N; ++i)
                {
                    int id1 = rnd.Next(indi1.Count);

                    int id2 = indi2.IndexOf(indi1[id1]);
                    int tmp = child[id1];
                    child[id1] = child[id2];
                    child[id2] = tmp;
                }
                return child;
            }
            catch (Exception ex)
            {
                return null;
            }

        }

        public double metrics(List<int> indi)
        {
            double ans = 0;
            for (int i = 1; i < indi.Count; ++i)
            {
                ans += this.distance[indi[i - 1]][indi[i]];
            }
            ans += this.distance[indi[indi.Count - 1]][indi[0]];
            return ans;
        }
        public void evaluate_new_indi(List<int> indi)
        {
            double score = metrics(indi);
            if (score < this.best_score)
            {
                this.best_score = score;
                this.best_indi = new List<int>(indi);
            }
        }
        public void mutation_stage()
        {
            int mutation_number = (int)(this.population.Count * this.mutation_share);
            for (int i = 0; i < mutation_number; ++i)
            {
                int id = rnd.Next(0, this.population.Count);
                mutate(id);

                evaluate_new_indi(this.population[id]);
            }
        }

        public void mutate(int indi_id)
        {
            try
            {
                List<int> indi = this.population[indi_id];
                int N = indi.Count;
                int n1 = this.rnd.Next(N);
                int n2 = this.rnd.Next(N);
                int tmp = indi[n1];
                indi[n1] = indi[n2];
                indi[n2] = tmp;
            }
            catch (Exception ex)
            {
            }

        }

        public void selection_stage() // Турнирная селекция
        {
            int turnaments_num = (int)(population.Count * turnaments_share);
            for (int i = 0; i < turnaments_num; ++i)
            {
                int id1 = rnd.Next(0, this.population.Count);
                int id2 = rnd.Next(0, this.population.Count);
                while (id1 == id2)
                {
                    id2 = rnd.Next(0, this.population.Count);
                }
                List<int> indi1 = this.population[id1];
                List<int> indi2 = this.population[id2];
                if (metrics(indi1) > metrics(indi2))
                {
                    this.population.RemoveAt(id2);
                }
                else
                {
                    this.population.RemoveAt(id1);
                }
            }
        }

    }
}
