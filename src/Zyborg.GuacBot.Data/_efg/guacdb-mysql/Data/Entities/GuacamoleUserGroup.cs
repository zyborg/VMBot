using System;
using System.Collections.Generic;

namespace Zyborg.GuacBot.GuacDB.Data.Entities
{
    public partial class GuacamoleUserGroup
    {
        public GuacamoleUserGroup()
        {
            #region Generated Constructor
            AffectedGuacamoleUserGroupPermissions = new HashSet<GuacamoleUserGroupPermission>();
            GuacamoleUserGroupAttributes = new HashSet<GuacamoleUserGroupAttribute>();
            GuacamoleUserGroupMembers = new HashSet<GuacamoleUserGroupMember>();
            #endregion
        }

        #region Generated Properties
        public int UserGroupId { get; set; }

        public int EntityId { get; set; }

        public bool Disabled { get; set; }

        #endregion

        #region Generated Relationships
        public virtual ICollection<GuacamoleUserGroupPermission> AffectedGuacamoleUserGroupPermissions { get; set; }

        public virtual GuacamoleEntity GuacamoleEntity { get; set; }

        public virtual ICollection<GuacamoleUserGroupAttribute> GuacamoleUserGroupAttributes { get; set; }

        public virtual ICollection<GuacamoleUserGroupMember> GuacamoleUserGroupMembers { get; set; }

        #endregion

    }
}
