using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Typhoon.Model;

namespace TyphoonTests
{
    public class CastleRightsTests
    {
        [TestClass]
        public class Equality
        {
            private readonly CastleRights c1 = new CastleRights(true, false, true, false);
            private readonly CastleRights c2 = new CastleRights(true, false, false, false);
            private readonly CastleRights c3 = new CastleRights(true, false, true, false);

            [TestMethod]
            [TestCategory("CastleRights")]
            public void CalculatesCorrectly()
            {
                Assert.IsFalse(c1 == c2);
                Assert.IsTrue(c1 != c2);
                Assert.IsFalse(c1.Equals(c2));

                Assert.IsFalse(c2 == c3);
                Assert.IsTrue(c2 != c3);
                Assert.IsFalse(c2.Equals(c3));

                Assert.IsTrue(c1 == c3);
                Assert.IsFalse(c1 != c3);
                Assert.IsTrue(c1.Equals(c3));
            }
        }

        [TestClass]
        public class GetHashCodeTest
        {
            private readonly CastleRights c1 = new CastleRights(true, false, true, false);
            private readonly CastleRights c2 = new CastleRights(true, false, false, false);
            private readonly CastleRights c3 = new CastleRights(true, false, true, false);

            [TestMethod]
            [TestCategory("CastleRights")]
            public void CalculatesCorrectly()
            {
                Assert.IsFalse(c1.GetHashCode() == c2.GetHashCode());
                Assert.IsTrue(c1.GetHashCode() == c3.GetHashCode());
            }

            [TestMethod]
            [TestCategory("CastleRights")]
            public void MatchesEquality()
            {
                Random r = new Random(8675309);
                for (int i = 0; i < 100; i++)
                {
                    CastleRights c4 = new CastleRights(
                        r.Next(1) == 1,
                        r.Next(1) == 1,
                        r.Next(1) == 1,
                        r.Next(1) == 1);
                    CastleRights c5 = new CastleRights(
                        r.Next(1) == 1,
                        r.Next(1) == 1,
                        r.Next(1) == 1,
                        r.Next(1) == 1);
                    Assert.IsTrue((c4 == c5) == (c4.GetHashCode() == c5.GetHashCode()));
                }
            }
        }

        [TestClass]
        public class FromFen
        {
            [TestMethod]
            [TestCategory("CastleRights")]
            public void CalculatesCorrectly()
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
}
