using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using TrackingCore.Interfaces;

namespace TrackingCore.Configuration
{
    internal static class EntityTrackingConfiguration
    {
        internal static bool IsTrackingEnabled(Type entityType)
        {
            TrackingConfigurationValue value = TrackingDataStore.EntityConfigStore.GetOrAdd(
            entityType.FullName,
            (key) => EntityConfigValueFactory(key, entityType)
            );

            return value.Value;
        }

        internal static TrackingConfigurationValue EntityConfigValueFactory(string key, Type entityType)
        {
            TrackChangesAttribute trackChangesAttribute =
                entityType.GetCustomAttributes(true).OfType<TrackChangesAttribute>().SingleOrDefault();
            bool value = trackChangesAttribute != null;

            return new TrackingConfigurationValue(value);
        }
    }
}