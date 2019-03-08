using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Lichen.AI;

namespace LichenTests
{
    
    public class EvaluateTests
    {
        [TestClass]
        public class EvaluatePosition
        {
            [TestMethod]
            public void StartingPositionHasZeroScore()
            {
                Assert.AreEqual(0, Evaluate.EvaluatePosition(new Lichen.Model.Position()));
            }
        }
    }
}
