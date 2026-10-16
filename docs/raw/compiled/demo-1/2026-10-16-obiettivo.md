# Obiettivo — demo-1

Facciamo un minidemo così cominciamo anche a vedere se ci sono problemi nelle build che facciamo.

- Va creata una scena di "raccordo" con 3 bottoni che rimandano alle 3 scene "panchina", "tribuna" e "index40".
- Sulle singole scene mettiamo un pulsante per fare il toggle del logger a video e un "back" per tornare alla schermata di scelta della scena.
- Ci troviamo: rinominiamo pure le scene in maniera che non abbiano il prefisso "Scena".
- Cancelliamo SampleScene e tutto ciò che non usiamo più da lì.
- Fai tutto per step.

## Punti di partenza nel wiki

- Le tre scene sono le POC di [Modifica del mazzo](../../wiki/business/modifica-del-mazzo.md) (decisione `in analisi`): la demo le mette in visione, non sceglie l'idea.
- Scene, Build Settings e prefab: [tecnico/scenes](../../wiki/tecnico/scenes.md). Prima POC usata solo da `SampleScene`: [tecnico/shared](../../wiki/tecnico/shared.md), [known-issues](../../wiki/tecnico/known-issues.md) blocco B.

## Scelte (2026-10-16)

- Scena di raccordo: **Menu** (`Assets/Scenes/Menu.unity`), prima nei Build Settings.
- Scene rinominate: `ScenaTribuna` → `Tribuna`, `ScenaIndex40` → `Index40`.
- Pulizia: via `SampleScene` e la prima POC che usava solo lei (`DeckManager3D`, `CardController`, `CardAnimations`, prefab `3DCard`), e anche il codice morto `CameraOrbit` e `ScriptProva`. Il `LightingData.asset` della cartella di SampleScene è usato da tutte le scene: si sposta, non si cancella.
- Build di prova: Windows 64 bit.
