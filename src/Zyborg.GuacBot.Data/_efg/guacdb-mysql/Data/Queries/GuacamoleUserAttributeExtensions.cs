using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace Zyborg.GuacBot.GuacDB.Data.Queries
{
    public static partial class GuacamoleUserAttributeExtensions
    {
        #region Generated Extensions
        public static IQueryable<Zyborg.GuacBot.GuacDB.Data.Entities.GuacamoleUserAttribute> ByUserId(this IQueryable<Zyborg.GuacBot.GuacDB.Data.Entities.GuacamoleUserAttribute> queryable, int userId)
        {
            return queryable.Where(q => q.UserId == userId);
        }

        public static Zyborg.GuacBot.GuacDB.Data.Entities.GuacamoleUserAttribute GetByKey(this IQueryable<Zyborg.GuacBot.GuacDB.Data.Entities.GuacamoleUserAttribute> queryable, int userId, string attributeName)
        {
            if (queryable is DbSet<Zyborg.GuacBot.GuacDB.Data.Entities.GuacamoleUserAttribute> dbSet)
                return dbSet.Find(userId, attributeName);

            return queryable.FirstOrDefault(q => q.UserId == userId
                && q.AttributeName == attributeName);
        }

        public static ValueTask<Zyborg.GuacBot.GuacDB.Data.Entities.GuacamoleUserAttribute> GetByKeyAsync(this IQueryable<Zyborg.GuacBot.GuacDB.Data.Entities.GuacamoleUserAttribute> queryable, int userId, string attributeName)
        {
            if (queryable is DbSet<Zyborg.GuacBot.GuacDB.Data.Entities.GuacamoleUserAttribute> dbSet)
                return dbSet.FindAsync(userId, attributeName);

            var task = queryable.FirstOrDefaultAsync(q => q.UserId == userId
                && q.AttributeName == attributeName);
            return new ValueTask<Zyborg.GuacBot.GuacDB.Data.Entities.GuacamoleUserAttribute>(task);
        }

        #endregion

    }
}
