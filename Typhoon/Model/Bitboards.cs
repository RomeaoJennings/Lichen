using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Typhoon.Model
{
    using Bitboard = UInt64;

    public static class Bitboards
    {
        public const int WHITE = 0;
        public const int BLACK = 1;

        public const int NUM_SQUARES = 64;

        public static byte[,] SquareDistance = new byte[NUM_SQUARES, NUM_SQUARES];

        public static readonly Bitboard[] SquareBitboards = new Bitboard[NUM_SQUARES];

        public static readonly Bitboard[] RowBitboards = new Bitboard[NUM_SQUARES];
        public static readonly Bitboard[] ColumnBitboards = new Bitboard[NUM_SQUARES];
        public static readonly Bitboard[] FrontDiagonalBitboards = new Bitboard[NUM_SQUARES];
        public static readonly Bitboard[] BackDiagonalBitboards = new Bitboard[NUM_SQUARES];

        public static readonly Bitboard[] KingBitboards = new Bitboard[NUM_SQUARES];
        public static readonly Bitboard[] KnightBitboards = new Bitboard[NUM_SQUARES];
        public static readonly Bitboard[,] PawnMoveBitboards = new Bitboard[2, NUM_SQUARES];
        public static readonly Bitboard[,] PawnAttkBitboards = new Bitboard[2, NUM_SQUARES];

        private const Bitboard DeBruijnSequence = 0x37E84A99DAE458F;
        private static readonly int[] MultiplyDeBruijnBitPosition =
        {
            0,  1, 17,  2, 18, 50,  3, 57,
            47, 19, 22, 51, 29,  4, 33, 58,
            15, 48, 20, 27, 25, 23, 52, 41,
            54, 30, 38,  5, 43, 34, 59,  8,
            63, 16, 49, 56, 46, 21, 28, 32,
            14, 26, 24, 40, 53, 37, 42,  7,
            62, 55, 45, 31, 13, 39, 36,  6,
            61, 44, 12, 35, 60, 11, 10,  9,
        };

        public static readonly Bitboard[] RookMagics =
        {
            0x4080019028834000UL, 0x2100144000210184UL, 0x1200120060809840UL, 0x0300081000600700UL,
            0x0200200600104408UL, 0x1300240003002218UL, 0x020008120002812CUL, 0x4180052140801900UL,
            0x3212002102008440UL, 0x2223802000400684UL, 0x4012004130620084UL, 0x20C4801001D80080UL,
            0x102B808034001800UL, 0x01020010495A0014UL, 0x04EF0041001C2200UL, 0x4C1200050882135CUL,
            0x0296818000204006UL, 0x0023850021084000UL, 0x0010450020001101UL, 0x0100090021003000UL,
            0x025931000801000CUL, 0x024E00804C000280UL, 0x4C078C0008102126UL, 0x0811620003006484UL,
            0x00A0800300204105UL, 0x0862030200406480UL, 0x0A1020030040B101UL, 0x1100D00100208900UL,
            0x02050011000C8800UL, 0x081A006200291004UL, 0x0241010400903A08UL, 0x3010DC06000480D1UL,
            0x329566400280008CUL, 0x4CD0002018400140UL, 0x0100801000802005UL, 0x4400201001000B02UL,
            0x62420008060010A0UL, 0x0402005012000804UL, 0x0A6010480C00092AUL, 0x0020928402000147UL,
            0x27A480C010288000UL, 0x0219A0005002C007UL, 0x0C10022000808010UL, 0x0E11224201120008UL,
            0x648A003460120018UL, 0x490200280C820011UL, 0x31C2220811040030UL, 0x0020048154020003UL,
            0x1004420B28810200UL, 0x220082004F511600UL, 0x0531006008C07500UL, 0x3A5270C00A02A200UL,
            0x0B01800400480080UL, 0x2A0400854D001900UL, 0x4461081026414400UL, 0x024D842110C18600UL,
            0x4052804222003102UL, 0x503627C003011281UL, 0x4001542000084101UL, 0x484BD81000200D01UL,
            0x181A0010780C6006UL, 0x2802000550840802UL, 0x2182821819300084UL, 0x2654038D06240142UL
        };

        public static readonly int[] RookShifts =
        {
            52, 53, 53, 53, 53, 53, 53, 52,
            54, 54, 54, 54, 54, 54, 54, 53,
            54, 54, 54, 54, 54, 54, 54, 53,
            54, 54, 54, 54, 54, 54, 54, 53,
            54, 54, 54, 54, 54, 54, 54, 53,
            54, 54, 54, 54, 54, 54, 54, 53,
            54, 54, 54, 54, 54, 54, 54, 53,
            52, 53, 53, 53, 53, 53, 53, 52
        };

        public static readonly Bitboard[] RookPremasks;
        public static readonly Bitboard[][] RookMoves;

        public static readonly Bitboard[] BishopMagics =
        {
            0x081408081802A0C0UL, 0x10100A0244006500UL, 0x48681D810210FD04UL, 0x48681D810210FD04UL,
            0x0184042000C80401UL, 0x4220882018800382UL, 0x1920842C76402100UL, 0x5A220142021052C9UL,
            0x4810400222044108UL, 0x00D89030013100A1UL, 0x0001502C04C14416UL, 0x6888680951012005UL,
            0x0148040C20110A20UL, 0x12188A2820183434UL, 0x65058C040C1E9805UL, 0x2000221041045020UL,
            0x1084B0C11024110BUL, 0x31440218501C2042UL, 0x00D4098818C400A8UL, 0x10EB013024008004UL,
            0x0008806408A009A1UL, 0x4024800110101110UL, 0x34460144050CA20CUL, 0x407A2B5247080800UL,
            0x0F06B0A620200204UL, 0x01280841204B0545UL, 0x4128040148044090UL, 0x1084004004030002UL,
            0x401100400400C044UL, 0x5875020145028080UL, 0x14D900C0060210CAUL, 0x14D900C0060210CAUL,
            0x70102A0A4A107028UL, 0x000884103C60A253UL, 0x4602010A40500280UL, 0x0386010040A401C0UL,
            0x1024D40400A4C100UL, 0x2921044200010110UL, 0x00A4050C4A04240CUL, 0x2C09420A02448040UL,
            0x082410080440E848UL, 0x006444100C420928UL, 0x0083012110001302UL, 0x000432203100C800UL,
            0x104A102030402A01UL, 0x05812D1103030200UL, 0x1318886810C01084UL, 0x080D453200860200UL,
            0x4380881C10A4C000UL, 0x1A06004308088408UL, 0x3600020113880000UL, 0x4444948360881085UL,
            0x10A400610C240383UL, 0x044CC06901010265UL, 0x29400A6802019240UL, 0x00304A7801488510UL,
            0x07A1808848264000UL, 0x48A2F52201242081UL, 0x100C0202024A4800UL, 0x5A04682802208802UL,
            0x0000700410020210UL, 0x0013412034902988UL, 0x4C1920887D471402UL, 0x016E024408120140UL,
        };

        public static readonly int[] BishopShifts =
        {
            58, 59, 59, 59, 59, 59, 59, 58,
            59, 59, 59, 59, 59, 59, 59, 59,
            59, 59, 57, 57, 57, 57, 59, 59,
            59, 59, 57, 55, 55, 57, 59, 59,
            59, 59, 57, 55, 55, 57, 59, 59,
            59, 59, 57, 57, 57, 57, 59, 59,
            59, 59, 59, 59, 59, 59, 59, 59,
            58, 59, 59, 59, 59, 59, 59, 58
        };

        public static readonly Bitboard[] BishopPremasks;
        public static readonly Bitboard[][] BishopMoves;

        public static Bitboard GetRookMoveBitboard(int square, Bitboard occupancyBitboard)
        {
            Debug.Assert(square >= 0 && square < 64);
            var index = ((occupancyBitboard & RookPremasks[square]) * RookMagics[square]) >> RookShifts[square];
            return RookMoves[square][index];
        }

        public static Bitboard GetBishopMoveBitboard(int square, Bitboard occupancyBitboard)
        {
            Debug.Assert(square >= 0 && square < 64);
            var index = ((occupancyBitboard & BishopPremasks[square]) * BishopMagics[square]) >> BishopShifts[square];
            return BishopMoves[square][index];
        }

        static Bitboards()
        {
            InitSquareDistances();
            InitSquareBitboards();
            InitRowBitboards();
            InitColumnBitboards();
            InitDiagonalBitboards();

            InitKingBitboards();
            InitKnightBitboards();
            InitPawnBitboards();

            BishopPremasks = MagicBitboardFactory.GenerateBishopOccupancyBitboards();
            BishopMoves = MagicBitboardFactory.InitMagics(
                BishopPremasks,
                BishopMagics,
                BishopShifts,
                MagicBitboardFactory.BishopOffsets
            );

            RookPremasks = MagicBitboardFactory.GenerateRookOccupancyBitboards();
            RookMoves = MagicBitboardFactory.InitMagics(
                RookPremasks,
                RookMagics,
                RookShifts,
                MagicBitboardFactory.RookOffsets
            );

        }

        private static void InitSquareDistances()
        {
            for (int sq1 = 0; sq1 < NUM_SQUARES; sq1++)
            {
                for (int sq2 = 0; sq2 <= sq1; sq2++)
                {
                    int rowDist = Math.Abs(GetRow(sq1) - GetRow(sq2));
                    int colDist = Math.Abs(GetColumn(sq1) - GetColumn(sq2));
                    byte dist = (byte)Math.Max(colDist, rowDist);
                    SquareDistance[sq1, sq2] = dist;
                    SquareDistance[sq2, sq1] = dist;
                }
            }
        }

        private static void InitSquareBitboards()
        {
            for (int square = 0; square < NUM_SQUARES; square++)
            {
                SquareBitboards[square] = 1UL << square;
            }
        }

        private static void InitRowBitboards()
        {
            var rows = new Bitboard[] { 0xFF00000000000000,
                                        0x00FF000000000000,
                                        0x0000FF0000000000,
                                        0x000000FF00000000,
                                        0x00000000FF000000,
                                        0x0000000000FF0000,
                                        0x000000000000FF00,
                                        0x00000000000000FF };

            for (int square = 0; square < NUM_SQUARES; square++)
            {
                RowBitboards[square] = rows[GetRow(square)];
            }
        }

        private static void InitColumnBitboards()
        {
            var cols = new Bitboard[] { 0x8080808080808080,
                                        0x4040404040404040,
                                        0x2020202020202020,
                                        0x1010101010101010,
                                        0x0808080808080808,
                                        0x0404040404040404,
                                        0x0202020202020202,
                                        0x0101010101010101 };

            for (int square = 0; square < NUM_SQUARES; square++)
            {
                ColumnBitboards[square] = cols[GetColumn(square)];
            }
        }

        private static void InitDiagonalBitboards()
        {
            for (int i = 0; i < NUM_SQUARES; i++)
            {
                FrontDiagonalBitboards[i] = GenerateDiagonalBitboard(i, 7);
                BackDiagonalBitboards[i] = GenerateDiagonalBitboard(i, 9);
            }
        }

        private static Bitboard GenerateDiagonalBitboard(int square, int offset)
        {
            Debug.Assert(square >= 0 && square < 64);

            Bitboard result = SquareBitboards[square];
            for (int i = 0; i < 2; i++) // Two passes, each in opposite directions
            {
                int curr = square;
                int next = curr + offset;
                while (next > 0 && next < 64 && SquareDistance[curr, next] == 1)
                {
                    result |= SquareBitboards[next];
                    curr = next;
                    next += offset;
                }
                offset *= -1; // Reverse the ray direction
            }
            return result;
        }

        private static void InitKingBitboards()
        {
            int[] offsets = { -9, -8, -7, -1, 1, 7, 8, 9 };
            for (int square = 0; square < NUM_SQUARES; square++)
            {
                KingBitboards[square] = GenerateBitboardFromOffsets(square, 1, offsets);
            }
        }

        private static void InitPawnBitboards()
        {
            int[][] offsets = new int[2][];
            offsets[WHITE] = new int[] { 7, 9 };
            offsets[BLACK] = new int[] { -7, -9 };

            for (int i = 8; i < 56; i++)
            {
                PawnAttkBitboards[WHITE, i] = GenerateBitboardFromOffsets(i, 1, offsets[WHITE]);
                PawnAttkBitboards[BLACK, i] = GenerateBitboardFromOffsets(i, 1, offsets[BLACK]);
                PawnMoveBitboards[WHITE, i] = SquareBitboards[i + 8];
                PawnMoveBitboards[BLACK, i] = SquareBitboards[i - 8];
                if (i < 16)
                {
                    PawnMoveBitboards[WHITE, i] |= SquareBitboards[i + 16];
                }
                if (i >= 48)
                {
                    PawnMoveBitboards[BLACK, i] |= SquareBitboards[i - 16];
                }
            }
        }

        private static void InitKnightBitboards()
        {
            int[] offsets = { 17, 15, 10, 6, -6, -10, -15, -17 };
            for (int square = 0; square < NUM_SQUARES; square++)
            {
                KnightBitboards[square] = GenerateBitboardFromOffsets(square, 5, offsets);
            }
        }

        public static Bitboard GenerateBitboardFromOffsets(int square, int maxDistance, int[] offsets)
        {
            Bitboard result = 0;
            foreach (int offset in offsets)
            {
                int currSquare = square + offset;
                if (currSquare >= 0 && currSquare < NUM_SQUARES && SquareDistance[currSquare, square] <= maxDistance)
                {
                    result |= SquareBitboards[currSquare];
                }
            }
            return result;
        }

        public static int GetColumn(int square)
        {
            Debug.Assert(square >= 0 && square < NUM_SQUARES);
            return 7 - square % 8;
        }

        public static int GetRow(int square)
        {
            Debug.Assert(square >= 0 && square < NUM_SQUARES);
            return 7 - square / 8;
        }

        public static int CountBits(Bitboard bitboard)
        {
            int cnt = 0;
            while (bitboard != 0)
            {
                bitboard &= (bitboard - 1);
                cnt++;
            }
            return cnt;
        }

        public static void PopLsb(ref Bitboard bitboard)
        {
            bitboard &= (bitboard - 1);
        }

        public static int BitScanForward(this Bitboard bitboard)
        {
            Debug.Assert(bitboard != 0);
            return MultiplyDeBruijnBitPosition[((ulong)((long)bitboard & -(long)bitboard) * DeBruijnSequence) >> 58];
        }

        public static int GetSquareFromName(string name)
        {
            int col = name[0] - 'a';
            int row = name[1] - '1';
            return 8 * row + (7 - col);
        }

        public static string GetNameFromSquare(int square)
        {
            Debug.Assert(square >= 0 && square < NUM_SQUARES);

            char rank = (char)('a' + (7 - square % 8));
            char file = (char)('1' + (square / 8));
            return rank.ToString() + file;
        }
    }
}
