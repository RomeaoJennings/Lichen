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
                BaseSearch search = new BaseSearch();
                var bestMove = search.IterativeDeepening(1, new Lichen.Model.Position());
            }
        }
    }
}
