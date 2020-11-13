using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace Zyborg.GuacBot.GuacDB.Data.Queries
{
    public static partial class GuacamoleUserExtensions
    {
        #region Generated Extensions
        public static Zyborg.GuacBot.GuacDB.Data.Entities.GuacamoleUser GetByEntityId(this IQueryable<Zyborg.GuacBot.GuacDB.Data.Entities.GuacamoleUser> queryable, int entityId)
        {
            return queryable.FirstOrDefault(q => q.EntityId == entityId);
        }

        public static Task<Zyborg.GuacBot.GuacDB.Data.Entities.GuacamoleUser> GetByEntityIdAsync(this IQueryable<Zyborg.GuacBot.GuacDB.Data.Entities.GuacamoleUser> queryable, int entityId)
        {
            return queryable.FirstOrDefaultAsync(q => q.EntityId == entityId);
        }

        public static Zyborg.GuacBot.GuacDB.Data.Entities.GuacamoleUser GetByKey(this IQueryable<Zyborg.GuacBot.GuacDB.Data.Entities.GuacamoleUser> queryable, int userId)
        {
            if (queryable is DbSet<Zyborg.GuacBot.GuacDB.Data.Entities.GuacamoleUser> dbSet)
                return dbSet.Find(userId);

            return queryable.FirstOrDefault(q => q.UserId == userId);
        }

        public static ValueTask<Zyborg.GuacBot.GuacDB.Data.Entities.GuacamoleUser> GetByKeyAsync(this IQueryable<Zyborg.GuacBot.GuacDB.Data.Entities.GuacamoleUser> queryable, int userId)
        {
            if (queryable is DbSet<Zyborg.GuacBot.GuacDB.Data.Entities.GuacamoleUser> dbSet)
                return dbSet.FindAsync(userId);

            var task = queryable.FirstOrDefaultAsync(q => q.UserId == userId);
            return new ValueTask<Zyborg.GuacBot.GuacDB.Data.Entities.GuacamoleUser>(task);
        }

        #endregion

    }
}
