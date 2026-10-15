# Decisione — accesso dell'agent all'Editor tramite unity-mcp

L'utente ha detto esplicitamente che la decisione [Accesso dell'agent all'Unity Editor](../../wiki/progetto/accesso-editor-agent.md) è **presa**: l'agent accede all'Editor tramite il server MCP ufficiale di Unity.

## Cosa è stato scelto

- Pacchetto `com.unity.ai.assistant` (`2.20.0-pre.2`, pre-release) nel progetto; il relay `C:/Users/<utente>/.unity/relay/relay_win.exe --mcp` è registrato in Claude Code come server `unity-mcp` (scope utente, fuori dal repo: ogni persona lo registra sulla sua macchina).
- Strumenti esposti: `Unity_RunCommand` (esegue codice C# nell'Editor), `Unity_GetConsoleLogs`, catture di Scene view e camera, generazione di asset.
- Funziona solo con l'Editor aperto sul progetto.

## Regola

Quando l'agent deve modificare qualcosa nell'Editor (scene, prefab, ScriptableObject, componenti, riferimenti) passa da `unity-mcp`, mai modificando a mano lo YAML. Sostituisce la regola precedente (modifiche facili nel YAML dopo conferma, il resto istruzioni passo passo per l'utente).

Dettagli proposti dall'agent, in `AGENTS.md`:

- le modifiche all'Editor restano elencate a parte rispetto al codice e l'utente conferma l'elenco prima che l'agent le esegua; leggere e verificare (Console, catture, ispezione della gerarchia) non richiede conferma;
- dopo la modifica l'agent salva scena o prefab dall'Editor e controlla la Console;
- dopo modifiche al codice l'agent controlla da solo la compilazione dalla Console;
- Editor chiuso o MCP che non risponde: l'agent chiede all'utente di aprire il progetto, non ripiega sullo YAML;
- Project Settings protetti, pacchetti e `Assets/Plugins/` restano fuori: si chiede prima, come oggi.

## Opzioni non adottate

- Script editor da menu: restano il modo di scrivere i generatori di asset (regola in `AGENTS.md`); l'agent ora li lancia da MCP invece di chiederlo all'utente.
- Unity in batchmode: non serve per il lavoro sull'Editor; resta un'opzione solo per la domanda aperta sulla build nella Definition of done.

## Da tenere d'occhio

- Il pacchetto è una pre-release: valutare il passaggio a una versione stabile quando esce.
- Play mode: la prova del gameplay resta all'utente finché non si verifica cosa l'MCP permette.
