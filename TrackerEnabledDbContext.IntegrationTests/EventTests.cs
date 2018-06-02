using Xunit;
using TrackerEnabledDbContext.Common.Configuration;
using TrackerEnabledDbContext.Common.Models;
using TrackerEnabledDbContext.Common.Testing;
using TrackerEnabledDbContext.Common.Testing.Extensions;
using TrackerEnabledDbContext.Common.Testing.Models;

namespace TrackerEnabledDbContext.IntegrationTests
{
    public class EventTests : PersistanceTests<TestTrackerContext>
    {
        [Fact]
        public void CanRaiseAddEvent()
        {
            using (var context = GetNewContextInstance())
            {
                EntityTracker.TrackAllProperties<TrackedModelWithMultipleProperties>();

                bool eventRaised = false;

                context.OnAuditLogGenerated += (sender, args) =>
                {
                    var eventEntity = args.Entity as TrackedModelWithMultipleProperties;

                    if (args.Log.EventType == EventType.Added &&
                        args.Log.TypeFullName == typeof (TrackedModelWithMultipleProperties).FullName &&
                        eventEntity != null)
                    {
                        eventRaised = true;
                    }
                };

                var entity = ObjectFactory.Create<TrackedModelWithMultipleProperties>(false);

                entity.Description = RandomText;

                context.TrackedModelsWithMultipleProperties.Add(entity);

                context.SaveChanges();

                //assert
                Assert.True(eventRaised);

                //make sure log is saved in database
                entity.AssertAuditForAddition(context, entity.Id, null,
                    x => x.Id,
                    x => x.Description);
            }
        }

        [Fact]
        public void CanRaiseModifyEvent()
        {
            //TODO: modify test tracker context and identity test tracker context so that on disposal they revert the changes
            using (var context = GetNewContextInstance())
            {
                EntityTracker.TrackAllProperties<TrackedModelWithMultipleProperties>();

                bool modifyEventRaised = false;

                context.OnAuditLogGenerated += (sender, args) =>
                {
                    var eventEntity = args.Entity as TrackedModelWithMultipleProperties;

                    if (args.Log.EventType == EventType.Modified &&
                        args.Log.TypeFullName == typeof(TrackedModelWithMultipleProperties).FullName &&
                        eventEntity != null)
                    {
                        modifyEventRaised = true;
                    }
                };

                var existingEntity = ObjectFactory
                    .Create<TrackedModelWithMultipleProperties>(save: true, testDbContext:context);

                string originalValue = existingEntity.Name;
                existingEntity.Name = RandomText;

                context.SaveChanges();

                //assert
                Assert.True(modifyEventRaised);

                existingEntity.AssertAuditForModification(context, existingEntity.Id, null,
                    new AuditLogDetail
                    {
                        PropertyName = nameof(existingEntity.Name),
                        OriginalValue = originalValue,
                        NewValue = existingEntity.Name
                    });
            }
        }

        [Fact]
        public void CanRaiseDeleteEvent()
        {
            using (var context = GetNewContextInstance())
            {
                EntityTracker.TrackAllProperties<NormalModel>();

                bool eventRaised = false;

                context.OnAuditLogGenerated += (sender, args) =>
                {
                    var eventEntity = args.Entity as NormalModel;

                    if (args.Log.EventType == EventType.Deleted &&
                        args.Log.TypeFullName == typeof(NormalModel).FullName &&
                        eventEntity != null)
                    {
                        eventRaised = true;
                    }
                };

                var existingEntity = ObjectFactory
                    .Create<NormalModel>(save: true, testDbContext: context);

                context.NormalModels.Remove(existingEntity);
                context.SaveChanges();

                //assert
                Assert.True(eventRaised);

                existingEntity.AssertAuditForDeletion(context, existingEntity.Id, null,
                    x => x.Description,
                    x => x.Id);
            }
        }

