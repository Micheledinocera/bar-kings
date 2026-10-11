# Obiettivo — git-line-endings

Impostare git per evitare che dopo ogni Play di Unity risultino modificati (e vengano committati) decine di asset che in realtà non sono cambiati.

Causa trovata in study-new-implementations: il Git di sistema ha `core.autocrlf=true`, quindi al checkout i file di testo diventano CRLF, mentre Unity li riscrive in LF. Soluzione: un `.gitattributes` che fissa LF per i file di testo anche nella copia di lavoro.

Nella stessa occasione il branch di default su GitHub è stato portato da `main` a `develop`, così dopo la chiusura di una PR il checkout va su `develop`.

Aggiunto su richiesta dell'utente: cancellazione automatica dei rami dopo il merge. Su GitHub è attiva l'impostazione del repo (`delete branch on merge`); in locale git non ha un aggancio sul merge remoto, quindi `/start` cancella i rami locali `feature/*` il cui remoto è sparito e la cui PR risulta unita. In locale è attivo `fetch.prune`.
