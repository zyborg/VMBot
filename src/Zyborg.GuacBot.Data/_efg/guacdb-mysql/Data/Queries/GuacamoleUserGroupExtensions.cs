using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace Zyborg.GuacBot.GuacDB.Data.Queries
{
    public static partial class GuacamoleUserGroupExtensions
    {
        #region Generated Extensions
        public static Zyborg.GuacBot.GuacDB.Data.Entities.GuacamoleUserGroup GetByEntityId(this IQueryable<Zyborg.GuacBot.GuacDB.Data.Entities.GuacamoleUserGroup> queryable, int entityId)
        {
            return queryable.FirstOrDefault(q => q.EntityId == entityId);
        }

        public static Task<Zyborg.GuacBot.GuacDB.Data.Entities.GuacamoleUserGroup> GetByEntityIdAsync(this IQueryable<Zyborg.GuacBot.GuacDB.Data.Entities.GuacamoleUserGroup> queryable, int entityId)
        {
            return queryable.FirstOrDefaultAsync(q => q.EntityId == entityId);
        }

        public static Zyborg.GuacBot.GuacDB.Data.Entities.GuacamoleUserGroup GetByKey(this IQueryable<Zyborg.GuacBot.GuacDB.Data.Entities.GuacamoleUserGroup> queryable, int userGroupId)
        {
            if (queryable is DbSet<Zyborg.GuacBot.GuacDB.Data.Entities.GuacamoleUserGroup> dbSet)
                return dbSet.Find(userGroupId);

            return queryable.FirstOrDefault(q => q.UserGroupId == userGroupId);
        }

        public static ValueTask<Zyborg.GuacBot.GuacDB.Data.Entities.GuacamoleUserGroup> GetByKeyAsync(this IQueryable<Zyborg.GuacBot.GuacDB.Data.Entities.GuacamoleUserGroup> queryable, int userGroupId)
        {
            if (queryable is DbSet<Zyborg.GuacBot.GuacDB.Data.Entities.GuacamoleUserGroup> dbSet)
                return dbSet.FindAsync(userGroupId);

            var task = queryable.FirstOrDefaultAsync(q => q.UserGroupId == userGroupId);
            return new ValueTask<Zyborg.GuacBot.GuacDB.Data.Entities.GuacamoleUserGroup>(task);
        }

        #endregion

    }
}
