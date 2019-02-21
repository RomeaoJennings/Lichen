using System;
using System.Collections.Generic;
using System.Diagnostics;

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

        public int PlayerToMove { get; private set; }
        public int HalfMoveClock { get; private set; }
        public int FullMoveNumber { get; private set; }
        public Bitboard EnPassentBitboard { get; private set; }
        public CastleRights CastleRights { get; private set; }

        public int Opponent
        {
            get { return PlayerToMove == WHITE ? BLACK : WHITE; }
        }

        public Bitboard AllPiecesBitboard
        {
            get { return pieces[WHITE][ALL_PIECES] | pieces[BLACK][ALL_PIECES]; }
        }

        public bool InCheck
        {
            get
            {
                return AttackersTo(pieces[KING][PlayerToMove].BitScanForward(), Opponent) != 0;
            }
        }

        

        public Board()
        {
            NewGame();
        }

        private void NewGame()
        {
            PlayerToMove = WHITE;
            CastleRights = CastleRights.All;
            FullMoveNumber = 1;
            HalfMoveClock = 0;
            EnPassentBitboard = 0;

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

        public void GetCastleMoves(List<Move> list)
        {
            if (PlayerToMove == WHITE)
            {
                if (CastleRights.WhiteKing &&
                    (AllPiecesBitboard & 0x6UL) == 0 &&
                    AttackersTo(E1, BLACK) == 0 &&
                    AttackersTo(F1, BLACK) == 0 &&
                    AttackersTo(G1, BLACK) == 0)
                {
                    list.Add(new Move(E1, G1, false, true, KING));
                }
                if (CastleRights.WhiteQueen &&
                    (AllPiecesBitboard & 0x70UL) == 0 &&
                    AttackersTo(E1, BLACK) == 0 &&
                    AttackersTo(D1, BLACK) == 0 &&
                    AttackersTo(C1, BLACK) == 0)
                {
                    list.Add(new Move(E1, C1, false, true, QUEEN));
                }
            }
            else
            {
                if (CastleRights.BlackKing &&
                    (AllPiecesBitboard & 0x600000000000000UL) == 0 &&
                    AttackersTo(E8, WHITE) == 0 &&
                    AttackersTo(F8, WHITE) == 0 &&
                    AttackersTo(G8, WHITE) == 0)
                {
                    list.Add(new Move(E8, G8, false, true, KING));
                }
                if (CastleRights.BlackQueen &&
                    (AllPiecesBitboard & 0x7000000000000000UL) == 0 &&
                    AttackersTo(E8, WHITE) == 0 &&
                    AttackersTo(D8, WHITE) == 0 &&
                    AttackersTo(C8, WHITE) == 0)
                {
                    list.Add(new Move(E8, C8, false, true, QUEEN));
                }
            }
        }


        public void DoMove(Move move)
        {
            int opponent = Opponent;
            int originSquare = move.OriginSquare();
            int destinationSquare = move.DestinationSquare();
            int promotionType = move.PromotionType();
            int capturePiece = move.CapturePiece();

            // Reset En Passent Bitboard, If Currently Set
            if (EnPassentBitboard != 0)
            {
                EnPassentBitboard = 0;
            }

            if (move.IsCastle())
            {
                CastleInfo castleInfo = Bitboards.CastleInfo[PlayerToMove][move.CastleDirection()];
                pieces[PlayerToMove][KING] ^= castleInfo.KingBitboard;
                pieces[PlayerToMove][ROOK] ^= castleInfo.RookBitboard;
                pieces[PlayerToMove][ALL_PIECES] ^= castleInfo.KingBitboard ^ castleInfo.RookBitboard;

                squares[castleInfo.KingOrigin] = EMPTY;
                squares[castleInfo.RookOrigin] = EMPTY;
                squares[castleInfo.KingDestination] = KING;
                squares[castleInfo.RookDestination] = ROOK;
            }
            else // Handle non-castle moves, including promotions and en passent
            {
                int movedPiece = squares[originSquare];
                Bitboard originSquareBitboard = Bitboards.SquareBitboards[originSquare];
                Bitboard destinationSquareBitboard = Bitboards.SquareBitboards[destinationSquare];

                // Update Origin Square
                squares[originSquare] = EMPTY;
                pieces[PlayerToMove][movedPiece] ^= originSquareBitboard;
                pieces[PlayerToMove][ALL_PIECES] ^= originSquareBitboard;

                // Update Destination Square
                squares[destinationSquare] = movedPiece;
                pieces[PlayerToMove][movedPiece] ^= destinationSquareBitboard;
                pieces[PlayerToMove][ALL_PIECES] ^= destinationSquareBitboard;

                // Handle En Passent and Promotion
                if (squares[destinationSquare] == PAWN)
                {
                    // Remove opponent's pawn when move is en passent
                    if (move.IsEnPassent())
                    {
                        int enPassentSquare = destinationSquare + Bitboards.EnPassentOffset[PlayerToMove];
                        Bitboard enPassentTargetBitboard =
                            Bitboards.SquareBitboards[enPassentSquare];
                        pieces[opponent][PAWN] ^= enPassentTargetBitboard;
                        pieces[opponent][ALL_PIECES] ^= enPassentTargetBitboard;
                        squares[enPassentSquare] = EMPTY;
                    }
                    // Set En Passent Square if double pawn push
                    else if (destinationSquare - originSquare == 16) // White double push
                    {
                        EnPassentBitboard = Bitboards.SquareBitboards[destinationSquare - 8];
                    }
                    else if (destinationSquare - originSquare == -16) // Black double push
                    {
                        EnPassentBitboard = Bitboards.SquareBitboards[destinationSquare + 8];
                    }

                    // Swap promotion piece for pawn when promotion occurs
                    if (promotionType != EMPTY)
                    {
                        pieces[PlayerToMove][PAWN] ^= destinationSquareBitboard;
                        pieces[PlayerToMove][promotionType] ^= destinationSquareBitboard;
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
            PlayerToMove = opponent;
        }

        public void UndoMove(BoardState previousState)
        {
            Move move = previousState.Move;
            int originSquare = move.OriginSquare();
            int destinationSquare = move.DestinationSquare();
            int capturePiece = move.CapturePiece();
            int promotionType = move.PromotionType();
            int opponent = PlayerToMove;
            PlayerToMove = Opponent;
            EnPassentBitboard = previousState.EnPassentBitboard;
            CastleRights = previousState.CastleRights;

            if (move.IsCastle())
            {
                CastleInfo castleInfo = Bitboards.CastleInfo[PlayerToMove][move.CastleDirection()];
                pieces[PlayerToMove][KING] ^= castleInfo.KingBitboard;
                pieces[PlayerToMove][ROOK] ^= castleInfo.RookBitboard;
                pieces[PlayerToMove][ALL_PIECES] ^= castleInfo.KingBitboard ^ castleInfo.RookBitboard;

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
                pieces[PlayerToMove][movedPiece] ^= destinationSquareBitboard;
                pieces[PlayerToMove][ALL_PIECES] ^= destinationSquareBitboard;

                // Update Origin Square
                squares[originSquare] = movedPiece;
                pieces[PlayerToMove][movedPiece] ^= originSquareBitboard;
                pieces[PlayerToMove][ALL_PIECES] ^= originSquareBitboard;

                if (move.IsEnPassent())
                {
                    int enPassentSquare = destinationSquare + Bitboards.EnPassentOffset[PlayerToMove];
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
                    pieces[PlayerToMove][promotionType] ^= originSquareBitboard;
                    pieces[PlayerToMove][PAWN] ^= originSquareBitboard;
                    squares[originSquare] = PAWN;
                }
            }
        }

        public void UpdateCastleRights(ref int originSquare, ref int destinationSquare)
        {
            if (originSquare == E1)
            {
                if (CastleRights.WhiteKing)
                { /* TODO: Zobrist */ }
                if (CastleRights.WhiteQueen)
                { /* TODO: Zobrist */ }
                CastleRights = new CastleRights(
                    false,
                    false,
                    CastleRights.BlackKing,
                    CastleRights.BlackQueen);
            }
            else if (CastleRights.WhiteKing &&
                (originSquare == H1 || destinationSquare == H1))
            {
                CastleRights = new CastleRights(
                    false,
                    CastleRights.WhiteQueen,
                    CastleRights.BlackKing,
                    CastleRights.BlackQueen);
            }
            else if (CastleRights.WhiteQueen &&
                (originSquare == A1 || destinationSquare == A1))
            {
                CastleRights = new CastleRights(
                    CastleRights.WhiteKing,
                    false,
                    CastleRights.BlackKing,
                    CastleRights.BlackQueen);
            }


            else if (originSquare == E8)
            {
                CastleRights = new CastleRights(
                    CastleRights.WhiteKing,
                    CastleRights.WhiteQueen,
                    false,
                    false);
            }
            else if (CastleRights.BlackKing &&
                (originSquare == H8 || destinationSquare == H8))
            {
                CastleRights = new CastleRights(
                    CastleRights.WhiteKing,
                    CastleRights.WhiteQueen,
                    false,
                    CastleRights.BlackQueen);
            }
            else if (CastleRights.BlackQueen &&
                (originSquare == A8 || destinationSquare == A8))
            {
                CastleRights = new CastleRights(
                    CastleRights.WhiteKing,
                    CastleRights.WhiteQueen,
                    CastleRights.BlackKing,
                    false);
            }
        }
        

        public Bitboard GetPinnedPiecesBitboard()
        {
            int opponent = Opponent;
            int kingSquare = pieces[PlayerToMove][KING].BitScanForward();
            Bitboard result = 0;
            Bitboard allPiecesBitboard = AllPiecesBitboard;
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

        public bool IsLegalMove(Move move, Bitboard pinnedPiecesBitboard)
        {
            int kingSquare = pieces[PlayerToMove][KING].BitScanForward();
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
                Bitboard capSqBB = Bitboards.SquareBitboards[destinationSquare + Bitboards.EnPassentOffset[PlayerToMove]];
                Bitboard occupied = AllPiecesBitboard 
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
        
        public List<Move> GetAllMoves()
        {
            List<Move> result = new List<Move>(32);
            int kingSquare = pieces[PlayerToMove][KING].BitScanForward();
            Bitboard checkersBitboard = AttackersTo(kingSquare, Opponent);
            if (checkersBitboard != 0)
            {
                GetEvasionMoves(result, checkersBitboard);
            }
            else
            {
                Bitboard destinationBitboard = ~pieces[PlayerToMove][ALL_PIECES];
                GetMoves(MoveType.All, result, destinationBitboard);
            }
            return result;
        }

        public void GetMoves(MoveType moveType, List<Move> list, Bitboard destinationBitboard)
        {
            // No need to get King moves for evasions, because they are 
            // already generated before call to GetMoves
            if (moveType != MoveType.Evasions)
            {
                GetAllStepPieceMoves(list, KING, PlayerToMove, destinationBitboard);
                GetCastleMoves(list);
            }

            GetAllSlidingPieceMoves(list, QUEEN, PlayerToMove, destinationBitboard);
            GetAllSlidingPieceMoves(list, ROOK, PlayerToMove, destinationBitboard);
            GetAllSlidingPieceMoves(list, BISHOP, PlayerToMove, destinationBitboard);
            GetAllStepPieceMoves(list, KNIGHT, PlayerToMove, destinationBitboard);
            GetAllPawnPushMoves(list, PlayerToMove, destinationBitboard);  
            GetAllPawnCaptureMoves(list, PlayerToMove, destinationBitboard);
        }

        public void GetEvasionMoves(List<Move> list, Bitboard checkersBitboard)
        {
            bool notDoubleCheck = (checkersBitboard & (checkersBitboard - 1)) == 0;
            int kingSquare = pieces[PlayerToMove][KING].BitScanForward();
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
                & ((~pieces[PlayerToMove][ALL_PIECES] & ~slideAttacksBitboard) // Not own pieces or attacked
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

        public void GenerateMovesFromBitboard(
            List<Move> list,
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

        public void GetAllStepPieceMoves(
            List<Move> list,
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

        public void GetAllSlidingPieceMoves(
            List<Move> list,
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

        public void GetSlidingPieceMoves(
            List<Move> list,
            int pieceType,
            int square,
            Bitboard destinationBitboard)
        {
            Bitboard allPieces = AllPiecesBitboard;
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

        public void GetAllPawnCaptureMoves(
            List<Move> list,
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

            // Handle rare instance that enpassent capture removes check
            if (EnPassentBitboard != 0 &&
                Bitboards.SquareBitboards[EnPassentBitboard.BitScanForward() + Bitboards.EnPassentOffset[color]]
                == destinationBitboard)
            {
                destinationBitboard |= EnPassentBitboard;
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
            if (EnPassentBitboard != 0)
            {
                int enPassentSquare = EnPassentBitboard.BitScanForward();
                if ((leftAttacks & EnPassentBitboard) != 0)
                {
                    list.Add(new Move(enPassentSquare + leftOffset, enPassentSquare, true, false));
                }
                if ((rightAttacks & EnPassentBitboard) != 0)
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

        public void GetAllPawnPushMoves(
            List<Move> list,
            int color,
            Bitboard destinationBitboard)
        {
            Bitboard emptySquares = ~AllPiecesBitboard;
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

        public void GeneratePromotions(
            List<Move> list,
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

        public void GeneratePromotions(
            List<Move> list,
            int originSquare,
            int destinationSquare)
        {
            int capturedPiece = squares[destinationSquare];
            list.Add(new Move(originSquare, destinationSquare, capturedPiece, QUEEN));
            list.Add(new Move(originSquare, destinationSquare, capturedPiece, ROOK));
            list.Add(new Move(originSquare, destinationSquare, capturedPiece, BISHOP));
            list.Add(new Move(originSquare, destinationSquare, capturedPiece, KNIGHT));
        }

        public void GeneratePawnMovesFromBitboard(
            List<Move> list,
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
        public Bitboard AttackersTo(int square, int color)
        {
            int opponent = color == WHITE ? BLACK : WHITE;
            Bitboard occupancyBitboard = AllPiecesBitboard;

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

                result.PlayerToMove = elements[1] == "w" ? WHITE : BLACK;
                result.CastleRights = CastleRights.FromFEN(elements[2]);
                result.EnPassentBitboard = 0;
                if (elements[3] != "-")
                {
                    result.EnPassentBitboard = Bitboards.SquareBitboards[Bitboards.GetSquareFromName(elements[3])];
                }
                result.HalfMoveClock = int.Parse(elements[4]);
                result.FullMoveNumber = int.Parse(elements[5]);
                return result;
            }
            catch (Exception e)
            {
                throw new ArgumentException($"Invalid FEN string: {fen}", e);
            }
        }
    }
}
