using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tests.RatanaLibrary.Common.Attributes
{
    /// <summary>
    /// This attribute specifies that the test should be run as part of the
    /// continuous integration set up.
    /// </summary>
    public class ContinuousAttribute : CategoryAttribute
    {
        public ContinuousAttribute() :base("Continuous")
        { }
    }
}
