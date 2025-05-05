using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RiimiGuru.Models.Data
{
    public class Syllable
    {
        public string getString { get; private set; }
        public int? rhymeGroup { get; private set; } = null;


        public Syllable(string WordTeksti)
        {
            getString = WordTeksti;
            rhymeGroup = rhymeGroup;
        }

        public void setRhymeGroup(int value)
        {
            rhymeGroup = value;
        }
    }
}
