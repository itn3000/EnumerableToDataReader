using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EnumerableToDataReader
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
    public class FieldNameAsAttribute : Attribute
    {
        public FieldNameAsAttribute(string name)
        {
            Name = name;
        }
        public string Name { get; set; }
    }
}
