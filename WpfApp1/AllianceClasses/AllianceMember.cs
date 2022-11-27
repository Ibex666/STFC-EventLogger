using STFC_EventLogger.MVVM;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security.Policy;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using System.Xml;

namespace STFC_EventLogger.AllianceClasses
{
    public class AllianceMember : IEquatable<AllianceMember?>, INotifyPropertyChanged
    {
        #region #- Private Fields -#

        private float? accuracyLevel;
        private float? accuracyScore;
        private float? accuracyPower;
        private uint? bestLevel;
        private ulong? bestScore;
        private ulong? bestPower;
        private SolidColorBrush accuracyLevelBrush;
        private SolidColorBrush accuracyScoreBrush;
        private SolidColorBrush accuracyPowerBrush;

        private ICommand? confirmLevelCommand;
        private ICommand? confirmPowerCommand;
        private ICommand? confirmScoreCommand;
        private int? eventRanking;
        private int? powerRanking;

        #endregion

        #region #- Constructor -#

        public AllianceMember()
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

            BestLevelImage = string.Empty;
            BestScoreImage = string.Empty;
            BestPowerImage = string.Empty;

            EventListName = new();

            PageType = PageTypes.Unknown;


            Application.Current.Dispatcher.Invoke(DispatcherPriority.Background, () =>
            {
                accuracyLevelBrush = new();
                accuracyScoreBrush = new();
                accuracyPowerBrush = new();
            });
        }
        public AllianceMember(XmlElement xml, SSTypeAnalyzer file)
        {
            Levels = new();
            Scores = new();
            Powers = new();

            Name = new OcrName(xml.SelectNodes("./TextLine[2]/String"), file);
            Rank = new OcrRank(xml.SelectSingleNode("./TextLine[1]/String"), file);
            Levels.Add(new OcrLevel(xml.SelectSingleNode("./TextLine[2]/String[1]"), file));

            BestLevel = new();
            BestScore = new();
            BestPower = new();

            accuracyLevelBrush = new();
            accuracyScoreBrush = new();
            accuracyPowerBrush = new();

            BestLevelImage = string.Empty;
            BestScoreImage = string.Empty;
            BestPowerImage = string.Empty;

            EventListName = new();

            PageType = file.PageType;


            Application.Current.Dispatcher.Invoke(DispatcherPriority.Background, () =>
            {
                accuracyLevelBrush = new();
                accuracyScoreBrush = new();
                accuracyPowerBrush = new();
            });
        }
        public AllianceMember(AllianceListEntry entry)
        {
            Levels = new();
            Scores = new();
            Powers = new();

            Name = entry.Name;
            Levels.AddRange(entry.Levels);
            Powers.AddRange(entry.Powers);
            Rank = new OcrRank();

            BestLevel = new();
            BestScore = new();
            BestPower = new();

            accuracyLevelBrush = new();
            accuracyScoreBrush = new();
            accuracyPowerBrush = new();

            BestLevelImage = string.Empty;
            BestScoreImage = string.Empty;
            BestPowerImage = string.Empty;

            EventListName = new();

            PageType = entry.File.PageType;


            Application.Current.Dispatcher.Invoke(DispatcherPriority.Background, () =>
            {
                accuracyLevelBrush = new();
                accuracyScoreBrush = new();
                accuracyPowerBrush = new();
            });
        }
        public AllianceMember(EventListEntry entry)
        {
            Levels = new();
            Scores = new();
            Powers = new();

            Name = entry.Name;
            EventListName = entry.Name;
            Scores.AddRange(entry.Scores);
            Rank = new OcrRank();

            BestLevel = new();
            BestScore = new();
            BestPower = new();

            accuracyLevelBrush = new();
            accuracyScoreBrush = new();
            accuracyPowerBrush = new();

            BestLevelImage = string.Empty;
            BestScoreImage = string.Empty;
            BestPowerImage = string.Empty;

            PageType = entry.File.PageType;


            Application.Current.Dispatcher.Invoke(DispatcherPriority.Background, () =>
            {
                accuracyLevelBrush = new();
                accuracyScoreBrush = new();
                accuracyPowerBrush = new();
            });
        }

