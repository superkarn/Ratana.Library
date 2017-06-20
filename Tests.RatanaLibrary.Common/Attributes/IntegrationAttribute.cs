using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tests.RatanaLibrary.Common.Attributes
{
    /// <summary>
    /// This attribute specifies that the test is an integration test.
    /// </summary>
    public class IntegrationAttribute : CategoryAttribute
    {
        public IntegrationAttribute() :base("Integration")
        { }
    }
}
