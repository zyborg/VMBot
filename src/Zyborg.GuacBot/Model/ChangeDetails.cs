using System.Collections.Generic;
using System.Linq;
using Amazon.EC2.Model;

namespace Zyborg.GuacBot.Model
{
    public class ChangeDetails
    {
        public string InstanceId { get; set; }
        public string Ec2StateChange { get; set; }
        public bool IsDelete { get; set; }

        public Instance Instance { get; set; }
        public string NameTag { get; set; }
        public string ConnectionName { get; set; }
        public string ConnectionProto { get; set; }

        public string GetTag(string key) =>
            Instance.Tags?.FirstOrDefault(x => x.Key == key)?.Value;

        public IEnumerable<Tag> GetTags(string startsWith = null) =>
            Instance.Tags?.Where(x => startsWith == null || x.Key.StartsWith(startsWith));
    }
}