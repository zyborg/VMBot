using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace Zyborg.GuacBot.GuacDB.Data.Queries
{
    public static partial class GuacamoleUserGroupPermissionExtensions
    {
        #region Generated Extensions
        public static IQueryable<Zyborg.GuacBot.GuacDB.Data.Entities.GuacamoleUserGroupPermission> ByAffectedUserGroupId(this IQueryable<Zyborg.GuacBot.GuacDB.Data.Entities.GuacamoleUserGroupPermission> queryable, int affectedUserGroupId)
        {
            return queryable.Where(q => q.AffectedUserGroupId == affectedUserGroupId);
        }

        public static IQueryable<Zyborg.GuacBot.GuacDB.Data.Entities.GuacamoleUserGroupPermission> ByEntityId(this IQueryable<Zyborg.GuacBot.GuacDB.Data.Entities.GuacamoleUserGroupPermission> queryable, int entityId)
        {
            return queryable.Where(q => q.EntityId == entityId);
        }

        public static Zyborg.GuacBot.GuacDB.Data.Entities.GuacamoleUserGroupPermission GetByKey(this IQueryable<Zyborg.GuacBot.GuacDB.Data.Entities.GuacamoleUserGroupPermission> queryable, int entityId, int affectedUserGroupId, string permission)
        {
            if (queryable is DbSet<Zyborg.GuacBot.GuacDB.Data.Entities.GuacamoleUserGroupPermission> dbSet)
                return dbSet.Find(entityId, affectedUserGroupId, permission);

            return queryable.FirstOrDefault(q => q.EntityId == entityId
                && q.AffectedUserGroupId == affectedUserGroupId
                    && q.Permission == permission);
            }

            public static ValueTask<Zyborg.GuacBot.GuacDB.Data.Entities.GuacamoleUserGroupPermission> GetByKeyAsync(this IQueryable<Zyborg.GuacBot.GuacDB.Data.Entities.GuacamoleUserGroupPermission> queryable, int entityId, int affectedUserGroupId, string permission)
            {
                if (queryable is DbSet<Zyborg.GuacBot.GuacDB.Data.Entities.GuacamoleUserGroupPermission> dbSet)
                    return dbSet.FindAsync(entityId, affectedUserGroupId, permission);

                var task = queryable.FirstOrDefaultAsync(q => q.EntityId == entityId
                    && q.AffectedUserGroupId == affectedUserGroupId
                        && q.Permission == permission);
                    return new ValueTask<Zyborg.GuacBot.GuacDB.Data.Entities.GuacamoleUserGroupPermission>(task);
                }

                #endregion

            }
        }
