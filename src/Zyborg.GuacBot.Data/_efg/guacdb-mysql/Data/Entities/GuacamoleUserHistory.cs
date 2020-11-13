using System;
using System.Collections.Generic;

namespace Zyborg.GuacBot.GuacDB.Data.Entities
{
    public partial class GuacamoleUserHistory
    {
        public GuacamoleUserHistory()
        {
            #region Generated Constructor
            #endregion
        }

        #region Generated Properties
        public int HistoryId { get; set; }

        public int? UserId { get; set; }

        public string Username { get; set; }

        public string RemoteHost { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime? EndDate { get; set; }

        #endregion

        #region Generated Relationships
        public virtual GuacamoleUser GuacamoleUser { get; set; }

        #endregion

    }
}
