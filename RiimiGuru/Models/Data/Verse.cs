using System;

namespace RiimiGuru.Models.Data
{
    public class Verse
    {
        public Line[] lines { get; set; } // Changed property name to PascalCase for consistency

        public Verse(Line[] lines)
        {
            this.lines = lines;
        }

        public Word[] GetWords()
        {
            var words = lines
                .SelectMany(line => line.words)
                .ToArray();

            return words;
        }
    }
    }
