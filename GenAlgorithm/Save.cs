using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.Metrics;

namespace GenAlgorithm_Kasumov
{
    public class Indi
    {
        public int Id { get; set; }
        public List<IndiGen>? indi { get; set; }

        public int? StateId { get; set; }
        public State State { get; set; }
    }

    public class IndiGen
    {
        public int Id { get; set; }
        public int gen { get; set; }
        public int? IndiId { get; set; }
        public Indi Indi { get; set; }
    }

    
    public class Path
    {
        public int Id { get; set; }
        public List<City> path{get;set; }
        public int? StateId { get; set; }   
        public State State { get; set; }
  
    }
    public class City
    {
        public int Id { get; set; }
        public int CityValue { get; set; }
        public int? PathId { get; set; }
        public Path Path { get; set; }

    }

    public class State
    {
        public int Id { get; set; }
        public string name { get; set; }
        public double best_score { get; set; } //Сохранить
        public double crossing_share { get; set; } //Сохранить
        public double turnaments_share { get; set; } //Сохранить
        public double mutation_share { get; set; } //Сохранить
       

        public List<BestIndiGen> BestIndi { get; set; }

        public List<Indi> Population { get; set; }

        public List<Path> Distances { get; set; }
    }

    public class BestIndiGen
    {
        public int Id { get; set; }
        public int gen { get; set; }

        public int? StateId { get; set; }
        public  State State { get; set; }
    }
    
    public class SaveContext : DbContext
    {
        public DbSet<State> States => Set<State>();
        public DbSet<BestIndiGen> BestIndi => Set<BestIndiGen>();
        public DbSet<Indi> Population => Set<Indi>();
        public DbSet<IndiGen> Indi => Set<IndiGen>();
        public DbSet<Path> Distance => Set<Path>();
        public DbSet<City> Path => Set<City>();

        public string connectionString;

        public SaveContext(string connectionString)
        {      
           this.connectionString = connectionString;

        }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite("Data Source=" + connectionString);
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
        }


    }
}
