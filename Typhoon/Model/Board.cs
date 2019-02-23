﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace Typhoon.Model
{
    using Bitboard = UInt64;

    public class Board
    {
        public const int WHITE = 0;
        public const int BLACK = 1;
        public const int NUM_SQUARES = 64;

        public const int KING = 0;
        public const int QUEEN = 1;
        public const int ROOK = 2;
        public const int BISHOP = 3;
        public const int KNIGHT = 4;
        public const int PAWN = 5;
        public const int ALL_PIECES = 6;
        public const int EMPTY = 7;

        #region SquareNames

        public const int A1 = 7;
        public const int B1 = 6;
        public const int C1 = 5;
        public const int D1 = 4;
        public const int E1 = 3;
        public const int F1 = 2;
        public const int G1 = 1;
        public const int H1 = 0;

        public const int A2 = 15;
        public const int B2 = 14;
        public const int C2 = 13;
        public const int D2 = 12;
        public const int E2 = 11;
        public const int F2 = 10;
        public const int G2 = 9;
        public const int H2 = 8;

        public const int A3 = 23;
        public const int B3 = 22;
        public const int C3 = 21;
        public const int D3 = 20;
        public const int E3 = 19;
        public const int F3 = 18;
        public const int G3 = 17;
        public const int H3 = 16;

        public const int A4 = 31;
        public const int B4 = 30;
        public const int C4 = 29;
        public const int D4 = 28;
        public const int E4 = 27;
        public const int F4 = 26;
        public const int G4 = 25;
        public const int H4 = 24;

        public const int A5 = 39;
        public const int B5 = 38;
        public const int C5 = 37;
        public const int D5 = 36;
        public const int E5 = 35;
        public const int F5 = 34;
        public const int G5 = 33;
        public const int H5 = 32;

        public const int A6 = 47;
        public const int B6 = 46;
        public const int C6 = 45;
        public const int D6 = 44;
        public const int E6 = 43;
        public const int F6 = 42;
        public const int G6 = 41;
        public const int H6 = 40;

        public const int A7 = 55;
        public const int B7 = 54;
        public const int C7 = 53;
        public const int D7 = 52;
        public const int E7 = 51;
        public const int F7 = 50;
        public const int G7 = 49;
        public const int H7 = 48;

        public const int A8 = 63;
        public const int B8 = 62;
        public const int C8 = 61;
        public const int D8 = 60;
        public const int E8 = 59;
        public const int F8 = 58;
        public const int G8 = 57;
        public const int H8 = 56;

        #endregion


        private Bitboard[][] pieces;
        private int[] squares;
        private Bitboard enPassentBitboard;
        private Bitboard allPiecesBitboard;
        private int playerToMove;
        private int halfMoveClock;
        private int fullMoveNumber;
        private CastleRights castleRights;
        private ulong zobristHash;

        public int PlayerToMove { get { return playerToMove; } }
        public int HalfMoveClock { get { return halfMoveClock; } }
        public int FullMoveNumber { get { return fullMoveNumber; } }
        public Bitboard EnPassentBitboard { get { return enPassentBitboard; } }
        public CastleRights CastleRights { get { return castleRights; } }
        public ulong Zobrist {  get { return zobristHash; } }

        public int Opponent
        {
            get { return playerToMove == WHITE ? BLACK : WHITE; }
        }

        public bool InCheck
        {
            get
            {
                return AttackersTo(pieces[KING][playerToMove].BitScanForward(), Opponent) != 0;
            }
        }

        public Board()
        {
            NewGame();
        }

        public Board(Board copy)
        {
            this.pieces = new Bitboard[2][];
            pieces[0] = new Bitboard[7];
            pieces[1] = new Bitboard[7];
            Array.Copy(copy.pieces[0], pieces[0], 7);
            Array.Copy(copy.pieces[1], pieces[1], 7);
            squares = copy.GetPieceSquares();
            playerToMove = copy.playerToMove;
            castleRights = new CastleRights(copy.castleRights);
            halfMoveClock = copy.halfMoveClock;
            fullMoveNumber = copy.fullMoveNumber;
            enPassentBitboard = copy.enPassentBitboard;
            zobristHash = copy.zobristHash;
        }

        private void NewGame()
        {
            playerToMove = WHITE;
            castleRights = CastleRights.All;
            fullMoveNumber = 1;
            halfMoveClock = 0;
            enPassentBitboard = 0;
            zobristHash = ZobristHash.NewGameHash;

            pieces = new Bitboard[2][];
            pieces[WHITE] = new Bitboard[7];
            pieces[BLACK] = new Bitboard[7];

            pieces[WHITE][PAWN] = 0xFF00UL;
            pieces[WHITE][KNIGHT] = 0x42UL;
            pieces[WHITE][BISHOP] = 0x24UL;
            pieces[WHITE][ROOK] = 0x81UL;
            pieces[WHITE][QUEEN] = 0x10UL;
            pieces[WHITE][KING] = 0x8UL;
            pieces[WHITE][ALL_PIECES] = 0xFFFF;

            pieces[BLACK][PAWN] = 0xFF000000000000UL;
            pieces[BLACK][KNIGHT] = 0x4200000000000000UL;
            pieces[BLACK][BISHOP] = 0x2400000000000000UL;
            pieces[BLACK][ROOK] = 0x8100000000000000UL;
            pieces[BLACK][QUEEN] = 0x1000000000000000UL;
            pieces[BLACK][KING] = 0x800000000000000UL;
            pieces[BLACK][ALL_PIECES] = 0xFFFF000000000000;
            allPiecesBitboard = pieces[WHITE][ALL_PIECES] | pieces[BLACK][ALL_PIECES];

            InitPieceSquares();
        }

        private void InitPieceSquares()
        {
            squares = new int[NUM_SQUARES];
            for (int i = 0; i < NUM_SQUARES; i++)
            {
                squares[i] = EMPTY;
            }

            for (int side = 0; side < 2; side++)
            {
                for (int piece = 0; piece < 6; piece++)
                {
                    Bitboard curr = pieces[side][piece];
                    while (curr != 0)
                    {
                        squares[curr.BitScanForward()] = piece;
                        Bitboards.PopLsb(ref curr);
                    }
                }
            }
        }

        public Bitboard GetPieceBitboard(int color, int pieceType)
        {
            return pieces[color][pieceType];
        }

        public int[] GetPieceSquares()
        {
            int[] result = new int[64];
            Array.Copy(squares, result, NUM_SQUARES);
            return result;
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void DoMove(Move move)
        {
            int opponent = Opponent;
            int originSquare = move.OriginSquare();
            int destinationSquare = move.DestinationSquare();
            int promotionType = move.PromotionType();
            int capturePiece = move.CapturePiece();

            // Reset En Passent Bitboard, If Currently Set
            if (enPassentBitboard != 0)
            {
                zobristHash ^= ZobristHash.EnPassentSquareHashes[enPassentBitboard.BitScanForward()];
                enPassentBitboard = 0;
            }

            if (move.IsCastle())
            {
                CastleInfo castleInfo = Bitboards.CastleInfo[playerToMove][move.CastleDirection()];
                pieces[playerToMove][KING] ^= castleInfo.KingBitboard;
                pieces[playerToMove][ROOK] ^= castleInfo.RookBitboard;
                pieces[playerToMove][ALL_PIECES] ^= castleInfo.KingBitboard ^ castleInfo.RookBitboard;

                squares[castleInfo.KingOrigin] = EMPTY;
                squares[castleInfo.RookOrigin] = EMPTY;
                squares[castleInfo.KingDestination] = KING;
                squares[castleInfo.RookDestination] = ROOK;

                zobristHash ^= castleInfo.Zobrist;
            }
            else // Handle non-castle moves, including promotions and en passent
            {
                int movedPiece = squares[originSquare];
                Bitboard originSquareBitboard = Bitboards.SquareBitboards[originSquare];
                Bitboard destinationSquareBitboard = Bitboards.SquareBitboards[destinationSquare];

                // Update Origin Square
                squares[originSquare] = EMPTY;
                pieces[playerToMove][movedPiece] ^= originSquareBitboard;
                pieces[playerToMove][ALL_PIECES] ^= originSquareBitboard;
                

                // Update Destination Square
                squares[destinationSquare] = movedPiece;
                pieces[playerToMove][movedPiece] ^= destinationSquareBitboard;
                pieces[playerToMove][ALL_PIECES] ^= destinationSquareBitboard;

                zobristHash ^= ZobristHash.PieceHashes[playerToMove][movedPiece][destinationSquare] ^
                    ZobristHash.PieceHashes[playerToMove][movedPiece][originSquare];

                // Handle En Passent and Promotion
                if (squares[destinationSquare] == PAWN)
                {
                    // Remove opponent's pawn when move is en passent
                    if (move.IsEnPassent())
                    {
                        int enPassentSquare = destinationSquare + Bitboards.EnPassentOffset[playerToMove];
                        Bitboard enPassentTargetBitboard =
                            Bitboards.SquareBitboards[enPassentSquare];
                        pieces[opponent][PAWN] ^= enPassentTargetBitboard;
                        pieces[opponent][ALL_PIECES] ^= enPassentTargetBitboard;
                        squares[enPassentSquare] = EMPTY;
                        zobristHash ^= ZobristHash.PieceHashes[opponent][PAWN][enPassentSquare];
                    }
                    // Set En Passent Square if double pawn push
                    else if (destinationSquare - originSquare == 16) // White double push
                    {
                        int targetSquare = destinationSquare - 8;
                        zobristHash ^= ZobristHash.EnPassentSquareHashes[targetSquare];
                        enPassentBitboard = Bitboards.SquareBitboards[targetSquare];
                    }
                    else if (destinationSquare - originSquare == -16) // Black double push
                    {
                        int targetSquare = destinationSquare + 8;
                        zobristHash ^= ZobristHash.EnPassentSquareHashes[targetSquare];
                        enPassentBitboard = Bitboards.SquareBitboards[targetSquare];
                    }

                    // Swap promotion piece for pawn when promotion occurs
                    if (promotionType != EMPTY)
                    {
                        pieces[playerToMove][PAWN] ^= destinationSquareBitboard;
                        pieces[playerToMove][promotionType] ^= destinationSquareBitboard;
                        squares[destinationSquare] = promotionType;
                    }
                }

                // Update opponent's bitboards if piece was captured (note: EP moves do not list a capture piece)
                if (capturePiece != EMPTY)
                {
                    pieces[opponent][capturePiece] ^= destinationSquareBitboard;
                    pieces[opponent][ALL_PIECES] ^= destinationSquareBitboard;
                }

                
            }
            UpdateCastleRights(ref originSquare, ref destinationSquare);
            playerToMove = opponent;
            allPiecesBitboard = pieces[WHITE][ALL_PIECES] | pieces[BLACK][ALL_PIECES];
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void UndoMove(BoardState previousState)
        {
            Move move = previousState.Move;
            int originSquare = move.OriginSquare();
            int destinationSquare = move.DestinationSquare();
            int capturePiece = move.CapturePiece();
            int promotionType = move.PromotionType();
            int opponent = playerToMove;
            playerToMove = Opponent;
            enPassentBitboard = previousState.EnPassentBitboard;
            castleRights = previousState.CastleRights;

            if (move.IsCastle())
            {
                CastleInfo castleInfo = Bitboards.CastleInfo[playerToMove][move.CastleDirection()];
                pieces[playerToMove][KING] ^= castleInfo.KingBitboard;
                pieces[playerToMove][ROOK] ^= castleInfo.RookBitboard;
                pieces[playerToMove][ALL_PIECES] ^= castleInfo.KingBitboard ^ castleInfo.RookBitboard;

                squares[castleInfo.KingDestination] = EMPTY;
                squares[castleInfo.RookDestination] = EMPTY;
                squares[castleInfo.KingOrigin] = KING;
                squares[castleInfo.RookOrigin] = ROOK;
            }
            else
            {
                int movedPiece = squares[destinationSquare];
     
                Bitboard destinationSquareBitboard = Bitboards.SquareBitboards[destinationSquare];
                Bitboard originSquareBitboard = Bitboards.SquareBitboards[originSquare];

                // Update Destination Square
                squares[destinationSquare] = EMPTY;
                pieces[playerToMove][movedPiece] ^= destinationSquareBitboard;
                pieces[playerToMove][ALL_PIECES] ^= destinationSquareBitboard;

                // Update Origin Square
                squares[originSquare] = movedPiece;
                pieces[playerToMove][movedPiece] ^= originSquareBitboard;
                pieces[playerToMove][ALL_PIECES] ^= originSquareBitboard;

                if (move.IsEnPassent())
                {
                    int enPassentSquare = destinationSquare + Bitboards.EnPassentOffset[playerToMove];
                    Bitboard enPassentTargetBitboard =
                        Bitboards.SquareBitboards[enPassentSquare];
                    pieces[opponent][PAWN] ^= enPassentTargetBitboard;
                    pieces[opponent][ALL_PIECES] ^= enPassentTargetBitboard;
                    squares[enPassentSquare] = PAWN;
                }
                // Update opponent's bitboards if piece was captured
                else if (capturePiece != EMPTY)
                {
                    pieces[opponent][capturePiece] ^= destinationSquareBitboard;
                    pieces[opponent][ALL_PIECES] ^= destinationSquareBitboard;
                    squares[destinationSquare] = capturePiece;
                }
                // Swap promotion piece for pawn when promotion occurs
                if (promotionType != EMPTY)
                {                    
                    pieces[playerToMove][promotionType] ^= originSquareBitboard;
                    pieces[playerToMove][PAWN] ^= originSquareBitboard;
                    squares[originSquare] = PAWN;
                }
            }
            allPiecesBitboard = pieces[WHITE][ALL_PIECES] | pieces[BLACK][ALL_PIECES];
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void GetCastleMoves(MoveList list)
        {
            if (playerToMove == WHITE)
            {
                if (castleRights.WhiteKing &&
                    (allPiecesBitboard & 0x6UL) == 0 &&
                    AttackersTo(E1, BLACK) == 0 &&
                    AttackersTo(F1, BLACK) == 0 &&
                    AttackersTo(G1, BLACK) == 0)
                {
                    list.Add(new Move(E1, G1, false, true, KING));
                }
                if (castleRights.WhiteQueen &&
                    (allPiecesBitboard & 0x70UL) == 0 &&
                    AttackersTo(E1, BLACK) == 0 &&
                    AttackersTo(D1, BLACK) == 0 &&
                    AttackersTo(C1, BLACK) == 0)
                {
                    list.Add(new Move(E1, C1, false, true, QUEEN));
                }
            }
            else
            {
                if (castleRights.BlackKing &&
                    (allPiecesBitboard & 0x600000000000000UL) == 0 &&
                    AttackersTo(E8, WHITE) == 0 &&
                    AttackersTo(F8, WHITE) == 0 &&
                    AttackersTo(G8, WHITE) == 0)
                {
                    list.Add(new Move(E8, G8, false, true, KING));
                }
                if (castleRights.BlackQueen &&
                    (allPiecesBitboard & 0x7000000000000000UL) == 0 &&
                    AttackersTo(E8, WHITE) == 0 &&
                    AttackersTo(D8, WHITE) == 0 &&
                    AttackersTo(C8, WHITE) == 0)
                {
                    list.Add(new Move(E8, C8, false, true, QUEEN));
                }
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void UpdateCastleRights(ref int originSquare, ref int destinationSquare)
        {
            if (originSquare == E1)
            {
                if (castleRights.WhiteKing)
                { /* TODO: Zobrist */ }
                if (castleRights.WhiteQueen)
                { /* TODO: Zobrist */ }
                castleRights = new CastleRights(
                    false,
                    false,
                    castleRights.BlackKing,
                    castleRights.BlackQueen);
            }
            else if (castleRights.WhiteKing &&
                (originSquare == H1 || destinationSquare == H1))
            {
                castleRights = new CastleRights(
                    false,
                    castleRights.WhiteQueen,
                    castleRights.BlackKing,
                    castleRights.BlackQueen);
            }
            else if (castleRights.WhiteQueen &&
                (originSquare == A1 || destinationSquare == A1))
            {
                castleRights = new CastleRights(
                    castleRights.WhiteKing,
                    false,
                    castleRights.BlackKing,
                    castleRights.BlackQueen);
            }
            else if (originSquare == E8)
            {
                castleRights = new CastleRights(
                    castleRights.WhiteKing,
                    castleRights.WhiteQueen,
                    false,
                    false);
            }
            else if (castleRights.BlackKing &&
                (originSquare == H8 || destinationSquare == H8))
            {
                castleRights = new CastleRights(
                    castleRights.WhiteKing,
                    castleRights.WhiteQueen,
                    false,
                    castleRights.BlackQueen);
            }
            else if (castleRights.BlackQueen &&
                (originSquare == A8 || destinationSquare == A8))
            {
                castleRights = new CastleRights(
                    castleRights.WhiteKing,
                    castleRights.WhiteQueen,
                    castleRights.BlackKing,
                    false);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Bitboard GetPinnedPiecesBitboard()
        {
            int opponent = Opponent;
            int kingSquare = pieces[playerToMove][KING].BitScanForward();
            Bitboard result = 0;
            Bitboard sliders = (pieces[opponent][QUEEN] | pieces[opponent][ROOK]) &
                (Bitboards.RowBitboards[kingSquare] | Bitboards.ColumnBitboards[kingSquare]);
            sliders |= (pieces[opponent][QUEEN] | pieces[opponent][BISHOP]) &
                (Bitboards.DiagonalBitboards[Bitboards.FORWARD,kingSquare] | 
                Bitboards.DiagonalBitboards[Bitboards.BACKWARD,kingSquare]);
            while (sliders != 0)
            {
                int sliderSquare = sliders.BitScanForward();
                Bitboards.PopLsb(ref sliders);
                Bitboard betweenBitboard = Bitboards.BetweenBitboards[kingSquare, sliderSquare] & allPiecesBitboard;
                
                // If exactly one piece between slider and king, it is pinned.
                if (betweenBitboard != 0)
                {
                    int betweenSquare = betweenBitboard.BitScanForward();
                    Bitboards.PopLsb(ref betweenBitboard);
                    if (betweenBitboard == 0)
                    {
                        result |= Bitboards.SquareBitboards[betweenSquare];
                    }
                }
            }
            return result;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool IsLegalMove(Move move, Bitboard pinnedPiecesBitboard)
        {
            int kingSquare = pieces[playerToMove][KING].BitScanForward();
            int opponent = Opponent;
            int originSquare = move.OriginSquare();
            int destinationSquare = move.DestinationSquare();
            
            if (originSquare == kingSquare)
            {
                // If king move, it's legal if it is a castle move or if the destination is not attacked.
                return move.IsCastle() || AttackersTo(destinationSquare, opponent) == 0;
            }
            else if (move.IsEnPassent())
            {
                Bitboard capSqBB = Bitboards.SquareBitboards[destinationSquare + Bitboards.EnPassentOffset[playerToMove]];
                Bitboard occupied = allPiecesBitboard 
                    ^ capSqBB ^ Bitboards.SquareBitboards[originSquare] 
                    ^ Bitboards.SquareBitboards[destinationSquare];
                return (Bitboards.GetRookMoveBitboard(kingSquare, occupied) & 
                    (pieces[opponent][QUEEN] | pieces[opponent][ROOK])) == 0 &&
                       (Bitboards.GetBishopMoveBitboard(kingSquare, occupied) & 
                       (pieces[opponent][QUEEN] | pieces[opponent][BISHOP])) == 0;
            }
            else
            {
                // Other moves are legal if the piece is not pinned, 
                // or its moving along the ray between the king and the attacker.
                // We assume that if we get here, we are not currently in double check,
                // because GetEvasionMoves would only return king moves.
                return pinnedPiecesBitboard == 0 ||
                    (Bitboards.SquareBitboards[originSquare] & pinnedPiecesBitboard) == 0 ||
                    Bitboards.AreAligned(kingSquare, originSquare, destinationSquare);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public MoveList GetAllMoves()
        {
            MoveList result = new MoveList();
            int kingSquare = pieces[playerToMove][KING].BitScanForward();
            Bitboard checkersBitboard = AttackersTo(kingSquare, Opponent);
            if (checkersBitboard != 0)
            {
                GetEvasionMoves(result, checkersBitboard);
            }
            else
            {
                Bitboard destinationBitboard = ~pieces[playerToMove][ALL_PIECES];
                GetMoves(MoveType.All, result, destinationBitboard);
            }
            return result;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void GetMoves(MoveType moveType, MoveList list, Bitboard destinationBitboard)
        {
            // No need to get King moves for evasions, because they are 
            // already generated before call to GetMoves
            if (moveType != MoveType.Evasions)
            {
                GetAllStepPieceMoves(list, KING, playerToMove, destinationBitboard);
                GetCastleMoves(list);
            }

            GetAllSlidingPieceMoves(list, QUEEN, playerToMove, destinationBitboard);
            GetAllSlidingPieceMoves(list, ROOK, playerToMove, destinationBitboard);
            GetAllSlidingPieceMoves(list, BISHOP, playerToMove, destinationBitboard);
            GetAllStepPieceMoves(list, KNIGHT, playerToMove, destinationBitboard);
            GetAllPawnPushMoves(list, playerToMove, destinationBitboard);  
            GetAllPawnCaptureMoves(list, playerToMove, destinationBitboard);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void GetEvasionMoves(MoveList list, Bitboard checkersBitboard)
        {
            bool notDoubleCheck = (checkersBitboard & (checkersBitboard - 1)) == 0;
            int kingSquare = pieces[playerToMove][KING].BitScanForward();
            int opponent = Opponent;

            // Get all squares attacked by sliders and remove them as possible escape squares
            Bitboard sliders = checkersBitboard & ~(pieces[opponent][KNIGHT] | pieces[opponent][PAWN]);
            Bitboard slideAttacksBitboard = 0;
            while (sliders != 0)
            {
                int sliderSquare = sliders.BitScanForward();
                Bitboards.PopLsb(ref sliders);
                slideAttacksBitboard |= Bitboards.LineBitboards[kingSquare, sliderSquare];
            }
            Bitboard kingMovesBitboard = Bitboards.KingBitboards[kingSquare] 
                & ((~pieces[playerToMove][ALL_PIECES] & ~slideAttacksBitboard) // Not own pieces or attacked
                | checkersBitboard); // Include adjacent sliders
            GenerateMovesFromBitboard(list, kingMovesBitboard, kingSquare);

            //If double check then only king moves are possible.
            if (notDoubleCheck)
            {
                int attackerSquare = checkersBitboard.BitScanForward();
                Bitboard destinationBitboard = Bitboards.SquareBitboards[attackerSquare] 
                    | Bitboards.BetweenBitboards[attackerSquare, kingSquare];
                GetMoves(MoveType.Evasions, list, destinationBitboard);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void GenerateMovesFromBitboard(
            MoveList list,
            Bitboard attacks,
            int originSquare)
        {
            while (attacks != 0)
            {
                int destinationSquare = attacks.BitScanForward();
                Bitboards.PopLsb(ref attacks);
                list.Add(new Move(originSquare, destinationSquare, squares[destinationSquare]));
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void GetAllStepPieceMoves(
            MoveList list,
            int pieceType,
            int color,
            Bitboard destinationBitboard)
        {
            Debug.Assert(pieceType == KING || pieceType == KNIGHT);
            Debug.Assert(color == WHITE || color == BLACK);

            Bitboard[] attackBitboards = pieceType == KING ? Bitboards.KingBitboards : Bitboards.KnightBitboards;

            Bitboard piece = pieces[color][pieceType];
            while (piece != 0)
            {
                int square = piece.BitScanForward();
                Bitboards.PopLsb(ref piece);
                Bitboard attacksBitboard = attackBitboards[square] & destinationBitboard;
                GenerateMovesFromBitboard(list, attacksBitboard, square);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void GetAllSlidingPieceMoves(
            MoveList list,
            int pieceType,
            int color, Bitboard destinationBitboard)
        {
            Debug.Assert(pieceType >= QUEEN && pieceType <= BISHOP);

            Bitboard pieceBitboard = pieces[color][pieceType];
            while (pieceBitboard != 0)
            {
                int square = pieceBitboard.BitScanForward();
                Bitboards.PopLsb(ref pieceBitboard);
                GetSlidingPieceMoves(list, pieceType, square, destinationBitboard);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void GetSlidingPieceMoves(
            MoveList list,
            int pieceType,
            int square,
            Bitboard destinationBitboard)
        {
            Bitboard allPieces = allPiecesBitboard;
            Bitboard attacks = 0;
            if (pieceType == ROOK || pieceType == QUEEN)
            {
                attacks = Bitboards.GetRookMoveBitboard(square, allPieces);
            }
            if (pieceType == BISHOP || pieceType == QUEEN)
            {
                attacks |= Bitboards.GetBishopMoveBitboard(square, allPieces);
            }
            attacks &= destinationBitboard;
            GenerateMovesFromBitboard(list, attacks, square);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void GetAllPawnCaptureMoves(
            MoveList list,
            int color,
            Bitboard destinationBitboard)
        {
            Bitboard opponentPieces;
            Bitboard leftAttacks;
            Bitboard rightAttacks;
            Bitboard leftPromotionAttacks;
            Bitboard rightPromotionAttacks;
            Bitboard promotionMask;
            int leftOffset;
            int rightOffset;
            Bitboard pawns = pieces[color][PAWN];

            // Handle rare instance that enpassent capture removes check blocker
            if (enPassentBitboard != 0 &&
                Bitboards.SquareBitboards[enPassentBitboard.BitScanForward() + Bitboards.EnPassentOffset[color]]
                == destinationBitboard)
            {
                destinationBitboard |= enPassentBitboard;
            }
            if (color == WHITE)
            {
                opponentPieces = pieces[BLACK][ALL_PIECES];
                leftAttacks = (pawns & ~Bitboards.ColumnBitboards[A1]) << 9;
                rightAttacks = (pawns & ~Bitboards.ColumnBitboards[H1]) << 7;
                leftOffset = -9;
                rightOffset = -7;
                promotionMask = Bitboards.RowBitboards[A8];
            }
            else
            {
                opponentPieces = pieces[WHITE][ALL_PIECES];
                leftAttacks = (pawns & ~Bitboards.ColumnBitboards[A1]) >> 7;
                rightAttacks = (pawns & ~Bitboards.ColumnBitboards[H1]) >> 9;
                leftOffset = 7;
                rightOffset = 9;
                promotionMask = Bitboards.RowBitboards[A1];
            }

            leftAttacks &= destinationBitboard;
            rightAttacks &= destinationBitboard;

            // Handle enPassent with single branch -- we usually dont get inside.
            if (enPassentBitboard != 0)
            {
                int enPassentSquare = enPassentBitboard.BitScanForward();
                if ((leftAttacks & enPassentBitboard) != 0)
                {
                    list.Add(new Move(enPassentSquare + leftOffset, enPassentSquare, true, false));
                }
                if ((rightAttacks & enPassentBitboard) != 0)
                {
                    list.Add(new Move(enPassentSquare + rightOffset, enPassentSquare, true, false));
                }
            }
            leftAttacks &= opponentPieces;
            rightAttacks &= opponentPieces;

            // Separate promotion moves for separate processing
            leftPromotionAttacks = leftAttacks & promotionMask;
            rightPromotionAttacks = rightAttacks & promotionMask;

            // Flip bits of promotion mask to mask off promotion pawns in regular attacks bitboards.
            promotionMask = ~promotionMask;
            leftAttacks &= promotionMask;
            rightAttacks &= promotionMask;

            GeneratePawnMovesFromBitboard(list, leftAttacks, leftOffset);
            GeneratePawnMovesFromBitboard(list, rightAttacks, rightOffset);
            GeneratePromotions(list, leftPromotionAttacks, leftOffset);
            GeneratePromotions(list, rightPromotionAttacks, rightOffset);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void GetAllPawnPushMoves(
            MoveList list,
            int color,
            Bitboard destinationBitboard)
        {
            Bitboard emptySquares = ~allPiecesBitboard;
            Bitboard pawnSingleMoves = pieces[color][PAWN];

            Bitboard pawnDoubleMoves;
            int pawnSingleMoveOffset;
            int pawnDoubleMoveOffset;
            Bitboard promotionMask;

            if (color == WHITE)
            {
                pawnSingleMoves <<= 8;
                pawnSingleMoves &= emptySquares;
                pawnDoubleMoves = (pawnSingleMoves & Bitboards.RowBitboards[A3]) << 8;
                pawnSingleMoveOffset = -8;
                pawnDoubleMoveOffset = -16;
                promotionMask = 0xFF00000000000000UL;
            }
            else
            {
                pawnSingleMoves >>= 8;
                pawnSingleMoves &= emptySquares;
                pawnDoubleMoves = (pawnSingleMoves & Bitboards.RowBitboards[A6]) >> 8;
                pawnSingleMoveOffset = 8;
                pawnDoubleMoveOffset = 16;
                promotionMask = 0xFFUL;
            }

            pawnSingleMoves &= destinationBitboard;
            pawnDoubleMoves &= emptySquares & destinationBitboard;

            // Handle Double Moves
            GeneratePawnMovesFromBitboard(list, pawnDoubleMoves, pawnDoubleMoveOffset);

            // Handle Non-Promotion Single Moves
            Bitboard nonPromotionPawns = ~promotionMask & pawnSingleMoves;
            GeneratePawnMovesFromBitboard(list, nonPromotionPawns, pawnSingleMoveOffset);

            // Handle Promotions
            Bitboard promotionPawns = promotionMask & pawnSingleMoves;
            GeneratePromotions(list, promotionPawns, pawnSingleMoveOffset);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void GeneratePromotions(
            MoveList list,
            Bitboard destinationBitboard,
            int originOffset)
        {
            while (destinationBitboard != 0)
            {
                int destinationSquare = destinationBitboard.BitScanForward();
                Bitboards.PopLsb(ref destinationBitboard);
                GeneratePromotions(list, destinationSquare + originOffset, destinationSquare);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void GeneratePromotions(
            MoveList list,
            int originSquare,
            int destinationSquare)
        {
            int capturedPiece = squares[destinationSquare];
            list.Add(new Move(originSquare, destinationSquare, capturedPiece, QUEEN));
            list.Add(new Move(originSquare, destinationSquare, capturedPiece, ROOK));
            list.Add(new Move(originSquare, destinationSquare, capturedPiece, BISHOP));
            list.Add(new Move(originSquare, destinationSquare, capturedPiece, KNIGHT));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void GeneratePawnMovesFromBitboard(
            MoveList list,
            Bitboard destinationsBitboard,
            int originOffset)
        {
            while (destinationsBitboard != 0)
            {
                int destinationSquare = destinationsBitboard.BitScanForward();
                Bitboards.PopLsb(ref destinationsBitboard);

                list.Add(new Move(destinationSquare + originOffset, destinationSquare, squares[destinationSquare]));
            }
        }

        // Returns all pieces of the passed color that attack the relevent square.
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Bitboard AttackersTo(int square, int color)
        {
            int opponent = color == WHITE ? BLACK : WHITE;
            Bitboard occupancyBitboard = allPiecesBitboard;

            Bitboard queenBishop = pieces[color][QUEEN];
            Bitboard queenRook = queenBishop;
            queenBishop |= pieces[color][BISHOP];
            queenRook |= pieces[color][ROOK];

            Bitboard result = (Bitboards.KingBitboards[square] & pieces[color][KING]);
            result |= (Bitboards.GetRookMoveBitboard(square, occupancyBitboard) & queenRook);
            result |= (Bitboards.GetBishopMoveBitboard(square, occupancyBitboard) & queenBishop);
            result |= Bitboards.KnightBitboards[square] & pieces[color][KNIGHT];
            result |= (Bitboards.PawnBitboards[opponent, square] & pieces[color][PAWN]);
            return result;
        }
        

        public static Board FromFEN(string fen)
        {
            Board result = new Board
            {
                pieces = new Bitboard[2][]
            };
            result.pieces[WHITE] = new Bitboard[7];
            result.pieces[BLACK] = new Bitboard[7];


            const string whitePieces = "KQRBNP";
            const string blackPieces = "kqrbnp";
            try
            {
                string[] elements = fen.Split(' ');
                string sq = elements[0].Replace("/", string.Empty);
                int square = 63;
                // Process squares
                foreach (char curr in sq)
                {
                    // Blank Squares
                    if (curr >= '0' && curr <= '9')
                    {
                        square -= int.Parse(curr.ToString());
                        continue;
                    }
                    // Black Pieces
                    else if (curr >= 'b' && curr <= 'r')
                    {
                        Bitboard toAdd = Bitboards.SquareBitboards[square];
                        result.pieces[BLACK][blackPieces.IndexOf(curr)] |= toAdd;
                        result.pieces[BLACK][ALL_PIECES] |= toAdd;
                    }
                    // White Pieces
                    else
                    {
                        Bitboard toAdd = Bitboards.SquareBitboards[square];
                        result.pieces[WHITE][whitePieces.IndexOf(curr)] |= toAdd;
                        result.pieces[WHITE][ALL_PIECES] |= toAdd;
                    }
                    square--;
                }
                result.InitPieceSquares();

                result.playerToMove = elements[1] == "w" ? WHITE : BLACK;
                result.castleRights = CastleRights.FromFEN(elements[2]);
                result.enPassentBitboard = 0;
                if (elements[3] != "-")
                {
                    result.enPassentBitboard = Bitboards.SquareBitboards[Bitboards.GetSquareFromName(elements[3])];
                }
                result.halfMoveClock = int.Parse(elements[4]);
                result.fullMoveNumber = int.Parse(elements[5]);
                return result;
            }
            catch (Exception e)
            {
                throw new ArgumentException($"Invalid FEN string: {fen}", e);
            }
        }
    }
}
