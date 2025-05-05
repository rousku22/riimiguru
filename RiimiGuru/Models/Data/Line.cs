using RiimiGuru.Interfaces;
using System;
using System.Windows.Shapes;

namespace RiimiGuru.Models.Data
{
    public class Line
    {
        public Word[] words { get; set; } // Changed property name to PascalCase for consistency

        public Line(Word[] words)
        {
            this.words = words;
        }

        public int getSyllableCount(Word[] wordArray)
        {
            int syllableCount = 0;
            foreach (var word in wordArray)
            {
                syllableCount += word.getSyllableCount();
            }
            return syllableCount;
        }

    }
}
