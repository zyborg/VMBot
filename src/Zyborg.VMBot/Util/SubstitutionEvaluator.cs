using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using Amazon.EC2.Model;

namespace Zyborg.VMBot.Util
{
    /// <summary>
    /// Utility class to perform fairly simple <i>substitution expression</i>
    /// evaluation against a target object.
    /// </summary>
    /// <remarks>
    /// This class can be used to evaluate a string that contains one or more
    /// embedded <b>Substitution Expressions</b>.  Each such expression will
    /// be evaluated against a <b>Target Object</b> to resolve the expressions
    /// which will replace the corresponding embedded expression in the output
    /// string.
    /// <para>
    /// A substitution expression is indicated by a starting and ending <c>%</c>
    /// token.  The content between the tokens is further decomposed into a
    /// <b>Substitution Key</b> and an optional <b>Substitution Argument</b>.
    /// The general form is as follows:
    ///     <code>
    ///         '%' <subst-key> [ ':' <subst-arg> ] '%'
    ///     </code>
    /// </para><para>
    /// The <b>key</b> is used to identify a specific <b>Substitution Handler</b>
    /// which must be previously registered with an instance of this class.
    /// When performing an evaluation, an instance of a Target Object will be
    /// provided and Handlers will be provided a reference to this instance along
    /// with the optional Argument if specified in order to resolve a value
    /// associated with the Key.
    /// </para><para>
    /// As an example, lets say that you have an instance of an evaluator for
    /// the <see cref="System.DateTime" /> type.  You can register the Key
    /// <c>DayOfWeek</c> to return the full name of the day of the week for
    /// the target <c>DateTime</c> instance:
    ///     <code>
    ///         var dtEval = new SubstitutionEvaluator<System.DateTime>()
    ///             .AddHandler("DayOfWeek", (dt, _) => dt.ToString("dddd"));
    ///         dtEval.Evaluate("%DayOfWeek", System.DateTime.UnixEpoch); // Thursday
    ///     </code>
    /// </para><para>
    /// Additionally, let's say you also wanted to support the short form,
    /// using the Key <c>DOW</c>:
    ///     <code>
    ///         dtEval.AddHandler("DOW", (dt, _) => dt.ToString("ddd"));
    ///         dtEval.Evaluate("%DOW%", System.DateTime.UnixEpoch); // Thu
    ///     </code>
    /// </para><para>
    /// Finally, let's allow the user choose the format to render the
    /// datetime value by providing an <i>optional</i> format string or
    /// using a standard default format if not provided as the Argument:
    ///     <code>
    ///         dtEval.AddHandler("DT_FORMAT", (dt, fmt) => dt.ToString(fmt ?? "yyyy/MM/dd"));
    ///         dtEval.Evaluate("%DT_FORMAT%", System.DateTime.UnixEpoch); // 1970/01/01
    ///         dtEval.Evaluate("%DT_FORMAT:MMM dd, yy%", System.DateTime.UnixEpoch); // Jan 01, 70
    ///     </code>
    /// </para>
    /// </remarks>
    /// <typeparam name="TTarget">Type of the Target Object instance which will be
    ///     used to evaluate expressions against.</typeparam>
    public class SubstitutionEvaluator<TTarget>
    {
        /// <summary>
        /// The escape token is used to define a substitution value by beginning
        /// and ending the value with a token.
        /// </summary>
        public const char Token = '%';
        public const char EscToken = '#';

