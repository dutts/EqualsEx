using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EqualsEx
{
    public enum MatchType
    {
        ValueMatch,
        ReferenceMatch,
        NoMatch
    }

    public class EqualsExResult
    {
        public MatchType ComparisonResult { get; private set; }
        public Dictionary<string, EqualsExResult> Properties { get; private set; }

        public EqualsExResult(MatchType result, Dictionary<string, EqualsExResult> properties)
        {
            ComparisonResult = result;
            Properties = properties;
        }
    }

    public static class EqualsExtended
    {

        public static EqualsExResult EqualsEx<T>(this T t, T o)
        {
            if (object.ReferenceEquals(t, o)) return new EqualsExResult(MatchType.ReferenceMatch, null);

            var propertyInfos = t.GetType().GetProperties();
            var propertyDicts = (propertyInfos.Any()) ? new Dictionary<string, EqualsExResult>() : null;
            foreach (var pi in propertyInfos)
            {
                var thisVal = pi.GetValue(t);
                var otherVal = pi.GetValue(o);

                //TODO: Handle IList and arrays

                if (pi.PropertyType.IsValueType)
                {
                    if (thisVal.Equals(otherVal)) propertyDicts.Add(pi.Name, new EqualsExResult(MatchType.ValueMatch, null));
                    else propertyDicts.Add(pi.Name, new EqualsExResult(MatchType.NoMatch, null));
                }
                else
                {
                    propertyDicts.Add(pi.Name, thisVal.EqualsEx(otherVal));
                }
            }
            return new EqualsExResult(MatchType.NoMatch, propertyDicts);
        }
    }
}
