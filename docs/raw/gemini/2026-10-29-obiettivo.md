# Obiettivo — gemini

Configurare e integrare l'ambiente di lavoro per Antigravity / Gemini:
- Mappare e rendere disponibili le skill di progetto (`start`, `close`, `ingest`, `lint-wiki`, `report`) ad Antigravity tramite `.agents/skills.json` senza duplicare i file né rompere la compatibilità con Claude.
- Verificare la corretta esecuzione dei comandi di linting e formattazione (`lint-wiki.mjs`, `csharpier`).
- Configurare e verificare gli accessi/toolchain necessari (MCP Unity relay per l'Editor, GitHub CLI per il lifecycle delle PR).
