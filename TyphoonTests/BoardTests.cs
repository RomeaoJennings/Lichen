using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Typhoon.Model;

namespace TyphoonTests
{
    [TestClass]
    public class BoardTests
    {
        [TestMethod]
        [TestCategory("Board")]
        public void Test_EqualityOperators()
        {
            Board b1 = new Board();
            Board b2 = new Board();
            Assert.AreEqual(true, b1 == b2);
            Assert.AreEqual(false, b1 != b2);
            Assert.AreEqual(true, b1.Equals(b2));
        }

        [TestMethod]
        [TestCategory("Board")]
        public void Test_GetHashCode_MatchesEquality()
        {
            Board b1 = new Board();
            Board b2 = new Board();
            Assert.AreEqual(true, b1.GetHashCode() == b2.GetHashCode());
        }
    }
}
