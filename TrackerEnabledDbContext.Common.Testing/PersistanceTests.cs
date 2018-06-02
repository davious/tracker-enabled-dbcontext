using System;
using Microsoft.EntityFrameworkCore.Storage;
using TrackerEnabledDbContext.Common.Configuration;
using TrackerEnabledDbContext.Common.Testing.Code;
using Xunit;

namespace TrackerEnabledDbContext.Common.Testing
{
    public class PersistanceTests<TContext>: IDisposable where TContext : ITestDbContext, new()
    {
        public PersistanceTests()
        {
            _transaction = Db.Database.BeginTransaction();
            GlobalTrackingConfig.Enabled = true;
            GlobalTrackingConfig.TrackEmptyPropertiesOnAdditionAndDeletion = false;
            GlobalTrackingConfig.DisconnectedContext = false;
            GlobalTrackingConfig.ClearFluentConfiguration();
        }

        private const string TestConnectionString = "DefaultTestConnection";
        private readonly RandomDataGenerator _randomDataGenerator = new RandomDataGenerator();

        protected TContext Db = new TContext();

        private IDbContextTransaction _transaction;

        protected bool RollBack = true;

        protected string RandomText => _randomDataGenerator.Get<string>();

        protected int RandomNumber => _randomDataGenerator.Get<int>();

        protected DateTime RandomDate => _randomDataGenerator.Get<DateTime>();

        protected char RandomChar => _randomDataGenerator.Get<char>();

        protected ObjectFactory<TContext> ObjectFactory = new ObjectFactory<TContext>();

        void IDisposable.Dispose()
        {
            if (RollBack)
            {
                _transaction?.Rollback();
            }
            else
            {
                _transaction?.Commit();
            }
        }
    }
}
