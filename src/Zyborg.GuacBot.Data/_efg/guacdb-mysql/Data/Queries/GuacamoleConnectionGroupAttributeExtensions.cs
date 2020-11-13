using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace Zyborg.GuacBot.GuacDB.Data.Queries
{
    public static partial class GuacamoleConnectionGroupAttributeExtensions
    {
        #region Generated Extensions
        public static IQueryable<Zyborg.GuacBot.GuacDB.Data.Entities.GuacamoleConnectionGroupAttribute> ByConnectionGroupId(this IQueryable<Zyborg.GuacBot.GuacDB.Data.Entities.GuacamoleConnectionGroupAttribute> queryable, int connectionGroupId)
        {
            return queryable.Where(q => q.ConnectionGroupId == connectionGroupId);
        }

        public static Zyborg.GuacBot.GuacDB.Data.Entities.GuacamoleConnectionGroupAttribute GetByKey(this IQueryable<Zyborg.GuacBot.GuacDB.Data.Entities.GuacamoleConnectionGroupAttribute> queryable, int connectionGroupId, string attributeName)
        {
            if (queryable is DbSet<Zyborg.GuacBot.GuacDB.Data.Entities.GuacamoleConnectionGroupAttribute> dbSet)
                return dbSet.Find(connectionGroupId, attributeName);

            return queryable.FirstOrDefault(q => q.ConnectionGroupId == connectionGroupId
                && q.AttributeName == attributeName);
        }

        public static ValueTask<Zyborg.GuacBot.GuacDB.Data.Entities.GuacamoleConnectionGroupAttribute> GetByKeyAsync(this IQueryable<Zyborg.GuacBot.GuacDB.Data.Entities.GuacamoleConnectionGroupAttribute> queryable, int connectionGroupId, string attributeName)
        {
            if (queryable is DbSet<Zyborg.GuacBot.GuacDB.Data.Entities.GuacamoleConnectionGroupAttribute> dbSet)
                return dbSet.FindAsync(connectionGroupId, attributeName);

            var task = queryable.FirstOrDefaultAsync(q => q.ConnectionGroupId == connectionGroupId
                && q.AttributeName == attributeName);
            return new ValueTask<Zyborg.GuacBot.GuacDB.Data.Entities.GuacamoleConnectionGroupAttribute>(task);
        }

        #endregion

    }
}