        [Fact]
        public void CanRaiseSoftDeleteEvent()
        {
            GlobalTrackingConfig.SetSoftDeletableCriteria<ISoftDeletable>
                (x=>x.IsDeleted);

            using (var context = GetNewContextInstance())
            {
                EntityTracker.TrackAllProperties<SoftDeletableModel>();

                bool eventRaised = false;

                context.OnAuditLogGenerated += (sender, args) =>
                {
                    var eventEntity = args.Entity as SoftDeletableModel;

                    if (args.Log.EventType == EventType.SoftDeleted &&
                        args.Log.TypeFullName == typeof(SoftDeletableModel).FullName &&
                        eventEntity != null)
                    {
                        eventRaised = true;
                    }
                };

                var existingEntity = ObjectFactory
                    .Create<SoftDeletableModel>(save: true, testDbContext: context);

                existingEntity.Delete();

                context.SaveChanges();

                //assert
                Assert.True(eventRaised);

                existingEntity.AssertAuditForSoftDeletion(context, existingEntity.Id, null,
                    new AuditLogDetail
                    {
                        PropertyName = nameof(existingEntity.IsDeleted),
                        OriginalValue = false.ToString(),
                        NewValue = true.ToString()
                    });
            }
        }

        [Fact]
        public void CanRaiseUnDeleteEvent()
        {
            GlobalTrackingConfig.SetSoftDeletableCriteria<ISoftDeletable>
                (x => x.IsDeleted);

            using (var context = GetNewContextInstance())
            {
                EntityTracker.TrackAllProperties<SoftDeletableModel>();

                bool eventRaised = false;

                context.OnAuditLogGenerated += (sender, args) =>
                {
                    var eventEntity = args.Entity as SoftDeletableModel;

                    if (args.Log.EventType == EventType.UnDeleted &&
                        args.Log.TypeFullName == typeof(SoftDeletableModel).FullName &&
                        eventEntity != null)
                    {
                        eventRaised = true;
                    }
                };

                var existingEntity = ObjectFactory
                    .Create<SoftDeletableModel>(save: true, testDbContext: context);
                
                existingEntity.Delete();

                context.SaveChanges();

                //now undelete
                existingEntity.IsDeleted = false;
                context.SaveChanges();

                //assert
                Assert.True(eventRaised);

                existingEntity.AssertAuditForUndeletion(context, existingEntity.Id, null,
                    new AuditLogDetail
                    {
                        PropertyName = nameof(existingEntity.IsDeleted),
                        OriginalValue = true.ToString(),
                        NewValue = false.ToString()
                    });
            }
        }

        [Fact]
        public void CanSkipTrackingUsingEvent()
        {
            using (var context = GetNewContextInstance())
            {
                EntityTracker.TrackAllProperties<TrackedModelWithMultipleProperties>();

                bool eventRaised = false;

                context.OnAuditLogGenerated += (sender, args) =>
                {
                    var eventEntity = args.Entity as TrackedModelWithMultipleProperties;

                    if (args.Log.EventType == EventType.Added &&
                        args.Log.TypeFullName == typeof(TrackedModelWithMultipleProperties).FullName &&
                        eventEntity != null)
                    {
                        eventRaised = true;
                        args.SkipSavingLog = true;
                    }
                };

                var entity = ObjectFactory.Create<TrackedModelWithMultipleProperties>(save:true, testDbContext:context);

                //assert
                Assert.True(eventRaised);

                //make sure log is saved in database
                entity.AssertNoLogs(context, entity.Id, EventType.Added);
            }
        }

        [Fact]
        public void CanChangeEntityInEvent()
        {
            using (var context = GetNewContextInstance())
            {
                EntityTracker.TrackAllProperties<TrackedModelWithMultipleProperties>();

                bool eventRaised = false;

                string modifiedValue = RandomText;

                context.OnAuditLogGenerated += (sender, args) =>
                {
                    var eventEntity = args.Entity as TrackedModelWithMultipleProperties;

                    if (args.Log.EventType == EventType.Modified &&
                        args.Log.TypeFullName == typeof(TrackedModelWithMultipleProperties).FullName &&
                        eventEntity != null)
                    {
                        eventEntity.Name = modifiedValue;
                        eventRaised = true;
                    }
                };

                var existingEntity = ObjectFactory
                    .Create<TrackedModelWithMultipleProperties>(save: true, testDbContext: context);

                string originalValue = existingEntity.Name;
                string newValue = RandomText;
                existingEntity.Name = newValue;

                context.SaveChanges();

                //assert
                Assert.True(eventRaised);

                existingEntity.AssertAuditForModification(context, existingEntity.Id, null,
                    new AuditLogDetail
                    {
                        PropertyName = nameof(existingEntity.Name),
                        OriginalValue = originalValue,
                        NewValue = newValue
                    });

                context.Entry(existingEntity).Reload();
                Assert.Equal(existingEntity.Name, modifiedValue);
            }
        }

        private TestTrackerContext GetNewContextInstance()
        {
            return new TestTrackerContext();
        }
    }
}
