using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace Zyborg.GuacBot.GuacDB.Data.Queries
{
    public static partial class GuacamoleConnectionGroupExtensions
    {
        #region Generated Extensions
        public static Zyborg.GuacBot.GuacDB.Data.Entities.GuacamoleConnectionGroup GetByKey(this IQueryable<Zyborg.GuacBot.GuacDB.Data.Entities.GuacamoleConnectionGroup> queryable, int connectionGroupId)
        {
            if (queryable is DbSet<Zyborg.GuacBot.GuacDB.Data.Entities.GuacamoleConnectionGroup> dbSet)
                return dbSet.Find(connectionGroupId);

            return queryable.FirstOrDefault(q => q.ConnectionGroupId == connectionGroupId);
        }

        public static ValueTask<Zyborg.GuacBot.GuacDB.Data.Entities.GuacamoleConnectionGroup> GetByKeyAsync(this IQueryable<Zyborg.GuacBot.GuacDB.Data.Entities.GuacamoleConnectionGroup> queryable, int connectionGroupId)
        {
            if (queryable is DbSet<Zyborg.GuacBot.GuacDB.Data.Entities.GuacamoleConnectionGroup> dbSet)
                return dbSet.FindAsync(connectionGroupId);

            var task = queryable.FirstOrDefaultAsync(q => q.ConnectionGroupId == connectionGroupId);
            return new ValueTask<Zyborg.GuacBot.GuacDB.Data.Entities.GuacamoleConnectionGroup>(task);
        }

        public static Zyborg.GuacBot.GuacDB.Data.Entities.GuacamoleConnectionGroup GetByConnectionGroupNameParentId(this IQueryable<Zyborg.GuacBot.GuacDB.Data.Entities.GuacamoleConnectionGroup> queryable, string connectionGroupName, int? parentId)
        {
            return queryable.FirstOrDefault(q => q.ConnectionGroupName == connectionGroupName
                && (q.ParentId == parentId || (parentId == null && q.ParentId == null)));
        }

        public static Task<Zyborg.GuacBot.GuacDB.Data.Entities.GuacamoleConnectionGroup> GetByConnectionGroupNameParentIdAsync(this IQueryable<Zyborg.GuacBot.GuacDB.Data.Entities.GuacamoleConnectionGroup> queryable, string connectionGroupName, int? parentId)
        {
            return queryable.FirstOrDefaultAsync(q => q.ConnectionGroupName == connectionGroupName
                && (q.ParentId == parentId || (parentId == null && q.ParentId == null)));
        }

        public static IQueryable<Zyborg.GuacBot.GuacDB.Data.Entities.GuacamoleConnectionGroup> ByParentId(this IQueryable<Zyborg.GuacBot.GuacDB.Data.Entities.GuacamoleConnectionGroup> queryable, int? parentId)
        {
            return queryable.Where(q => (q.ParentId == parentId || (parentId == null && q.ParentId == null)));
        }

        #endregion

    }
}
