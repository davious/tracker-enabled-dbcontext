﻿using Xunit;
using TrackerEnabledDbContext.Common.Testing;
using TrackerEnabledDbContext.Common.Testing.Extensions;
using TrackerEnabledDbContext.Common.Testing.Models;

namespace TrackerEnabledDbContext.Identity.IntegrationTests
{
    public class UsernameConfigurationTests : PersistanceTests<TestTrackerIdentityContext>
    {
        [Fact]
        public void Can_use_username_factory()
        {
            Db.ConfigureUsername(()=> "bilal");

            NormalModel model = 
                ObjectFactory.Create<NormalModel>();
            Db.NormalModels.Add(model);
            Db.SaveChanges();
            model.Id.AssertIsNotZero();

            model.AssertAuditForAddition(Db, model.Id,
                "bilal",
                x => x.Id,
                x => x.Description
                );
        }

        [Fact]
        public void Can_username_factory_override_default_username()
        {
            Db.ConfigureUsername(() => "bilal");
            Db.ConfigureUsername("rahul");

            NormalModel model =
                ObjectFactory.Create<NormalModel>();
            Db.NormalModels.Add(model);
            Db.SaveChanges();
            model.Id.AssertIsNotZero();

            model.AssertAuditForAddition(Db, model.Id,
                "bilal",
                x => x.Id,
                x => x.Description
                );
        }

        [Fact]
        public void Can_use_default_username()
        {
            Db.ConfigureUsername("rahul");

            NormalModel model =
                ObjectFactory.Create<NormalModel>();
            Db.NormalModels.Add(model);
            Db.SaveChanges();
            model.Id.AssertIsNotZero();

            model.AssertAuditForAddition(Db, model.Id,
                "rahul",
                x => x.Id,
                x => x.Description
                );
        }
    }
}