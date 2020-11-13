using System;
using System.Collections.Generic;

namespace Zyborg.GuacBot.GuacDB.Data.Entities
{
    public partial class GuacamoleUserGroupMember
    {
        public GuacamoleUserGroupMember()
        {
            #region Generated Constructor
            #endregion
        }

        #region Generated Properties
        public int UserGroupId { get; set; }

        public int MemberEntityId { get; set; }

        #endregion

        #region Generated Relationships
        public virtual GuacamoleUserGroup GuacamoleUserGroup { get; set; }

        public virtual GuacamoleEntity MemberGuacamoleEntity { get; set; }

        #endregion

    }
}
