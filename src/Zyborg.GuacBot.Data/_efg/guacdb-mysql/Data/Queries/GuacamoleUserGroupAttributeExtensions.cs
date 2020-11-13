using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace Zyborg.GuacBot.GuacDB.Data.Queries
{
    public static partial class GuacamoleUserGroupAttributeExtensions
    {
        #region Generated Extensions
        public static IQueryable<Zyborg.GuacBot.GuacDB.Data.Entities.GuacamoleUserGroupAttribute> ByUserGroupId(this IQueryable<Zyborg.GuacBot.GuacDB.Data.Entities.GuacamoleUserGroupAttribute> queryable, int userGroupId)
        {
            return queryable.Where(q => q.UserGroupId == userGroupId);
        }

        public static Zyborg.GuacBot.GuacDB.Data.Entities.GuacamoleUserGroupAttribute GetByKey(this IQueryable<Zyborg.GuacBot.GuacDB.Data.Entities.GuacamoleUserGroupAttribute> queryable, int userGroupId, string attributeName)
        {
            if (queryable is DbSet<Zyborg.GuacBot.GuacDB.Data.Entities.GuacamoleUserGroupAttribute> dbSet)
                return dbSet.Find(userGroupId, attributeName);

            return queryable.FirstOrDefault(q => q.UserGroupId == userGroupId
                && q.AttributeName == attributeName);
        }

        public static ValueTask<Zyborg.GuacBot.GuacDB.Data.Entities.GuacamoleUserGroupAttribute> GetByKeyAsync(this IQueryable<Zyborg.GuacBot.GuacDB.Data.Entities.GuacamoleUserGroupAttribute> queryable, int userGroupId, string attributeName)
        {
            if (queryable is DbSet<Zyborg.GuacBot.GuacDB.Data.Entities.GuacamoleUserGroupAttribute> dbSet)
                return dbSet.FindAsync(userGroupId, attributeName);

            var task = queryable.FirstOrDefaultAsync(q => q.UserGroupId == userGroupId
                && q.AttributeName == attributeName);
            return new ValueTask<Zyborg.GuacBot.GuacDB.Data.Entities.GuacamoleUserGroupAttribute>(task);
        }

        #endregion

    }
}
