using System;
using System.Collections.Generic;

namespace Zyborg.GuacBot.GuacDB.Data.Entities
{
    public partial class GuacamoleSharingProfileParameter
    {
        public GuacamoleSharingProfileParameter()
        {
            #region Generated Constructor
            #endregion
        }

        #region Generated Properties
        public int SharingProfileId { get; set; }

        public string ParameterName { get; set; }

        public string ParameterValue { get; set; }

        #endregion

        #region Generated Relationships
        public virtual GuacamoleSharingProfile GuacamoleSharingProfile { get; set; }

        #endregion

    }
}
