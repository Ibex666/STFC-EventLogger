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

        internal static void InvertImages()
        {
            List<Task> tasks = new();
            List<SSTypeAnalyzer> tmpFiles = new();
            using SemaphoreSlim semaphore = new(V.us.MaxParallelTasks);
            foreach (var file in V.filesToScan)
            {
                semaphore.Wait();

                var t = Task.Factory.StartNew(() =>
                {
                    using Image img = Image.FromFile(file.FileName);
                    using Image imgNeg = ImageFunctions.InvertUnsafe(img);

                    string fileNeg = Path.GetTempFileName();
                    imgNeg.Save(fileNeg);
                    tmpFiles.Add(new SSTypeAnalyzer(fileNeg, ImageTypes.Negative));
                    V.tmpFiles.Add(fileNeg);

                    semaphore.Release();
                });
                tasks.Add(t);
            }
            Task.WaitAll(tasks.ToArray());

            V.filesToScan.AddRange(tmpFiles);
        }
        internal static void AnalyzeScreenshots()
        {
            List<Task> tasks = new();
            using SemaphoreSlim semaphore = new(V.us.MaxParallelTasks);
            foreach (var file in V.filesToScan)
            {
                semaphore.Wait();

                var t = Task.Factory.StartNew(() =>
                {
                    file.Analyze();

                    semaphore.Release();
                });
                tasks.Add(t);
            }
            Task.WaitAll(tasks.ToArray());
        }
        internal static void ScanMemberList()
        {
            List<Task> tasks = new();
            using SemaphoreSlim semaphore = new(V.us.MaxParallelTasks);
            foreach (var file in V.filesToScan.Where(_ => _.PageType == PageTypes.MemberList))
            {
                semaphore.Wait();
                var t = Task.Factory.StartNew(() =>
                {
                    Pix image = Pix.LoadFromFile(file.FileName);

                    using (var engine = new TesseractEngine(System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"tessdata"), "eng", EngineMode.TesseractOnly))
                    {
                        using var page = engine.Process(image, V.us.RectAllianceNames);
                        XmlDocument xdoc = new();
                        xdoc.LoadXml(page.GetAltoText(0));
                        var nodes = xdoc.SelectNodes("//TextBlock");
                        if (nodes != null)
                        {
                            foreach (XmlElement node in nodes)
                            {
                                AllianceMember _am = new(node, file);

                                if (_am.Name.WC < 1)
                                {
                                    V.notRecognizedNames.Add(_am.Name);
                                    continue;
                                }

                                _am.Levels.Add(ScanMemberLevel(image, new Rect(_am.Levels[0].X1 - 10, _am.Levels[0].Y1 - 10, _am.Levels[0].Width + 20, _am.Levels[0].Height + 20), file, ScanMethods.Fast));
                                _am.Levels.Add(ScanMemberLevel(image, new Rect(_am.Levels[0].X1 - 10, _am.Levels[0].Y1 - 10, _am.Levels[0].Width + 20, _am.Levels[0].Height + 20), file, ScanMethods.Best));
                                _am.Powers.Add(ScanMemberPower(image, new Rect(V.us.RectAlliancePower.X, _am.Rank.Y, V.us.RectAlliancePower.Width, _am.Levels[0].Height + _am.Levels[0].Y - _am.Rank.Y), file, ScanMethods.Tesseract));
                                _am.Powers.Add(ScanMemberPower(image, new Rect(V.us.RectAlliancePower.X, _am.Rank.Y, V.us.RectAlliancePower.Width, _am.Levels[0].Height + _am.Levels[0].Y - _am.Rank.Y), file, ScanMethods.Fast));
                                _am.Powers.Add(ScanMemberPower(image, new Rect(V.us.RectAlliancePower.X, _am.Rank.Y, V.us.RectAlliancePower.Width, _am.Levels[0].Height + _am.Levels[0].Y - _am.Rank.Y), file, ScanMethods.Best));

                                if (V.allianceMembers.Contains(_am))
                                {
                                    int idx = V.allianceMembers.IndexOf(_am);
                                    V.allianceMembers[idx].Levels.AddRange(_am.Levels);
                                    V.allianceMembers[idx].Powers.AddRange(_am.Powers);
                                }
                                else
                                {
                                    V.allianceMembers.Add(_am);
                                }

                            }
                        }
                    }

                    image.Dispose();
                    semaphore.Release();
                });
                tasks.Add(t);
            }
            Task.WaitAll(tasks.ToArray());
        }
        internal static void ScanEventList()
        {
            List<Task> tasks = new();

            using SemaphoreSlim semaphore = new(V.us.MaxParallelTasks);
            foreach (var file in V.filesToScan.Where(_ => _.PageType == PageTypes.EventList))
            {
                semaphore.Wait();

                var t = Task.Factory.StartNew(() =>
                {
                    Pix image = Pix.LoadFromFile(file.FileName);

                    using (var engine = new TesseractEngine(System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"tessdata"), "eng", EngineMode.TesseractOnly))
                    {
                        using var page = engine.Process(image, V.us.RectEventNames);
                        XmlDocument xdoc = new();
                        xdoc.LoadXml(page.GetAltoText(0));
                        var nodes = xdoc.SelectNodes("//TextLine/SP/../..");
                        if (nodes != null)
                        {
                            foreach (XmlElement node in nodes)
                            {
                                OcrName _name = new(node.SelectNodes(".//String[position()>1]"), file);

                                AllianceMember? member = V.allianceMembers.SingleOrDefault(_ => _.Name == _name);
                                if (member != null)
                                {
                                    int idx = V.allianceMembers.IndexOf(member);
                                    V.allianceMembers[idx].EventListName = _name;

                                    member.Scores.Add(ScanMemberScore(image, new Rect(V.us.RectEventScores.X1, _name.Y1 - 10, V.us.RectEventScores.Width, _name.Height + 20), file, ScanMethods.Tesseract));
                                    member.Scores.Add(ScanMemberScore(image, new Rect(V.us.RectEventScores.X1, _name.Y1 - 10, V.us.RectEventScores.Width, _name.Height + 20), file, ScanMethods.Fast));
                                    member.Scores.Add(ScanMemberScore(image, new Rect(V.us.RectEventScores.X1, _name.Y1 - 10, V.us.RectEventScores.Width, _name.Height + 20), file, ScanMethods.Best));
                                }
                                else
                                {
                                    if (_name.WC < 1)
                                    {
                                        V.notRecognizedNames.Add(_name);
                                    }
                                }
                            }
                        }
                    }
                    image.Dispose();
                    semaphore.Release();
                });
                tasks.Add(t);
            }
            Task.WaitAll(tasks.ToArray());
        }

        internal static OcrPower ScanMemberPower(Pix image, Rect scanArea, SSTypeAnalyzer file, ScanMethods scanMethod)
        {
            GetEngineModeData(scanMethod, out string tessdata, out EngineMode engineMode);

            OcrPower ret = new()
            {
                FileName = file.FileName
            };
            if (scanArea.X < 0 | scanArea.Y < 0 | scanArea.X2 > image.Width | scanArea.Y2 > image.Height)
                return ret;

            try
            {
                using var engine = new TesseractEngine(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, tessdata), "eng", engineMode);
                using var page = engine.Process(image, scanArea);
                XmlDocument xdoc = new();
                xdoc.LoadXml(page.GetAltoText(0));
                var nodes = xdoc.SelectNodes("//String");
                if (nodes != null && nodes.Count > 0)
                {
                    ret = new OcrPower(nodes[0], file);
                }
            }
            catch (AccessViolationException) { }

            return ret;
        }
        internal static OcrLevel ScanMemberLevel(Pix image, Rect scanArea, SSTypeAnalyzer file, ScanMethods scanMethod)
        {
            GetEngineModeData(scanMethod, out string tessdata, out EngineMode engineMode);

            OcrLevel ret = new()
            {
                FileName = file.FileName
            };
            if (scanArea.X < 0 | scanArea.Y < 0 | scanArea.X2 > image.Width | scanArea.Y2 > image.Height)
                return ret;

            try
            {
                using var engine = new TesseractEngine(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, tessdata), "eng", engineMode);
                using var page = engine.Process(image, scanArea);
                XmlDocument xdoc = new();
                xdoc.LoadXml(page.GetAltoText(0));
                var nodes = xdoc.SelectNodes("//String");
                if (nodes != null && nodes.Count > 0)
                {
                    ret = new OcrLevel(nodes[0], file);
                }
            }
            catch (AccessViolationException) { }

            return ret;
        }
        internal static OcrScore ScanMemberScore(Pix image, Rect scanArea, SSTypeAnalyzer file, ScanMethods scanMethod)
        {
            GetEngineModeData(scanMethod, out string tessdata, out EngineMode engineMode);

            OcrScore ret = new()
            {
                FileName = file.FileName
            };
            if (scanArea.X < 0 | scanArea.Y < 0 | scanArea.X2 > image.Width | scanArea.Y2 > image.Height)
                return ret;

            try
            {
                using var engine = new TesseractEngine(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, tessdata), "eng", engineMode);
                using var page = engine.Process(image, scanArea);
                XmlDocument xdoc = new();
                xdoc.LoadXml(page.GetAltoText(0));
                var nodes = xdoc.SelectNodes("//String");
                if (nodes != null && nodes.Count > 0)
                {
                    ret = new OcrScore(nodes[0], file);
                }
            }
            catch (AccessViolationException) { }

            return ret;
        }

    }
}
