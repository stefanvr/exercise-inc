# **Feedback op je plan: Sport App**

*Speed climbing- en trainingsapp in .NET MAUI: feedback, datamodel, dataopslag en een opgeschoond plan*

# **Wat goed is**

* Je kent het domein goed: het onderscheid tussen single, double en competitie is logisch en je denkt al na over statistieken (PB, gemiddelde, sessiegemiddelde).

* Je denkt vanuit schermen en gebruikersflow, en dat is een goed startpunt.

* Je hebt ideeën voor de lange termijn (grafieken, sortering), die je kunt bewaren voor later.

# **Wat ik als docent zou aanpassen**

## **1\. Bepaal eerst wat de MVP is**

Je prioriteiten 1 t/m 17 staan er wel, maar zonder inhoud. Koppel ze aan features, bijvoorbeeld met MoSCoW (Must/Should/Could/Won't). Mijn voorstel voor de eerste versie: single-sessies loggen plus basisstatistieken. Daarna volgen double, competitie en krachttraining. Grafieken komen als laatste.

## **2\. Maak je begrippen exact**

"PB", "average" en "session average" komen er meerdere keren in voor, en niet overal is duidelijk wat ze betekenen. Leg ze één keer vast:

* *PB* \= snelste geldige poging.

* *Average* \= gemiddelde van alle geldige pogingen. Bepaal of een DNF/DNS meetelt (ik zou zeggen van niet).

* *Session average* \= gemiddelde binnen één sessie.

* Bij double: is de tijd de som van beide runs, of tel je ze apart? Dat moet je definiëren voordat je kunt bouwen.

## **3\. Voorkom dubbel werk**

Je beschrijft single en double bijna identiek, en krachttraining en calisthenics ook. Bouw dus één generiek ontwerp met een parameter (RunType \= Single | Double, ExerciseType \= Strength | Calisthenics). Dat scheelt je veel code en bugs.

## **4\. Let op gelijktijdige sessies**

Je noteert dat een single- en een double-sessie tegelijk kunnen lopen. Dat heeft invloed op je architectuur: je hebt een sessie-service nodig die meerdere actieve sessies bijhoudt, niet één globale "huidige sessie".

## **5\. Ontwerp eerst je datamodel**

Mijn voorstel:

| Entiteit | Belangrijkste velden |
| :---- | :---- |
| Session | Id, Type (Single/Double), StartUtc, EndUtc, Notes |
| Attempt | Id, SessionId, TimeMs (int?), Status (Valid/DNF/DNS), TimeMs2 (voor double), Notes, TimestampUtc |
| Competition | Id, DateUtc, Placement, Notes |
| CompetitionAttempt | Id, CompetitionId, TimeMs?, Status |
| ExerciseDefinition | Id, Name, Type, UsesKg, UsesSeconds |
| ExerciseLog | Id, ExerciseId, DateUtc |
| SetEntry | Id, ExerciseLogId, Reps, Kg?, Seconds? |

Twee tips hierbij:

* Sla tijden op als **integer milliseconden**, niet als double of string.

* Sla **geen afgeleide statistieken** op (PB, gemiddelden). Bereken ze uit de pogingen, dan kunnen ze nooit uit sync raken met je data.

## **6\. Architectuur voor .NET MAUI**

Gebruik MVVM met CommunityToolkit.Mvvm, dependency injection, en een aparte service/repository-laag. Zet de statistiekberekeningen in een losse klasse die je met xUnit kunt testen. Dat is ook goed bewijs voor je opleiding.

## **7\. Taal**

Voor de Engelstalige app: gebruik .resx\-resourcebestanden voor alle UI-teksten en hardcode ze nergens. Zo kun je later makkelijk Nederlands toevoegen. Je plan bevat nu spelfouten (statestiek, avarage, Priotiteid), dus loop de Engelse termen één keer netjes na.

# **Data veilig opslaan**

## **Aanbevolen: SQLite in de privémap van de app**

* Gebruik sqlite-net-pcl (simpel) of EF Core met SQLite (meer structuur, migraties).

* Bewaar het bestand in FileSystem.AppDataDirectory. Dat is de sandbox van je app, die andere apps niet kunnen lezen.

* Gebruik **geen** Preferences of losse JSON-bestanden voor je hoofddata. Die zijn niet gemaakt voor relationele data en je krijgt geen transacties.

## **Betrouwbaarheid is belangrijker dan beveiliging bij dit type app**

* **Sla elke poging direct op** in de database, niet pas bij het afronden van de sessie. Dan verlies je niets als de app crasht of de batterij leeg raakt.

* Gebruik transacties bij het opslaan van een heel setje (bijvoorbeeld een compleet krachttrainingslog).

* Voeg een **schemaversie of migraties** toe, zodat updates je bestaande data niet breken.

* Zorg voor **back-up en export**: een export naar JSON of CSV via de deelfunctie van MAUI (Share). Het grootste risico is een kwijtgeraakte of kapotte telefoon, niet hackers.

## **Versleuteling (optioneel)**

Je trainingsdata is weinig gevoelig, dus de sandbox is voor een eerste versie voldoende. Wil je het toch, gebruik dan SQLCipher (SQLitePCLRaw.bundle\_e\_sqlcipher) en bewaar de sleutel in SecureStorage (Android Keystore / iOS Keychain). Nooit de sleutel hardcoden.

## **Als je later cloud-sync wilt**

Gebruik een backend met authenticatie (bijvoorbeeld Supabase, Firebase of Azure), alleen via HTTPS, en zet API-sleutels niet in de app-code.

# **Je plan, netter en in het Engels**

## **Main Menu**

* General Statistics

* Climbing Statistics

* Speed Climbing Statistics

* Stretch Timers (presets: 30 s / 5 s, plus custom)

## **Speed Climbing**

*Note: a single run and a double run can be active at the same time.*

| Section | Features |
| :---- | :---- |
| **Single: Home** | All-time PB · Weekly PB · All-time average · Weekly average · Button to statistics · Start Session button |
| **Single: Active session** | Session PB · Session average · Session duration · Attempt list (time, date/time, DNS/DNF, notes) · Save attempt · Finish session (with confirmation) · Recent sessions (last 5/10: date/time, average, best, DNF count, details view with notes) · Sorting |
| **Double: Home \+ Active session** | Same as Single, but each attempt has two required time inputs. Statistics are shown per run and combined. |
| **Competition** | Best competition PB · Personal single PB · Last competition average and best · Statistics button · Start new competition · 3 inputs (time or DNF) · Save competition · List of all competitions (best, average, DNF count, placement) |
| **Statistics (Single & Double)** | PB · PB this week/month · Session average PB · Average this week/month · Session average this week/month · Future: charts · Double: per run and combined · Competition: best PB, best average, charts |

## **Training**

| Section | Features |
| :---- | :---- |
| **Strength / Calisthenics** *(one shared model)* | Log exercise (date, per set: kg, seconds, reps) · Table of previous 5/10 times per exercise · Add new exercise (options: kg, seconds, exercise type) |

# **Aanbevolen volgorde van bouwen**

1. Datamodel en SQLite-laag, met unit tests voor de statistieken

2. Single-sessie: start, pogingen opslaan, afronden

3. Statistiekpagina voor single

4. Double (hergebruik van de single-logica)

5. Competitie

6. Krachttraining en calisthenics

7. Export/back-up, daarna grafieken