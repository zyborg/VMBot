using System;
using System.Collections.Generic;

namespace Zyborg.GuacBot.GuacDB.Data.Entities
{
    public partial class GuacamoleSharingProfilePermission
    {
        public GuacamoleSharingProfilePermission()
        {
            #region Generated Constructor
            #endregion
        }

        #region Generated Properties
        public int EntityId { get; set; }

        public int SharingProfileId { get; set; }

        public string Permission { get; set; }

        #endregion

        #region Generated Relationships
        public virtual GuacamoleEntity GuacamoleEntity { get; set; }

        public virtual GuacamoleSharingProfile GuacamoleSharingProfile { get; set; }

        #endregion

    }
}
