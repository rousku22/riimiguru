using RiimiGuru.Models.Data;

using System.Windows.Controls;

/// <summary>
/// RhymingProcessor käsittelee sanoituksen rakenteen ja riimit.
/// 
/// - Tavuttaa sanoituksen käyttämällä Python-skriptiä (Voikko-kirjasto)
/// - Muuntaa tuloksen Lyrics-rakenteeksi: Lyrics → Verse → Line → Word → Syllable
/// - Ryhmittelee sanat niiden tavujen perusteella riimiryhmiin
/// 
/// Tätä luokkaa käyttää TextAnalyzer, joka vastaa tuloksen näyttämisestä käyttöliittymässä.
/// </summary>
public class RhymingProcessor
{
    private readonly string _scriptPath;

    // Konstruktori, joka ottaa Python-skriptin polun ja Voikko-kirjaston polun
    public RhymingProcessor(string pythonScriptPath)
    {
        _scriptPath = pythonScriptPath;
    }

    // Tavuttaa sanoituksen ja rakentaa Lyrics-olion säkeistö-, rivi- ja sanarakenteineen
    public Lyrics ProcessLyrics(string lyrics)
    {
        var scriptRunner = new PythonScriptRunner(_scriptPath);
        var verseStrings = scriptRunner.HyphenateWord(CleanLyrics(lyrics)).Split(new[] { '.' }, StringSplitOptions.RemoveEmptyEntries);

        var verses = verseStrings
            .Select(vs => new Verse(
                vs.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                  .Select(ls => new Line(
                      ls.Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries)
                        .Select(ws => new Word(
                            ws.Replace("-", ""), // alkuperäinen sana ilman tavuviivoja
                            ws.Split(new[] { '-' }, StringSplitOptions.RemoveEmptyEntries)
                              .Select(t => new Syllable(t))
                              .ToArray()))
                        .ToArray()))
                  .ToArray()))
            .ToArray();

