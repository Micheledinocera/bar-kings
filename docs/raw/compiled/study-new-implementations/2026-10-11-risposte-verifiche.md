# Risposte alle verifiche e indicazioni dell'utente

Risposte alle domande aperte dell'[analisi del codice](./2026-10-11-analisi-codice.md) e della [proposta di regole](./2026-10-11-proposta-regole.md).

## Verifiche

- **Asset carta rigenerati a ogni Play**: confermato. Dopo un Play git mostra come modificati i 40 `.asset` di `Assets/Instances/Cards/` con i loro `.meta`, più `Assets/Resources/DOTweenSettings.asset`. Il contenuto però è **identico** a quello committato (`git diff` vuoto, stesso hash di `HEAD`, GUID invariati): è solo rumore in `git status`, non c'è niente da committare.
  - Da fare: il generatore **non deve ricreare un asset che esiste già** (o una logica simile: si genera solo dal menu, o solo ciò che manca).
  - `DOTweenSettings.asset` è il file di configurazione di DOTween in `Resources/`: DOTween lo riscrive quando si inizializza, non dipende dagli script del progetto. Da tenere d'occhio, non da correggere.
  - Probabile causa del rumore: `core.autocrlf=true` mentre Unity scrive i file con fine riga LF. Da valutare un `.gitattributes` per i file YAML di Unity (`*.asset`, `*.meta`, `*.unity`, `*.prefab` con `eol=lf`), in una feature di codice.
- **Nomi dei file diversi dalle classi**: nessun errore né avviso in Console. Resta una questione di leggibilità, non un bug.
- **`CardSlot.index` fuori ordine, doppioni fissi, carte mancanti**: sono **voluti**. Fanno parte dei test: la panchina nasce proprio per gestire i doppioni e i buchi. Il problema `dead-code` su `CardSlot.index` / `CardSlotContainer.CardSlotList` resta (il codice non li legge), ma i valori non sono un errore.

## Indicazioni

- **Scene gemelle temporanee**: `Panchina`, `ScenaTribuna`, `ScenaIndex40` esistono per approfondire ogni idea senza rompere la versione precedente. A un certo punto se ne terrà una sola e le altre verranno eliminate: non vanno mantenute per sempre. Quindi: niente architettura pensata per tenere in vita tre varianti a lungo. Il nucleo condiviso serve a non copiare il codice durante le prove, non a fare un sistema di varianti definitivo.
- **Identificatori**: i nomi misti italiano/inglese restano come sono. L'agent scrive il codice nuovo in inglese, ma non segnala né corregge i nomi dati dagli umani.
- **Contributor umani**: alcuni contributor sono persone. **Nessuna regola** (lint, formattazione, git, convenzioni) **deve mai essere bloccante**. Al massimo, al `/close` di una feature si guarda se c'è roba nuova fatta da umani che non rispetta le regole e la si sistema in quel momento.
  - Oggi `/close` si ferma se `dotnet csharpier check` fallisce, se manca un `.meta`, o su altri controlli (`.claude/skills/close/SKILL.md`): va rivisto perché sistemi o segnali invece di fermarsi. Da decidere quali controlli restano stop veri (es. un ramo sbagliato, conflitti di merge) e quali diventano correzioni automatiche o avvisi.

## Ordine dei lavori confermato

Si segue la proposta dell'analisi:

1. generatore delle carte (cartella `Editor/`, nessuna rigenerazione di asset esistenti, nessuna chiamata a runtime);
2. formattazione e convenzioni in una feature dedicata;
3. nucleo condiviso (modello / vista / regola della variante), prima di lavorare su tribuna e index>40.

## Applicato in questo studio (2026-10-11)

- `AGENTS.md`: nuove sezioni *Rules are never blocking*, *Scripts*, *Unity Editor* con le regole confermate dalla [proposta](./2026-10-11-proposta-regole.md); identificatori misti accettati. Namespace e assembly definition **non** adottati per ora: si decidono nella feature del nucleo condiviso.
- `CONTRIBUTING.md` e `.github/pull_request_template.md`: la formattazione è facoltativa per le persone, la applica `/close`.
- `.claude/skills/close/SKILL.md`: i controlli di repo (passo 5) non fermano più la chiusura. CSharpier fa `format` invece di `check` e committa, i `.meta` mancanti vengono committati o segnalati, il codice scritto dalle persone viene sistemato se il fix è sicuro, gli errori in Console finiscono nelle *Notes* della PR; il lint del wiki non ferma più. Restano stop veri solo: ramo non `feature/*`, lavoro non committato (ignorando il rumore dei fine riga di Unity), `gh` non autenticato.
- `/close` passo 1: "studio senza codice" ora chiede invece di fermarsi, "raw intoccati" avvisa invece di fermarsi. `/start`: il controllo del working tree pulito ignora il rumore dei fine riga di Unity. `/ingest` resta com'è: il suo unico stop (modifiche non committate nel wiki) protegge lavoro non salvato.
- **Regola degli studi esplicitata** (richiesta dell'utente): uno studio può modificare qualunque file `*.md` (wiki, raw, `AGENTS.md`, `CONTRIBUTING.md`, skill, template) oltre a `docs/` e `.claude/`. Regole, skill e documenti decisi in uno studio si aggiornano nello studio stesso, senza lasciarli in sospeso tra i commit. Aggiornati `AGENTS.md`, `CONTRIBUTING.md`, `docs/CLAUDE.md`, `/start`, `/close`; la pagina wiki `progetto/git-flow-e-convenzioni.md` la aggiorna l'ingest.
