using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using Typhoon.Model;

namespace TyphoonTests
{
    using Bitboard = UInt64;

    public class BoardTests
    {
        [TestClass]
        public class AttackersTo
        {
            [TestMethod]
            public void ComputesCorrectly()
            {
                Position board = Position.FromFen("rnbqkbnr/2p1pppp/1p2B3/1R1p3Q/2PKPN2/2N5/P2P1PPP/2B4R b k - 12 16");
                Assert.AreEqual(0x8413C200000UL, board.AttackersTo(Position.D5, Position.WHITE));

                board = Position.FromFen("rnbqkbnr/2pp2pp/R3pp2/1P1P4/5N2/6PB/1PP1PP1P/1NBQK2R b Kk - 2 9");
                Assert.AreEqual(0x801004010000UL, board.AttackersTo(Position.E6, Position.WHITE));

                board = Position.FromFen("2b1kbnr/2ppq1pp/r3Pp2/1P6/3n1N2/6PB/1PP1PP1P/1NBQK2R w Kk - 5 13");
                Assert.AreEqual(0x18800010000000UL, board.AttackersTo(Position.E6, Position.BLACK));
            }
        }

        [TestClass]
        public class GenerateMovesFromBitboard
        {
            [TestMethod]
            public void GeneratesCorrectMovesForKnightOnF3()
            {
                Position board = Position.FromFen("r1bqkbnr/2p2ppp/p2p4/1p2p1B1/3nP3/3P1N2/PPPN1PPP/R2QKBR1 w Qkq b6 0 7");
                Move[] expectedMoves = {
                    new Move(Position.F3, Position.D4, Position.KNIGHT),
                    new Move(Position.F3, Position.E5, Position.PAWN),
                    new Move(Position.F3, Position.H4, Position.EMPTY)
                };
                MoveList actualMoves = new MoveList();
                Bitboard attacks = ~board.GetPieceBitboard(Position.WHITE, Position.ALL_PIECES)
                    & Bitboards.KnightBitboards[Position.F3];
                int square = Position.F3;

                board.GenerateMovesFromBitboard(actualMoves, attacks, square);
                TestUtils.TestArrayEquality(expectedMoves, actualMoves.ToArray());
            }
        }

        [TestClass]
        public class GetAllStepPieceMoves
        {
            [TestMethod]
            public void GeneratesCorrectMoves()
            {
                Position board = Position.FromFen("r1bqkbnr/2p2ppp/p2p4/1p2p1B1/3nP3/3P1N2/PPPN1PPP/R2QKBR1 w Qkq b6 0 7");
                Move[] expectedMoves = {
                    new Move(Position.F3, Position.D4, Position.KNIGHT),
                    new Move(Position.F3, Position.E5, Position.PAWN),
                    new Move(Position.F3, Position.H4),
                    new Move(Position.D2, Position.B1),
                    new Move(Position.D2, Position.B3),
                    new Move(Position.D2, Position.C4)
                };

                MoveList actualMoves = new MoveList();
                Bitboard destinationBitboard = ~board.GetPieceBitboard(Position.WHITE, Position.ALL_PIECES);

                board.GetAllStepPieceMoves(actualMoves, Position.KNIGHT, Position.WHITE, destinationBitboard);
                TestUtils.TestArrayEquality(expectedMoves, actualMoves.ToArray());
            }
        }

        [TestClass]
        public class GetSlidingPieceMoves
        {
            [TestMethod]
            public void GeneratesCorrectBishopMoves()
            {
                Position board = Position.FromFen("r1bqkb1r/pppp1ppp/2n2n2/1B2p3/P3P3/3P4/1PP2PPP/RNBQK1NR b KQkq - 0 4");
                Move[] expectedMoves =
                {
                    new Move(Position.B5, Position.A6),
                    new Move(Position.B5, Position.C6, Position.KNIGHT),
                    new Move(Position.B5, Position.C4)
                };

                MoveList actualMoves = new MoveList();
                Bitboard destinationBitboard = ~board.GetPieceBitboard(Position.WHITE, Position.ALL_PIECES);
                board.GetSlidingPieceMoves(
                    actualMoves,
                    Position.BISHOP,
                    Position.B5,
                    destinationBitboard);
                TestUtils.TestArrayEquality(expectedMoves, actualMoves.ToArray());
            }

