using System;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Threading;
using RiimiGuru.Interfaces;
using RiimiGuru.Models;
using RiimiGuru.Services;

namespace RiimiGuru
{
    public partial class MainWindow : Window
    {
        private DispatcherTimer _updateTimer;
        private readonly RhymingProcessor _rhymingProcessor;
        private readonly TextAnalyzer _textAnalyzer;
        private readonly ScrollPositionManager _scrollPositionManager;

        private const double ExpandedWidth = 1600;
        private const double CollapsedWidth = 1200;
        private const double ExpanderWidth = 15;
        private GridLength _originalExpanderColumnWidth;

        public MainWindow()
        {
            InitializeComponent();
            InitializeUpdateTimer();

            _originalExpanderColumnWidth = new GridLength(0, GridUnitType.Star);
            ExpanderColumnRight.Width = _originalExpanderColumnWidth;

            string baseDir = AppDomain.CurrentDomain.BaseDirectory;

            string scriptPath = Path.Combine(baseDir, "Voikko", "voikk.py");

            _rhymingProcessor = new RhymingProcessor(scriptPath);
            _textAnalyzer = new TextAnalyzer(_rhymingProcessor, riimiRyhmaPuu);
            _scrollPositionManager = new ScrollPositionManager();

        }

        private void InitializeUpdateTimer()
        {
            _updateTimer = new DispatcherTimer
            {
                Interval = TimeSpan.FromSeconds(2)
            };
            _updateTimer.Tick += OnUpdateTimerTick;
            _updateTimer.Start();
        }

        private void OnUpdateTimerTick(object sender, EventArgs e)
        {
            UpdateTextWithRhymes();
        }

        private void UpdateTextWithRhymes()
        {
            _scrollPositionManager.SaveScrollPosition(writing);

            string lyricsText = ExtractTextFromRichTextBox(writing);
            _textAnalyzer.ProcessAndDisplayText(
                lyricsText,
                writing_Copy,
                useAllSyllablesOption.IsChecked == true,
                backgroundColorOption.IsChecked == true
            );

            _scrollPositionManager.RestoreScrollPosition(writing);
        }

        private string ExtractTextFromRichTextBox(RichTextBox richTextBox)
        {
            return new TextRange(richTextBox.Document.ContentStart, richTextBox.Document.ContentEnd).Text;
        }

        private void AdjustColumnWidths(double leftExpanderStars, double rightExpanderStars)
        {
            double totalStars = MainGrid.ColumnDefinitions
                .Where(c => c.Width.IsStar)
                .Sum(c => ((GridLength)c.Width).Value);

            double remainingStars = totalStars - (leftExpanderStars + rightExpanderStars);
            double scaleFactor = remainingStars / (totalStars - (leftExpanderStars + rightExpanderStars));

            foreach (var column in MainGrid.ColumnDefinitions)
            {
                if (column != ExpanderColumnLeft && column != ExpanderColumnRight && column.Width.IsStar)
                {
                    column.Width = new GridLength(((GridLength)column.Width).Value * scaleFactor, GridUnitType.Star);
                }
            }

            ExpanderColumnLeft.Width = new GridLength(leftExpanderStars, GridUnitType.Star);
            ExpanderColumnRight.Width = new GridLength(rightExpanderStars, GridUnitType.Star);
        }

        private void UpdateExpanderWidths()
        {
            double leftStars = LogExpanderLeft.IsExpanded ? ExpanderWidth : 0;
            double rightStars = LogExpanderRight.IsExpanded ? ExpanderWidth : 0;
            AdjustColumnWidths(leftStars, rightStars);
        }

        private void LogExpanderLeft_Expanded(object sender, RoutedEventArgs e) => UpdateExpanderWidths();
        private void LogExpanderLeft_Collapsed(object sender, RoutedEventArgs e) => UpdateExpanderWidths();
        private void LogExpanderRight_Expanded(object sender, RoutedEventArgs e) => UpdateExpanderWidths();
        private void LogExpanderRight_Collapsed(object sender, RoutedEventArgs e) => UpdateExpanderWidths();

        private ScrollViewer GetScrollViewer(DependencyObject element)
        {
            if (element is ScrollViewer viewer) return viewer;

            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(element); i++)
            {
                var child = VisualTreeHelper.GetChild(element, i);
                var result = GetScrollViewer(child);
                if (result != null) return result;
            }

            return null;
        }

        private void updateButton_Click(object sender, RoutedEventArgs e)
        {
            UpdateTextWithRhymes();
        }
    }
}
