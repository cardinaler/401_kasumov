using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GenAlgorithm_Kasumov
{

    public partial class LoadAppContext : DbContext
    {
        public string connectionString;

        public LoadAppContext(string connectionString)
        {
            this.connectionString = connectionString;
        }

        public LoadAppContext(DbContextOptions<LoadAppContext> options)
            : base(options)
        {
        }

        public virtual DbSet<State> States => Set<State>();
        public virtual DbSet<BestIndiGen> BestIndi => Set<BestIndiGen>();
        public virtual DbSet<Indi> Population => Set<Indi>();
        public virtual DbSet<IndiGen> Indi => Set<IndiGen>();
        public virtual DbSet<Path> Distance => Set<Path>();
        public virtual DbSet<City> Path => Set<City>();

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
            => optionsBuilder.UseSqlite("Data Source="+connectionString);


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
