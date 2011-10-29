using NUnit.Framework;

namespace DBTests
{
    [TestFixture]
    public class AdventureWorksTests : TestBase
    {
        protected override int CommandTimeout
        {
            get { return 60; }
        }

        protected override string ConnectionString
        {
            get { return "Data Source=.;Initial Catalog=AdventureWorksLT;Integrated Security=true"; }
        }

        protected override string TestSchema
        {
            get { return "Test"; }
        }
    }
}
