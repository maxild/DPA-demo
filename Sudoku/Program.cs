using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace Sudoku
{
    internal static class Program
    {
        private static int Main()
        {
/*      if (args.Length == 0)
      {
        Console.WriteLine("Please provide input board");
        return;
      }

      var filePath = args[0];
*/

            // Sudoku/bin/Debug/net5.0/Sudoku.dll means pwd is net5.0 directory
            // and we have to '3 times up' to get to Sudoku dir containing Data sub-dir
            var filePath = "../../../Data/sudoku.txt";

            if (!File.Exists(filePath))
            {
                Console.WriteLine("File doesn't exist");
                return 1;
            }

            var inputBoard = CreateInputBoard(filePath);

            var sw = new Stopwatch();
            sw.Start();
            var resultBoard = Solver.Solve(inputBoard, out var steps);
            sw.Stop();

            if (resultBoard is null)
            {
                Console.WriteLine("The sudoku is not solved");
                return 2;
            }

            CheckSolution(resultBoard);

            Console.WriteLine($"Solved in {steps} steps, took {sw.Elapsed}");

            PrintResultBoard(resultBoard);

            return 0;
        }

        private static Board CreateInputBoard(string filePath)
        {
            var boardData = new int[9][];
            using var reader = File.OpenText(filePath);
            for (var i = 0; i < 9; i++)
            {
                // find non-empty line containing at least single ","
                string? line;
                do
                {
                    line = reader.ReadLine() ?? string.Empty;
                } while (!line.Contains(",", StringComparison.Ordinal));

                var numbers = line.Split(",")
                    .Select(s => int.TryParse(s, out var value) ? value : NumberEx.Unknown)
                    .ToArray();

                if (numbers.Length != 9)
                    throw new ArgumentException("Invalid data format");

                boardData[i] = numbers;
            }

            return Board.CreateInitialBoard(boardData);
        }

        private static void CheckSolution(Board board)
        {
            for (var row = 0; row < 9; row++)
            {
                var rowHashset = new HashSet<int>();
                for (var col = 0; col < 9; col++)
                    if (!rowHashset.Add(board.Cells[row, col].Number))
                        throw new InvalidOperationException();
            }

            for (var col = 0; col < 9; col++)
            {
                var colHashset = new HashSet<int>();
                for (var row = 0; row < 9; row++)
                    if (!colHashset.Add(board.Cells[row, col].Number))
                        throw new InvalidOperationException();
            }
        }

        private static void PrintResultBoard(Board resultBoard)
        {
            for (var row = 0; row < 9; row++)
            {
                for (var col = 0; col < 9; col++)
                {
                    Console.Write(resultBoard.Cells[row, col].Number);
                    Console.Write(", ");
                }

                Console.WriteLine();
            }
        }
    }
}
