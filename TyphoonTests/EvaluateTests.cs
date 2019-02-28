using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Typhoon.Search;

namespace TyphoonTests
{
    
    public class EvaluateTests
    {
        [TestClass]
        public class EvaluatePosition
        {
            [TestMethod]
            public void StartingPositionHasZeroScore()
            {
                Assert.AreEqual(0, Evaluate.EvaluatePosition(new Typhoon.Model.Position()));
            }
        }
    }
}
