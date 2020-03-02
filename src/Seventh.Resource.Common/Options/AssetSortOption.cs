using System.Collections.Generic;
using System.Collections.Immutable;
using System.Text.RegularExpressions;

namespace Seventh.Resource.Common.Options
{
    public class SortOptions
    {
        public SortOptions()
        {
            RevSpecRules.Add(new Regex(@"\d"),false);
            RevSpecRules.Add(new Regex(@"[Rr]ule"),true);
            RevSpecRules.Add(new Regex(@"[Uu]pdate"),true);
            RevSpecRules.Add(new Regex(@"[Tt]op"),true);
        }

        public ImmutableDictionary<string, string> Rules { get; }
        public Dictionary<Regex,bool> RevSpecRules { get; } = new Dictionary<Regex,bool>();
    }
}   