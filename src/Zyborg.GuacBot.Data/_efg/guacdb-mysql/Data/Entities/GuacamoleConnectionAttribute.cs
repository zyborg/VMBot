using System;
using System.Collections.Generic;

namespace Zyborg.GuacBot.GuacDB.Data.Entities
{
    public partial class GuacamoleConnectionAttribute
    {
        public GuacamoleConnectionAttribute()
        {
            #region Generated Constructor
            #endregion
        }

        #region Generated Properties
        public int ConnectionId { get; set; }

        public string AttributeName { get; set; }

        public string AttributeValue { get; set; }

        #endregion

        #region Generated Relationships
        public virtual GuacamoleConnection GuacamoleConnection { get; set; }

        #endregion

    }
}
