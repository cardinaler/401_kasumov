namespace GenAlgorithm_Kasumov
{
    public class GenAlg
    {
        public List<List<int>> distance;
        public List<List<int>> initial_population;
        public List<List<int>> population;
        public List<List<int>> permutations;
        public List<int> best_indi;
        public List<int> real_indi;

        public double best_score;
        public double real_score;

        public int IndividNums;
        public double crossing_share;
        public double turnaments_share;
        public double mutation_share;

        Random rnd;

        public bool debug;
        public GenAlg(List<List<int>> distance, int IndividNums, double turnaments_share, double crossing_share, double mutation_share, bool debug_mode = false)
        {
            this.distance = distance;
            this.initial_population = new List<List<int>>();
            this.population = new List<List<int>>();
            this.permutations = new List<List<int>>();

            this.best_indi = new List<int>();
            this.real_indi = new List<int>();

            this.best_score = 0;
            this.real_score = 0;

            this.IndividNums = IndividNums;
            this.crossing_share = crossing_share;
            this.turnaments_share = turnaments_share;
            this.mutation_share = mutation_share;

            this.rnd = new Random(DateTime.Now.Ticks.GetHashCode());

            this.debug = debug_mode;
            generate_population();
        }
        public void LifeCycle()
        {
            crossover_stage();
            mutation_stage();
            selection_stage();
        }
        void generate_population()
        {
            List<int> perm = new List<int>();
            List<int> nums = new List<int>();
            for (int i = 0; i < this.distance.Count; ++i)
            {
                nums.Add(i);
            }
            generate_permutations(perm, nums);
            for (int i = 0; i < this.IndividNums; ++i)
            {
                List<int> indi = this.permutations[rnd.Next(this.permutations.Count)];
                this.population.Add(indi);
                evaluate_new_indi(indi);
            }
            this.initial_population = new List<List<int>>(this.population);
        }
        void generate_permutations(List<int> perm, List<int> nums)
        {
            if (nums.Count == 0)
            {
                this.permutations.Add(perm);
            }
            for (int i = 0; i < nums.Count; ++i)
            {
                List<int> new_perm = new List<int>(perm);
                new_perm.Add(nums[i]);
                List<int> new_nums = new List<int>(nums);
                new_nums.Remove(nums[i]);
                generate_permutations(new_perm, new_nums);
            }
        }

        void crossover_stage()
        {
            int crossing_num = (int)(this.initial_population.Count * this.crossing_share);
            for (int i = 0; i < crossing_num; ++i)
            {
                int id1 = rnd.Next(0, this.initial_population.Count);
                int id2 = rnd.Next(0, this.initial_population.Count);
                while (id1 == id2)
                {
                    id2 = rnd.Next(0, this.initial_population.Count);
                }
                List<int> child = cross(this.initial_population[id1], this.initial_population[id2]);
                this.population.Add(child);

                evaluate_new_indi(child);
            }
        }
        public List<int> cross(List<int> indi1, List<int> indi2)
        {
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
            if (score > this.best_score)
            {
                this.best_score = score;
                this.best_indi = indi;
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
            List<int> indi = this.population[indi_id];
            int N = indi.Count;
            int n1 = this.rnd.Next(N);
            int n2 = this.rnd.Next(N);
            int tmp = indi[n1];
            indi[n1] = indi[n2];
            indi[n2] = tmp;
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

        public void find_real_solution_brutforce()
        {
            for(int i = 0; i < this.permutations.Count; ++i)
            {
                double score = metrics(this.permutations[i]);
                if(score > real_score)
                {
                    real_score = score;
                    real_indi = this.permutations[i];
                }
            }
        }
    }
}
