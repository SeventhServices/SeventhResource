using System;
using System.Collections.Generic;
using System.Linq;

namespace SeventhServices.Resource.CommonTest.Comparer
{
    public class ClassPropertyComparer : IEqualityComparer<object>
    {
        public ClassPropertyComparer()
        {
            
        }

        public ClassPropertyComparer(IEnumerable<string> exceptProperty)
        {
            _exceptProperty = exceptProperty;
        }

        private readonly IEnumerable<string> _exceptProperty;

        bool IEqualityComparer<object>.Equals(object x, object y)
        {
            if (x == y)
            {
                return true;
            }
            if (x == null || y == null)
            {
                return false;
            }

            var xType = x.GetType();
            var yType = y.GetType();

            if (xType.FullName != yType.FullName)
            {
                return false;
            }

            var xProperties = xType.GetProperties();
            var yProperties = yType.GetProperties();

            if (xProperties.Length != yProperties.Length)
            {
                return false;
            }

            for (var i = 0; i < xProperties.Length; i++)
            {
                if (xProperties[i].Name != yProperties[i].Name)
                {
                    return false;
                }

                if (_exceptProperty != null)
                {
                    var shouldExcept = _exceptProperty
                        .Any(exceptProperty => xProperties[i].Name == exceptProperty);

                    if (shouldExcept)
                    {
                        continue;
                    }
 
                }

                var xValue = xProperties[i].GetValue(x);
                var yValue = yProperties[i].GetValue(y);

                var compareResult = xValue switch
                {
                    ValueType vX => Equals(vX, (ValueType) yValue),
                    string vX => vX == (string) yValue,
                    _ => Equals(xValue, yValue)
                };

                if (!compareResult)
                {
                    return false;
                }
            }

            return true;
        }

        int IEqualityComparer<object>.GetHashCode(object obj)
        {
            unchecked
            {
                var properties = obj.GetType().GetProperties();
                var hashCode = 0;
                for (var i = 0; i < properties.Length; i++)
                {
                    var value = properties[i].GetValue(obj);
                    if ( i == 0 )
                    {
                        hashCode = value != null ? value.GetHashCode() : 0;
                    }
                    hashCode = (hashCode * 397) ^ (value != null ? value.GetHashCode() : 0);
                }
                return hashCode;
            }
        }
    }
}