using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace Zyborg.GuacBot.GuacDB.Data.Queries
{
    public static partial class GuacamoleUserPermissionExtensions
    {
        #region Generated Extensions
        public static IQueryable<Zyborg.GuacBot.GuacDB.Data.Entities.GuacamoleUserPermission> ByAffectedUserId(this IQueryable<Zyborg.GuacBot.GuacDB.Data.Entities.GuacamoleUserPermission> queryable, int affectedUserId)
        {
            return queryable.Where(q => q.AffectedUserId == affectedUserId);
        }

        public static IQueryable<Zyborg.GuacBot.GuacDB.Data.Entities.GuacamoleUserPermission> ByEntityId(this IQueryable<Zyborg.GuacBot.GuacDB.Data.Entities.GuacamoleUserPermission> queryable, int entityId)
        {
            return queryable.Where(q => q.EntityId == entityId);
        }

        public static Zyborg.GuacBot.GuacDB.Data.Entities.GuacamoleUserPermission GetByKey(this IQueryable<Zyborg.GuacBot.GuacDB.Data.Entities.GuacamoleUserPermission> queryable, int entityId, int affectedUserId, string permission)
        {
            if (queryable is DbSet<Zyborg.GuacBot.GuacDB.Data.Entities.GuacamoleUserPermission> dbSet)
                return dbSet.Find(entityId, affectedUserId, permission);

            return queryable.FirstOrDefault(q => q.EntityId == entityId
                && q.AffectedUserId == affectedUserId
                    && q.Permission == permission);
            }

            public static ValueTask<Zyborg.GuacBot.GuacDB.Data.Entities.GuacamoleUserPermission> GetByKeyAsync(this IQueryable<Zyborg.GuacBot.GuacDB.Data.Entities.GuacamoleUserPermission> queryable, int entityId, int affectedUserId, string permission)
            {
                if (queryable is DbSet<Zyborg.GuacBot.GuacDB.Data.Entities.GuacamoleUserPermission> dbSet)
                    return dbSet.FindAsync(entityId, affectedUserId, permission);

                var task = queryable.FirstOrDefaultAsync(q => q.EntityId == entityId
                    && q.AffectedUserId == affectedUserId
                        && q.Permission == permission);
                    return new ValueTask<Zyborg.GuacBot.GuacDB.Data.Entities.GuacamoleUserPermission>(task);
                }

                #endregion

            }
        }
