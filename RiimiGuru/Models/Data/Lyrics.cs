using System;

namespace RiimiGuru.Models.Data
{
    public class Lyrics
    {
        public Verse[] verses { get; set; } // Changed property name to PascalCase for consistency


        public Lyrics(Verse[] verses)
        {
            this.verses = verses;
        }

        public Word[] getAllLyrics()
        {
            var lyrics = verses
                .SelectMany(verse => verse.lines)              // Flatten the lines from each verse
                .SelectMany(line => line.words)                // Flatten the words from each line
                .ToArray();

            return lyrics;

        }
    }
}
