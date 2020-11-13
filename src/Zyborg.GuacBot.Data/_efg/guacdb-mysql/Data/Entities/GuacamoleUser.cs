using System;
using System.Collections.Generic;

namespace Zyborg.GuacBot.GuacDB.Data.Entities
{
    public partial class GuacamoleUser
    {
        public GuacamoleUser()
        {
            #region Generated Constructor
            AffectedGuacamoleUserPermissions = new HashSet<GuacamoleUserPermission>();
            GuacamoleConnectionHistories = new HashSet<GuacamoleConnectionHistory>();
            GuacamoleUserAttributes = new HashSet<GuacamoleUserAttribute>();
            GuacamoleUserHistories = new HashSet<GuacamoleUserHistory>();
            GuacamoleUserPasswordHistories = new HashSet<GuacamoleUserPasswordHistory>();
            #endregion
        }

        #region Generated Properties
        public int UserId { get; set; }

        public int EntityId { get; set; }

        public Byte[] PasswordHash { get; set; }

        public Byte[] PasswordSalt { get; set; }

        public DateTime PasswordDate { get; set; }

        public bool Disabled { get; set; }

        public bool Expired { get; set; }

        public TimeSpan? AccessWindowStart { get; set; }

        public TimeSpan? AccessWindowEnd { get; set; }

        public DateTime? ValidFrom { get; set; }

        public DateTime? ValidUntil { get; set; }

        public string Timezone { get; set; }

        public string FullName { get; set; }

        public string EmailAddress { get; set; }

        public string Organization { get; set; }

        public string OrganizationalRole { get; set; }

        #endregion

        #region Generated Relationships
        public virtual ICollection<GuacamoleUserPermission> AffectedGuacamoleUserPermissions { get; set; }

        public virtual ICollection<GuacamoleConnectionHistory> GuacamoleConnectionHistories { get; set; }

        public virtual GuacamoleEntity GuacamoleEntity { get; set; }

        public virtual ICollection<GuacamoleUserAttribute> GuacamoleUserAttributes { get; set; }

        public virtual ICollection<GuacamoleUserHistory> GuacamoleUserHistories { get; set; }

        public virtual ICollection<GuacamoleUserPasswordHistory> GuacamoleUserPasswordHistories { get; set; }

        #endregion

    }
}
