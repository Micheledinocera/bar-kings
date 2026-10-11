# Modifiche all'Editor fatte dall'agent

Emerso durante card-generator: l'utente ha assegnato il `CardDatabase` in `Panchina` e `SampleScene`; la modifica nelle scene era una sola riga (`deckDatabase: {fileID: 11400000, guid: …, type: 2}`) e l'agent l'ha replicata su `ScenaIndex40` e `ScenaTribuna`.

## Regola concordata (scritta in `AGENTS.md`)

- In ogni feature le modifiche all'Editor si elencano **a parte** rispetto a quelle al codice.
- Le modifiche **facili** le fa l'agent nel YAML, **solo dopo conferma** dell'utente, così sono subito verificabili in Unity: assegnare un riferimento a un asset esistente, cambiare il valore di un campo di un componente esistente, ripetere la stessa modifica di una riga su scene identiche.
- Tutto il resto (creare GameObject o componenti, istanze di prefab e override, struttura dei prefab): istruzioni passo passo per l'utente.

## Decisione aperta: accesso dell'agent all'Editor

L'utente la studierà più avanti. Opzioni emerse:

1. **Script editor** scritti dall'agent e lanciati dall'utente da un menu (`Tools/…`): nessuna dipendenza nuova, ma serve un clic.
2. **Unity in batchmode** (`Unity.exe -batchmode -projectPath … -executeMethod …`): l'agent esegue script editor e verifica la compilazione dal log; funziona solo a Editor chiuso. Risponderebbe anche alla domanda sulla build nella Definition of done.
3. **Server MCP per Unity** (pacchetti open source come CoplayDev/unity-mcp; da verificare quale sia il più maturo e se Unity ne offra uno ufficiale per la 6.4): accesso all'Editor aperto (Console, GameObject, menu, Play). È un pacchetto nuovo (decisione da prendere, `AGENTS.md`) e codice di terzi che controlla l'Editor.
