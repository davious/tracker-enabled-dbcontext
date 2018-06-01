using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TrackerEnabledDbContext.Common.Configuration
{
    //https://stackoverflow.com/questions/2958921/entity-framework-4-how-to-find-the-primary-key
    internal static class DbContextExtensions
    {
        public static IEnumerable<PropertyConfiguerationKey> GetKeyNames<TEntity>(this DbContext context)
            where TEntity : class
        {
            return context.GetKeyNames(typeof(TEntity));
        }

        public static IEnumerable<PropertyConfiguerationKey> GetKeyNames(this DbContext context, Type entityType)
        {
            // Get metadata for given CLR type
            var entityMetadata = context.Model.GetEntityTypes();

            return entityMetadata.Select(x => new PropertyConfiguerationKey(x.Name, entityType.FullName));
        }
    }
}
