using System;
using System.Collections.Generic;

namespace Zyborg.GuacBot.GuacDB.Data.Entities
{
    public partial class GuacamoleUserPermission
    {
        public GuacamoleUserPermission()
        {
            #region Generated Constructor
            #endregion
        }

        #region Generated Properties
        public int EntityId { get; set; }

        public int AffectedUserId { get; set; }

        public string Permission { get; set; }

        #endregion

        #region Generated Relationships
        public virtual GuacamoleUser AffectedGuacamoleUser { get; set; }

        public virtual GuacamoleEntity GuacamoleEntity { get; set; }

        #endregion

    }
}
