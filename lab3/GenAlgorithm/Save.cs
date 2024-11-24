using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.Metrics;

namespace GenAlgorithm_Kasumov
{
    public class Indi
    {
        public Guid ID { get; set; }
        public List<int>? path { get; set; }
    }
    public class BestIndi
    {
        public Guid ID { get; set; }
        public List<int>? path { get; set; }
    }
    public class State
    {
        public Guid ID { get; set; }
        public double best_score { get; set; } //Сохранить
        public double crossing_share { get; set; } //Сохранить
        public double turnaments_share { get; set; } //Сохранить
        public double mutation_share { get; set; } //Сохранить
    }
    public class PathDistance
    {
        [Key]
        public int ID { get; set; }
        public List<int>? PathDs { get; set; }
    }
    public class SaveContext : DbContext
    {
        public DbSet<State> States => Set<State>();
        public DbSet<Indi> Population => Set <Indi>(); 
        public DbSet<BestIndi> BestIndi => Set<BestIndi>();
        public DbSet<PathDistance> Distances => Set<PathDistance>();

        public string connectionString;

        public SaveContext(string connectionString)
        {      
           this.connectionString = connectionString;
           Database.EnsureDeleted();
           Database.EnsureCreated();   
        }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite("Data Source=" + connectionString);
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<PathDistance>().HasKey(p => p.ID);
        }


    }
}
