using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace Zyborg.GuacBot.GuacDB.Data.Queries
{
    public static partial class GuacamoleEntityExtensions
    {
        #region Generated Extensions
        public static Zyborg.GuacBot.GuacDB.Data.Entities.GuacamoleEntity GetByKey(this IQueryable<Zyborg.GuacBot.GuacDB.Data.Entities.GuacamoleEntity> queryable, int entityId)
        {
            if (queryable is DbSet<Zyborg.GuacBot.GuacDB.Data.Entities.GuacamoleEntity> dbSet)
                return dbSet.Find(entityId);

            return queryable.FirstOrDefault(q => q.EntityId == entityId);
        }

        public static ValueTask<Zyborg.GuacBot.GuacDB.Data.Entities.GuacamoleEntity> GetByKeyAsync(this IQueryable<Zyborg.GuacBot.GuacDB.Data.Entities.GuacamoleEntity> queryable, int entityId)
        {
            if (queryable is DbSet<Zyborg.GuacBot.GuacDB.Data.Entities.GuacamoleEntity> dbSet)
                return dbSet.FindAsync(entityId);

            var task = queryable.FirstOrDefaultAsync(q => q.EntityId == entityId);
            return new ValueTask<Zyborg.GuacBot.GuacDB.Data.Entities.GuacamoleEntity>(task);
        }

        public static Zyborg.GuacBot.GuacDB.Data.Entities.GuacamoleEntity GetByTypeName(this IQueryable<Zyborg.GuacBot.GuacDB.Data.Entities.GuacamoleEntity> queryable, string type, string name)
        {
            return queryable.FirstOrDefault(q => q.Type == type
                && q.Name == name);
        }

        public static Task<Zyborg.GuacBot.GuacDB.Data.Entities.GuacamoleEntity> GetByTypeNameAsync(this IQueryable<Zyborg.GuacBot.GuacDB.Data.Entities.GuacamoleEntity> queryable, string type, string name)
        {
            return queryable.FirstOrDefaultAsync(q => q.Type == type
                && q.Name == name);
        }

        #endregion

    }
}
