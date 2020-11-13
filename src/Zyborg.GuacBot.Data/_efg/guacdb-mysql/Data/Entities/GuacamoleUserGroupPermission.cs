using System;
using System.Collections.Generic;

namespace Zyborg.GuacBot.GuacDB.Data.Entities
{
    public partial class GuacamoleUserGroupPermission
    {
        public GuacamoleUserGroupPermission()
        {
            #region Generated Constructor
            #endregion
        }

        #region Generated Properties
        public int EntityId { get; set; }

        public int AffectedUserGroupId { get; set; }

        public string Permission { get; set; }

        #endregion

        #region Generated Relationships
        public virtual GuacamoleUserGroup AffectedGuacamoleUserGroup { get; set; }

        public virtual GuacamoleEntity GuacamoleEntity { get; set; }

        #endregion

    }
}
