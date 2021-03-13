using System;
using System.Collections.Generic;
using System.Data;
using System.IO;

namespace TriangleSum
{

	public class Program
	{

		static void Main(string[] args)
		{
			// obtains the input and parses it into memory
			var triangle = getTriangle();

            // splits the triangle into rows
			string[] splitTriangle = triangle.Split('\n');

            // trims the table and converts it into an accessible lower triangular matrix which is the same as a pyramid structure
			var tableHolder = FlattenTheTriangleIntoTable(splitTriangle);
			var watch = System.Diagnostics.Stopwatch.StartNew();

            // calculates the maximum sum
			var maxSum = calcMax(tableHolder);
			watch.Stop();
			var elapsedMs = watch.ElapsedMilliseconds;

			Console.WriteLine("The maximum sum is " + maxSum +
				"\n" + "Runtime for this method took " + elapsedMs + " ms");
		}

		// Is used to obtain the triangle fed as a string
		private static string getTriangle()
		{
			const string triangle = @"215
            192 124
            117 269 442
            218 836 347 235
            320 805 522 417 345
            229 601 728 835 133 124
            248 202 277 433 207 263 257
            359 464 504 528 516 716 871 182
            461 441 426 656 863 560 380 171 923
            381 348 573 533 448 632 387 176 975 449
            223 711 445 645 245 543 931 532 937 541 444
            330 131 333 928 376 733 017 778 839 168 197 197
            131 171 522 137 217 224 291 413 528 520 227 229 928
            223 626 034 683 839 052 627 310 713 999 629 817 410 121
            924 622 911 233 325 139 721 218 253 223 107 233 230 124 233";

            /*
			const string triangle = @"1
            8 9
            1 5 9
            4 5 2 3";
            */

			return triangle;

		}

		private static int calcMax(int[,] triangle)
		{
			int lines = triangle.GetLength(0);

			// Some point to moving upward from the nodes by evaluating conditions (i.e. parity mismatch) from the second to last nodes
			// and up and then summing the values bottom-up. But taking this approach would clearly violate the parity structure of
			// the pyramid. Hence, we need to create a separate number for each iteration together with a validation on
			// path eligibility, i.e. the existence of a child with opposite parity            

			int[,] arrayPath = new int[lines, lines + 1];
			bool[,] arrayBool = new bool[lines, lines + 1];

			// For loop to calculate the true/false map and array map of values for true nodes
			for (int r = 1; r < lines - 1; r++)
			{
				for (int c = 0; c <= r; c++)
				{
					int childNode = 0;
					int scndChildNode = 0;
					int prntNode = 0;
					int scndPrntNode = 0;

					childNode = triangle[r + 1, c];

					try
					{
						scndChildNode = triangle[r + 1, c + 1];
					}

					catch (Exception e)
					{
						childNode = scndChildNode;
						// ^ Can set them equal as only one parity condition is required for the outer most node
					}

					prntNode = triangle[r - 1, c];

					try
					{
						scndPrntNode = triangle[r - 1, c - 1];
					}

					catch (Exception e)
					{
						scndPrntNode = prntNode;
						// ^ Can set them equal as only one parity condition is required for the outer most node
					}

					var val = triangle[r, c];
					arrayBool[r, c] = true;

					// if the current value is even, run a check to see if it's parents and its children are of opposite value.
					// provision added for cases of outermost values, i.e. the value is 0. 

					if (val % 2 == 0)
					{
						if (childNode % 2 == 0 && scndChildNode % 2 == 0 ||
							triangle[r - 1, c] == 0 && scndPrntNode % 2 == 0 ||
							prntNode % 2 == 0 && scndPrntNode % 2 == 0)
						{
							arrayBool[r, c] = false;
						}

						// if the current node is a possible path I add the value to the second array for later to sum it up
						if (arrayBool[r, c])
						{
							arrayPath[r, c] = val;
						}

					}

					else
					{
						if (childNode % 2 != 0 && scndChildNode % 2 != 0 ||
							triangle[r - 1, c] == 0 && scndPrntNode % 2 != 0 ||
							prntNode % 2 != 0 && scndPrntNode % 2 != 0)
						{
							arrayBool[r, c] = false;
						}

                        // if the current node is a possible path I add the value to the second array for later to sum it up
						if (arrayBool[r, c])
						{
							arrayPath[r, c] = val;
						}

					}

				}

			}

			// Afterwards I add the first and last values to the array. Since it is assumed that there is a solution to the triangle, this should be allowed.            
			arrayPath[0, 0] = triangle[0, 0];

			// For loop to add the last values and their corresponding true/false values
			for (int i = 0; i < lines; i++)
			{
				var prntNode = 0;
				var scndPrntNode = 0;
				arrayPath[lines - 1, i] = triangle[lines - 1, i];
				var val = arrayPath[lines - 1, i];
				prntNode = triangle[lines - 2, i];

				try
				{
					scndPrntNode = triangle[lines - 2, i - 1];
				}
				catch (Exception e)
				{
					prntNode = scndPrntNode;
				}

				arrayBool[lines - 1, i] = true;
				var val2 = triangle[lines - 1, i];

				if (val2 % 2 == 0)
				{
					if (arrayPath[lines - 2, i] == 0 && scndPrntNode % 2 == 0 ||
						prntNode % 2 == 0 && scndPrntNode % 2 == 0)
					{
						arrayBool[lines - 1, i] = false;
					}
				}
				else
				{
					if (arrayPath[lines - 2, i] == 0 && scndPrntNode % 2 != 0 ||
						prntNode % 2 != 0 && scndPrntNode % 2 != 0)
					{
						arrayBool[lines - 1, i] = false;
					}
				}
				if (arrayBool[lines - 1, i])
				{
					arrayPath[lines - 1, i] = val2;
				}
			}

			int posLines = arrayPath.GetLength(0);

			// Now finally to calculate the maxmimum node value. We start from the second to last row and move our way up
			// These iterations are done by comparing two arrays. One with booleans for its path evaluation (true/false) map
			// and another to continously add the sums up together (the arrayPath). 

			for (int row = posLines - 2; row >= 0; row--)
			{
				for (int col = 0; col <= row; col++)
				{
					var child = arrayBool[row + 1, col];
					var scndChild = arrayBool[row + 1, col + 1];

					if (child && scndChild)
					{
						arrayPath[row, col] += Math.Max(arrayPath[row + 1, col], arrayPath[row + 1, col + 1]);
					}
					else if (child && scndChild == false)
					{

						arrayPath[row, col] += arrayPath[row + 1, col];
					}
					else if (child == false && scndChild)
					{
						arrayPath[row, col] += arrayPath[row + 1, col + 1];
					}

				}

			}
			return arrayPath[0, 0];
		}

		private static int[,] FlattenTheTriangleIntoTable(string[] splitTriangle)
		{
			// creates a new table with dimensions equal to the number of rows parsed by the input element and columns
			// equal to the number of rows +1. 

			int[,] tableHolder = new int[splitTriangle.Length, splitTriangle.Length + 1];

			for (int row = 0; row < splitTriangle.Length; row++)
			{
				var eachCharactersInRow = splitTriangle[row].Trim().Split(' ');
				for (int column = 0; column < eachCharactersInRow.Length; column++)
				{
					int.TryParse(eachCharactersInRow[column], out int number);
					tableHolder[row, column] = number;
				}
			}
			return tableHolder;
		}

	}

}
