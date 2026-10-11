---
tipo: decisione
stato: aperta
aggiornato: 2026-10-11
verificato: 2026-10-11
fonti:
  - raw/compiled/card-generator/2026-10-11-modifiche-editor.md
---

# Accesso dell'agent all'Unity Editor

Come l'agent può fare modifiche nell'Editor e verificare il progetto (compilazione, Console, Play) invece di passare ogni volta dall'utente. L'utente la studierà più avanti.

## Cosa sappiamo

- Oggi l'agent non ha accesso all'Editor: compilazione, Console e Play li verifica l'utente (vedi [Definition of done](./definition-of-done.md)).
- **Regola in vigore nel frattempo** (`AGENTS.md`, *Writing code*): in ogni feature le modifiche all'Editor si elencano a parte rispetto al codice.
  - Quelle **facili** l'agent le fa nel YAML, **solo dopo conferma** dell'utente, che le verifica subito in Unity: assegnare un riferimento a un asset esistente (una riga con il GUID preso dal `.meta`), cambiare il valore di un campo di un componente esistente, ripetere la stessa modifica su scene identiche.
  - Il resto (creare GameObject o componenti, istanze di prefab e override, struttura dei prefab) diventa istruzioni passo passo per l'utente.
- Primo caso: in card-generator l'agent ha assegnato `deckDatabase` in `ScenaIndex40` e `ScenaTribuna` copiando la riga che l'utente aveva prodotto nelle altre due scene.

## Opzioni

| Opzione | Come | Pro | Contro |
|---|---|---|---|
| **Script editor da menu** | L'agent scrive un comando `Tools/…` che fa la modifica, l'utente lo lancia | Nessuna dipendenza nuova; modifiche fatte da Unity, quindi corrette | Serve sempre un clic dell'utente |
| **Unity in batchmode** | `Unity.exe -batchmode -projectPath … -executeMethod …`, log letto dall'agent | L'agent verifica da solo la compilazione e lancia script editor; risponde alla domanda sulla build della Definition of done | Solo a Editor chiuso (Unity blocca il progetto aperto); avvio lento |
| **Server MCP per Unity** | Pacchetto nell'Editor che espone strumenti all'agent (es. CoplayDev/unity-mcp) | Accesso all'Editor aperto: Console, GameObject, menu, Play | Pacchetto nuovo (decisione da prendere, `AGENTS.md`); codice di terzi che controlla l'Editor; da verificare quale sia il più maturo e se Unity ne offra uno ufficiale per la 6.4 |

## Alternative scartate

Nessuna alternativa scartata finora.

## Domande aperte

- Quale opzione, o quale combinazione (es. batchmode per la compilazione al `/close`, MCP per il lavoro sulle scene)?
- Con un accesso diretto, quali modifiche all'Editor l'agent può fare senza conferma?
