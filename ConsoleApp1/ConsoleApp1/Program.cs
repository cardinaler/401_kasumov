using GenAlgorithm_Kasumov;
using System;
class Program
{
    private static bool keepRunning = true;
    public static void print_indi(List<int> indi)
        {
            Console.Write('(');
            for (int i = 0; i < indi.Count - 1; ++i)
            {
                Console.Write($"{indi[i]}, ");
            }
            Console.WriteLine($"{indi[indi.Count - 1]})");
        }
    public static void print_population(List<List<int>> population)
        {
            Console.WriteLine("Population");
            for (int i = 0; i < population.Count; ++i)
            {
                for (int j = 0; j < population[i].Count; ++j)
                {
                    Console.Write(population[i][j]);
                    Console.Write(' ');
                }
                Console.WriteLine();
                Console.WriteLine();
            }
        }
    
    static void Main(string[] args)
    {
        int IndividNums = 10;
        double crossing_share = 0.2;
        double turnaments_share = 0.3;
        double mutation_share = 0.1;
        bool debug = true ;
        
        List<List<int>> distance = new List<List<int>>{
                            new List<int>{0, 34, 2, 6},
                            new List<int>{34, 0, 7, 8},
                            new List<int>{2, 7, 0, 11},
                            new List<int>{6, 8, 11,0 }};

        GenAlg alg = new GenAlg(distance, IndividNums, turnaments_share, crossing_share, mutation_share, debug);
        int cnt = 0;
        Console.CancelKeyPress += delegate (object sender, ConsoleCancelEventArgs e) {
            e.Cancel = true;
            keepRunning = false;
        };
        while (keepRunning)
        {
            cnt++; 
            Console.WriteLine($"Population number {cnt}");
            alg.LifeCycle();
            print_population(alg.population);
            
            Console.WriteLine();
        }
        Console.WriteLine($"Best score: {alg.best_score}");
        Console.Write($"Best solution:");
        print_indi(alg.best_indi);

        alg.find_real_solution_brutforce();
        Console.WriteLine();
        Console.WriteLine($"Real score:{alg.real_score}");
        Console.Write($"Real solution:");
        print_indi(alg.real_indi);
        
        
    
    
    }
    protected static void myHandler(object sender, ConsoleCancelEventArgs args)
    {
        Console.WriteLine("\nThe read operation has been interrupted.");
    }
}