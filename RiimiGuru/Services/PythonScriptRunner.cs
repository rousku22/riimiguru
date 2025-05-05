using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Controls;

/// <summary>
/// PythonScriptRunner vastaa Python-skriptin suorittamisesta ulkoisena prosessina.
/// 
/// Skripti saa parametreina: sanat, voikko-kirjaston polku
/// Palauttaa Pythonin tulosteen, tai virheen stringinä.
/// </summary>
public class PythonScriptRunner
{
    private readonly string _scriptPath;     // Python-skriptin tiedostopolku
    private readonly string _voikkoPath;     // Voikko-kirjaston juurihakemisto

    public PythonScriptRunner(string scriptPath)
    {
        _scriptPath = scriptPath;
        _voikkoPath = Path.Combine(Path.GetDirectoryName(_scriptPath), "5");
    }

    // Suorittaa Python-skriptin annetulla sanalla ja toiminnolla
    public string RunScript(string word)
    {
        try
        {
            // Määritetään suoritettava prosessi
            ProcessStartInfo start = new ProcessStartInfo
            {
                FileName = "python", // Varmista, että "python" toimii komentoriviltä
                Arguments = $"\"{_scriptPath}\" \"{word}\" \"{_voikkoPath}\"",
                UseShellExecute = false,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                CreateNoWindow = true
            };

            using (Process process = Process.Start(start))
            {
                // Luetaan tulosteet
                string result = process.StandardOutput.ReadToEnd();
                string error = process.StandardError.ReadToEnd();
                process.WaitForExit();

                if (!string.IsNullOrEmpty(error))
                {
                    throw new Exception($"Python error: {error}");
                }

                return result.Trim(); // Palautetaan skriptin tuloste ilman rivinvaihtoja
            }
        }
        catch (Exception ex)
        {
            return $"Error: {ex.Message}";
        }
    }

    // Palauttaa sanan tavutetun version
    public string HyphenateWord(string word)
    {
        string a = RunScript(word);
        //MessageBox.Show(a); //Näytä tavutetun sanan muodot
        return a;
    }
}