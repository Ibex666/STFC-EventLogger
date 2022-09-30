using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net;
using System.Net.Security;
using System.Runtime.CompilerServices;
using System.Security;
using System.Windows.Controls.Primitives;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Xml;

namespace STFC_EventLogger.AllianceClasses
{
    internal class AllianceMember : IEquatable<AllianceMember?>, INotifyPropertyChanged
    {
        private float? accuracyLevel;
        private float? accuracyScore;
        private float? accuracyPower;
        private uint? bestLevel;
        private ulong? bestScore;
        private ulong? bestPower;
        private SolidColorBrush accuracyLevelBrush;
        private SolidColorBrush accuracyScoreBrush;
        private SolidColorBrush accuracyPowerBrush;

        public OcrName Name { get; set; }
        public OcrName EventListName { get; set; }
        public OcrRank Rank { get; set; }
        public uint? BestLevel
        {
            get => bestLevel;
            set
            {
                bestLevel = value;
                AccuracyLevel = 1;
                OnPropertyChanged();
            }
        }
        public ulong? BestScore
        {
            get => bestScore;
            set
            {
                bestScore = value;
                AccuracyScore = 1;
                OnPropertyChanged();
            }
        }
        public ulong? BestPower
        {
            get => bestPower;
            set
            {
                bestPower = value;
                AccuracyPower = 1;
                OnPropertyChanged();
            }
        }
        public BitmapImage? BestLevelImage { get; set; }
        public BitmapImage? BestScoreImage { get; set; }
        public BitmapImage? BestPowerImage { get; set; }

        public float? AccuracyLevel
        {
            get => accuracyLevel;
            private set
            {
                accuracyLevel = value;
                OnPropertyChanged();

                AccuracyLevelBrush = CalcAccuracyBrush(value, V.us.AccuracyLevelBrushLimits);
            }
        }
        public float? AccuracyScore
        {
            get => accuracyScore;
            private set
            {
                accuracyScore = value;
                OnPropertyChanged();

                AccuracyScoreBrush = CalcAccuracyBrush(value, V.us.AccuracyScoreBrushLimits);
            }
        }
        public float? AccuracyPower
        {
            get => accuracyPower;
            private set
            {
                accuracyPower = value;
                OnPropertyChanged();

                AccuracyPowerBrush = CalcAccuracyBrush(value, V.us.AccuracyPowerBrushLimits);
            }
        }

        public SolidColorBrush AccuracyLevelBrush
        {
            get { return accuracyLevelBrush; }
            private set
            {
                accuracyLevelBrush = value;
                OnPropertyChanged();
            }
        }
        public SolidColorBrush AccuracyScoreBrush
        {
            get { return accuracyScoreBrush; }
            private set
            {
                accuracyScoreBrush = value;
                OnPropertyChanged();
            }
        }
        public SolidColorBrush AccuracyPowerBrush
        {
            get { return accuracyPowerBrush; }
            private set
            {
                accuracyPowerBrush = value;
                OnPropertyChanged();
            }
        }

        public List<OcrLevel> Levels { get; set; }
        public List<OcrScore> Scores { get; set; }
        public List<OcrPower> Powers { get; set; }

        public int? PowerRanking { get; set; }
        public int? EventRanking { get; set; }

        #region #- Events -#

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string propertyName = "") =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

        #endregion

        internal AllianceMember()
        {
            Name = new OcrName();
            Rank = new OcrRank();
            Levels = new();
            Scores = new();
            Powers = new();

            BestLevel = new();
            BestScore = new();
            BestPower = new();

            accuracyLevelBrush = new();
            accuracyScoreBrush = new();
            accuracyPowerBrush = new();

            EventListName = new();
        }
        public AllianceMember(XmlElement xml, SSTypeAnalyzer file)
        {
            Levels = new();
            Scores = new();
            Powers = new();

            Name = new OcrName(xml.SelectNodes("./TextLine[2]/String[position()>1]"), file);
            Rank = new OcrRank(xml.SelectSingleNode("./TextLine[1]/String"), file);
            Levels.Add(new OcrLevel(xml.SelectSingleNode("./TextLine[2]/String[1]"), file));

            BestLevel = new();
            BestScore = new();
            BestPower = new();

            accuracyLevelBrush = new();
            accuracyScoreBrush = new();
            accuracyPowerBrush = new();

            EventListName = new();
        }




