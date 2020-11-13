using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace Zyborg.GuacBot.GuacDB.Data.Queries
{
    public static partial class GuacamoleConnectionParameterExtensions
    {
        #region Generated Extensions
        public static IQueryable<Zyborg.GuacBot.GuacDB.Data.Entities.GuacamoleConnectionParameter> ByConnectionId(this IQueryable<Zyborg.GuacBot.GuacDB.Data.Entities.GuacamoleConnectionParameter> queryable, int connectionId)
        {
            return queryable.Where(q => q.ConnectionId == connectionId);
        }

        public static Zyborg.GuacBot.GuacDB.Data.Entities.GuacamoleConnectionParameter GetByKey(this IQueryable<Zyborg.GuacBot.GuacDB.Data.Entities.GuacamoleConnectionParameter> queryable, int connectionId, string parameterName)
        {
            if (queryable is DbSet<Zyborg.GuacBot.GuacDB.Data.Entities.GuacamoleConnectionParameter> dbSet)
                return dbSet.Find(connectionId, parameterName);

            return queryable.FirstOrDefault(q => q.ConnectionId == connectionId
                && q.ParameterName == parameterName);
        }

        public static ValueTask<Zyborg.GuacBot.GuacDB.Data.Entities.GuacamoleConnectionParameter> GetByKeyAsync(this IQueryable<Zyborg.GuacBot.GuacDB.Data.Entities.GuacamoleConnectionParameter> queryable, int connectionId, string parameterName)
        {
            if (queryable is DbSet<Zyborg.GuacBot.GuacDB.Data.Entities.GuacamoleConnectionParameter> dbSet)
                return dbSet.FindAsync(connectionId, parameterName);

            var task = queryable.FirstOrDefaultAsync(q => q.ConnectionId == connectionId
                && q.ParameterName == parameterName);
            return new ValueTask<Zyborg.GuacBot.GuacDB.Data.Entities.GuacamoleConnectionParameter>(task);
        }

        #endregion

    }
}
