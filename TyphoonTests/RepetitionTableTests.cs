using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Lichen.Model;

namespace LichenTests
{
    public class RepetitionTableTests
    {
        [TestClass]
        public class AddKey
        {
            [TestMethod]
            public void IncrementsCountForCommonEntries()
            {
                Random r = new Random(12345678);
                long r1 = (long)r.Next() << 32;
                ulong r2 = (ulong)r.Next();
                ulong r3 = (ulong)r.Next();
                ulong r4 = (ulong)r.Next();


                ulong testVal = (ulong)r1 | r2;
                ulong testVal2 = (ulong)r1 | r3; // TestVal2 Should be different than 1 but hash to same index.
                ulong testVal3 = (ulong)r1 | r4;
                RepetitionTable rt = new RepetitionTable();

                for (int i = 1; i < 11; i++)
                {
                    Assert.AreEqual(i, rt.AddPosition(testVal));
                    Assert.AreEqual(i, rt.AddPosition(testVal2));
                    Assert.AreEqual(i, rt.AddPosition(testVal3));
                }
                
            }
        }

        [TestClass]
        public class RemovePosition
        {
            [TestMethod]
            public void ComputesCorrectly()
            {
                Random r = new Random(12345678);
                long r1 = (long)r.Next() << 32;
                ulong r2 = (ulong)r.Next();
                ulong r3 = (ulong)r.Next();
                ulong r4 = (ulong)r.Next();


                ulong testVal = (ulong)r1 | r2;
                ulong testVal2 = (ulong)r1 | r3; // TestVal2 Should be different than 1 but hash to same index.
                ulong testVal3 = (ulong)r1 | r4;
                RepetitionTable rt = new RepetitionTable();
                
                // Add count of 2 to TestVals 1 and 2
                for (int i=0;i<2;i++)
                {
                    rt.AddPosition(testVal);
                    rt.AddPosition(testVal2);
                }
                rt.AddPosition(testVal3);

                rt.RemovePosition(testVal);
                Assert.AreEqual(1, rt.GetCount(testVal));
                Assert.AreEqual(2, rt.GetCount(testVal2));
                rt.RemovePosition(testVal3);
                Assert.AreEqual(0, rt.GetCount(testVal3));
                rt.RemovePosition(testVal);
                Assert.AreEqual(0, rt.GetCount(testVal));
                Assert.AreEqual(2, rt.GetCount(testVal2));
                rt.RemovePosition(testVal2);
                Assert.AreEqual(1, rt.GetCount(testVal2));
            }
        }

        [TestClass]
        public class GetCount
        {
            [TestMethod]
            public void ComputesCorrectly()
            {
                Random r = new Random(8675309);
                RepetitionTable rt = new RepetitionTable();
                for (int i = 0; i < 100; i++)
                {
                    int randCnt = r.Next(10);
                    ulong value = r.NextULong();
                    for (int j = 0; j < randCnt; j++)
                    {
                        rt.AddPosition(value);
                        Assert.AreEqual(j + 1, rt.GetCount(value));
                    }
                }
            }
        }
    }
}