            [TestMethod]
            public void GeneratesCorrectRookMoves()
            {
                Position board = Position.FromFen("rnbqkbn1/ppp1ppp1/6r1/2Rp3p/7P/8/PPPPPPP1/RNBQKBN1 w Qq d6 0 5");
                Move[] expectedMoves =
                {
                    new Move(Position.C5, Position.B5),
                    new Move(Position.C5, Position.A5),
                    new Move(Position.C5, Position.C6),
                    new Move(Position.C5, Position.C7, Position.PAWN),
                    new Move(Position.C5, Position.D5, Position.PAWN),
                    new Move(Position.C5, Position.C4),
                    new Move(Position.C5, Position.C3)
                };

                MoveList actualMoves = new MoveList();
                Bitboard destinationBitboard = ~board.GetPieceBitboard(Position.WHITE, Position.ALL_PIECES);
                board.GetSlidingPieceMoves(
                    actualMoves,
                    Position.ROOK,
                    Position.C5,
                    destinationBitboard);
                TestUtils.TestArrayEquality(expectedMoves, actualMoves.ToArray());
            }

            [TestMethod]
            public void GeneratesCorrectQueenMoves()
            {
                Position board = Position.FromFen("r1bqkb1r/pppp1ppp/2n5/1B2p2n/P3P3/3P4/1PP2PPP/RNBQK1NR w KQkq - 1 5");
                Move[] expectedMoves =
                {
                    new Move(Position.D1, Position.D2),
                    new Move(Position.D1, Position.E2),
                    new Move(Position.D1, Position.F3),
                    new Move(Position.D1, Position.G4),
                    new Move(Position.D1, Position.H5, Position.KNIGHT)
                };

                MoveList actualMoves = new MoveList();

                Bitboard destinationBitboard = ~board.GetPieceBitboard(Position.WHITE, Position.ALL_PIECES);
                board.GetSlidingPieceMoves(
                    actualMoves,
                    Position.QUEEN,
                    Position.D1,
                    destinationBitboard);
                TestUtils.TestArrayEquality(expectedMoves, actualMoves.ToArray());
            }
        }

        [TestClass]
        public class GetAllSlidingPieceMoves
        {
            [TestMethod]
            public void GetsAllBishopMoves()
            {
                Position board = Position.FromFen("rnbq1knr/1pp1bppp/p2p4/1B2p2Q/P3P2P/3P4/1PP2PP1/RNB1K1NR b KQ a3 0 6");
                Move[] expectedWhiteBishopMoves =
                {
                    new Move(Position.C1, Position.D2),
                    new Move(Position.C1, Position.E3),
                    new Move(Position.C1, Position.F4),
                    new Move(Position.C1, Position.G5),
                    new Move(Position.C1, Position.H6),
                    new Move(Position.B5, Position.A6, Position.PAWN),
                    new Move(Position.B5, Position.C4),
                    new Move(Position.B5, Position.C6),
                    new Move(Position.B5, Position.D7),
                    new Move(Position.B5, Position.E8)
                };
                Bitboard destinationBitboard = ~board.GetPieceBitboard(Position.WHITE, Position.ALL_PIECES);
                MoveList actualMoves = new MoveList();
                board.GetAllSlidingPieceMoves(actualMoves, Position.BISHOP, Position.WHITE, destinationBitboard);
                TestUtils.TestArrayEquality(expectedWhiteBishopMoves, actualMoves.ToArray());
            }
        }

