using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace Zyborg.GuacBot.GuacDB.Data.Queries
{
    public static partial class GuacamoleConnectionExtensions
    {
        #region Generated Extensions
        public static Zyborg.GuacBot.GuacDB.Data.Entities.GuacamoleConnection GetByKey(this IQueryable<Zyborg.GuacBot.GuacDB.Data.Entities.GuacamoleConnection> queryable, int connectionId)
        {
            if (queryable is DbSet<Zyborg.GuacBot.GuacDB.Data.Entities.GuacamoleConnection> dbSet)
                return dbSet.Find(connectionId);

            return queryable.FirstOrDefault(q => q.ConnectionId == connectionId);
        }

        public static ValueTask<Zyborg.GuacBot.GuacDB.Data.Entities.GuacamoleConnection> GetByKeyAsync(this IQueryable<Zyborg.GuacBot.GuacDB.Data.Entities.GuacamoleConnection> queryable, int connectionId)
        {
            if (queryable is DbSet<Zyborg.GuacBot.GuacDB.Data.Entities.GuacamoleConnection> dbSet)
                return dbSet.FindAsync(connectionId);

            var task = queryable.FirstOrDefaultAsync(q => q.ConnectionId == connectionId);
            return new ValueTask<Zyborg.GuacBot.GuacDB.Data.Entities.GuacamoleConnection>(task);
        }

        public static Zyborg.GuacBot.GuacDB.Data.Entities.GuacamoleConnection GetByConnectionNameParentId(this IQueryable<Zyborg.GuacBot.GuacDB.Data.Entities.GuacamoleConnection> queryable, string connectionName, int? parentId)
        {
            return queryable.FirstOrDefault(q => q.ConnectionName == connectionName
                && (q.ParentId == parentId || (parentId == null && q.ParentId == null)));
        }

        public static Task<Zyborg.GuacBot.GuacDB.Data.Entities.GuacamoleConnection> GetByConnectionNameParentIdAsync(this IQueryable<Zyborg.GuacBot.GuacDB.Data.Entities.GuacamoleConnection> queryable, string connectionName, int? parentId)
        {
            return queryable.FirstOrDefaultAsync(q => q.ConnectionName == connectionName
                && (q.ParentId == parentId || (parentId == null && q.ParentId == null)));
        }

        public static IQueryable<Zyborg.GuacBot.GuacDB.Data.Entities.GuacamoleConnection> ByParentId(this IQueryable<Zyborg.GuacBot.GuacDB.Data.Entities.GuacamoleConnection> queryable, int? parentId)
        {
            return queryable.Where(q => (q.ParentId == parentId || (parentId == null && q.ParentId == null)));
        }

        #endregion

    }
}
