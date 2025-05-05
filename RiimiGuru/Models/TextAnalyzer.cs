using RiimiGuru.Models.Data;
using System.Windows.Controls;
using System.Windows.Documents;

using System.Windows.Media;

/// <summary>
/// TextAnalyzer vastaa koko riimianalyysin ohjauksesta käyttöliittymän ja analytiikkalogiikan välillä.
///
/// - Kutsuu RhymingProcessor-luokkaa saadakseen Lyrics-olion
/// - Visualisoi riimirymmät
///     - Näyttää säkeistöt RichTextBox-komponentissa (tavut korostettu värein)
///     - Lisää riimiryhmät TreeView-komponenttiin hierarkkisena listauksena

/// </summary>
public class TextAnalyzer
{
    private readonly RhymingProcessor _rhymingProcessor;
    private readonly TreeView _treeView;

    public TextAnalyzer(RhymingProcessor rhymingProcessor, TreeView treeView)
    {
        _rhymingProcessor = rhymingProcessor ?? throw new ArgumentNullException(nameof(rhymingProcessor));
        _treeView = treeView ?? throw new ArgumentNullException(nameof(treeView));
    }

    // Käsittelee tekstin: tavuttaa, tunnistaa riimit, näyttää RichTextBoxissa ja lisää TreeViewiin
    public void ProcessAndDisplayText(string lyrics, RichTextBox targetRichTextBox, bool analyzeAllSyllables, bool useBackground)
    {
        var allLyricsProcessed = _rhymingProcessor.ProcessLyrics(lyrics);
        var verses = allLyricsProcessed.verses;

        try
        {
            targetRichTextBox.Document.Blocks.Clear();

            foreach (var verse in verses)
            {
                var verseRhymes = _rhymingProcessor.GroupRhymesBySyllables(verse.GetWords(), analyzeAllSyllables);

                ProcessAndDisplayVerse(verse, targetRichTextBox, analyzeAllSyllables, useBackground, verseRhymes, useBackground);
                targetRichTextBox.Document.Blocks.Add(new Paragraph { LineHeight = 6 });
                PopulateTreeView(verseRhymes);
            }

        }
        catch (Exception ex)
        {
        }
    }

    // Käsittelee yhden säkeistön ja lisää sen värjätyt rivit RichTextBoxiin
    private void ProcessAndDisplayVerse(Verse verse, RichTextBox targetRichTextBox, bool analyzeAllSyllables, bool useBackground, Dictionary<HashSet<string>, HashSet<string>> verseRhymes, bool useBackGround)
    {
        try
        {
            var coloredSyllablesLines = MarkRhymingSyllables(verseRhymes, verse, analyzeAllSyllables, useBackGround);
            var lines = verse.lines;

            // Käsittelee jokaisen rivin säkeistössä

            for (int i = 0; i < coloredSyllablesLines.Count; i++)
            {
                var paragraph = new Paragraph();
                targetRichTextBox.Document.Blocks.Add(coloredSyllablesLines[i]);

            }

        }
        catch (Exception ex)
        {
        }
    }

    // Näyttää rivin tavumäärän (esimerkiksi [4])
    private Run LineCount(RiimiGuru.Models.Data.Line line)
    {
        int syllableCount = line.getSyllableCount(line.words);

        return new Run($"[{syllableCount}] ") { Foreground = Brushes.Gray };
    }

    // Värjää sanat tai tavut niiden riimiryhmän mukaan ja palauttaa kappaleet
    private List<Paragraph> MarkRhymingSyllables(Dictionary<HashSet<string>, HashSet<string>> rhymeGroupsInVerse, Verse verse, bool analyzeAllSyllables, bool useBackground)
    {
        var rhymeGroups = new List<HashSet<string>>(rhymeGroupsInVerse.Keys);
        var styledVerse = new List<Paragraph>();

        foreach (var line in verse.lines)
        {
            var verseParagraph = new Paragraph { LineHeight = 6 };

            // Lisää tavumäärän rivin alkuun
            verseParagraph.Inlines.Add(LineCount(line));

            foreach (var word in line.words)
            {
                if (!analyzeAllSyllables)
                {
                    // Värjää koko sanan viimeisen tavun perusteella
                    var lastSyllable = word.getSyllables.Last();
                    Brush color = DetermineColor(rhymeGroups, lastSyllable.getString, useBackground);
                    var styledWord = CreateRun(word.getString, color, useBackground);
                    verseParagraph.Inlines.Add(styledWord);
                }
                else
                {
                    // Värjää jokaisen tavun erikseen
                    foreach (var syllable in word.getSyllables)
                    {
                        Brush color = DetermineColor(rhymeGroups, syllable.getString, useBackground);
                        var syllableRun = CreateRun(syllable.getString, color, useBackground);
                        verseParagraph.Inlines.Add(syllableRun);
                    }
                }

                // Lisää väli sanojen väliin
                verseParagraph.Inlines.Add(new Run(" "));
            }

            styledVerse.Add(verseParagraph);
        }

        return styledVerse;
    }

