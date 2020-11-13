using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Amazon.EC2.Model;

namespace Zyborg.VMBot.Util
{
    /// <summary>
    /// A <see cref="SubstitutionEvaluator{TTarget}" /> for the AWS Instance model type.
    /// </summary>
    /// <remarks>
    /// In addition to the <see cref="SubstitutionEvaluator{TTarget}#AddCommonHandlers"
    /// >Common Substitution Keys</see>, this evaluator adds the following
    /// Instance-specific Keys:
    /// <list type="table">
    ///     <listheader>
    ///         <term>Substitution Key</term>
    ///     </listheader>
    ///     <item>
    ///         <term>ID</term>
    ///         <description>Resolves to the Instance ID.
    ///             An optional RegEx expression can be specified as
    ///             the Substitution Argument to transform the ID.  If
    ///             specified, the Argument should be specified as two
    ///             components a match expression and a replacement expression
    ///             separated by a semicolon.  The following examples depict
    ///             the result when evaluated against an ID of <c>i-123456abcdefg</c>.
    ///             <example>
    ///                 An expression of <c>%ID%</c> gives you <c>i-123456abcdefg</c>.
    ///             </example>
    ///             <example>
    ///                 An expression of <c>%ID:(....)$;\1%</c> gives you <c>defg</c>.
    ///             </example>
    ///             <example>
    ///                 An expression of <c>%ID:(^.{6}).*(.{4}$);\1...\2%</c> gives you <c>i-1234...defg</c>.
    ///             </example>
    ///         </description>
    ///     </item>
    ///     <item>
    ///         <term>PRIVATE_IP</term>
    ///         <description>Resolves to the private IP address of the Target Instance.
    ///             The substitution argument is ignored.
    ///             <example><c>10.10.10.10</c></example>
    ///         </description>
    ///     </item>
    ///     <item>
    ///         <term>PUBLIC_IP</term>
    ///         <description>Resolves to the public IP address of the Target Instance.
    ///             The substitution argument is ignored.
    ///             <example><c>30.20.10.1</c></example>
    ///         </description>
    ///     </item>
    ///     <item>
    ///         <term>PRIVATE_DNS</term>
    ///         <description>Resolves to the private DNS name of the Target Instance.
    ///             The substitution argument is ignored.
    ///             <example><c>ip-10-10-10-10.ec2.internal</c></example>
    ///         </description>
    ///     </item>
    ///     <item>
    ///         <term>PUBLIC_DNS</term>
    ///         <description>Resolves to the public DNS name of the Target Instance.
    ///             The substitution argument is ignored.
    ///             <example><c>ec2-30-20-10-1.compute-1.amazonaws.com</c></example>
    ///         </description>
    ///     </item>
    ///     <item>
    ///         <term>LAUNCH_TIME</term>
    ///         <description>Resolves to the Launch Time of the Target Instance.
    ///             The substitution argument can optionally specify a <c>DateTime</c>
    ///             format string.
    ///                 using an expression value of
    ///                 <c>%LAUNCH_TIME:yyyyMMdd_HHmmss%</c> could evaluate to the
    ///                 value <c>20200522_161430</c>
    ///         </description>
    ///     </item>
    ///     <item>
    ///         <term>TAG</term>
    ///         <description>Evaluates to the value of a resource tag with the name
    ///             specified by the substitution argument.
    ///             If the tag is missing or otherwise resolves to null,
    ///             the expression resolves to the original substitution
    ///             expression.
    ///             <example>
    ///                 using an expression value of
    ///                 <c>%TAG:Name%</c> could evaluate to the
    ///                 value <c>My_First_VM</c>
    ///             </example>
    ///         </description>
    ///     </item>
    ///     <item>
    ///         <term>TAG?</term>
    ///         <description>This key resolves exactly as the <c>TAG</c>
    ///             key except that a resolved null-value resolution
    ///             will resolve to the empty string.
    ///         </description>
    ///     </item>
    /// </list>
    /// </remarks>
    public class EC2Evaluator : SubstitutionEvaluator<Instance>
    {
        private static readonly IReadOnlyDictionary<string, Func<Instance, string, string>> InstanceHandlers =
            new Dictionary<string, Func<Instance, string, string>>
            {
                ["ID"] = (inst, regex) => !(RegexParts(regex) is string[] parts)
                    ? inst.InstanceId
                    : Regex.Replace(inst.InstanceId, parts[0], parts[1]),

                ["PRIVATE_IP"] = (inst, _) => inst.PrivateIpAddress,
                ["PUBLIC_IP"] = (inst, _) => inst.PublicIpAddress,
                ["PRIVATE_DNS"] = (inst, _) => inst.PrivateDnsName,
                ["PUBLIC_DNS"] = (inst, _) => inst.PublicDnsName,

                ["LAUNCH_TIME"] = (inst, fmt) => inst.LaunchTime.ToString(fmt),

                ["TAG"] = (inst, arg) => inst.Tags?.FirstOrDefault(t => t.Key == arg)?.Value
                    ?? $"%TAG:{arg}%",
                ["TAG?"] = (inst, arg) => inst.Tags?.FirstOrDefault(t => t.Key == arg)?.Value,
            };

        public EC2Evaluator()
        {
            base.AddCommonHandlers()
                .AddHandlers(InstanceHandlers);
        }

        private static string[] RegexParts(string regex)
        {
            if (string.IsNullOrWhiteSpace(regex))
                return null;

            var parts = regex.Split(';', 2);
            if (parts?.Length != 2)
                return null;

            return parts;
        }
    }
}