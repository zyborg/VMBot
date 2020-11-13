using System;
using System.Collections.Generic;

namespace Zyborg.GuacBot.GuacDB.Data.Entities
{
    public partial class GuacamoleConnectionGroupAttribute
    {
        public GuacamoleConnectionGroupAttribute()
        {
            #region Generated Constructor
            #endregion
        }

        #region Generated Properties
        public int ConnectionGroupId { get; set; }

        public string AttributeName { get; set; }

        public string AttributeValue { get; set; }

        #endregion

        #region Generated Relationships
        public virtual GuacamoleConnectionGroup GuacamoleConnectionGroup { get; set; }

        #endregion

    }
}