        public void MergeData()
        {
            MergeLevelData();
            MergeScoreData();
            MergePowerData();
        }

        private void MergeLevelData()
        {
            if (Levels != null && Levels.Count > 0)
            {
                Dictionary<uint, int> tmp = new();

                foreach (var item in Levels)
                {
                    if (item.Value == null || item.Value == 0)
                        continue;

                    if (tmp.ContainsKey((uint)item.Value) == false)
                    {
                        tmp.Add((uint)item.Value, 1);
                    }
                    else
                    {
                        tmp[(uint)item.Value] += 1;
                    }
                }

                if (tmp.Count > 0)
                {
                    var _vs = tmp.First(_sc => _sc.Value == tmp.Max(item => item.Value));
                    BestLevel = _vs.Key;
                    var _bestLevel = Levels.FirstOrDefault(_ => _.Value == _vs.Key & _.ImageType == ImageTypes.Positive);
                    if (_bestLevel == null)
                    {
                        _bestLevel = Levels.First(_ => _.Value == _vs.Key);
                    }

                    AccuracyLevel = (float)Math.Round(_vs.Value / (double)Levels.Count, 3);

                    using var img = System.Drawing.Image.FromFile(_bestLevel.FileName);
                    BestLevelImage = ImageFunctions.ImageFromBuffer(ImageFunctions.CropImage(
                        img,
                        _bestLevel.X1 - 5,
                        _bestLevel.Y1 - 5,
                        _bestLevel.X2 + 5,
                        _bestLevel.Y2 + 5,
                        System.Drawing.Imaging.ImageFormat.Png));
                }
                else
                {
                    if (Levels.Count > 0)
                    {
                        using var img = System.Drawing.Image.FromFile(Name.FileName);
                        BestLevelImage = ImageFunctions.ImageFromBuffer(ImageFunctions.CropImage(
                            img,
                            Levels[0].X1 - 5,
                            Levels[0].Y1 - 5,
                            Levels[0].X2 + 5,
                            Levels[0].Y2 + 5,
                            System.Drawing.Imaging.ImageFormat.Png));
                    }
                    else
                    {
                        using var img = System.Drawing.Image.FromFile(Name.FileName);
                        BestLevelImage = ImageFunctions.ImageFromBuffer(ImageFunctions.CropImage(
                            img,
                            V.us.RectAllianceNames.X1,
                            Name.Y1 - 5,
                            V.us.RectAllianceNames.X2,
                            Name.Y2 + 5,
                            System.Drawing.Imaging.ImageFormat.Png));
                    }

                    AccuracyLevel = 0;
                }
            }
            else
            {
                BestLevel = null;
                AccuracyLevel = null;
            }
        }
        private void MergeScoreData()
        {
            if (Scores != null && Scores.Count > 0)
            {
                Dictionary<ulong, int> tmp = new();

                foreach (var item in Scores)
                {
                    if (item.Value == null || item.Value == 0)
                        continue;

                    if (tmp.ContainsKey((ulong)item.Value) == false)
                    {
                        tmp.Add((ulong)item.Value, 1);
                    }
                    else
                    {
                        tmp[(ulong)item.Value] += 1;
                    }
                }

                if (tmp.Count > 0)
                {
                    var _vs = tmp.First(_sc => _sc.Value == tmp.Max(item => item.Value));
                    BestScore = _vs.Key;
                    var _bestScore = Scores.FirstOrDefault(_ => _.Value == _vs.Key & _.ImageType == ImageTypes.Positive);
                    if (_bestScore == null)
                    {
                        _bestScore = Scores.First(_ => _.Value == _vs.Key);
                    }

                    AccuracyScore = (float)Math.Round(_vs.Value / (double)Scores.Count, 3);

                    using var img = System.Drawing.Image.FromFile(_bestScore.FileName);
                    BestScoreImage = ImageFunctions.ImageFromBuffer(ImageFunctions.CropImage(
                        img,
                        V.us.RectEventScores.X1,
                        _bestScore.Y1 - 5,
                        V.us.RectEventScores.X2,
                        _bestScore.Y2 + 5,
                        System.Drawing.Imaging.ImageFormat.Png));
                }
                else
                {
                    using var img = System.Drawing.Image.FromFile(EventListName.FileName);
                    BestScoreImage = ImageFunctions.ImageFromBuffer(ImageFunctions.CropImage(
                        img,
                        V.us.RectEventScores.X1,
                        EventListName.Y1 - 5,
                        V.us.RectEventScores.X2,
                        EventListName.Y2 + 5,
                        System.Drawing.Imaging.ImageFormat.Png));

                    AccuracyScore = 0;
                }
            }
            else
            {
                BestScore = null;
                AccuracyScore = null;
            }
        }
        private void MergePowerData()
        {
            if (Powers != null && Powers.Count > 0)
            {
                Dictionary<ulong, int> tmp = new();

                foreach (var item in Powers)
                {
                    if (item.Value == null || item.Value == 0)
                        continue;

                    if (tmp.ContainsKey((ulong)item.Value) == false)
                    {
                        tmp.Add((ulong)item.Value, 1);
                    }
                    else
                    {
                        tmp[(ulong)item.Value] += 1;
                    }
                }

                if (tmp.Count > 0)
                {
                    var _vs = tmp.First(_sc => _sc.Value == tmp.Max(item => item.Value));
                    BestPower = _vs.Key;
                    var _bestPower = Powers.FirstOrDefault(_ => _.Value == _vs.Key & _.ImageType == ImageTypes.Positive);
                    if (_bestPower == null)
                    {
                        _bestPower = Powers.First(_ => _.Value == _vs.Key);
                    }
                    AccuracyPower = (float)Math.Round(_vs.Value / (double)Powers.Count, 3);

                    using var img = System.Drawing.Image.FromFile(_bestPower.FileName);
                    BestPowerImage = ImageFunctions.ImageFromBuffer(ImageFunctions.CropImage(
                        img,
                        V.us.RectAlliancePower.X1,
                        _bestPower.Y1 - 5,
                        V.us.RectAlliancePower.X2,
                        _bestPower.Y2 + 5,
                        System.Drawing.Imaging.ImageFormat.Png));
                }
                else
                {
                    using var img = System.Drawing.Image.FromFile(Name.FileName);
                    BestPowerImage = ImageFunctions.ImageFromBuffer(ImageFunctions.CropImage(
                        img,
                        V.us.RectAlliancePower.X1,
                        Rank.Y1,
                        V.us.RectAlliancePower.X2,
                        Name.Y2,
                        System.Drawing.Imaging.ImageFormat.Png));

                    AccuracyPower = 0;
                }
            }
            else
            {
                BestPower = null;
                AccuracyPower = null;
            }
        }


