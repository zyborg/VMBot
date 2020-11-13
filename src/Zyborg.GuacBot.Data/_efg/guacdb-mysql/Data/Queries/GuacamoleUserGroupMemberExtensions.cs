using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace Zyborg.GuacBot.GuacDB.Data.Queries
{
    public static partial class GuacamoleUserGroupMemberExtensions
    {
        #region Generated Extensions
        public static IQueryable<Zyborg.GuacBot.GuacDB.Data.Entities.GuacamoleUserGroupMember> ByMemberEntityId(this IQueryable<Zyborg.GuacBot.GuacDB.Data.Entities.GuacamoleUserGroupMember> queryable, int memberEntityId)
        {
            return queryable.Where(q => q.MemberEntityId == memberEntityId);
        }

        public static IQueryable<Zyborg.GuacBot.GuacDB.Data.Entities.GuacamoleUserGroupMember> ByUserGroupId(this IQueryable<Zyborg.GuacBot.GuacDB.Data.Entities.GuacamoleUserGroupMember> queryable, int userGroupId)
        {
            return queryable.Where(q => q.UserGroupId == userGroupId);
        }

        public static Zyborg.GuacBot.GuacDB.Data.Entities.GuacamoleUserGroupMember GetByKey(this IQueryable<Zyborg.GuacBot.GuacDB.Data.Entities.GuacamoleUserGroupMember> queryable, int userGroupId, int memberEntityId)
        {
            if (queryable is DbSet<Zyborg.GuacBot.GuacDB.Data.Entities.GuacamoleUserGroupMember> dbSet)
                return dbSet.Find(userGroupId, memberEntityId);

            return queryable.FirstOrDefault(q => q.UserGroupId == userGroupId
                && q.MemberEntityId == memberEntityId);
        }

        public static ValueTask<Zyborg.GuacBot.GuacDB.Data.Entities.GuacamoleUserGroupMember> GetByKeyAsync(this IQueryable<Zyborg.GuacBot.GuacDB.Data.Entities.GuacamoleUserGroupMember> queryable, int userGroupId, int memberEntityId)
        {
            if (queryable is DbSet<Zyborg.GuacBot.GuacDB.Data.Entities.GuacamoleUserGroupMember> dbSet)
                return dbSet.FindAsync(userGroupId, memberEntityId);

            var task = queryable.FirstOrDefaultAsync(q => q.UserGroupId == userGroupId
                && q.MemberEntityId == memberEntityId);
            return new ValueTask<Zyborg.GuacBot.GuacDB.Data.Entities.GuacamoleUserGroupMember>(task);
        }

        #endregion

    }
}
