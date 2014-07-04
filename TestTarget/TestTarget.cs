using System;
using FIFA.Framework;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace TestTarget
{
    [TestContainer]
    public class TestTarget
    {
        [Test]
        public void PassedTest()
        {
            Assert.AreEqual(1, 1);
        }

        [Test]
        public void FailedTest()
        {
            Assert.AreEqual(2, 1);
        }
    }
}
