using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tests.RatanaLibrary.Common.Attributes
{
    /// <summary>
    /// This attribute specifies that the test is a unit test; testing a singular unit of function.
    /// </summary>
    public class UnitAttribute : CategoryAttribute
    {
        public UnitAttribute() :base("Unit")
        { }
    }
}
