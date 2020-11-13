using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace Zyborg.GuacBot.GuacDB.Data.Queries
{
    public static partial class GuacamoleSharingProfileExtensions
    {
        #region Generated Extensions
        public static IQueryable<Zyborg.GuacBot.GuacDB.Data.Entities.GuacamoleSharingProfile> ByPrimaryConnectionId(this IQueryable<Zyborg.GuacBot.GuacDB.Data.Entities.GuacamoleSharingProfile> queryable, int primaryConnectionId)
        {
            return queryable.Where(q => q.PrimaryConnectionId == primaryConnectionId);
        }

        public static Zyborg.GuacBot.GuacDB.Data.Entities.GuacamoleSharingProfile GetByKey(this IQueryable<Zyborg.GuacBot.GuacDB.Data.Entities.GuacamoleSharingProfile> queryable, int sharingProfileId)
        {
            if (queryable is DbSet<Zyborg.GuacBot.GuacDB.Data.Entities.GuacamoleSharingProfile> dbSet)
                return dbSet.Find(sharingProfileId);

            return queryable.FirstOrDefault(q => q.SharingProfileId == sharingProfileId);
        }

        public static ValueTask<Zyborg.GuacBot.GuacDB.Data.Entities.GuacamoleSharingProfile> GetByKeyAsync(this IQueryable<Zyborg.GuacBot.GuacDB.Data.Entities.GuacamoleSharingProfile> queryable, int sharingProfileId)
        {
            if (queryable is DbSet<Zyborg.GuacBot.GuacDB.Data.Entities.GuacamoleSharingProfile> dbSet)
                return dbSet.FindAsync(sharingProfileId);

            var task = queryable.FirstOrDefaultAsync(q => q.SharingProfileId == sharingProfileId);
            return new ValueTask<Zyborg.GuacBot.GuacDB.Data.Entities.GuacamoleSharingProfile>(task);
        }

        public static Zyborg.GuacBot.GuacDB.Data.Entities.GuacamoleSharingProfile GetBySharingProfileNamePrimaryConnectionId(this IQueryable<Zyborg.GuacBot.GuacDB.Data.Entities.GuacamoleSharingProfile> queryable, string sharingProfileName, int primaryConnectionId)
        {
            return queryable.FirstOrDefault(q => q.SharingProfileName == sharingProfileName
                && q.PrimaryConnectionId == primaryConnectionId);
        }

        public static Task<Zyborg.GuacBot.GuacDB.Data.Entities.GuacamoleSharingProfile> GetBySharingProfileNamePrimaryConnectionIdAsync(this IQueryable<Zyborg.GuacBot.GuacDB.Data.Entities.GuacamoleSharingProfile> queryable, string sharingProfileName, int primaryConnectionId)
        {
            return queryable.FirstOrDefaultAsync(q => q.SharingProfileName == sharingProfileName
                && q.PrimaryConnectionId == primaryConnectionId);
        }

        #endregion

    }
}
