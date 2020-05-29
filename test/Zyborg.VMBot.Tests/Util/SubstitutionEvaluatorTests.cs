using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Amazon.EC2.Model;
using Zyborg.VMBot.Util;
using Xunit;

namespace VMBot.Tests.Util
{
    public class SubstitutionEvaluatorTests
    {
        public const string TestEnvVarName = "TestEnvVar";
        public const string TestEnvVarValue = "Test Environment Variable Value!";

        [Theory]
        [InlineData(null, null)]
        [InlineData("", "")]
        [InlineData(" ", " ")]
        [InlineData("a", "a")]
        [InlineData("a b c", "a b c")]
        [InlineData("%", "%")]
        [InlineData(" a %", " a %")]
        [InlineData("% c ", "% c ")]
        [InlineData(" a % c ", " a % c ")]

        [InlineData("%foo%", "%foo%")]
        [InlineData(" a %foo%", " a %foo%")]
        [InlineData("%foo% c ", "%foo% c ")]
        [InlineData(" a %foo% c ", " a %foo% c ")]
        [InlineData(" a %foo%% c ", " a %foo%% c ")]

        [InlineData("%ENV:NoSuchEnvVar%", "%ENV:NoSuchEnvVar%")]
        [InlineData("%ENV?:NoSuchEnvVar%", "")]
        [InlineData("%ENV:" + TestEnvVarName + "%", TestEnvVarValue)]
        [InlineData("%ENV?:" + TestEnvVarName + "%", TestEnvVarValue)]

        [InlineData("%PROP:NoSuchProperty%", "%PROP:NoSuchProperty%")]
        [InlineData("%PROP?:NoSuchProperty%", "")]
        [InlineData("%PROP:ProcessName%", "dotnet")]
        [InlineData("%PROP?:ProcessName%", "dotnet")]

        [InlineData("%PROC_NAME%", "%PROC_NAME%")]

        public Task eval_with_only_common_handlers(string expression, string expected)
        {
            System.Environment.SetEnvironmentVariable(TestEnvVarName, TestEnvVarValue,
                EnvironmentVariableTarget.Process);

            var eval = new SubstitutionEvaluator<Process>()
                .AddCommonHandlers();
            var proc = Process.GetCurrentProcess();

            Assert.Equal(expected, eval.Evaluate(expression, proc));
            return Task.CompletedTask;
        }

        [Theory]
        [InlineData("%PROC_NAME%", "dotnet")]
        [InlineData("%PROC_NAME% / %PROC_EXITED%", "dotnet / False")]
        // ** These don't work because we can't get StartInfo for a process we didn't start :-( **
        // [InlineData("%START_VERB%", "dotnet")]
        // [InlineData("%START_VERBS%", "dotnet")]
        // [InlineData("%START_ENV%", "")]
        // [InlineData("%START_ENV:%", "")]
        // [InlineData("%START_ENV:%", "")]
        // [InlineData("%START_ENV:NoSuchEnvVar%", "")]
        // [InlineData("%START_ENV:" + TestEnvVarName + "%", TestEnvVarValue)]
        public Task eval_proc_with_common_and_custom_handlers(string expression, string expected)
        {
            System.Environment.SetEnvironmentVariable(TestEnvVarName, TestEnvVarValue,
                EnvironmentVariableTarget.Process);

            var eval = new SubstitutionEvaluator<Process>()
                .AddCommonHandlers()
                .AddHandler("PROC_NAME", (p, _) => p.ProcessName)
                .AddHandler("START_TIME", (p, _) => p.StartTime.ToString())
                .AddHandlers(new Dictionary<string, Func<Process, string, string>>
                {
                    ["PROC_EXITED"] = (p, _) => p.HasExited.ToString(),
                    ["PROC_HOST"] = (p, _) => p.MachineName,
                    ["START_USER"] = (p, _) => p.StartInfo.UserName,
                    ["START_VERB"] = (p, _) => p.StartInfo.Verb,
                    ["START_VERBS"] = (p, _) => string.Join(",", p.StartInfo.Verbs),
                    ["START_ENV"] = (p, a) => p.StartInfo.EnvironmentVariables[a],
                });
            var proc = Process.GetCurrentProcess();

            Assert.Equal(expected, eval.Evaluate(expression, proc));
            return Task.CompletedTask;
        }


