using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Seventh.Resource.Common.Options
{
    public class SortOptions
    {
        public SortOptions()
        {
            RevSpecRules.Add(new Regex(@"_\d+[\._]"), false);
            RevSpecRules.Add(new Regex(@"[Rr]ule"), true);
            RevSpecRules.Add(new Regex(@"[Uu]pdate"), true);
            RevSpecRules.Add(new Regex(@"[Tt]op"), true);
            RevSpecRules.Add(new Regex(@"_\d{1}[\.]"), true);
            ConsumeRules.Add(new Regex(@"^m_[A-Za-z0-9_]+\.sql$"), "sql:");
            ConsumeRules.Add(new Regex(@"\.acb$"), "acb:");
            ConsumeRules.Add(new Regex(@"^^M[A-Za-z0-9_]+\."), "music:");
        }

        public Dictionary<Regex, string> ConsumeRules { get; } = new Dictionary<Regex, string>();
        public Dictionary<Regex, bool> RevSpecRules { get; } = new Dictionary<Regex, bool>();
    }
}