using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using TrackerEnabledDbContext.Common.Testing;
using TrackerEnabledDbContext.Common.Testing.Models;

namespace TrackerEnabledDbContext.Identity.IntegrationTests
{
    public class TestTrackerIdentityContext : TrackerIdentityContext, ITestDbContext
    {
        protected static readonly string TestConnectionString = Environment.GetEnvironmentVariable("TestGenericConnectionString")
            ?? "DefaultTestConnection";

        public TestTrackerIdentityContext()
            : base()
        {
            Database.EnsureCreated();
            Database.EnsureDeleted();
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(TestConnectionString);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<ModelWithCompositeKey>()
                        .HasKey(c => new { c.Key1, c.Key2 });
            modelBuilder.Entity<ModelWithComplexType>()
                        .OwnsOne(e => e.ComplexType);
        }

        public DbSet<NormalModel> NormalModels { get; set; }
        public DbSet<ParentModel> ParentModels { get; set; }
        public DbSet<ChildModel> Children { get; set; }
        public DbSet<ModelWithCompositeKey> ModelsWithCompositeKey { get; set; }
        public DbSet<ModelWithConventionalKey> ModelsWithConventionalKey { get; set; }
        public DbSet<ModelWithSkipTracking> ModelsWithSkipTracking { get; set; }
        public DbSet<POCO> POCOs { get; set; }
        public DbSet<TrackedModelWithMultipleProperties> TrackedModelsWithMultipleProperties { get; set; }
        public DbSet<TrackedModelWithCustomTableAndColumnNames> TrackedModelsWithCustomTableAndColumnNames { get; set; }
        public DbSet<SoftDeletableModel> SoftDeletableModels { get; set; }

        public DbSet<ModelWithComplexType> ModelsWithComplexType { get; set; }
    }
}