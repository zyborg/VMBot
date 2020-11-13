using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace Zyborg.GuacBot.GuacDB.Data.Queries
{
    public static partial class GuacamoleSystemPermissionExtensions
    {
        #region Generated Extensions
        public static IQueryable<Zyborg.GuacBot.GuacDB.Data.Entities.GuacamoleSystemPermission> ByEntityId(this IQueryable<Zyborg.GuacBot.GuacDB.Data.Entities.GuacamoleSystemPermission> queryable, int entityId)
        {
            return queryable.Where(q => q.EntityId == entityId);
        }

        public static Zyborg.GuacBot.GuacDB.Data.Entities.GuacamoleSystemPermission GetByKey(this IQueryable<Zyborg.GuacBot.GuacDB.Data.Entities.GuacamoleSystemPermission> queryable, int entityId, string permission)
        {
            if (queryable is DbSet<Zyborg.GuacBot.GuacDB.Data.Entities.GuacamoleSystemPermission> dbSet)
                return dbSet.Find(entityId, permission);

            return queryable.FirstOrDefault(q => q.EntityId == entityId
                && q.Permission == permission);
        }

        public static ValueTask<Zyborg.GuacBot.GuacDB.Data.Entities.GuacamoleSystemPermission> GetByKeyAsync(this IQueryable<Zyborg.GuacBot.GuacDB.Data.Entities.GuacamoleSystemPermission> queryable, int entityId, string permission)
        {
            if (queryable is DbSet<Zyborg.GuacBot.GuacDB.Data.Entities.GuacamoleSystemPermission> dbSet)
                return dbSet.FindAsync(entityId, permission);

            var task = queryable.FirstOrDefaultAsync(q => q.EntityId == entityId
                && q.Permission == permission);
            return new ValueTask<Zyborg.GuacBot.GuacDB.Data.Entities.GuacamoleSystemPermission>(task);
        }

        #endregion

    }
}
