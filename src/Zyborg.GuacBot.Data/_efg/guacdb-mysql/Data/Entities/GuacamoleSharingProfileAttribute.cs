using System;
using System.Collections.Generic;

namespace Zyborg.GuacBot.GuacDB.Data.Entities
{
    public partial class GuacamoleSharingProfileAttribute
    {
        public GuacamoleSharingProfileAttribute()
        {
            #region Generated Constructor
            #endregion
        }

        #region Generated Properties
        public int SharingProfileId { get; set; }

        public string AttributeName { get; set; }

        public string AttributeValue { get; set; }

        #endregion

        #region Generated Relationships
        public virtual GuacamoleSharingProfile GuacamoleSharingProfile { get; set; }

        #endregion

    }
}
