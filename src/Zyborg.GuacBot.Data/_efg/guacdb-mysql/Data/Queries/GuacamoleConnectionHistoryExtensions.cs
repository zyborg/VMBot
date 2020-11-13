using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace Zyborg.GuacBot.GuacDB.Data.Queries
{
    public static partial class GuacamoleConnectionHistoryExtensions
    {
        #region Generated Extensions
        public static IQueryable<Zyborg.GuacBot.GuacDB.Data.Entities.GuacamoleConnectionHistory> ByConnectionId(this IQueryable<Zyborg.GuacBot.GuacDB.Data.Entities.GuacamoleConnectionHistory> queryable, int? connectionId)
        {
            return queryable.Where(q => (q.ConnectionId == connectionId || (connectionId == null && q.ConnectionId == null)));
        }

        public static IQueryable<Zyborg.GuacBot.GuacDB.Data.Entities.GuacamoleConnectionHistory> ByConnectionIdStartDate(this IQueryable<Zyborg.GuacBot.GuacDB.Data.Entities.GuacamoleConnectionHistory> queryable, int? connectionId, DateTime startDate)
        {
            return queryable.Where(q => (q.ConnectionId == connectionId || (connectionId == null && q.ConnectionId == null))
                && q.StartDate == startDate);
        }

        public static IQueryable<Zyborg.GuacBot.GuacDB.Data.Entities.GuacamoleConnectionHistory> ByEndDate(this IQueryable<Zyborg.GuacBot.GuacDB.Data.Entities.GuacamoleConnectionHistory> queryable, DateTime? endDate)
        {
            return queryable.Where(q => (q.EndDate == endDate || (endDate == null && q.EndDate == null)));
        }

        public static Zyborg.GuacBot.GuacDB.Data.Entities.GuacamoleConnectionHistory GetByKey(this IQueryable<Zyborg.GuacBot.GuacDB.Data.Entities.GuacamoleConnectionHistory> queryable, int historyId)
        {
            if (queryable is DbSet<Zyborg.GuacBot.GuacDB.Data.Entities.GuacamoleConnectionHistory> dbSet)
                return dbSet.Find(historyId);

            return queryable.FirstOrDefault(q => q.HistoryId == historyId);
        }

        public static ValueTask<Zyborg.GuacBot.GuacDB.Data.Entities.GuacamoleConnectionHistory> GetByKeyAsync(this IQueryable<Zyborg.GuacBot.GuacDB.Data.Entities.GuacamoleConnectionHistory> queryable, int historyId)
        {
            if (queryable is DbSet<Zyborg.GuacBot.GuacDB.Data.Entities.GuacamoleConnectionHistory> dbSet)
                return dbSet.FindAsync(historyId);

            var task = queryable.FirstOrDefaultAsync(q => q.HistoryId == historyId);
            return new ValueTask<Zyborg.GuacBot.GuacDB.Data.Entities.GuacamoleConnectionHistory>(task);
        }

        public static IQueryable<Zyborg.GuacBot.GuacDB.Data.Entities.GuacamoleConnectionHistory> BySharingProfileId(this IQueryable<Zyborg.GuacBot.GuacDB.Data.Entities.GuacamoleConnectionHistory> queryable, int? sharingProfileId)
        {
            return queryable.Where(q => (q.SharingProfileId == sharingProfileId || (sharingProfileId == null && q.SharingProfileId == null)));
        }

        public static IQueryable<Zyborg.GuacBot.GuacDB.Data.Entities.GuacamoleConnectionHistory> ByStartDate(this IQueryable<Zyborg.GuacBot.GuacDB.Data.Entities.GuacamoleConnectionHistory> queryable, DateTime startDate)
        {
            return queryable.Where(q => q.StartDate == startDate);
        }

        public static IQueryable<Zyborg.GuacBot.GuacDB.Data.Entities.GuacamoleConnectionHistory> ByUserId(this IQueryable<Zyborg.GuacBot.GuacDB.Data.Entities.GuacamoleConnectionHistory> queryable, int? userId)
        {
            return queryable.Where(q => (q.UserId == userId || (userId == null && q.UserId == null)));
        }

        #endregion

    }
}
