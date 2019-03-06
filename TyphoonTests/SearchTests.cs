using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Typhoon.AI;

namespace TyphoonTests
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
                var bestMove = search.IterativeDeepening(1, new Typhoon.Model.Position());
            }
        }
    }
}
