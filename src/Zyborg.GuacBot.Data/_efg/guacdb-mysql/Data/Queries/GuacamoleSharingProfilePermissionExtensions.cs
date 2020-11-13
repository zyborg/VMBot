using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace Zyborg.GuacBot.GuacDB.Data.Queries
{
    public static partial class GuacamoleSharingProfilePermissionExtensions
    {
        #region Generated Extensions
        public static IQueryable<Zyborg.GuacBot.GuacDB.Data.Entities.GuacamoleSharingProfilePermission> ByEntityId(this IQueryable<Zyborg.GuacBot.GuacDB.Data.Entities.GuacamoleSharingProfilePermission> queryable, int entityId)
        {
            return queryable.Where(q => q.EntityId == entityId);
        }

        public static Zyborg.GuacBot.GuacDB.Data.Entities.GuacamoleSharingProfilePermission GetByKey(this IQueryable<Zyborg.GuacBot.GuacDB.Data.Entities.GuacamoleSharingProfilePermission> queryable, int entityId, int sharingProfileId, string permission)
        {
            if (queryable is DbSet<Zyborg.GuacBot.GuacDB.Data.Entities.GuacamoleSharingProfilePermission> dbSet)
                return dbSet.Find(entityId, sharingProfileId, permission);

            return queryable.FirstOrDefault(q => q.EntityId == entityId
                && q.SharingProfileId == sharingProfileId
                    && q.Permission == permission);
            }

            public static ValueTask<Zyborg.GuacBot.GuacDB.Data.Entities.GuacamoleSharingProfilePermission> GetByKeyAsync(this IQueryable<Zyborg.GuacBot.GuacDB.Data.Entities.GuacamoleSharingProfilePermission> queryable, int entityId, int sharingProfileId, string permission)
            {
                if (queryable is DbSet<Zyborg.GuacBot.GuacDB.Data.Entities.GuacamoleSharingProfilePermission> dbSet)
                    return dbSet.FindAsync(entityId, sharingProfileId, permission);

                var task = queryable.FirstOrDefaultAsync(q => q.EntityId == entityId
                    && q.SharingProfileId == sharingProfileId
                        && q.Permission == permission);
                    return new ValueTask<Zyborg.GuacBot.GuacDB.Data.Entities.GuacamoleSharingProfilePermission>(task);
                }

                public static IQueryable<Zyborg.GuacBot.GuacDB.Data.Entities.GuacamoleSharingProfilePermission> BySharingProfileId(this IQueryable<Zyborg.GuacBot.GuacDB.Data.Entities.GuacamoleSharingProfilePermission> queryable, int sharingProfileId)
                {
                    return queryable.Where(q => q.SharingProfileId == sharingProfileId);
                }

                #endregion

            }
        }
