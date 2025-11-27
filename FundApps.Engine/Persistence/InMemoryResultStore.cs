using System.Collections.Generic;
using FundApps.Engine.Domain;

namespace FundApps.Engine.Persistence
{
    public class InMemoryResultStore
    {
        private readonly List<RuleResult> _results = [];
        public void Save(IReadOnlyList<RuleResult> result) => _results.AddRange(result);
        public IReadOnlyList<RuleResult> GetAll() => _results.AsReadOnly();
    }
}
