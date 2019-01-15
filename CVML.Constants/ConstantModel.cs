using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CVML.Constants
{
    class ConstantModel
    {
        public String Name { get; set; }
        public String Module { get; set; }
        public String DataType { get; set; }
        public List<ConstantValue> Values { get; set; }
    }
    class ConstantValue
    {
        public String Framework { get; set; }
        public String Value { get; set; }
    }
}