        [TestClass]
        public class GetPawnPushMoves
        {
            [TestMethod]
            public void GetsCorrectMovesFromOpeningPosition()
            {
                string fen = "rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1";
                Move[] expectedWhiteMoves =
                {
                    new Move(Position.A2, Position.A3),
                    new Move(Position.A2, Position.A4),
                    new Move(Position.B2, Position.B3),
                    new Move(Position.B2, Position.B4),
                    new Move(Position.C2, Position.C3),
                    new Move(Position.C2, Position.C4),
                    new Move(Position.D2, Position.D3),
                    new Move(Position.D2, Position.D4),
                    new Move(Position.E2, Position.E3),
                    new Move(Position.E2, Position.E4),
                    new Move(Position.F2, Position.F3),
                    new Move(Position.F2, Position.F4),
                    new Move(Position.G2, Position.G3),
                    new Move(Position.G2, Position.G4),
                    new Move(Position.H2, Position.H3),
                    new Move(Position.H2, Position.H4),
                };
                TestUtils.TestPawnMoves(false, fen, expectedWhiteMoves, Position.WHITE);

                Move[] expectedBlackMoves =
                {
                    new Move(Position.A7, Position.A6),
                    new Move(Position.A7, Position.A5),
                    new Move(Position.B7, Position.B6),
                    new Move(Position.B7, Position.B5),
                    new Move(Position.C7, Position.C6),
                    new Move(Position.C7, Position.C5),
                    new Move(Position.D7, Position.D6),
                    new Move(Position.D7, Position.D5),
                    new Move(Position.E7, Position.E6),
                    new Move(Position.E7, Position.E5),
                    new Move(Position.F7, Position.F6),
                    new Move(Position.F7, Position.F5),
                    new Move(Position.G7, Position.G6),
                    new Move(Position.G7, Position.G5),
                    new Move(Position.H7, Position.H6),
                    new Move(Position.H7, Position.H5),
                };
                TestUtils.TestPawnMoves(false, fen, expectedBlackMoves, Position.BLACK);
            }

            [TestMethod]
            public void HandlesBlockedPiecesForWhite()
            {
                string fen = "7k/8/8/2pPp3/2P1P3/6pP/P1P3P1/7K w - - 0 1";
                Move[] expectedMoves =
                {
                    new Move(Position.A2, Position.A3),
                    new Move(Position.A2, Position.A4),
                    new Move(Position.C2, Position.C3),
                    new Move(Position.D5, Position.D6),
                    new Move(Position.H3, Position.H4)
                };
                TestUtils.TestPawnMoves(false, fen, expectedMoves, Position.WHITE);
            }

            [TestMethod]
            public void HandlesBlockedPiecesForBlack()
            {
                string fen = "8/3ppp2/3P4/p3P2p/P2K4/8/4k3/8 b - - 0 1";
                Move[] expectedMoves =
                {
                    new Move(Position.E7, Position.E6),
                    new Move(Position.F7, Position.F6),
                    new Move(Position.F7, Position.F5),
                    new Move(Position.H5, Position.H4)
                };
                TestUtils.TestPawnMoves(false, fen, expectedMoves, Position.BLACK);
            }

            [TestMethod]
            public void HandlesPromotions()
            {
                string fen = "5r2/1P3P2/8/4k1K1/8/8/4p2p/7R w - - 0 1";
                Move[] expectedWhiteMoves =
                {
                    new Move(Position.B7, Position.B8, Position.EMPTY, Position.QUEEN),
                    new Move(Position.B7, Position.B8, Position.EMPTY, Position.ROOK),
                    new Move(Position.B7, Position.B8, Position.EMPTY, Position.BISHOP),
                    new Move(Position.B7, Position.B8, Position.EMPTY, Position.KNIGHT)
                };
                TestUtils.TestPawnMoves(false, fen, expectedWhiteMoves, Position.WHITE);

                Move[] expectedBlackMoves =
                {
                    new Move(Position.E2, Position.E1, Position.EMPTY, Position.QUEEN),
                    new Move(Position.E2, Position.E1, Position.EMPTY, Position.ROOK),
                    new Move(Position.E2, Position.E1, Position.EMPTY, Position.BISHOP),
                    new Move(Position.E2, Position.E1, Position.EMPTY, Position.KNIGHT)
                };
                TestUtils.TestPawnMoves(false, fen, expectedBlackMoves, Position.BLACK);
            }
        }

