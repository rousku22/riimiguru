"""
Käyttötarkoitus:
- Tavuttaa annetun tekstin sanat
- Palauttaa rakenteen, jossa:
    . säkeistöt eroteltu pisteillä
    , rivit eroteltu pilkuilla
    ; sanat eroteltu puolipisteillä
    - tavut eroteltu yhdysmerkillä

Esimerkki palautemuoto:
    kis-sa;koi-ra,juok-see.hauk-kuu

Tämä rakenne mahdollistaa Lyrics → Verse → Line → Word → Syllable -rakenteen palauttamisen C#-puolelle.
"""

import sys
import os
import re
from libvoikko import Voikko

def hyphenate_words(raw_text: str, path: str) -> str:
    # 1. Asetetaan Voikko-kirjaston hakupolku
    this_file = os.path.abspath(__file__)
    base_dir = os.path.dirname(this_file)
    Voikko.setLibrarySearchPath(os.path.join(base_dir, "64"))
    v = Voikko('fi', base_dir)

    # 2. Normalisoidaan rivinvaihdot Unix-muotoon (\n)
    text = raw_text.replace("\r\n", "\n").replace("\r", "\n").strip()

    # 3. Jaetaan säkeistöt tyhjien rivien perusteella
    stanzas_in = re.split(r"\n\s*\n", text)
    stanzas_out = []

    for stanza in stanzas_in:
        # 4. Jaetaan säkeistön rivit yksittäisten rivinvaihtojen perusteella
        lines_in = stanza.strip().split("\n")
        lines_out = []

        for line in lines_in:
            # 5. Jaetaan rivi sanoihin välilyönneillä
            words = re.split(r"\s+", line.strip())
            hyph_words = []

            for w in words:
                if not w:
                    continue
                try:
                    # 6. Tavutetaan sana Voikolla
                    parts = v.hyphenate(w)

                   # print(f"DEBUG: Tavutus sanalle '{w}': {parts}", file=sys.stderr)

                    hyph_words.append("".join(parts) if parts else w)
                except Exception as e:
                    # Jos tavutus epäonnistuu, palautetaan alkuperäinen sana
                    print(f"DEBUG: Virhe tavutuksessa sanalle '{w}': {e}", file=sys.stderr) # Virhedebug
                    hyph_words.append(w)

            # 7. Rivin sanat yhdistetään puolipisteillä
            lines_out.append(";".join(hyph_words))

        # 8. Säkeistön rivit yhdistetään pilkuilla
        stanzas_out.append(",".join(lines_out))

    # 9. Koko teksti yhdistetään pisteillä säkeistöjen välillä
    return ".".join(stanzas_out)

if __name__ == "__main__":
    # Tarkistetaan komentoriviparametrien määrä
    if len(sys.argv) != 3:
        print("Usage: python voikk.py <lyrics> <voikko_root>")
        sys.exit(1)

    lyrics, voikko_root = sys.argv[1], sys.argv[2]
    print(hyphenate_words(lyrics, voikko_root))