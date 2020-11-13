using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace Zyborg.GuacBot.GuacDB.Data.Queries
{
    public static partial class GuacamoleUserPasswordHistoryExtensions
    {
        #region Generated Extensions
        public static Zyborg.GuacBot.GuacDB.Data.Entities.GuacamoleUserPasswordHistory GetByKey(this IQueryable<Zyborg.GuacBot.GuacDB.Data.Entities.GuacamoleUserPasswordHistory> queryable, int passwordHistoryId)
        {
            if (queryable is DbSet<Zyborg.GuacBot.GuacDB.Data.Entities.GuacamoleUserPasswordHistory> dbSet)
                return dbSet.Find(passwordHistoryId);

            return queryable.FirstOrDefault(q => q.PasswordHistoryId == passwordHistoryId);
        }

        public static ValueTask<Zyborg.GuacBot.GuacDB.Data.Entities.GuacamoleUserPasswordHistory> GetByKeyAsync(this IQueryable<Zyborg.GuacBot.GuacDB.Data.Entities.GuacamoleUserPasswordHistory> queryable, int passwordHistoryId)
        {
            if (queryable is DbSet<Zyborg.GuacBot.GuacDB.Data.Entities.GuacamoleUserPasswordHistory> dbSet)
                return dbSet.FindAsync(passwordHistoryId);

            var task = queryable.FirstOrDefaultAsync(q => q.PasswordHistoryId == passwordHistoryId);
            return new ValueTask<Zyborg.GuacBot.GuacDB.Data.Entities.GuacamoleUserPasswordHistory>(task);
        }

        public static IQueryable<Zyborg.GuacBot.GuacDB.Data.Entities.GuacamoleUserPasswordHistory> ByUserId(this IQueryable<Zyborg.GuacBot.GuacDB.Data.Entities.GuacamoleUserPasswordHistory> queryable, int userId)
        {
            return queryable.Where(q => q.UserId == userId);
        }

        #endregion

    }
}
