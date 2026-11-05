# Obiettivo — santini-and-amari-first-implementation

Voglio introdurre nel gioco un paio di santini ed un paio di amari per creare la meccanica.

- **Santini**: inizierei con **San Francesco** e **San Matteo** dalle idee in `docs/raw/compiled/study-roadmap/2026-10-29-idee-santini-amari.md` (feature `study-roadmap`, PR #26):
  - San Francesco d'Assisi: quando un cavallo (9) si risolve nel path, applica il suo effetto due volte;
  - San Matteo: quando si risolve una carta di denari, +50% Lire (nelle idee c'era anche la variante "lo shop costa un decimo in meno").
- **HP**: restano tra un fight e l'altro.
- **Amari**: vanno considerati come oggetti consumabili utilizzabili durante il fight (tra la fine di un path e l'inizio di un altro). Ne introdurrei due, solo per verificare il corretto funzionamento:
  - uno che cura una percentuale degli HP;
  - uno che aumenta il danno permanentemente di 5.
- **Testing manuale**: durante la run devo poter modificare in maniera dinamica, su Unity, la lista di santini e di amari in possesso del player.

Decisioni coinvolte (in analisi, l'implementazione è una prova e non le prende): [Meccaniche della run](../../wiki/business/meccaniche-della-run.md), [Parte RPG](../../wiki/business/parte-rpg.md).
