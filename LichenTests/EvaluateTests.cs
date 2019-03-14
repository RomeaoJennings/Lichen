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
                Evaluate eval = new Evaluate();
                Assert.AreEqual(0, eval.EvaluatePosition(new Lichen.Model.Position()));
            }
        }
    }
}
