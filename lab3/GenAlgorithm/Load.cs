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

        public virtual DbSet<State> States { get; set; }
        public virtual DbSet<Indi> Population { get; set; }
        public virtual DbSet<BestIndi> BestIndi{ get; set; }
        public virtual DbSet<PathDistance> Distances { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
            => optionsBuilder.UseSqlite("Data Source="+connectionString);


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
