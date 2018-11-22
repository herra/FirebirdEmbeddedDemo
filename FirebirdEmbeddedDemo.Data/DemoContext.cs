using System;
using Microsoft.EntityFrameworkCore;
using System.Data.Common;
using FirebirdSql.EntityFrameworkCore.Firebird.Extensions;

namespace FirebirdEmbeddedDemo.Data
{
    public class DemoContext : DbContext
    {
        private readonly string _connectionString;

        public DemoContext(string connectionString)
        {
            _connectionString = connectionString;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseFirebird(_connectionString);
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
        }

        public virtual DbSet<Todos> Todos { get; set; }
    }
}
