using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RiimiGuru.Models.Data
{
    public class Word
    {
        public string getString { get; private set; }
        public Syllable[] getSyllables { get; private set; }

        public Word(string WordTeksti, Syllable[] tavut)
        {
            getString = WordTeksti;
            getSyllables = tavut ?? Array.Empty<Syllable>();
        }

        public int getSyllableCount() {  return this.getSyllables.Length; }
    }
}