        [TestClass]
        public class GetPawnCaptureMoves
        {
            [TestMethod]
            public void GetPawnCaptureMoves_ForWhite_WithoutPromotion()
            {
                string fen = "2bqkbnr/r1p1pp2/p1n3pp/1p1p1P1P/P3P3/2N5/1PPP2P1/R1BQKBNR w KQk - 1 8";
                Move[] expectedMoves =
                {
                    new Move(Position.A4, Position.B5, Position.PAWN),
                    new Move(Position.E4, Position.D5, Position.PAWN),
                    new Move(Position.F5, Position.G6, Position.PAWN),
                    new Move(Position.H5, Position.G6, Position.PAWN)
                };
                TestUtils.TestPawnMoves(true, fen, expectedMoves, Position.WHITE);
            }

            [TestMethod]
            public void GetPawnCaptureMoves_ForBlack_WithoutPromotion()
            {
                string fen = "rnbqkbnr/2pp1p1p/1p2p3/p2N2p1/1P3P1P/8/P1PPP1P1/R1BQKBNR b KQkq - 1 5";
                Move[] expectedMoves =
                {
                    new Move(Position.A5, Position.B4, Position.PAWN),
                    new Move(Position.E6, Position.D5, Position.KNIGHT),
                    new Move(Position.G5, Position.F4, Position.PAWN),
                    new Move(Position.G5, Position.H4, Position.PAWN)
                };
                TestUtils.TestPawnMoves(true, fen, expectedMoves, Position.BLACK);
            }

            [TestMethod]
            public void GetPawnCaptureMoves_White_EnPassent()
            {
                string fen = "rnbqkbnr/1pp1pppp/8/2PpP3/p7/8/PP1P1PPP/RNBQKBNR w KQkq d6 0 5";
                Move[] expectedMoves =
                {
                    new Move(Position.C5, Position.D6, true, false),
                    new Move(Position.E5, Position.D6, true, false)
                };
                TestUtils.TestPawnMoves(true, fen, expectedMoves, Position.WHITE);
            }

            [TestMethod]
            public void GetPawnCaptureMoves_Black_EnPassent()
            {
                string fen = "rn1qkbnr/pbp1p1pp/8/8/3pPp2/8/1PPP1PPP/RNBQKBNR b KQkq e3 0 6";
                Move[] expectedMoves =
                {
                    new Move(Position.D4, Position.E3, true, false),
                    new Move(Position.F4, Position.E3, true, false)
                };
                TestUtils.TestPawnMoves(true, fen, expectedMoves, Position.BLACK);
            }

            [TestMethod]
            public void GetPawnCaptureMoves_WithPromotion()
            {
                string fen = "3r1n2/4P3/8/K7/8/2k5/6p1/5R1R w - - 0 1";
                Move[] expectedWhiteMoves =
                {
                    new Move(Position.E7, Position.D8, Position.ROOK,Position.QUEEN),
                    new Move(Position.E7, Position.D8, Position.ROOK,Position.ROOK),
                    new Move(Position.E7, Position.D8, Position.ROOK,Position.BISHOP),
                    new Move(Position.E7, Position.D8, Position.ROOK,Position.KNIGHT),
                    new Move(Position.E7, Position.F8, Position.KNIGHT,Position.QUEEN),
                    new Move(Position.E7, Position.F8, Position.KNIGHT,Position.ROOK),
                    new Move(Position.E7, Position.F8, Position.KNIGHT,Position.BISHOP),
                    new Move(Position.E7, Position.F8, Position.KNIGHT,Position.KNIGHT)
                };
                TestUtils.TestPawnMoves(true, fen, expectedWhiteMoves, Position.WHITE);

                Move[] expectedBlackMoves =
                {
                    new Move(Position.G2, Position.F1, Position.ROOK,Position.QUEEN),
                    new Move(Position.G2, Position.F1, Position.ROOK,Position.ROOK),
                    new Move(Position.G2, Position.F1, Position.ROOK,Position.BISHOP),
                    new Move(Position.G2, Position.F1, Position.ROOK,Position.KNIGHT),
                    new Move(Position.G2, Position.H1, Position.ROOK,Position.QUEEN),
                    new Move(Position.G2, Position.H1, Position.ROOK,Position.ROOK),
                    new Move(Position.G2, Position.H1, Position.ROOK,Position.BISHOP),
                    new Move(Position.G2, Position.H1, Position.ROOK,Position.KNIGHT)
                };
                TestUtils.TestPawnMoves(true, fen, expectedBlackMoves, Position.BLACK);
            }
        }

