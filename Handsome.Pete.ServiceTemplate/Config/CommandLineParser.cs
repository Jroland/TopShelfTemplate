using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Handsome.Pete.ServiceTemplate
{
    public class CommandLineParser
    {
        private readonly Regex _paramMatch = new Regex(@"(/\w+):?(.*)", RegexOptions.Compiled);

        public Dictionary<string, string> ParamList { get; set; }

        #region Public Methods...
        public IEnumerable<KeyValuePair<string, string>> GetAppSettings()
        {
            return ExtractSetting("/a");
        }

        public IEnumerable<KeyValuePair<string, string>> GetConnectionStrings()
        {
            return ExtractSetting("/c");
        }

        // ReSharper disable ParameterTypeCanBeEnumerable.Local
        // Can't be IEnumberable or it wont be found by the service entry point.
        public CommandLineParser(string[] args)
        // ReSharper restore ParameterTypeCanBeEnumerable.Local
        {
            ParamList = new Dictionary<string, string>();

            foreach (var arg in args)
            {
                Match match = _paramMatch.Match(arg);
                if (match.Groups.Count >= 3)
                {
                    ParamList.Add(match.Groups[1].Value, match.Groups[2].Value);
                }
            }
        }
        #endregion

        #region Private Methods...
        private IEnumerable<KeyValuePair<string, string>> ExtractSetting(string command)
        {
            return ParamList.Where(x => x.Key == command).Select(x =>
            {
                var av = x.Value.Split(new[] { '|' }, StringSplitOptions.RemoveEmptyEntries);
                if (av.Length != 2) throw new IndexOutOfRangeException("App or ConnectionString settings must have a piped value in this format:  key|value");

                return new KeyValuePair<string, string>(av[0], av[1]);
            });
        }
        #endregion
    }
}
