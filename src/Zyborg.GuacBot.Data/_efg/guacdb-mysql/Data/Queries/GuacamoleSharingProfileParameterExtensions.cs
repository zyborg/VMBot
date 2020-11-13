using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace Zyborg.GuacBot.GuacDB.Data.Queries
{
    public static partial class GuacamoleSharingProfileParameterExtensions
    {
        #region Generated Extensions
        public static IQueryable<Zyborg.GuacBot.GuacDB.Data.Entities.GuacamoleSharingProfileParameter> BySharingProfileId(this IQueryable<Zyborg.GuacBot.GuacDB.Data.Entities.GuacamoleSharingProfileParameter> queryable, int sharingProfileId)
        {
            return queryable.Where(q => q.SharingProfileId == sharingProfileId);
        }

        public static Zyborg.GuacBot.GuacDB.Data.Entities.GuacamoleSharingProfileParameter GetByKey(this IQueryable<Zyborg.GuacBot.GuacDB.Data.Entities.GuacamoleSharingProfileParameter> queryable, int sharingProfileId, string parameterName)
        {
            if (queryable is DbSet<Zyborg.GuacBot.GuacDB.Data.Entities.GuacamoleSharingProfileParameter> dbSet)
                return dbSet.Find(sharingProfileId, parameterName);

            return queryable.FirstOrDefault(q => q.SharingProfileId == sharingProfileId
                && q.ParameterName == parameterName);
        }

        public static ValueTask<Zyborg.GuacBot.GuacDB.Data.Entities.GuacamoleSharingProfileParameter> GetByKeyAsync(this IQueryable<Zyborg.GuacBot.GuacDB.Data.Entities.GuacamoleSharingProfileParameter> queryable, int sharingProfileId, string parameterName)
        {
            if (queryable is DbSet<Zyborg.GuacBot.GuacDB.Data.Entities.GuacamoleSharingProfileParameter> dbSet)
                return dbSet.FindAsync(sharingProfileId, parameterName);

            var task = queryable.FirstOrDefaultAsync(q => q.SharingProfileId == sharingProfileId
                && q.ParameterName == parameterName);
            return new ValueTask<Zyborg.GuacBot.GuacDB.Data.Entities.GuacamoleSharingProfileParameter>(task);
        }

        #endregion

    }
}
