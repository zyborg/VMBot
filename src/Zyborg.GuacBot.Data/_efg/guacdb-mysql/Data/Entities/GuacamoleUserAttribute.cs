using System;
using System.Collections.Generic;

namespace Zyborg.GuacBot.GuacDB.Data.Entities
{
    public partial class GuacamoleUserAttribute
    {
        public GuacamoleUserAttribute()
        {
            #region Generated Constructor
            #endregion
        }

        #region Generated Properties
        public int UserId { get; set; }

        public string AttributeName { get; set; }

        public string AttributeValue { get; set; }

        #endregion

        #region Generated Relationships
        public virtual GuacamoleUser GuacamoleUser { get; set; }

        #endregion

    }
}
