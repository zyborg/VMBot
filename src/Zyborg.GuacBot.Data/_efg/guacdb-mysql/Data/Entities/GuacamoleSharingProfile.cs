using System;
using System.Collections.Generic;

namespace Zyborg.GuacBot.GuacDB.Data.Entities
{
    public partial class GuacamoleSharingProfile
    {
        public GuacamoleSharingProfile()
        {
            #region Generated Constructor
            GuacamoleConnectionHistories = new HashSet<GuacamoleConnectionHistory>();
            GuacamoleSharingProfileAttributes = new HashSet<GuacamoleSharingProfileAttribute>();
            GuacamoleSharingProfileParameters = new HashSet<GuacamoleSharingProfileParameter>();
            GuacamoleSharingProfilePermissions = new HashSet<GuacamoleSharingProfilePermission>();
            #endregion
        }

        #region Generated Properties
        public int SharingProfileId { get; set; }

        public string SharingProfileName { get; set; }

        public int PrimaryConnectionId { get; set; }

        #endregion

        #region Generated Relationships
        public virtual ICollection<GuacamoleConnectionHistory> GuacamoleConnectionHistories { get; set; }

        public virtual ICollection<GuacamoleSharingProfileAttribute> GuacamoleSharingProfileAttributes { get; set; }

        public virtual ICollection<GuacamoleSharingProfileParameter> GuacamoleSharingProfileParameters { get; set; }

        public virtual ICollection<GuacamoleSharingProfilePermission> GuacamoleSharingProfilePermissions { get; set; }

        public virtual GuacamoleConnection PrimaryGuacamoleConnection { get; set; }

        #endregion

    }
}
