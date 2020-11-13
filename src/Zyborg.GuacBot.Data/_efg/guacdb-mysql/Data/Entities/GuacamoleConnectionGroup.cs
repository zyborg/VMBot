using System;
using System.Collections.Generic;

namespace Zyborg.GuacBot.GuacDB.Data.Entities
{
    public partial class GuacamoleConnectionGroup
    {
        public GuacamoleConnectionGroup()
        {
            #region Generated Constructor
            GuacamoleConnectionGroupAttributes = new HashSet<GuacamoleConnectionGroupAttribute>();
            GuacamoleConnectionGroupPermissions = new HashSet<GuacamoleConnectionGroupPermission>();
            ParentGuacamoleConnectionGroups = new HashSet<GuacamoleConnectionGroup>();
            ParentGuacamoleConnections = new HashSet<GuacamoleConnection>();
            #endregion
        }

        #region Generated Properties
        public int ConnectionGroupId { get; set; }

        public int? ParentId { get; set; }

        public string ConnectionGroupName { get; set; }

        public string Type { get; set; }

        public int? MaxConnections { get; set; }

        public int? MaxConnectionsPerUser { get; set; }

        public bool EnableSessionAffinity { get; set; }

        #endregion

        #region Generated Relationships
        public virtual ICollection<GuacamoleConnectionGroupAttribute> GuacamoleConnectionGroupAttributes { get; set; }

        public virtual ICollection<GuacamoleConnectionGroupPermission> GuacamoleConnectionGroupPermissions { get; set; }

        public virtual GuacamoleConnectionGroup ParentGuacamoleConnectionGroup { get; set; }

        public virtual ICollection<GuacamoleConnectionGroup> ParentGuacamoleConnectionGroups { get; set; }

        public virtual ICollection<GuacamoleConnection> ParentGuacamoleConnections { get; set; }

        #endregion

    }
}
