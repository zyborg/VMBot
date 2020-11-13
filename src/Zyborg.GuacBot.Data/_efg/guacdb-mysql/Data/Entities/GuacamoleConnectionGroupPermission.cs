using System;
using System.Collections.Generic;

namespace Zyborg.GuacBot.GuacDB.Data.Entities
{
    public partial class GuacamoleConnectionGroupPermission
    {
        public GuacamoleConnectionGroupPermission()
        {
            #region Generated Constructor
            #endregion
        }

        #region Generated Properties
        public int EntityId { get; set; }

        public int ConnectionGroupId { get; set; }

        public string Permission { get; set; }

        #endregion

        #region Generated Relationships
        public virtual GuacamoleConnectionGroup GuacamoleConnectionGroup { get; set; }

        public virtual GuacamoleEntity GuacamoleEntity { get; set; }

        #endregion

    }
}