        [Theory]
        [InlineData("%MY_FORMAT%", "1970/01/01 00:00:00")]
        [InlineData("%DATETIME_KINDS%", "Local,Unspecified,Utc")]
        [InlineData(" %THE_YEAR% %THE_MONTH% %THE_DAY% %THE_DOW% ", " 1970 1 1 Thu ")]
        [InlineData(" %THE_YEAR% %THE_MONTH% %THE_DAY% %THE_DAYOFWEEK% ", " 1970 1 1 Thursday ")]
        
        [InlineData("%TOSTRIN%", "%TOSTRIN%")]
        [InlineData("%TOSTRING%", "1/1/1970 12:00:00 AM")]
        [InlineData("%TOSTRING:%", "1/1/1970 12:00:00 AM")]
        [InlineData("%TOSTRING:''%", "")]
        [InlineData("%TOSTRING:M''%", "1")]
        [InlineData("%TOSTRING:MM%", "01")]
        [InlineData("%TOSTRING:MMM%", "Jan")]
        [InlineData("%TOSTRING:MMMM%", "January")]

        [InlineData("%ADD_SECS:90%", "1970/01/01 00:01:30")]
        [InlineData("%ADD_MINS:90%", "1970/01/01 01:30:00")]
        [InlineData("%ADD:11:22:33%", "1970/01/01 11:22:33")]
        [InlineData("%ADD:366%", "1971/01/02 00:00:00")]
        [InlineData("%ADD:-366%", "1968/12/31 00:00:00")]
        public Task eval_epoch_with_common_and_custom_handlers(string expression, string expected)
        {
            var myFormat = "yyyy/MM/dd HH:mm:ss";
            var eval = new SubstitutionEvaluator<DateTime>()
                .AddCommonHandlers()
                .AddHandler("DATETIME_KINDS", (d, _) =>
                    string.Join(",", Enum.GetNames(typeof(DateTimeKind)).OrderBy(k => k)))
                .AddHandler("THE_YEAR", (d, _) => d.Year.ToString())
                .AddHandler("THE_MONTH", (d, _) => d.Month.ToString())
                .AddHandler("THE_DAY", (d, _) => d.Day.ToString())
                .AddHandler("THE_DOW", (d, _) => d.ToString("ddd"))
                .AddHandler("THE_DAYOFWEEK", (d, _) => d.ToString("dddd"))
                .AddHandlers(new Dictionary<string, Func<DateTime, string, string>>
                {
                    ["MY_FORMAT"] = (d, _) => d.ToString(myFormat),
                    ["TOSTRING"]  = (d, fmt) => d.ToString(fmt),
                    ["ADD"]       = (d, tspan) => d.Add(TimeSpan.Parse(tspan)).ToString(myFormat),
                    ["ADD_SECS"]  = (d, secs) => d.AddSeconds(int.Parse(secs)).ToString(myFormat),
                    ["ADD_MINS"]  = (d, mins) => d.AddMinutes(int.Parse(mins)).ToString(myFormat),
                });
            var proc = Process.GetCurrentProcess();

            Assert.Equal(expected, eval.Evaluate(expression, DateTime.UnixEpoch));
            return Task.CompletedTask;
        }

        [Theory]
        [InlineData("%", "%")]
        [InlineData(" % ", " % ")]
        [InlineData(" %% ", " % ")]
        [InlineData(" %% % ", " % % ")]

        [InlineData("%PCT%", "%")]
        [InlineData("%PCT:%", "%")]
        [InlineData("%PCT:-1%", "%")]
        [InlineData("%PCT:2%", "%%")]
        [InlineData(" %PCT:10% %PCT:3% %PCT:10", " %%%%%%%%%% %%% %PCT:10")]

        [InlineData("%ESC%", "")]
        [InlineData("%ESC:%", "")]
        [InlineData("%ESC: %", " ")]
        [InlineData("%ESC: a b c %", " a b c ")]
        [InlineData("%ESC:#44#6F#67%", "Dog")]
        [InlineData("%ESC:#44#6F#6%", "Do#6")]
        [InlineData("%ESC:#44##67%", "D#g")]
        [InlineData("%ESC:Cats #26 Dogs%", "Cats & Dogs")]
        [InlineData("%ESC:#3b #23 #25 #26 #3a%", "; # % & :")]
        public Task eval_escape_forms(string expression, string expected)
        {
            var eval = new SubstitutionEvaluator<object>()
                .AddCommonHandlers();
            Assert.Equal(expected, eval.Evaluate(expression, null));
            return Task.CompletedTask;
        }
    }
}