        public void ConfirmLevel()
        {
            if (BestLevel != null)
                AccuracyLevel = 1;
        }
        public void ConfirmScore()
        {
            if (BestScore != null)
                AccuracyScore = 1;
            else
                BestScore = 0;
        }
        public void ConfirmPower()
        {
            if (BestPower != null)
                AccuracyPower = 1;
        }

        private static SolidColorBrush CalcAccuracyBrush(float? value, List<AccuracyBrushLimits> limits)
        {
            if (value == null)
                return new SolidColorBrush(Color.FromRgb(236, 0, 255));

            foreach (var item in limits)
            {
                if (value >= item.Min & value <= item.Max)
                {
                    return item.Brush;
                }
            }
            return Brushes.Transparent;
        }


        public override bool Equals(object? obj)
        {
            return Equals(obj as AllianceMember);
        }

        public bool Equals(AllianceMember? other)
        {
            return other is not null &&
                   Name.Value == other.Name.Value;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Name.Value);
        }

        public override string? ToString()
        {
            return $"{Name.Value} / {Name.WC} / {Rank.Value} / {BestLevel} / {BestPower} / {PowerRanking} / {BestScore} / {EventRanking}";
        }

        public static bool operator ==(AllianceMember? left, AllianceMember? right)
        {
            return EqualityComparer<AllianceMember>.Default.Equals(left, right);
        }

        public static bool operator !=(AllianceMember? left, AllianceMember? right)
        {
            return !(left == right);
        }
    }
}