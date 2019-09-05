using NUnit.Framework;

namespace Tests.Ratana.Library.Attributes
{
    /// <summary>
    /// This attribute specifies that the test should be run as part of the
    /// continuous integration set up.
    /// </summary>
    public class RequireThirdPartyServiceAttribute : CategoryAttribute
    {
        public RequireThirdPartyServiceAttribute() : base("RequireThirdPartyService")
        { }
    }
}
