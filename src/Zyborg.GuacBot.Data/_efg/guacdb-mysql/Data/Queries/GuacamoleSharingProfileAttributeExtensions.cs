using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace Zyborg.GuacBot.GuacDB.Data.Queries
{
    public static partial class GuacamoleSharingProfileAttributeExtensions
    {
        #region Generated Extensions
        public static IQueryable<Zyborg.GuacBot.GuacDB.Data.Entities.GuacamoleSharingProfileAttribute> BySharingProfileId(this IQueryable<Zyborg.GuacBot.GuacDB.Data.Entities.GuacamoleSharingProfileAttribute> queryable, int sharingProfileId)
        {
            return queryable.Where(q => q.SharingProfileId == sharingProfileId);
        }

        public static Zyborg.GuacBot.GuacDB.Data.Entities.GuacamoleSharingProfileAttribute GetByKey(this IQueryable<Zyborg.GuacBot.GuacDB.Data.Entities.GuacamoleSharingProfileAttribute> queryable, int sharingProfileId, string attributeName)
        {
            if (queryable is DbSet<Zyborg.GuacBot.GuacDB.Data.Entities.GuacamoleSharingProfileAttribute> dbSet)
                return dbSet.Find(sharingProfileId, attributeName);

            return queryable.FirstOrDefault(q => q.SharingProfileId == sharingProfileId
                && q.AttributeName == attributeName);
        }

        public static ValueTask<Zyborg.GuacBot.GuacDB.Data.Entities.GuacamoleSharingProfileAttribute> GetByKeyAsync(this IQueryable<Zyborg.GuacBot.GuacDB.Data.Entities.GuacamoleSharingProfileAttribute> queryable, int sharingProfileId, string attributeName)
        {
            if (queryable is DbSet<Zyborg.GuacBot.GuacDB.Data.Entities.GuacamoleSharingProfileAttribute> dbSet)
                return dbSet.FindAsync(sharingProfileId, attributeName);

            var task = queryable.FirstOrDefaultAsync(q => q.SharingProfileId == sharingProfileId
                && q.AttributeName == attributeName);
            return new ValueTask<Zyborg.GuacBot.GuacDB.Data.Entities.GuacamoleSharingProfileAttribute>(task);
        }

        #endregion

    }
}
