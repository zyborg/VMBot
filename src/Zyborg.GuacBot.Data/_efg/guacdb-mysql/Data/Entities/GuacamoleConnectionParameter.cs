using System;
using System.Collections.Generic;

namespace Zyborg.GuacBot.GuacDB.Data.Entities
{
    public partial class GuacamoleConnectionParameter
    {
        public GuacamoleConnectionParameter()
        {
            #region Generated Constructor
            #endregion
        }

        #region Generated Properties
        public int ConnectionId { get; set; }

        public string ParameterName { get; set; }

        public string ParameterValue { get; set; }

        #endregion

        #region Generated Relationships
        public virtual GuacamoleConnection GuacamoleConnection { get; set; }

        #endregion

    }
}