        return new Lyrics(verses);
    }

    // Poistaa tekstistä tarpeettomat merkit ennen käsittelyä
    private string CleanLyrics(string lyrics)
    {
        return lyrics.Replace(",", "")
                     .Replace("!", "");
    }

    // Ryhmittelee sanat viimeisen tai kaikkien tavujen perusteella
    public Dictionary<HashSet<string>, HashSet<string>> GroupRhymesBySyllables(Word[] words, bool everySyllable = false)
    {
        var syllableGroups = words
            .SelectMany(word => everySyllable ? word.getSyllables : new[] { word.getSyllables.Last() },
                        (word, syllable) => new { word, syllable = syllable.getString.ToLower() })
            .GroupBy(ws => ws.syllable)
            .ToDictionary(g => g.Key, g => g.Select(ws => ws.word).ToList());

        var rhymingGroups = new Dictionary<HashSet<string>, HashSet<string>>();
        var processedKeys = new HashSet<string>();

        foreach (var key in syllableGroups.Keys)
        {
            if (processedKeys.Contains(key)) continue;

            var rhymingSyllables = new HashSet<string> { key };
            var unprocessedSyllables = new Queue<string>();
            unprocessedSyllables.Enqueue(key);

            while (unprocessedSyllables.Count > 0)
            {
                var currentKey = unprocessedSyllables.Dequeue();
                processedKeys.Add(currentKey);

                if (!rhymingGroups.ContainsKey(rhymingSyllables))
                {
                    rhymingGroups[rhymingSyllables] = new HashSet<string>();
                }

                foreach (var word in syllableGroups[currentKey])
                {
                    rhymingGroups[rhymingSyllables].Add(word.getString.ToLower());
                }

                foreach (var otherKey in syllableGroups.Keys)
                {
                    if (key != otherKey && !processedKeys.Contains(otherKey) && DetermineRhymeType(key, otherKey) == RhymeType.Perfect)
                    {
                        unprocessedSyllables.Enqueue(otherKey);
                        rhymingSyllables.Add(otherKey);

                        foreach (var word in syllableGroups[otherKey])
                        {
                            rhymingGroups[rhymingSyllables].Add(word.getString.ToLower());
                        }
                    }
                }
            }
        }

        return rhymingGroups.Where(group => group.Value.Count > 1)
                            .ToDictionary(group => group.Key, group => group.Value);
    }

    // Riimityypit: täydellinen, vokaali-, konsonantti-, supistettu/lavennettu, ei riimiä
    public enum RhymeType
    {
        Perfect,
        VowelRhyme,
        ConsonantRhyme,
        SupressedOrExtended,
        None
    }

    // Päättelee kahden tavun riimityypin
    public RhymeType DetermineRhymeType(string tavu1, string tavu2)
    {
        var (vowels1, consonants1) = SplitVowelsAndConsonants(tavu1);
        var (vowels2, consonants2) = SplitVowelsAndConsonants(tavu2);

        if (IsPerfectRhyme(tavu1, tavu2)) return RhymeType.Perfect;
        if (IsVowelRhyme(vowels1, vowels2, consonants1, consonants2)) return RhymeType.VowelRhyme;
        if (IsConsonantRhyme(consonants1, consonants2, vowels1.Length, vowels2.Length)) return RhymeType.ConsonantRhyme;
        if (IsSupressedOrExtended(tavu1, tavu2, vowels1.Length, vowels2.Length)) return RhymeType.SupressedOrExtended;

        return RhymeType.None;
    }

    // Täydellinen riimi: sama vokaalialku ja identtinen loppu
    private bool IsPerfectRhyme(string tavu1, string tavu2)
    {
        var (vowelIndex1, consonantsBeforeVowel1) = GetVowelAndConsonantsBefore(tavu1);
        var (vowelIndex2, consonantsBeforeVowel2) = GetVowelAndConsonantsBefore(tavu2);

        if (vowelIndex1 == -1 || vowelIndex2 == -1 || tavu1[vowelIndex1] != tavu2[vowelIndex2]) return false;
        if (tavu1.Substring(vowelIndex1) != tavu2.Substring(vowelIndex2)) return false;

        return !consonantsBeforeVowel1.Equals(consonantsBeforeVowel2, StringComparison.OrdinalIgnoreCase);
    }

    // Jakaa tavun vokaaleihin ja konsonantteihin
    private (string vowels, string consonants) SplitVowelsAndConsonants(string tavu)
    {
        var vowels = string.Empty;
        var consonants = string.Empty;

        foreach (var c in tavu.ToLower())
        {
            if ("aeiouyäö".Contains(c)) vowels += c;
            else if (char.IsLetter(c)) consonants += c;
        }

        return (vowels, consonants);
    }

    // Vokaaliriimi: samat vokaalit samassa järjestyksessä
    private bool IsVowelRhyme(string vowels1, string vowels2, string consonants1, string consonants2)
    {
        if (vowels1.Length != vowels2.Length) return false;

        for (int i = 0; i < vowels1.Length; i++)
            if (vowels1[i] != vowels2[i]) return false;

        return true;
    }

    // Konsonanttiriimi: samat konsonantit samoissa paikoissa, vokaalit voivat vaihdella
    private bool IsConsonantRhyme(string consonants1, string consonants2, int vowelsCount1, int vowelsCount2)
    {
        if (consonants1.Length != consonants2.Length || vowelsCount1 != vowelsCount2) return false;

        for (int i = 0; i < consonants1.Length; i++)
            if (consonants1[i] != consonants2[i]) return false;

        return true;
    }

    // Supistettu/lavennettu riimi: tavut ovat lähes samanlaisia, mutta lopussa eroa
    private bool IsSupressedOrExtended(string tavu1, string tavu2, int vowelsCount1, int vowelsCount2)
    {
        if (vowelsCount1 != vowelsCount2) return false;

        bool tavu1EndsWithN = tavu1.EndsWith("n", StringComparison.OrdinalIgnoreCase);
        bool tavu2EndsWithN = tavu2.EndsWith("n", StringComparison.OrdinalIgnoreCase);

        if (tavu1.Substring(0, tavu1.Length - 1) == tavu2.Substring(0, tavu2.Length - 1))
        {
            return (tavu1EndsWithN && !tavu2EndsWithN) || (!tavu1EndsWithN && tavu2EndsWithN);
        }

        return false;
    }

    // Etsii tavun ensimmäisen vokaalin ja sitä edeltävät konsonantit
    private (int vowelIndex, string consonantsBeforeVowel) GetVowelAndConsonantsBefore(string tavu)
    {
        int vowelIndex = tavu.IndexOfAny(new[] { 'a', 'e', 'i', 'o', 'u', 'y', 'ä', 'ö' });
        if (vowelIndex == -1) return (-1, string.Empty);

        string consonantsBeforeVowel = tavu.Substring(0, vowelIndex);
         return (vowelIndex, consonantsBeforeVowel);
    }
}