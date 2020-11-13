using System;
using System.Collections.Generic;

namespace Zyborg.GuacBot.GuacDB.Data.Entities
{
    public partial class GuacamoleConnection
    {
        public GuacamoleConnection()
        {
            #region Generated Constructor
            GuacamoleConnectionAttributes = new HashSet<GuacamoleConnectionAttribute>();
            GuacamoleConnectionHistories = new HashSet<GuacamoleConnectionHistory>();
            GuacamoleConnectionParameters = new HashSet<GuacamoleConnectionParameter>();
            GuacamoleConnectionPermissions = new HashSet<GuacamoleConnectionPermission>();
            PrimaryGuacamoleSharingProfiles = new HashSet<GuacamoleSharingProfile>();
            #endregion
        }

        #region Generated Properties
        public int ConnectionId { get; set; }

        public string ConnectionName { get; set; }

        public int? ParentId { get; set; }

        public string Protocol { get; set; }

        public int? ProxyPort { get; set; }

        public string ProxyHostname { get; set; }

        public string ProxyEncryptionMethod { get; set; }

        public int? MaxConnections { get; set; }

        public int? MaxConnectionsPerUser { get; set; }

        public int? ConnectionWeight { get; set; }

        public bool FailoverOnly { get; set; }

        #endregion

        #region Generated Relationships
        public virtual ICollection<GuacamoleConnectionAttribute> GuacamoleConnectionAttributes { get; set; }

        public virtual ICollection<GuacamoleConnectionHistory> GuacamoleConnectionHistories { get; set; }

        public virtual ICollection<GuacamoleConnectionParameter> GuacamoleConnectionParameters { get; set; }

        public virtual ICollection<GuacamoleConnectionPermission> GuacamoleConnectionPermissions { get; set; }

        public virtual GuacamoleConnectionGroup ParentGuacamoleConnectionGroup { get; set; }

        public virtual ICollection<GuacamoleSharingProfile> PrimaryGuacamoleSharingProfiles { get; set; }

        #endregion

    }
}
