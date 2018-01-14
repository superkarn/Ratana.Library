using NUnit.Framework;

namespace Tests.RatanaLibrary.Attributes
{
    /// <summary>
    /// This attribute specifies that the test is a unit test; testing a singular unit of function.
    /// </summary>
    public class UnitAttribute : CategoryAttribute
    {
        public UnitAttribute() : base("Unit")
        { }
    }
}
