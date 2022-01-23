using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PyraAppCs
{
    class Pyra
    {
        enum moves { eU, eUp, eR, eRp, eL, eLp, eB, eBp };

        moves[] possmoves = { moves.eU, moves.eUp, moves.eR, moves.eRp, moves.eL, moves.eLp, moves.eB, moves.eBp };
        UInt64 solvedState = 39258858879;

        UInt64 swap2(UInt64 k, int pos1, int pos2)
        {
            UInt64 xors = ((k >> pos1) ^ (k >> pos2)) & 3;
            return k ^ ((xors << pos1) | (xors << pos2));
        }

        UInt64 swap1(UInt64 k, int pos1, int pos2)
        {
            UInt64 xors = ((k >> pos1) ^ (k >> pos2)) & 1;
            return k ^ ((xors << pos1) | (xors << pos2));
        }

        UInt64 twistC(UInt64 k, int pos)
        {
            k = swap1(k, pos, pos - 1);
            k = swap1(k, pos - 1, pos - 2);
            return k;
        }

        UInt64 twistCC(UInt64 k, int pos)
        {
            k = swap1(k, pos - 1, pos - 2);
            k = swap1(k, pos, pos - 1);
            return k;
        }


        UInt64 U(UInt64 k)
        {
            k = twistC(k, 35);

            k = swap2(k, 22, 10);
            k = swap2(k, 10, 16);

            k = swap2(k, 20, 8);
            k = swap2(k, 8, 14);

            return k;
        }

        UInt64 Up(UInt64 k)
        {
            k = twistCC(k, 35);

            k = swap2(k, 10, 16);
            k = swap2(k, 22, 10);

            k = swap2(k, 8, 14);
            k = swap2(k, 20, 8);

            return k;
        }

        UInt64 R(UInt64 k)
        {
            k = twistC(k, 29);

            k = swap2(k, 20, 4);
            k = swap2(k, 4, 6);

            k = swap2(k, 10, 18);
            k = swap2(k, 18, 2);

            return k;
        }

        UInt64 Rp(UInt64 k)
        {
            k = twistCC(k, 29);

            k = swap2(k, 4, 6);
            k = swap2(k, 20, 4);

            k = swap2(k, 18, 2);
            k = swap2(k, 10, 18);

            return k;
        }

        UInt64 L(UInt64 k)
        {
            k = twistC(k, 32);

            k = swap2(k, 22, 12);
            k = swap2(k, 12, 4);

            k = swap2(k, 14, 0);
            k = swap2(k, 0, 18);

            return k;
        }

        UInt64 Lp(UInt64 k)
        {
            k = twistCC(k, 32);

            k = swap2(k, 12, 4);
            k = swap2(k, 22, 12);

            k = swap2(k, 0, 18);
            k = swap2(k, 14, 0);

            return k;
        }

        UInt64 B(UInt64 k)
        {
            k = twistC(k, 26);

            k = swap2(k, 8, 2);
            k = swap2(k, 2, 12);

            k = swap2(k, 16, 6);
            k = swap2(k, 6, 0);

            return k;
        }

        UInt64 Bp(UInt64 k)
        {
            k = twistCC(k, 26);

            k = swap2(k, 2, 12);
            k = swap2(k, 8, 2);

            k = swap2(k, 6, 0);
            k = swap2(k, 16, 6);

            return k;
        }

        UInt64 doMove(UInt64 k, moves move)
        {
            switch (move)
            {
                case moves.eR:
                    return R(k);
                case moves.eRp:
                    return Rp(k);
                case moves.eL:
                    return L(k);
                case moves.eLp:
                    return Lp(k);
                case moves.eU:
                    return U(k);
                case moves.eUp:
                    return Up(k);
                case moves.eB:
                    return B(k);
                case moves.eBp:
                    return Bp(k);
            }

            return 0;
        }

        public void distParallel()
        {
            moves[] possmoves = { moves.eU, moves.eUp, moves.eR, moves.eRp, moves.eL, moves.eLp, moves.eB, moves.eBp };

            var overview = new ConcurrentHashSet<UInt64>[12];
            for (int i = 0; i < 12; i++)
            {
                overview[i] = new ConcurrentHashSet<UInt64>();
            }


            overview[0].Add(solvedState);
            for (int i = 0; i < 12; i++)
            {
                var a = Parallel.ForEach(overview[i], pos =>
                {
                    foreach (var move in possmoves)
                    {
                        UInt64 posn = doMove(pos, move);
                        if (i == 0)
                        {
                            if (!overview[i].Contains(posn))
                            {
                                overview[i + 1].Add(posn);
                            }
                        }
                        else
                        {
                            if (!overview[i - 1].Contains(posn) && !overview[i].Contains(posn))
                            {
                                overview[i + 1].Add(posn);
                            }
                        }
                    }
                });
            }
            for (int i = 0; i < 12; i++)
            {
                Console.WriteLine($"{i}\t{overview[i].Count}");
            }
        }
    
        public void dist()
        {
            HashSet<UInt64>[] overview = new HashSet<UInt64>[12];
            for (int i = 0; i < 12; i++)
            {
                overview[i] = new HashSet<UInt64>();
            }

            overview[0].Add(solvedState);
            for (int i = 0; i < 12; i++)
            {
                foreach (var pos in overview[i])
                {
                    foreach (var move in possmoves)
                    {
                        UInt64 posn = doMove(pos, move);
                        if (i == 0)
                        {
                            if (!overview[i].Contains(posn))
                            {
                                overview[i + 1].Add(posn);
                            }
                        }
                        else
                        {
                            if (!overview[i - 1].Contains(posn) && !overview[i].Contains(posn))
                            {
                                overview[i + 1].Add(posn);
                            }
                        }
                    }
                }
            }
            for (int i = 0; i < 12; i++)
            {
                Console.WriteLine($"{i}\t{overview[i].Count}");
            }
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            var runParallel = args.Length > 0 && args[0] == "p";
            var watch = new Stopwatch();

            watch.Start();

            if (runParallel)
            {
                new Pyra().distParallel();
            }
            else
            {
                new Pyra().dist();
            }

            watch.Stop();

            Console.WriteLine($"Execution Time: {watch.ElapsedMilliseconds} ms");
            //Console.ReadLine();
        }
    }
}
