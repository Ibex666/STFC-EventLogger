using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Tesseract;
using System.Xml;
using System.Threading;
using System.Diagnostics.Metrics;
using System.Diagnostics;
using System.Drawing;
using STFC_EventLogger.AllianceClasses;
using System.Xml.Linq;

namespace STFC_EventLogger
{
    /// <summary>
    /// Enthält statische Funktionen
    /// </summary>
    internal static class F
    {
        /// <summary>
        /// Calculate the difference between 2 strings using the Levenshtein distance algorithm
        /// </summary>
        /// <param name="source1">First string</param>
        /// <param name="source2">Second string</param>
        /// <returns></returns>
        internal static int LevensteinDistance(string source1, string source2) //O(n*m)
        {
            var source1Length = source1.Length;
            var source2Length = source2.Length;

            var matrix = new int[source1Length + 1, source2Length + 1];

            // First calculation, if one entry is empty return full length
            if (source1Length == 0)
                return source2Length;

            if (source2Length == 0)
                return source1Length;

            // Initialization of matrix with row size source1Length and columns size source2Length
            for (var i = 0; i <= source1Length; matrix[i, 0] = i++) { }
            for (var j = 0; j <= source2Length; matrix[0, j] = j++) { }

            // Calculate rows and collumns distances
            for (var i = 1; i <= source1Length; i++)
            {
                for (var j = 1; j <= source2Length; j++)
                {
                    var cost = (source2[j - 1] == source1[i - 1]) ? 0 : 1;

                    matrix[i, j] = Math.Min(
                        Math.Min(matrix[i - 1, j] + 1, matrix[i, j - 1] + 1),
                        matrix[i - 1, j - 1] + cost);
                }
            }
            // return result
            return matrix[source1Length, source2Length];
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="scanMethod"></param>
        /// <param name="tessdata"></param>
        /// <param name="engineMode"></param>
        internal static void GetEngineModeData(ScanMethods scanMethod, out string tessdata, out EngineMode engineMode)
        {
            switch (scanMethod)
            {
                case ScanMethods.Tesseract:
                    tessdata = @"tessdata";
                    engineMode = EngineMode.TesseractOnly;
                    break;
                case ScanMethods.Fast:
                    tessdata = @"tessdata_fast";
                    engineMode = EngineMode.LstmOnly;
                    break;
                case ScanMethods.Best:
                    tessdata = @"tessdata_best";
                    engineMode = EngineMode.LstmOnly;
                    break;
                default:
                    tessdata = @"tessdata";
                    engineMode = EngineMode.TesseractOnly;
                    break;
            }
        }
    }
}