        private static readonly IReadOnlyDictionary<string, Func<TTarget, string, string>> CommonHandlers =
            new Dictionary<string, Func<TTarget, string, string>>
            {
                ["NOW_DATE"] = (inst, _) => DateTime.Now.ToString("yyyy_MM_dd"),
                ["NOW_TIME"] = (inst, _) => DateTime.Now.ToString("HH_mm_ss"),
                ["UTC_DATE"] = (inst, _) => DateTime.UtcNow.ToString("yyyy_MM_dd"),
                ["UTC_TIME"] = (inst, _) => DateTime.UtcNow.ToString("HH_mm_ss"),

                ["NOW"] = (inst, fmt) => DateTime.Now.ToString(fmt ?? "yyyyMMdd_HHmmss"),
                ["UTC"] = (inst, fmt) => DateTime.UtcNow.ToString(fmt ?? "yyyyMMdd_HHmmss"),

                ["ENV"] = (inst, env) => System.Environment.GetEnvironmentVariable(env)
                    ?? $"%ENV:{env}%",
                ["ENV?"] = (inst, env) => System.Environment.GetEnvironmentVariable(env),

                ["PROP"] = (inst, prop) => typeof(TTarget).GetProperty(prop)?.GetValue(inst)?.ToString()
                    ?? $"%PROP:{prop}%",
                ["PROP?"] = (inst, prop) => typeof(TTarget).GetProperty(prop)?.GetValue(inst)?.ToString(),

                [string.Empty] = (inst, num) => "%",
                ["PCT"] = (inst, num) => new string('%',
                    ushort.TryParse(num, out var numVal) ? numVal : 1),
                ["ESC"] = (inst, str) => ParseEscape(str),
            };

        private IDictionary<string, Func<TTarget, string, string>> _keyHandlers =
            new Dictionary<string, Func<TTarget, string, string>>();

        public SubstitutionEvaluator<TTarget> AddHandler(string key, Func<TTarget, string, string> handler)
        {
            _keyHandlers[key] = handler;
            return this;
        }

        public SubstitutionEvaluator<TTarget> AddHandlers(
            IEnumerable<KeyValuePair<string, Func<TTarget, string, string>>> handlers)
        {
            return handlers.Select(h => AddHandler(h.Key, h.Value)).LastOrDefault() ?? this;
        }

        /// <summary>
        /// Adds common substitution keys and handlers that are useful across most scenarios.
        /// </summary>
        /// <remarks>
        /// The keys that are added are:
        /// <list type="table">
        ///     <listheader>
        ///         <term>Substitution Key</term>
        ///     </listheader>
        ///     <item>
        ///         <term>NOW_DATE</term>
        ///         <description>Resolves to the current date as a result
        ///             of <c>DateTime.Now</c> using the format <c>yyyy_MM_dd</c>.
        ///             The substitution argument is ignored.
        ///             <example>
        ///                 <c>2020_05_22</c> representing the date May 22, 2020
        ///             </example>
        ///         </description>
        ///     </item>
        ///     <item>
        ///         <term>NOW_TIME</term>
        ///         <description>Resolves to the current time as a result
        ///             of <c>DateTime.Now</c> using the format <c>HH_mm_ss</c>.
        ///             The substitution argument is ignored.
        ///             <example>
        ///                 <c>14_24_33</c> representing the time 2:24:33pm
        ///             </example>
        ///         </description>
        ///     </item>
        ///     <item>
        ///         <term>UTC_DATE</term>
        ///         <description>Resolves to the current UTC date as a result
        ///             of <c>DateTime.UtcNow</c> using the format <c>yyyy_MM_dd</c>.
        ///             The substitution argument is ignored.
        ///             <example>
        ///                 <c>2020_05_22</c> representing the date May 22, 2020
        ///             </example>
        ///         </description>
        ///     </item>
        ///     <item>
        ///         <term>UTC_TIME</term>
        ///         <description>Resolves to the current UTC time as a result
        ///             of <c>DateTime.UtcNow</c> using the format <c>HH_mm_ss</c>.
        ///             The substitution argument is ignored.
        ///             <example>
        ///                 <c>14_24_33</c> representing the time 2:24:33pm
        ///             </example>
        ///         </description>
        ///     </item>
        ///     <item>
        ///         <term>NOW</term>
        ///         <description>Resolves to the current datetime as a result
        ///             of <c>DateTime.Now</c> optionally using the substitution
        ///             argument as the format.  If no substitution argument is
        ///             provided, the default format is <c>yyyyMMdd_HHmmss</c>.
        ///             <example>
        ///                 <c>20200522_142433</c> representing the date
        ///                 May 22, 2020 at 2:24:33pm.
        ///             </example>
        ///         </description>
        ///     </item>
        ///     <item>
        ///         <term>UTC</term>
        ///         <description>Resolves to the current datetime as a result
        ///             of <c>DateTime.UtcNow</c> optionally using the substitution
        ///             argument as the format.  If no substitution argument is
        ///             provided, the default format is <c>yyyyMMdd_HHmmss</c>.
        ///             <example>
        ///                 <c>20200522_142433</c> representing the date
        ///                 May 22, 2020 at 2:24:33pm.
        ///             </example>
        ///         </description>
        ///     </item>
        ///     <item>
        ///         <term>ENV</term>
        ///         <description>Resolves to the value of an environment variable
        ///             specified by the substitution argument.
        ///             If the environment variable is missing or otherwise resolves
        ///             to null, the expression resolves to the original substitution
        ///             expression.
        ///             <example>
        ///                 using an expression value of
        ///                 <c>%ENV:EnvironmentName%</c> would evaluate to the
        ///                 value <c>Debug</c> when executing under the Debug
        ///                 configuration.
        ///             </example>
        ///         </description>
        ///     </item>
        ///     <item>
        ///         <term>ENV?</term>
        ///         <description>This key resolves exactly as the <c>ENV</c>
        ///             key except that a resolved null-value resolution
        ///             will resolve to the empty string.
        ///         </description>
        ///     </item>
        ///     <item>
        ///         <term>PROP</term>
        ///         <description>Resolves to the value of the property whose
        ///             name is specified by the substitution argument applied
        ///             If the named property does not exist on the target type
        ///             or the property value resolve to <c>null</c>, the
        ///             expression resolves to the original substitution
        ///             expression.
        ///             <example>
        ///                 using an expression value of
        ///                 <c>%PROP:ProcessName%</c> applied to an instance of
        ///                 the <c>System.Diagnostics.Process</c> class would
        ///                 resolve to the associated process' name.
        ///             </example>
        ///         </description>
        ///     </item>
        ///     <item>
        ///         <term>PROP?</term>
        ///         <description>This key resolves exactly as the <c>PROP</c>
        ///             key except that a resolved null-value resolution
        ///             will resolve to the empty string.
        ///         </description>
        ///     </item>
        /// </list>
        /// </remarks>
        /// <returns></returns>
        public SubstitutionEvaluator<TTarget> AddCommonHandlers() => AddHandlers(CommonHandlers);