        [TestClass]
        public class FromFen
        {
            public void GeneratesProperStartingPosition()
            {
                Position b = new Position();
                Position startingPos = Position.FromFen("rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1");

                for (int color = 0; color < 2; color++)
                {
                    for (int piece = 0; piece < 7; piece++)
                    {
                        Assert.AreEqual(b.GetPieceBitboard(color, piece), startingPos.GetPieceBitboard(color, piece));
                    }
                }
                TestUtils.TestArrayEquality(b.GetPieceSquares(), startingPos.GetPieceSquares());
                Assert.AreEqual(b.FullMoveNumber, startingPos.FullMoveNumber);
                Assert.AreEqual(b.HalfMoveClock, startingPos.HalfMoveClock);
                Assert.AreEqual(b.EnPassentBitboard, startingPos.EnPassentBitboard);
            }

            [TestMethod]
            public void GeneratesBoardFromSicilainPosition()
            {
                Position sicilian = Position.FromFen("rnbqkbnr/pp1ppppp/8/2p5/4P3/5N2/PPPP1PPP/RNBQKB1R b KQkq - 1 2");

                Assert.AreEqual(0x8UL, sicilian.GetPieceBitboard(Position.WHITE, Position.KING));
                Assert.AreEqual(0x10UL, sicilian.GetPieceBitboard(Position.WHITE, Position.QUEEN));
                Assert.AreEqual(0x81UL, sicilian.GetPieceBitboard(Position.WHITE, Position.ROOK));
                Assert.AreEqual(0x24UL, sicilian.GetPieceBitboard(Position.WHITE, Position.BISHOP));
                Assert.AreEqual(0x40040UL, sicilian.GetPieceBitboard(Position.WHITE, Position.KNIGHT));
                Assert.AreEqual(0x800F700UL, sicilian.GetPieceBitboard(Position.WHITE, Position.PAWN));
                Assert.AreEqual(0x804F7FDUL, sicilian.GetPieceBitboard(Position.WHITE, Position.ALL_PIECES));

                Assert.AreEqual(0x800000000000000UL, sicilian.GetPieceBitboard(Position.BLACK, Position.KING));
                Assert.AreEqual(0x1000000000000000UL, sicilian.GetPieceBitboard(Position.BLACK, Position.QUEEN));
                Assert.AreEqual(0x8100000000000000UL, sicilian.GetPieceBitboard(Position.BLACK, Position.ROOK));
                Assert.AreEqual(0x2400000000000000UL, sicilian.GetPieceBitboard(Position.BLACK, Position.BISHOP));
                Assert.AreEqual(0x4200000000000000UL, sicilian.GetPieceBitboard(Position.BLACK, Position.KNIGHT));
                Assert.AreEqual(0xDF002000000000UL, sicilian.GetPieceBitboard(Position.BLACK, Position.PAWN));
                Assert.AreEqual(0xFFDF002000000000UL, sicilian.GetPieceBitboard(Position.BLACK, Position.ALL_PIECES));

                Assert.AreEqual(1, sicilian.HalfMoveClock);
                Assert.AreEqual(2, sicilian.FullMoveNumber);
                Assert.AreEqual(0UL, sicilian.EnPassentBitboard);

                int[] squares = sicilian.GetPieceSquares();
                Assert.AreEqual(squares[37], Position.PAWN);
                Assert.AreEqual(squares[53], Position.EMPTY);
                Assert.AreEqual(squares[18], Position.KNIGHT);
                Assert.AreEqual(squares[1], Position.EMPTY);
            }
        }

