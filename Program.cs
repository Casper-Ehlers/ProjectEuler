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
            // prints the maximum sum 
            var maxSum = calcMax(tableHolder);
            watch.Stop();
            var elapsedMs = watch.ElapsedMilliseconds;            

            Console.WriteLine("The maximum sum is " + maxSum +
                System.Environment.NewLine +
                "Runtime for this method took " + elapsedMs + " ms");

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

            //const string triangle = @"1
            //8 9
            //1 5 9
            //4 5 2 3";

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

            for (int r = 1; r < lines - 1; r++)
            {
                
                for (int c = 0; c <= r; c++)
                {                                        
                    // could not get ?? operator working therefore I have to use these ugly try catch expressions    
                    int childNode = 0;                    
                    int scndChildNode = 0;
                    int prntNode = 0;
                    int scndPrntNode = 0;                    

                    try
                    {
                        int.TryParse(triangle[r + 1, c].ToString(), out childNode);
                        int.TryParse(triangle[r + 1, c + 1].ToString(), out scndChildNode);
                    }
                    catch (Exception ex)
                    {
                        try
                        {
                            int.TryParse(triangle[r + 1, c].ToString(), out childNode);
                        }
                        catch (Exception ex_2)
                        {
                            int.TryParse(triangle[r + 1, c + 1].ToString(), out scndChildNode);
                            childNode = scndChildNode;
                        }
                        scndChildNode = childNode;
                        // ^ Can set them equal as only one parity condition is required
                    }

                    try
                    {
                        int.TryParse(triangle[r - 1, c].ToString(), out prntNode);
                        int.TryParse(triangle[r - 1, c - 1].ToString(), out scndPrntNode);
                    }
                    catch (Exception ex)
                    {
                        try
                        {
                            int.TryParse(triangle[r - 1, c].ToString(), out prntNode);
                        }
                        catch (Exception ex_2)
                        {
                            int.TryParse(triangle[r - 1, c - 1].ToString(), out scndPrntNode);
                            prntNode = scndPrntNode;
                        }
                        scndPrntNode = prntNode;
                        // ^ Can set them equal as only one parity condition is required
                    }

                    var val = triangle[r, c];
                    arrayBool[r, c] = true;
                    
                    if (val % 2 == 0)
                    {

                        if (childNode % 2 == 0 && scndChildNode % 2 == 0)
                        {
                            arrayBool[r, c] = false;
                        }
                        if (prntNode % 2 == 0 && scndPrntNode % 2 == 0)
                        {
                            arrayBool[r, c] = false;
                        }
                        if (arrayBool[r, c] == true)
                        {
                            arrayPath[r, c] = val;
                        }

                    }

                    else
                    {

                        if (childNode % 2 != 0 && scndChildNode % 2 != 0)
                        {
                            arrayBool[r, c] = false;
                        }
                        if (prntNode % 2 != 0 && scndPrntNode % 2 != 0)
                        {
                            arrayBool[r, c] = false;
                        }
                        if (arrayBool[r, c] == true)
                        {                         
                            arrayPath[r, c] = val;
                        }

                    }

                }

            }
                                  
            // Finally I add the first and last values to the array. Since it is assumed that there is a solution to the triangle, this should be allowed.            
            arrayPath[0, 0] = triangle[0, 0];

            for (int i = 0; i < lines; i++)
            {
                arrayPath[lines - 1, i] = triangle[lines - 1, i];
            }

            int posLines = arrayPath.GetLength(0);

            // We start from the second to last row and move our way up.
            // These iterations are done by comparing two arrays. One with booleans for its path evaluation
            // and another to continously add the sums up together. 
            for (int row = posLines - 2; row >= 0; row--)
            {

                for (int col = 0; col <= row; col++)
                {
                                                          
                    var trueChild = arrayBool[row + 1, col];
                    var trueScndChild = arrayBool[row + 1, col + 1];                    
                    
                    if (trueChild == true && trueScndChild == true)
                    {
                        arrayPath[row, col] += Math.Max(arrayPath[row + 1, col], arrayPath[row + 1, col + 1]);
                    }

                    else if (trueChild == true && trueScndChild == false)
                    {
                        arrayPath[row, col] += arrayPath[row + 1, col];
                    }

                    else if (trueChild == false && trueScndChild == false)
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
                    int number;
                    int.TryParse(eachCharactersInRow[column], out number);
                    tableHolder[row, column] = number;
                }
            }
            return tableHolder;
        }       

}
}
