# Implementazione — preset-buchi

Regole in prova: la decisione [Modifica del mazzo](../../wiki/business/modifica-del-mazzo.md) resta `in analisi`.

## Preset con carte scelte

- `DeckPreset` ha due liste in più: `chosenRemoved` (carte tolte sempre) e `chosenDuplicates` (`ChosenCopies`: carta + copie aggiunte all'originale). I conteggi di prima (`removedCards`, `duplicates` con `CardPick`) restano e si sommano alle carte scelte: servono per Copie-n (n−1 tolte a caso + 1 carta scelta) e per i preset "pieni" (carte scelte tolte + doppioni a caso).
- Le carte tolte a caso non toccano mai le carte scelte. Con liste vuote lo stesso seed dà la stessa distribuzione di prima.
- Preset Builder: griglia 4×9 come il tavolo (righe = semi, colonne = valori, re esclusi): −1 = tolta, n = n copie in più.
- Menu `Tools/Genera Preset Forme`: Copie2/4/8 (asso di denari), Colonna, Diagonale, Quadrato2/3/4 in versione Secca e Piena, SemeSecca (denari 1–9), PariSecca.
- **PariSecca toglie 2, 4, 6 e 8 (16 carte)**, non le 12 carte dello studio: con fante = 8 anche i fanti sono pari.
- Nei preset secchi (meno di 40 carte) gli slot vuoti a inizio partita sono gli ultimi (bastoni e parte di spade), come deciso nello studio: sono buchi veri e possono formare forme "per caso" già al path 0.

## Simulazione

- **Ogni partita simulata finisce subito dopo il path che scopre il 4° re** (regola in prova per la tribuna). L'opzione "nessuna carta coperta", come nella scena, è stata tolta su richiesta dell'utente: rallentava le simulazioni e lì ogni forma voluta esce sempre.
- Strategia dell'autoplay (`AutoplayStrategy`): a caso, o mirata (prima gli slot coperti delle carte tolte, a caso fra loro).
- Il bottone Simula gioca ogni preset con le due strategie, sulle stesse distribuzioni (stessi seed). Partite per preset: 300 (prima 1000), sul prefab `SimulationButton`.
- **Nota dell'utente: con le regole di oggi ogni path crea al massimo un buco**, lo slot di partenza: il path si ferma su un doppione o su un buco, e lascia vuoto solo lo slot da cui è partito. Se si ferma su un buco, lo riempie: in quel caso i buchi restano gli stessi. Quindi una forma di k slot richiede almeno k path; ad esempio il quadrato 4×4 ne richiede almeno 16.
- Buchi: solo gli slot vuoti a fine incontro. Per ogni buco si tiene il path dopo cui è diventato buco per l'ultima volta (0 = dalla distribuzione); il path di una forma è il più alto fra i suoi slot.
- Forme (sulla griglia 4×10, tutte le posizioni): colonna (10), diagonale di 4 celle su valori consecutivi, in giù o in su (14), quadrati 2×2 (27), 3×3 (16), 4×4 (7). Si contano tutte le istanze, non solo la più grande (un 3×3 contiene quattro 2×2).

## CSV

Richiesta dell'utente: pochi numeri, solo **buchi a fine partita** e **quante forme** si creano. Niente colonna `mode` (è nel nome del file).

- `-runs.csv`: preset, strategy, game, seed, table_duplicates, path, length, end, bench_fill, kings_face_up, holes (dopo il path), cards.
- `-games.csv`: una riga per partita: preset, strategy, game, seed, table_duplicates, paths, face_down_at_end, holes, numero di forme per tipo (column, diagonal, square2, square3, square4), `shapes` con ogni forma come "nome @path".
- `-averages.csv`: per preset e strategia: table_duplicates, paths_per_game (= path al 4° re), avg_path_length, first_path_length, longest_path, face_down_at_end, holes, forme per partita di ogni tipo.

## Preset Builder e preset della scena (richieste dell'utente)

- Il Preset Builder ha solo la griglia delle carte da togliere e aggiungere: tolti i campi carte tolte a caso, doppioni a caso e `CardPick`. I preset li supportano ancora; i nuovi preset dal builder li hanno a zero.
- L'utente ha fatto pulizia dei preset (tolti quelli standard e quelli delle forme; restano i suoi). Se il preset della scena non esiste più, `TableDeckManager` prende il **primo della libreria**; con la libreria vuota usa il **mazzo normale** (40 carte, niente tolto né aggiunto; nome "Normale" nel log e nelle opzioni). Per questo `TableDeckManager` ha un riferimento alla libreria dei preset.
- Il menu `Tools/Genera Preset Mazzo` e `Tools/Genera Preset Forme` ricreano i preset cancellati se rilanciati.