    // Päättelee värin sen perusteella kuuluuko tavu johonkin riimiryhmään
    private Brush DetermineColor(List<HashSet<string>> rhymeGroups, string text, bool useBackground)
    {
        Brush color = useBackground ? Brushes.Transparent : Brushes.White;
        foreach (var rhymeGroup in rhymeGroups)
        {
            if (rhymeGroup.Contains(text))
            {
                color = GetColorForIndex(rhymeGroups.IndexOf(rhymeGroup), useBackground);
                break;
            }
        }
        return color;
    }

    // Luo tekstielementin halutulla värityksellä
    private Run CreateRun(string text, Brush color, bool useBackground)
    {
        return new Run(text)
        {
            Background = useBackground ? color : Brushes.Transparent,
            Foreground = useBackground ? Brushes.White : color
        };
    }

    // Palauttaa väriarvon annetulle indeksille, käyttää tummaa tai vaaleaa palettia taustakäytöstä riippuen
    private SolidColorBrush GetColorForIndex(int index, bool useDarkColors)
    {
        // Tummat värit (foreground-tilaan)
        Color[] darkColors = new Color[]
        {
        Color.FromRgb(0, 125, 181),
        Color.FromRgb(255, 0, 246),
        Color.FromRgb(255, 0, 86),
        Color.FromRgb(181, 0, 255),
        Color.FromRgb(0, 118, 255),
        Color.FromRgb(120, 130, 49),
        Color.FromRgb(107, 104, 130),
        Color.FromRgb(229, 111, 254),
        Color.FromRgb(164, 36, 0),
        Color.FromRgb(0, 100, 1),
        Color.FromRgb(0, 143, 156),
        Color.FromRgb(232, 94, 190),
        Color.FromRgb(0, 185, 23),
        Color.FromRgb(106, 130, 108),
        Color.FromRgb(255, 110, 65),
        Color.FromRgb(255, 0, 0),
        Color.FromRgb(38, 52, 0),
        Color.FromRgb(0, 0, 255),
        Color.FromRgb(98, 14, 0),
        Color.FromRgb(0, 21, 68),
        Color.FromRgb(67, 0, 44),
        Color.FromRgb(1, 0, 103)
        };

        // Vaaleat värit (taustakäyttöön)
        Color[] lightColors = new Color[]
        {
        Color.FromRgb(0, 155, 255),
        Color.FromRgb(0, 174, 126),
        Color.FromRgb(254, 137, 0),
        Color.FromRgb(95, 173, 78),
        Color.FromRgb(255, 116, 163),
        Color.FromRgb(0, 255, 0),
        Color.FromRgb(194, 140, 159),
        Color.FromRgb(0, 255, 120),
        Color.FromRgb(213, 255, 0),
        Color.FromRgb(255, 229, 2),
        Color.FromRgb(152, 255, 82),
        Color.FromRgb(255, 147, 126),
        Color.FromRgb(190, 153, 112),
        Color.FromRgb(255, 166, 254),
        Color.FromRgb(255, 177, 103),
        Color.FromRgb(144, 251, 146),
        Color.FromRgb(0, 255, 198),
        Color.FromRgb(222, 255, 116),
        Color.FromRgb(1, 208, 255),
        Color.FromRgb(255, 219, 102),
        Color.FromRgb(1, 255, 254),
        Color.FromRgb(165, 255, 210),
        Color.FromRgb(189, 211, 147),
        Color.FromRgb(189, 198, 255),
        Color.FromRgb(145, 208, 203),
        Color.FromRgb(255, 238, 232)
        };

        // Valitaan käytettävä paletti
        Color[] colors = useDarkColors ? darkColors : lightColors;

        // Jos indeksit loppuvat, aloitetaan alusta
        Color color = colors[index % colors.Length];

        return new SolidColorBrush(color);
    }

    // Lisää loppusointuryhmät puunäkymään (TreeView)
    private void PopulateTreeView(Dictionary<HashSet<string>, HashSet<string>> rhymeGroups)
    {
        _treeView.Items.Clear();

        foreach (var (rhymeGroup, rhymes) in rhymeGroups)
        {
            var rhymeGroupNode = new TreeViewItem
            {
                Header = $"{string.Join(", ", rhymeGroup)}"
            };

            foreach (var rhyme in rhymes)
            {
                rhymeGroupNode.Items.Add(new TreeViewItem { Header = rhyme });
            }

            _treeView.Items.Add(rhymeGroupNode);
        }
    }
}