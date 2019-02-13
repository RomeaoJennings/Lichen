using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Typhoon.Model;

namespace TyphoonTests
{
    [TestClass]
    public class CastleRightsTests
    {
        CastleRights c1 = new CastleRights(true, false, true, false);
        CastleRights c2 = new CastleRights(true, false, false, false);
        CastleRights c3 = new CastleRights(true, false, true, false);

        [TestMethod]
        [TestCategory("CastleRights")]
        public void Test_Equality()
        {
            Assert.AreEqual(false, c1 == c2);
            Assert.AreEqual(true, c1 != c2);
            Assert.AreEqual(false, c1.Equals(c2));

            Assert.AreEqual(false, c2 == c3);
            Assert.AreEqual(true, c2 != c3);
            Assert.AreEqual(false, c2.Equals(c3));

            Assert.AreEqual(true, c1 == c3);
            Assert.AreEqual(false, c1 != c3);
            Assert.AreEqual(true, c1.Equals(c3));
        }

        [TestMethod]
        [TestCategory("CastleRights")]
        public void Test_GetHashCode()
        {
            Assert.AreEqual(false, c1.GetHashCode() == c2.GetHashCode());
            Assert.AreEqual(true, c1.GetHashCode() == c3.GetHashCode());
        }

        [TestMethod]
        [TestCategory("CastleRights")]
        public void Test_GetHashCode_MatchesEquality()
        {
            Random r = new Random(8675309);
            for (int i = 0; i < 100; i++)
            {
                CastleRights c1 = new CastleRights(r.Next(1) == 1, r.Next(1) == 1, r.Next(1) == 1, r.Next(1) == 1);
                CastleRights c2 = new CastleRights(r.Next(1) == 1, r.Next(1) == 1, r.Next(1) == 1, r.Next(1) == 1);
                Assert.AreEqual(c1 == c2, c1.GetHashCode() == c2.GetHashCode());
            }
        }

        [TestMethod]
        [TestCategory("CastleRights")]
        public void Test_FromFEN()
        {
            Assert.AreEqual(CastleRights.All, CastleRights.FromFEN("KQkq"));
            Assert.AreNotEqual(CastleRights.All, CastleRights.FromFEN("KQk"));
            Assert.AreNotEqual(CastleRights.All, CastleRights.FromFEN("-"));

            CastleRights cr = new CastleRights(true, false, false, false);
            Assert.AreEqual(cr, CastleRights.FromFEN("K"));
            Assert.AreNotEqual(cr, CastleRights.FromFEN("k"));
        }
    }
}
