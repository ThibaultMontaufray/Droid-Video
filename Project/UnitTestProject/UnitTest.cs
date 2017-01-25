using Droid_video;
using NUnit.Framework;

namespace UnitTestProject
{
    [TestFixture]
    public class UnitTest
    {
        [Test]
        public void TestUTRuns()
        {
            Assert.IsTrue(true);
        }
        [Test]
        public void Test_demo()
        {
            try
            {
                Demo vf = new Demo(null);
                Assert.IsTrue(true);
            }
            catch (System.Exception exp)
            {
                Assert.Fail(exp.Message);
            }
        }
        [Test]
        public void Test_interfaceVideo()
        {
            try
            {
                Interface_vdo intVdo = new Interface_vdo(new System.Collections.Generic.List<string>());
                var v1 = intVdo.CurrentVideo.Path;

                Assert.IsTrue(true);
            }
            catch (System.Exception exp)
            {
                Assert.Fail(exp.Message);
            }
        }
    }
}
