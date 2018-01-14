using NUnit.Framework;

namespace Tests.RatanaLibrary.Attributes
{
    /// <summary>
    /// This attribute specifies that the test is an integration test.
    /// </summary>
    public class IntegrationAttribute : CategoryAttribute
    {
        public IntegrationAttribute() : base("Integration")
        { }
    }
}
