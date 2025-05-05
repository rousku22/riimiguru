# RiimiGuru
WPF-sovellus suomenkielisten sanoitusten tavuttamiseen ja riimianalyysiin.

## Sisällysluettelo

- [Asennus](#asennus)
- [Käyttö](#käyttö)

## Asennus

1. Kopioi tai kloonaa repositorio:
   ```bash
   git clone https://github.com/rousku22/riimiguru.git
   cd riimiguru
   ```
2. Asenna Python-riippuvuudet:
   ```bash
   pip3 install libvoikko
   ```
3. Rakenna C#-projekti Visual Studio Codessa tai Visual Studiossa.

## Käyttö

**WPF-sovellus:**
1. Aja `RiimiGuru.sln` Visual Studiossa.
2. Syötä sanat pääikkunan tekstikenttään.
3. Valitse asetukset (käytä kaikkia tavuja, taustaväri).

## Esimerkki käytöstä

![Esimerkki käytöstä](docs/example.png)

## Tulevaisuuden kehityssuunnitelmat

Seuraavia ominaisuuksia on suunnitteilla ja kehitettävää:

- **Riimityyppien tunnistus**:
  - Puhtaat riimit (täydellinen yhtäläisyys ensimmäisestä riimitavun vokaalista loppuun, vokaalia edeltävä konsonantti on erilainen ja tavumäärien parillisuus/pairittomuus huomioidaan).
  - Epäpuhtaat riimit:
    - Identtiset riimit (täydellinen yhtäläisyys, myös alkukonsonantit).
    - Vokaaliriimit (samat vokaalit samoissa kohdissa, konsonantit voivat vaihdella).
    - Konsonanttiriimit (samat konsonantit samoissa kohdissa, vokaalien määrä vakio).
    - Lavennetut/supistetut riimit (N-kirjaimen lisäys tai poisto viimeiseltä tavulta).
  - *Huom:* Puhtaan riimiparin on lähdettävä painottomalta tavulta; jos näin ei ole, näytetään varoitus "RIIMIPARI LÄHTEE PAINOTTOMALTA TAVULTA JA ON SIIS EPÄPUHDAS !!".

- **Riimien visualisointi**:
  - Kuinka moni­tavuinen riimi kullakin riimiparilla on (esim. tavuloogien yhteenveto).
  - Graafiset esitykset riimityypeistä ja -ryhmistä.

- **Numeerinen lyriikan analyysi**:
  - Tavu-, sana- ja riimitilastot (esim. tavutiheys, riimitiheys per säe).
  - Sanapituisuuksien jakauma, sanastoanalyysi.