        public string Evaluate(string expr, TTarget target)
        {
            if (string.IsNullOrWhiteSpace(expr))
                // Shortcircuit the simple cases
                return expr;

            var builder = new StringBuilder();
            
            int posB = -1;
            int posA = expr.IndexOf(Token);
            while (posA > posB)
            {
                // Append the preceding expression leading up to the start token
                builder.Append(expr.Substring(posB + 1, posA - posB - 1));

                posB = expr.IndexOf(Token, posA + 1);
                if (posB < 0)
                {
                    // Lone start token without an end token
                    posB = posA - 1;
                    break;
                }

                var subst = expr.Substring(posA + 1, posB - posA - 1);
                var substParts = subst.Split(':', 2);
                var substArg = substParts.Length > 1 ? substParts[1] : null;
                
                if (_keyHandlers.TryGetValue(substParts[0], out var substFunc))
                {
                    // If we find a matching substitution key, append the
                    // evaluation of that key's handler function
                    builder.Append(substFunc(target, substArg));
                }
                else
                {
                    // Else, we just append the original string unchanged
                    builder.Append(expr.Substring(posA, posB - posA + 1));
                }

                posA = expr.IndexOf(Token, posB + 1);
            }

            // Append the remainder of the expression
            builder.Append(expr.Substring(posB + 1));

            return builder.ToString();
        }

        public static string ParseEscape(string s)
        {
            if (string.IsNullOrWhiteSpace(s))
                return s;

            StringBuilder buff = new StringBuilder();
            int lastEscPos = s.Length - 3; // The last position that could be used for escaping
            int posB = 0;
            int posA = s.IndexOf(EscToken);
            while (posA >= posB && posA <= lastEscPos)
            {
                buff.Append(s.Substring(posB, posA - posB));

                string hex = s.Substring(posA + 1, 2);
                if (ushort.TryParse(hex, NumberStyles.HexNumber, null, out var val))
                {
                    buff.Append((char)val);
                    posB = posA + 3;
                }
                else
                {
                    buff.Append(s[posA]);
                    posB = posA + 1;
                }
                posA = s.IndexOf(EscToken, posB);
            }

            buff.Append(s.Substring(posB));

            return buff.ToString();
        }
    }
}