# Setup Antigravity / Gemini

Data: 2026-10-29

## Cosa è stato configurato

1. **Ignorati in `.gitignore`**:
   - Aggiunte le regole `.agents/` e `.agent/` nel file `.gitignore` per evitare di tracciare file di configurazione locali dell'agente.

2. **Skill di workspace per Antigravity**:
   - Creata la configurazione `.agents/skills.json` puntata su `.claude/skills`.
   - Creata una giunzione NTFS (`.agents/skills` -> `.claude/skills`) per garantire la scoperta automatica nativa di tutte le 5 skill (`start`, `close`, `ingest`, `lint-wiki`, `report`) senza duplicare file e mantenendo la compatibilità al 100% con Claude Code.

3. **Integrazione Unity MCP**:
   - Creato il plugin workspace `.agents/plugins/unity/` con `plugin.json` e `mcp_config.json`.
   - Configurato l'eseguibile di relay `C:/Users/Aru/.unity/relay/relay_win.exe` con argomento `--mcp`.

4. **Verifiche di ambiente**:
   - `lint-wiki.mjs`: eseguito con successo (0 errori, 0 warning).
   - `csharpier`: verificato e funzionante (`1.3.0`).
   - Git-flow: configurati i rami di default nel `git config` locale (`develop`, `main`, prefix `feature/`).