        #endregion

        #region #- Public Properties -#

        public OcrName Name { get; set; }
        public OcrName EventListName { get; set; }
        public OcrRank Rank { get; set; }
        public uint? BestLevel
        {
            get => bestLevel;
            set
            {
                if (value != bestLevel)
                {
                    bestLevel = value;
                    AccuracyLevel = 1;
                    OnPropertyChanged();
                    BestLevelChanged?.Invoke(this, new EventArgs());
                }
            }
        }
        public ulong? BestScore
        {
            get => bestScore;
            set
            {
                if (value != bestScore)
                {
                    bestScore = value;
                    AccuracyScore = 1;
                    OnPropertyChanged();
                    BestScoreChanged?.Invoke(this, new EventArgs());
                }
            }
        }
        public ulong? BestPower
        {
            get => bestPower;
            set
            {
                if (value != bestPower)
                {
                    bestPower = value;
                    AccuracyPower = 1;
                    OnPropertyChanged();
                    BestPowerChanged?.Invoke(this, new EventArgs());
                }
            }
        }
        public string BestLevelImage { get; set; }
        public string BestScoreImage { get; set; }
        public string BestPowerImage { get; set; }

        public float? AccuracyLevel
        {
            get => accuracyLevel;
            private set
            {
                accuracyLevel = value;
                OnPropertyChanged();

                Application.Current.Dispatcher.Invoke(DispatcherPriority.Background, () => AccuracyLevelBrush = CalcAccuracyBrush(value, V.allianceLeaderBoard.SelectedUserConfig.AccuracyLevelBrushLimits));
            }
        }
        public float? AccuracyScore
        {
            get => accuracyScore;
            private set
            {
                accuracyScore = value;
                OnPropertyChanged();

                Application.Current.Dispatcher.Invoke(DispatcherPriority.Background, () => AccuracyScoreBrush = CalcAccuracyBrush(value, V.allianceLeaderBoard.SelectedUserConfig.AccuracyScoreBrushLimits));
            }
        }
        public float? AccuracyPower
        {
            get => accuracyPower;
            private set
            {
                accuracyPower = value;
                OnPropertyChanged();

                Application.Current.Dispatcher.Invoke(DispatcherPriority.Background, () => AccuracyPowerBrush = CalcAccuracyBrush(value, V.allianceLeaderBoard.SelectedUserConfig.AccuracyPowerBrushLimits));
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

        public int? PowerRanking
        {
            get => powerRanking; set
            {
                powerRanking = value;
                OnPropertyChanged();
            }
        }
        public int? EventRanking
        {
            get => eventRanking; set
            {
                eventRanking = value;
                OnPropertyChanged();
            }
        }

        public string NameColumnTooltip
        {
            get
            {
                return
                    "Alliance:" +
                    Environment.NewLine +
                    $"{Name.Value} / {Name.Content} / {Name.WC,0:p1}" +
                    Environment.NewLine +
                    Environment.NewLine +
                    "Event:" +
                    Environment.NewLine +
                    $"{EventListName.Value} / {EventListName.Content} / {EventListName.WC,0:p1}";
            }
        }

        public ICommand ConfirmLevelCommand
        {
            get
            {
                confirmLevelCommand ??= new RelayCommand(
                        p => BestLevel != null && BestLevel > 0 && AccuracyLevel < 1,
                        p => ConfirmLevel());
                return confirmLevelCommand;
            }
        }
        public ICommand ConfirmPowerCommand
        {
            get
            {
                confirmPowerCommand ??= new RelayCommand(
                        p => BestPower != null && BestPower > 0 && AccuracyPower < 1,
                        p => ConfirmPower());
                return confirmPowerCommand;
            }
        }
        public ICommand ConfirmScoreCommand
        {
            get
            {
                confirmScoreCommand ??= new RelayCommand(
                        p => BestScore != null && AccuracyScore < 1,
                        p => ConfirmScore());
                return confirmScoreCommand;
            }
        }

        public PageTypes PageType { get; set; }

        #endregion

        #region #- Events -#

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string propertyName = "") =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

        public event EventHandler? BestLevelChanged;
        public event EventHandler? BestScoreChanged;
        public event EventHandler? BestPowerChanged;

        #endregion

        #region #- Instance Methods -#

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

                    BestLevelImage = ImageFunctions.CropImage(
                        _bestLevel.FileName,
                        _bestLevel.X1 - 5,
                        _bestLevel.Y1 - 5,
                        _bestLevel.X2 + 5,
                        _bestLevel.Y2 + 5,
                        System.Drawing.Imaging.ImageFormat.Png);
                }
                else
                {
                    if (Levels.Count > 0)
                    {
                        BestLevelImage = ImageFunctions.CropImage(
                                Name.FileName,
                                Levels[0].X1 - 5,
                                Levels[0].Y1 - 5,
                                Levels[0].X2 + 5,
                                Levels[0].Y2 + 5,
                                System.Drawing.Imaging.ImageFormat.Png);
                    }
                    else
                    {
                        BestLevelImage = ImageFunctions.CropImage(
                                    Name.FileName,
                                    V.allianceLeaderBoard.SelectedUserConfig.RectAllianceNames.Start.X,
                                    Name.Y1 - 5,
                                    V.allianceLeaderBoard.SelectedUserConfig.RectAllianceNames.End.X,
                                    Name.Y2 + 5,
                                    System.Drawing.Imaging.ImageFormat.Png);
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

                    BestScoreImage = ImageFunctions.CropImage(
                                    _bestScore.FileName,
                                    V.allianceLeaderBoard.SelectedUserConfig.RectEventScores.Start.X,
                                    _bestScore.Y1 - 5,
                                    V.allianceLeaderBoard.SelectedUserConfig.RectEventScores.End.X,
                                    _bestScore.Y2 + 5,
                                    System.Drawing.Imaging.ImageFormat.Png);
                }
                else
                {
                    BestScoreImage = ImageFunctions.CropImage(
                                        EventListName.FileName,
                                        V.allianceLeaderBoard.SelectedUserConfig.RectEventScores.Start.X,
                                        EventListName.Y1 - 5,
                                        V.allianceLeaderBoard.SelectedUserConfig.RectEventScores.End.X,
                                        EventListName.Y2 + 5,
                                        System.Drawing.Imaging.ImageFormat.Png);

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

                    BestPowerImage = ImageFunctions.CropImage(
                        _bestPower.FileName,
                        V.allianceLeaderBoard.SelectedUserConfig.RectAlliancePower.Start.X,
                        _bestPower.Y1 - 5,
                        V.allianceLeaderBoard.SelectedUserConfig.RectAlliancePower.End.X,
                        _bestPower.Y2 + 5,
                        System.Drawing.Imaging.ImageFormat.Png);
                }
                else
                {
                    BestPowerImage = ImageFunctions.CropImage(
                        Name.FileName,
                        V.allianceLeaderBoard.SelectedUserConfig.RectAlliancePower.Start.X,
                        Rank.Y1,
                        V.allianceLeaderBoard.SelectedUserConfig.RectAlliancePower.End.X,
                        Name.Y2,
                        System.Drawing.Imaging.ImageFormat.Png);

                    AccuracyPower = 0;
                }
            }
            else
            {
                BestPower = null;
                AccuracyPower = null;
            }
        }

        private void ConfirmLevel()
        {
            AccuracyLevel = 1;
        }
        private void ConfirmPower()
        {
            AccuracyPower = 1;
        }
        private void ConfirmScore()
        {
            AccuracyScore = 1;
        }

        #endregion

        #region #- Static Methods -#

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

        #endregion

        #region #- Interface/Overridden Methods -#

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

        #endregion

        #region #- Operators -#

        public static bool operator ==(AllianceMember? left, AllianceMember? right)
        {
            return EqualityComparer<AllianceMember>.Default.Equals(left, right);
        }
        public static bool operator !=(AllianceMember? left, AllianceMember? right)
        {
            return !(left == right);
        }

        #endregion
    }
}