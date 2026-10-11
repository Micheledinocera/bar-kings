# Obiettivo — git-line-endings

Impostare git per evitare che dopo ogni Play di Unity risultino modificati (e vengano committati) decine di asset che in realtà non sono cambiati.

Causa trovata in study-new-implementations: il Git di sistema ha `core.autocrlf=true`, quindi al checkout i file di testo diventano CRLF, mentre Unity li riscrive in LF. Soluzione: un `.gitattributes` che fissa LF per i file di testo anche nella copia di lavoro.

Nella stessa occasione il branch di default su GitHub è stato portato da `main` a `develop`, così dopo la chiusura di una PR il checkout va su `develop`.
