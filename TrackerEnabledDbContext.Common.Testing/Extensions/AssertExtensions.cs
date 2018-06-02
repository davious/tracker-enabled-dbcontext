using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace TrackerEnabledDbContext.Common.Testing.Extensions
{
    public static class AssertExtensions
    {
        public static bool AssertTrue(this bool value, string errorMessage = null)
        {
            Assert.True(value, errorMessage);
            return value;
        }

        public static int AssertIsNotZero(this int value, string errorMessage = null)
        {
            Assert.NotEqual(0, value);
            return value;
        }

        public static long AssertIsNotZero(this long value, string errorMessage = null)
        {
            Assert.NotEqual(0, value);
            return value;
        }

        public static double AssertIsNotZero(this double value, string errorMessage = null)
        {
            Assert.NotEqual(0, value);
            return value;
        }

        public static IEnumerable<T> AssertCountIsNotZero<T>(this IEnumerable<T> collection, string errorMessage = null)
        {
            if (!collection.Any()) Assert.True(false, errorMessage ?? "collection has zero records");
            return collection;
        }

        public static IEnumerable<T> AssertAny<T>(this IEnumerable<T> collection, Func<T, bool> predicate,
            string errorMessage = null)
        {
            if (!collection.Any(predicate)) Assert.True(false, errorMessage);
            return collection;
        }

        public static IEnumerable<T> AssertCount<T>(this IEnumerable<T> collection, int expectedCount,
            string errorMessage = null)
        {
            Assert.Equal(expectedCount, collection.Count());
            return collection;
        }

        public static T AssertIsNotNull<T>(this T value, string errorMessage = null)
        {
            Assert.NotNull(value);
            return value;
        }
    }
}