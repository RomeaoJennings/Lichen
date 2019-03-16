using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Lichen.AI;

namespace LichenTests
{
    
    public class SearchTests
    {
        [TestClass]
        public class IterativeDeepening
        {
            [TestMethod]
            public void TestMethod1()
            {
                Search search = new Search();
                var bestMove = search.IterativeDeepening(1,0, new Lichen.Model.Position());
            }
        }
    }
}
