using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace Zyborg.GuacBot.GuacDB.Data.Queries
{
    public static partial class GuacamoleConnectionAttributeExtensions
    {
        #region Generated Extensions
        public static IQueryable<Zyborg.GuacBot.GuacDB.Data.Entities.GuacamoleConnectionAttribute> ByConnectionId(this IQueryable<Zyborg.GuacBot.GuacDB.Data.Entities.GuacamoleConnectionAttribute> queryable, int connectionId)
        {
            return queryable.Where(q => q.ConnectionId == connectionId);
        }

        public static Zyborg.GuacBot.GuacDB.Data.Entities.GuacamoleConnectionAttribute GetByKey(this IQueryable<Zyborg.GuacBot.GuacDB.Data.Entities.GuacamoleConnectionAttribute> queryable, int connectionId, string attributeName)
        {
            if (queryable is DbSet<Zyborg.GuacBot.GuacDB.Data.Entities.GuacamoleConnectionAttribute> dbSet)
                return dbSet.Find(connectionId, attributeName);

            return queryable.FirstOrDefault(q => q.ConnectionId == connectionId
                && q.AttributeName == attributeName);
        }

        public static ValueTask<Zyborg.GuacBot.GuacDB.Data.Entities.GuacamoleConnectionAttribute> GetByKeyAsync(this IQueryable<Zyborg.GuacBot.GuacDB.Data.Entities.GuacamoleConnectionAttribute> queryable, int connectionId, string attributeName)
        {
            if (queryable is DbSet<Zyborg.GuacBot.GuacDB.Data.Entities.GuacamoleConnectionAttribute> dbSet)
                return dbSet.FindAsync(connectionId, attributeName);

            var task = queryable.FirstOrDefaultAsync(q => q.ConnectionId == connectionId
                && q.AttributeName == attributeName);
            return new ValueTask<Zyborg.GuacBot.GuacDB.Data.Entities.GuacamoleConnectionAttribute>(task);
        }

        #endregion

    }
}
