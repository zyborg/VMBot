using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.JsonPatch;
using Zyborg.GuacBot.GuacAPI.Model;

namespace Zyborg.GuacBot.GuacAPI
{
    public partial class GuacClient
    {
        public Task<Dictionary<string, UserGroup>> GetUserGroups(string dataSource = null) =>
            GetDataSourceEntity<Dictionary<string, UserGroup>>("userGroups", dataSource);
        
        public async Task<UserGroup> GetUserGroup(string identifier, string dataSource = null, UserGroupDetails? details = null)
        {
            var result = await GetDataSourceEntity<UserGroup>("userGroups", dataSource, identifier);

            if (details != null)
            {
                if (details.Value.HasFlag(UserGroupDetails.MemberUsers))
                    result.MemberUsers = await GetDataSourceEntity<List<string>>(
                        "userGroups", dataSource, $"{identifier}/memberUsers");
                if (details.Value.HasFlag(UserGroupDetails.ParentUserGroups))
                    result.ParentUserGroups = await GetDataSourceEntity<List<string>>(
                        "userGroups", dataSource, $"{identifier}/userGroups");
                if (details.Value.HasFlag(UserGroupDetails.MemberUserGroups))
                    result.MemberUserGroups = await GetDataSourceEntity<List<string>>(
                        "userGroups", dataSource, $"{identifier}/memberUserGroups");
                if (details.Value.HasFlag(UserGroupDetails.Permissions))
                    result.Permissions = await GetDataSourceObject(
                        "userGroups", dataSource, $"{identifier}/permissions");
            }

            return result;
        }

        public Task<UserGroup> CreateUserGroup(UserGroup group, string dataSource = null) =>
            PostDataSourceEntity(group, "userGroups", dataSource);
        
        public Task AddUserGroupMemberUsers(string groupId, IEnumerable<string> users,
            string dataSource = null) =>
            PatchDataSourceEntity(users.Select(user => new JsonPatchValue<string>
            {
                Op = "add",
                Path = "/",
                Value = user,
            }), $"userGroups/{groupId}", dataSource, subpath: "memberUsers");

        public Task RemoveUserGroupMemberUsers(string groupId, IEnumerable<string> users,
            string dataSource = null) =>
            PatchDataSourceEntity(users.Select(user => new JsonPatchValue<string>
            {
                Op = "remove",
                Path = "/",
                Value = user,
            }), $"userGroups/{groupId}", dataSource, subpath: "memberUsers");
        
        public Task AddUserGroupMemberGroups(string groupId, IEnumerable<string> groups,
            string dataSource = null) =>
            PatchDataSourceEntity(groups.Select(group => new JsonPatchValue<string>
            {
                Op = "add",
                Path = "/",
                Value = group,
            }), $"userGroups/{groupId}", dataSource, subpath: "memberUserGroups");

        public Task RemoveUserGroupMemberGroups(string groupId, IEnumerable<string> groups,
            string dataSource = null) =>
            PatchDataSourceEntity(groups.Select(group => new JsonPatchValue<string>
            {
                Op = "remove",
                Path = "/",
                Value = group,
            }), $"userGroups/{groupId}", dataSource, subpath: "memberUserGroups");
        
        public Task AddUserGroupParentGroups(string groupId, IEnumerable<string> groups,
            string dataSource = null) =>
            PatchDataSourceEntity(groups.Select(group => new JsonPatchValue<string>
            {
                Op = "add",
                Path = "/",
                Value = group,
            }), $"userGroups/{groupId}", dataSource, subpath: "userGroups");

        public Task RemoveUserGroupParentGroups(string groupId, IEnumerable<string> groups,
            string dataSource = null) =>
            PatchDataSourceEntity(groups.Select(group => new JsonPatchValue<string>
            {
                Op = "remove",
                Path = "/",
                Value = group,
            }), $"userGroups/{groupId}", dataSource, subpath: "userGroups");

        public Task AddUserGroupConnections(string groupId,
            IEnumerable<string> connections = null,
            IEnumerable<string> connectionGroups = null,
            string dataSource = null)
        {
            var patches = JsonPatchValue<string>.Empty;
            if (connections != null)
            {
                patches = patches.Concat(connections.Select(conn => new JsonPatchValue<string>
                {
                    Op = "add",
                    Path = $"/connectionPermissions/{conn}",
                    Value = "READ",
                }));
            }
            if (connectionGroups != null)
            {
                patches = patches.Concat(connectionGroups.Select(group => new JsonPatchValue<string>
                {
                    Op = "add",
                    Path = $"/connectionGroupPermissions/{group}",
                    Value = "READ",
                }));
            }

            return PatchDataSourceEntity(patches, $"userGroups/{groupId}", dataSource, subpath: "permissions");

        }

        public Task RemoveUserGroupConnections(string groupId,
            IEnumerable<string> connections = null,
            IEnumerable<string> connectionGroups = null,
            string dataSource = null)
        {
            var patches = JsonPatchValue<string>.Empty;
            if (connections != null)
            {
                patches = patches.Concat(connections.Select(conn => new JsonPatchValue<string>
                {
                    Op = "remove",
                    Path = $"/connectionPermissions/{conn}",
                    Value = "READ",
                }));
            }
            if (connectionGroups != null)
            {
                patches = patches.Concat(connectionGroups.Select(group => new JsonPatchValue<string>
                {
                    Op = "remove",
                    Path = $"/connectionGroupPermissions/{group}",
                    Value = "READ",
                }));
            }

            return PatchDataSourceEntity(patches, $"userGroups/{groupId}", dataSource, subpath: "permissions");

        }

        // public Task RemoveUserGroupConnection(string groupId, IEnumerable<string> connections,
        //     string dataSource = null) =>
        //     PatchDataSourceEntity(connections.Select(conn => new JsonPatchValue<string>
        //     {
        //         Op = "remove",
        //         Path = $"/connectionPermissions/{conn}",
        //         Value = "READ",
        //     }), $"userGroups/{groupId}", dataSource, subpath: "permissions");

        // public Task AddUserGroupConnectionGroups(string groupId, IEnumerable<string> connectionGroups,
        //     string dataSource = null) =>
        //     PatchDataSourceEntity(connectionGroups.Select(conn => new JsonPatchValue<string>
        //     {
        //         Op = "add",
        //         Path = $"/connectionGroupPermissions/{conn}",
        //         Value = "READ",
        //     }), $"userGroups/{groupId}", dataSource, subpath: "permissions");

        // public Task RemoveUserGroupConnectionGroups(string groupId, IEnumerable<string> connectionGroups,
        //     string dataSource = null) =>
        //     PatchDataSourceEntity(connectionGroups.Select(conn => new JsonPatchValue<string>
        //     {
        //         Op = "remove",
        //         Path = $"/connectionGroupPermissions/{conn}",
        //         Value = "READ",
        //     }), $"userGroups/{groupId}", dataSource, subpath: "permissions");
    }
}