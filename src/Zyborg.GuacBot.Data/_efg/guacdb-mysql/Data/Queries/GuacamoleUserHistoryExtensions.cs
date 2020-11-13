using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace Zyborg.GuacBot.GuacDB.Data.Queries
{
    public static partial class GuacamoleUserHistoryExtensions
    {
        #region Generated Extensions
        public static IQueryable<Zyborg.GuacBot.GuacDB.Data.Entities.GuacamoleUserHistory> ByEndDate(this IQueryable<Zyborg.GuacBot.GuacDB.Data.Entities.GuacamoleUserHistory> queryable, DateTime? endDate)
        {
            return queryable.Where(q => (q.EndDate == endDate || (endDate == null && q.EndDate == null)));
        }

        public static Zyborg.GuacBot.GuacDB.Data.Entities.GuacamoleUserHistory GetByKey(this IQueryable<Zyborg.GuacBot.GuacDB.Data.Entities.GuacamoleUserHistory> queryable, int historyId)
        {
            if (queryable is DbSet<Zyborg.GuacBot.GuacDB.Data.Entities.GuacamoleUserHistory> dbSet)
                return dbSet.Find(historyId);

            return queryable.FirstOrDefault(q => q.HistoryId == historyId);
        }

        public static ValueTask<Zyborg.GuacBot.GuacDB.Data.Entities.GuacamoleUserHistory> GetByKeyAsync(this IQueryable<Zyborg.GuacBot.GuacDB.Data.Entities.GuacamoleUserHistory> queryable, int historyId)
        {
            if (queryable is DbSet<Zyborg.GuacBot.GuacDB.Data.Entities.GuacamoleUserHistory> dbSet)
                return dbSet.FindAsync(historyId);

            var task = queryable.FirstOrDefaultAsync(q => q.HistoryId == historyId);
            return new ValueTask<Zyborg.GuacBot.GuacDB.Data.Entities.GuacamoleUserHistory>(task);
        }

        public static IQueryable<Zyborg.GuacBot.GuacDB.Data.Entities.GuacamoleUserHistory> ByStartDate(this IQueryable<Zyborg.GuacBot.GuacDB.Data.Entities.GuacamoleUserHistory> queryable, DateTime startDate)
        {
            return queryable.Where(q => q.StartDate == startDate);
        }

        public static IQueryable<Zyborg.GuacBot.GuacDB.Data.Entities.GuacamoleUserHistory> ByUserId(this IQueryable<Zyborg.GuacBot.GuacDB.Data.Entities.GuacamoleUserHistory> queryable, int? userId)
        {
            return queryable.Where(q => (q.UserId == userId || (userId == null && q.UserId == null)));
        }

        public static IQueryable<Zyborg.GuacBot.GuacDB.Data.Entities.GuacamoleUserHistory> ByUserIdStartDate(this IQueryable<Zyborg.GuacBot.GuacDB.Data.Entities.GuacamoleUserHistory> queryable, int? userId, DateTime startDate)
        {
            return queryable.Where(q => (q.UserId == userId || (userId == null && q.UserId == null))
                && q.StartDate == startDate);
        }

        #endregion

    }
}
