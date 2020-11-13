using System;
using System.Collections.Generic;

namespace Zyborg.GuacBot.GuacDB.Data.Entities
{
    public partial class GuacamoleConnectionHistory
    {
        public GuacamoleConnectionHistory()
        {
            #region Generated Constructor
            #endregion
        }

        #region Generated Properties
        public int HistoryId { get; set; }

        public int? UserId { get; set; }

        public string Username { get; set; }

        public string RemoteHost { get; set; }

        public int? ConnectionId { get; set; }

        public string ConnectionName { get; set; }

        public int? SharingProfileId { get; set; }

        public string SharingProfileName { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime? EndDate { get; set; }

        #endregion

        #region Generated Relationships
        public virtual GuacamoleConnection GuacamoleConnection { get; set; }

        public virtual GuacamoleSharingProfile GuacamoleSharingProfile { get; set; }

        public virtual GuacamoleUser GuacamoleUser { get; set; }

        #endregion

    }
}