        [TestClass]
        public class GetPinnedPiecesBitboard
        {
            [TestMethod]
            public void CorretlyIdentifiesPinnedPieces()
            {
                Position board = Position.FromFen("rnbqkbnr/2pppppp/1p6/pB5Q/4P3/8/PPPP1PPP/RNB1K1NR b KQkq - 1 3");
                Assert.AreEqual(0x14000000000000UL, board.GetPinnedPiecesBitboard());
            }
        }

        [TestClass]
        public class ToFen
        {
            [TestMethod]
            public void GeneratesCorrectlyFromOpeningPosition()
            {
                Position board = new Position();
                Assert.AreEqual("rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1", board.ToFen());
            }
        }

        [TestClass]
        public class ZobristHash
        {
            [TestMethod]
            public void UniqueForEachPosition()
            {
                Random r = new Random();
                Position b = new Position();
                Dictionary<ulong, string> dictionary = new Dictionary<ulong, string>();
                Stack<BoardState> stack = new Stack<BoardState>();

                int cnt = 0;
                Bitboard pinners = b.GetPinnedPiecesBitboard();
                var moves = b.GetAllMoves().ToArray().Where(m => b.IsLegalMove(m, pinners)).ToList();
                // Add 100 random moves, unless checkmate or stalemate occurs first.
                while (moves.Count > 0 && cnt++ <= 1000)
                {
                    var moveToMake = moves[r.Next(moves.Count)];
                    stack.Push(new BoardState(moveToMake, b));
                    b.DoMove(moveToMake);
                    // If move already exists in table, but not matching fen, then we have a problem.
                    if (dictionary.ContainsKey(b.Zobrist) &&
                        dictionary[b.Zobrist] != TestUtils.StripMoveNumsFromFen(b.ToFen()))
                    {
                        Assert.Fail();
                    }
                    if (!dictionary.ContainsKey(b.Zobrist))
                    {
                        dictionary.Add(b.Zobrist, TestUtils.StripMoveNumsFromFen(b.ToFen()));
                    }
                    
                    pinners = b.GetPinnedPiecesBitboard();
                    moves = b.GetAllMoves().ToArray().Where(m => b.IsLegalMove(m, pinners)).ToList();
                }

                while (stack.Count != 1)
                {
                    var curr = stack.Pop();
                    b.UndoMove(curr);
                    if (!dictionary.ContainsKey(b.Zobrist) || 
                        dictionary[b.Zobrist] != TestUtils.StripMoveNumsFromFen(b.ToFen()))
                    {
                        Assert.Fail();
                    }
                }
            }
        }

        [TestClass]
        public class See
        {
            [TestMethod]
            public void RookTakesE5()
            {
                Position position = Position.FromFen("1k1r4/1pp4p/p7/4p3/8/P5P1/1PP4P/2K1R3 w - - 0 0");

                Assert.AreEqual(100, position.See(new Move(Position.E1, Position.E5, Position.PAWN)));
            }

            [TestMethod]
            public void KnightTakese5()
            {
                Position position = Position.FromFen("1k1r3q/1ppn3p/p4b2/4p3/8/P2N2P1/1PP1R1BP/2K1Q3 w - - 0 0");
                Assert.AreEqual(-225, position.See(new Move(Position.D3, Position.E5, Position.PAWN)));
            }
        
        }
    }
}