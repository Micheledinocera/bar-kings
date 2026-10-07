# Come contribuire

Convenzioni del repo, valide per le persone e per gli agenti AI (anche per le skill `/start`, `/ingest`, `/close`).

## Rami (git-flow)

- `main`: solo release, taggate `v<versione>` (es. `v0.1.0`).
- `develop`: integrazione. Le PR puntano qui.
- `feature/<nome>`: un ramo per ogni task. Si apre da `develop` con `/start <nome>` (oppure `git flow feature start <nome>`).
- `bugfix/`, `release/`, `hotfix/`: previsti da git-flow, ma le skill gestiscono solo `feature/*`: per ora anche i fix si fanno come feature.

`<nome>` è breve, in inglese, in kebab-case: `deck-shuffle`, `llm-wiki`. Le feature di sola analisi hanno il prefisso `study-` (`study-scoring`): si chiudono anche con l'analisi parziale e si riprendono con una feature nuova. **Non toccano il codice**: solo `docs/`, `.claude/` e file `*.md`. Per implementare ciò che uno studio ha deciso si chiude lo studio e si apre una feature nuova. Una feature che riprende un tema già trattato tiene il nome con un numero: `study-scoring` → `study-scoring-2`.

**Allineamento con `develop`**: si fa con un merge di `origin/develop` nel ramo della feature (mai rebase di un ramo già pushato). `/close` lo fa da solo prima dell'ingest; se i conflitti sono solo nel wiki, prende il wiki di `develop` e rifà l'ingest con tutti i raw della feature.

**Unity e conflitti**: scene (`.unity`), prefab e asset sono YAML serializzati da Unity e si uniscono male. Due feature aperte insieme non modificano la stessa scena o lo stesso prefab: se serve, ci si accorda prima.

## Commit

Formato [Conventional Commits](https://www.conventionalcommits.org/), in **inglese**:

```
<tipo>(<ambito opzionale>): <cosa cambia, all'imperativo, max ~72 caratteri>

<corpo opzionale: perché, non come>
```

| Tipo | Quando |
|---|---|
| `feat` | Nuova funzionalità |
| `fix` | Correzione di un bug |
| `docs` | Solo documentazione (anche wiki: `docs(wiki): …`) |
| `refactor` | Modifica del codice senza cambiare il comportamento |
| `test` | Test |
| `build` / `ci` | Pacchetti, build, pipeline |
| `chore` | Tutto il resto (configurazione, pulizia, asset di terze parti) |

Ambiti tipici: `cards`, `deck`, `camera`, `ui`, `scene`, `art`, `wiki`, `raw`, `docs`.

Esempi: `feat(deck): shuffle the deck at game start`, `fix(cards): keep card rotation after drag`, `docs(wiki): update from deck-shuffle`.

**Commit atomici**: una modifica logica per commit, così la PR si rilegge passo per passo. Ogni asset si committa con il suo `.meta`.

## Pull request

1. Prima di aprirla, in locale: `dotnet tool restore` e `dotnet csharpier check Assets/Scripts` (formattazione), poi apri il progetto in Unity e verifica che la Console non abbia errori di compilazione. Per le feature `study-` non servono: `/close` verifica invece che non abbiano toccato codice.
2. Si apre verso `develop` con `/close`, che aggiorna il wiki (commit `docs(wiki): …`), fa il push e crea la PR. A mano: `gh pr create --base develop`.
3. Titolo nello stesso formato dei commit; descrizione dal template (`.github/pull_request_template.md`).
4. Nessun merge senza revisione. Nella revisione si rilegge anche il commit del wiki.

**Merge**:

- feature e bugfix verso `develop`: **squash** (un commit per feature, con il titolo della PR);
- release e hotfix verso `main`, e il loro ritorno su `develop`: **merge normale**, altrimenti `main` e `develop` divergono;
- dopo il merge il ramo remoto si cancella (da GitHub o in automatico se attivo); quello locale con `git branch -D feature/<nome>` (dopo lo squash git non lo vede come unito). `/start` segnala i rami locali già uniti.

## Documentazione

- Regole per scrivere codice: `AGENTS.md`.
- Wiki di progetto, raw e ingest: [docs/CLAUDE.md](docs/CLAUDE.md).
