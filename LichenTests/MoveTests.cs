using System;
using Lichen.Model;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace TyphoonTests
{
    public class MoveTests
    {
        [TestClass]
        public class MovedPiece
        {
            [TestMethod]
            public void CalculatesCorrectlyWithRegularMove()
            {
                Move move = new Move(Position.A5, Position.H3, Position.PAWN, Position.QUEEN, Position.ROOK);

                Assert.AreEqual(Position.A5, move.OriginSquare());
                Assert.AreEqual(Position.H3, move.DestinationSquare());
                Assert.AreEqual(Position.PAWN, move.MovedPiece());
                Assert.AreEqual(Position.QUEEN, move.CapturePiece());
                Assert.AreEqual(Position.ROOK, move.PromotionType());
            }
        }
    }
}
