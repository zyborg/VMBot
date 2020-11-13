using System;
using System.Collections.Generic;

namespace Zyborg.GuacBot.GuacDB.Data.Entities
{
    public partial class GuacamoleUserGroupAttribute
    {
        public GuacamoleUserGroupAttribute()
        {
            #region Generated Constructor
            #endregion
        }

        #region Generated Properties
        public int UserGroupId { get; set; }

        public string AttributeName { get; set; }

        public string AttributeValue { get; set; }

        #endregion

        #region Generated Relationships
        public virtual GuacamoleUserGroup GuacamoleUserGroup { get; set; }

        #endregion

    }
}
