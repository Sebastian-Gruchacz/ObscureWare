namespace Obscureware.Console.Operations
{
    using System;

    public static class Extensions
    {

        // http://stackoverflow.com/questions/837155/fastest-function-to-generate-excel-column-letters-in-c-sharp

        public static string ToAlphaEnum(this uint @value)
        {
            string columnString = string.Empty;
            decimal columnNumber = @value;
            while (columnNumber > 0)
            {
                decimal currentLetterNumber = (columnNumber - 1) % 26;
                char currentLetter = (char)(currentLetterNumber + 65);
                columnString = currentLetter + columnString;
                columnNumber = (columnNumber - (currentLetterNumber + 1)) / 26;
            }

            return columnString;
        }

        public static int FromAlphaEnum(this string @value)
        {
            int retVal = 0;
            string col = @value.ToUpper();
            for (int iChar = col.Length - 1; iChar >= 0; iChar--)
            {
                char colPiece = col[iChar];
                int colNum = colPiece - 64;
                retVal = retVal + colNum * (int)Math.Pow(26, col.Length - (iChar + 1));
            }
            return retVal;
        }
    }
}