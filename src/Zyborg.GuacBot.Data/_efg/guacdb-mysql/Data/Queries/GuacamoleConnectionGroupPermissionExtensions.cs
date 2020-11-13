using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace Zyborg.GuacBot.GuacDB.Data.Queries
{
    public static partial class GuacamoleConnectionGroupPermissionExtensions
    {
        #region Generated Extensions
        public static IQueryable<Zyborg.GuacBot.GuacDB.Data.Entities.GuacamoleConnectionGroupPermission> ByConnectionGroupId(this IQueryable<Zyborg.GuacBot.GuacDB.Data.Entities.GuacamoleConnectionGroupPermission> queryable, int connectionGroupId)
        {
            return queryable.Where(q => q.ConnectionGroupId == connectionGroupId);
        }

        public static IQueryable<Zyborg.GuacBot.GuacDB.Data.Entities.GuacamoleConnectionGroupPermission> ByEntityId(this IQueryable<Zyborg.GuacBot.GuacDB.Data.Entities.GuacamoleConnectionGroupPermission> queryable, int entityId)
        {
            return queryable.Where(q => q.EntityId == entityId);
        }

        public static Zyborg.GuacBot.GuacDB.Data.Entities.GuacamoleConnectionGroupPermission GetByKey(this IQueryable<Zyborg.GuacBot.GuacDB.Data.Entities.GuacamoleConnectionGroupPermission> queryable, int entityId, int connectionGroupId, string permission)
        {
            if (queryable is DbSet<Zyborg.GuacBot.GuacDB.Data.Entities.GuacamoleConnectionGroupPermission> dbSet)
                return dbSet.Find(entityId, connectionGroupId, permission);

            return queryable.FirstOrDefault(q => q.EntityId == entityId
                && q.ConnectionGroupId == connectionGroupId
                    && q.Permission == permission);
            }

            public static ValueTask<Zyborg.GuacBot.GuacDB.Data.Entities.GuacamoleConnectionGroupPermission> GetByKeyAsync(this IQueryable<Zyborg.GuacBot.GuacDB.Data.Entities.GuacamoleConnectionGroupPermission> queryable, int entityId, int connectionGroupId, string permission)
            {
                if (queryable is DbSet<Zyborg.GuacBot.GuacDB.Data.Entities.GuacamoleConnectionGroupPermission> dbSet)
                    return dbSet.FindAsync(entityId, connectionGroupId, permission);

                var task = queryable.FirstOrDefaultAsync(q => q.EntityId == entityId
                    && q.ConnectionGroupId == connectionGroupId
                        && q.Permission == permission);
                    return new ValueTask<Zyborg.GuacBot.GuacDB.Data.Entities.GuacamoleConnectionGroupPermission>(task);
                }

                #endregion

            }
        }
