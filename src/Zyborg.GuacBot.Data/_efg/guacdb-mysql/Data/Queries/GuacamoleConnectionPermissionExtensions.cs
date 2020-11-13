using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace Zyborg.GuacBot.GuacDB.Data.Queries
{
    public static partial class GuacamoleConnectionPermissionExtensions
    {
        #region Generated Extensions
        public static IQueryable<Zyborg.GuacBot.GuacDB.Data.Entities.GuacamoleConnectionPermission> ByConnectionId(this IQueryable<Zyborg.GuacBot.GuacDB.Data.Entities.GuacamoleConnectionPermission> queryable, int connectionId)
        {
            return queryable.Where(q => q.ConnectionId == connectionId);
        }

        public static IQueryable<Zyborg.GuacBot.GuacDB.Data.Entities.GuacamoleConnectionPermission> ByEntityId(this IQueryable<Zyborg.GuacBot.GuacDB.Data.Entities.GuacamoleConnectionPermission> queryable, int entityId)
        {
            return queryable.Where(q => q.EntityId == entityId);
        }

        public static Zyborg.GuacBot.GuacDB.Data.Entities.GuacamoleConnectionPermission GetByKey(this IQueryable<Zyborg.GuacBot.GuacDB.Data.Entities.GuacamoleConnectionPermission> queryable, int entityId, int connectionId, string permission)
        {
            if (queryable is DbSet<Zyborg.GuacBot.GuacDB.Data.Entities.GuacamoleConnectionPermission> dbSet)
                return dbSet.Find(entityId, connectionId, permission);

            return queryable.FirstOrDefault(q => q.EntityId == entityId
                && q.ConnectionId == connectionId
                    && q.Permission == permission);
            }

            public static ValueTask<Zyborg.GuacBot.GuacDB.Data.Entities.GuacamoleConnectionPermission> GetByKeyAsync(this IQueryable<Zyborg.GuacBot.GuacDB.Data.Entities.GuacamoleConnectionPermission> queryable, int entityId, int connectionId, string permission)
            {
                if (queryable is DbSet<Zyborg.GuacBot.GuacDB.Data.Entities.GuacamoleConnectionPermission> dbSet)
                    return dbSet.FindAsync(entityId, connectionId, permission);

                var task = queryable.FirstOrDefaultAsync(q => q.EntityId == entityId
                    && q.ConnectionId == connectionId
                        && q.Permission == permission);
                    return new ValueTask<Zyborg.GuacBot.GuacDB.Data.Entities.GuacamoleConnectionPermission>(task);
                }

                #endregion

            }
        }
