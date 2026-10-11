# Progetto

Come si costruisce e si porta avanti il progetto: repository, versione di Unity e pacchetti, convenzioni, wiki e studi. Torna all'[indice del wiki](../index.md).

- [Wiki di progetto](./wiki-di-progetto.md): LLM wiki ripreso dal marketplace e adattato a Unity: ciclo raw → ingest → wiki legato alle feature, aree di base `business`/`progetto`/`tecnico`, `tecnico/` per classe e per scena, "Used by" via GUID, lint e check di `/close` · presa
- [Branching, commit e pull request](./git-flow-e-convenzioni.md): git-flow, studi che modificano solo documentazione (qualunque `*.md`), regole mai bloccanti, Conventional Commits atomici con `.meta`, PR verso `develop` con squash riviste da Michele, allineamento con merge, niente feature parallele sulla stessa scena o prefab · presa
- [Definition of done delle feature di codice](./definition-of-done.md): nessuna regola bloccante: `/close` formatta con CSharpier, committa i `.meta`, sistema il codice delle persone e mette il resto nelle note della PR; Console e Play confermati dall'utente; test e build ancora aperti · presa
- [Tipi di ramo e release](./tipi-di-ramo-e-release.md): `bugfix/`, `hotfix/`, `release/` con le skill o tutto come feature; flusso di release verso `main` · aperta
