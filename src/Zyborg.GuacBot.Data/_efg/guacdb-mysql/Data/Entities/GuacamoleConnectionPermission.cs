using System;
using System.Collections.Generic;

namespace Zyborg.GuacBot.GuacDB.Data.Entities
{
    public partial class GuacamoleConnectionPermission
    {
        public GuacamoleConnectionPermission()
        {
            #region Generated Constructor
            #endregion
        }

        #region Generated Properties
        public int EntityId { get; set; }

        public int ConnectionId { get; set; }

        public string Permission { get; set; }

        #endregion

        #region Generated Relationships
        public virtual GuacamoleConnection GuacamoleConnection { get; set; }

        public virtual GuacamoleEntity GuacamoleEntity { get; set; }

        #endregion

    }
}
