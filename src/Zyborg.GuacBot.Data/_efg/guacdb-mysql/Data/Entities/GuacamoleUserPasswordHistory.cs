using System;
using System.Collections.Generic;

namespace Zyborg.GuacBot.GuacDB.Data.Entities
{
    public partial class GuacamoleUserPasswordHistory
    {
        public GuacamoleUserPasswordHistory()
        {
            #region Generated Constructor
            #endregion
        }

        #region Generated Properties
        public int PasswordHistoryId { get; set; }

        public int UserId { get; set; }

        public Byte[] PasswordHash { get; set; }

        public Byte[] PasswordSalt { get; set; }

        public DateTime PasswordDate { get; set; }

        #endregion

        #region Generated Relationships
        public virtual GuacamoleUser GuacamoleUser { get; set; }

        #endregion

    }
}
