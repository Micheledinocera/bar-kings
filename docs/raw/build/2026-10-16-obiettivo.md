# Obiettivo — build

Mettiamo a posto ste cose della build, partendo dalle known issues.

Punto di partenza: [Known issues, blocco I — Build](../../wiki/tecnico/known-issues.md#block-i--build) e [scenes, Build](../../wiki/tecnico/scenes.md#build) (dettaglio in `docs/raw/compiled/demo-1/2026-10-16-build.md`):

- peso della build Windows (~331 MB, quasi tutto texture: fronti delle carte non compressi, tre texture 4K del tavolo);
- la build riscrive asset URP e Project Settings (servizi Unity attivi), che compaiono in `git status`;
- Company nei Player Settings ancora `DefaultCompany`.

Domande aperte collegate: build prima della PR in [Definition of done](../../wiki/progetto/definition-of-done.md#domande-aperte), build nel flusso di release in [Tipi di ramo e release](../../wiki/progetto/tipi-di-ramo-e-release.md#domande-aperte).
