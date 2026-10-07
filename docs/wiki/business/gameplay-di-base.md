---
tipo: decisione
stato: presa
aggiornato: 2026-10-07
verificato: 2026-10-07
fonti:
  - raw/compiled/study-initialize-llm-wiki/2026-06-10-idee-embrionali.md
  - raw/compiled/study-initialize-llm-wiki/2026-06-10-idee-gameplay.md
  - raw/compiled/study-initialize-llm-wiki/2026-07-01-prima-poc.md
  - raw/compiled/study-initialize-llm-wiki/2026-10-07-concept.md
  - raw/compiled/study-initialize-llm-wiki/2026-10-07-chiarimenti-gameplay.md
---

# Gameplay di base: cicli sul solitario dei 4 re

## Decisione

Si parte dall'**idea 3** delle note di gameplay: i **cicli** sul solitario dei 4 re.

- **Mazzo**: 36 carte del mazzo italiano; all'inizio della partita (o di ogni incontro) si aggiungono i 4 re, per 40 carte (il numero di re è modificabile).
- **Tavolo**: le carte sono disposte su 4 file; ogni carta ha il suo posto.
- **Path**: si sceglie una carta e si continuano a girare carte finché non si torna a quella iniziale. Il ciclo chiuso è un path.
- **Risoluzione**: chiuso il ciclo, il path si risolve: ogni carta genera un evento, e lì parte la parte RPG.
- **Manuale e automatico insieme**: si sceglie una sola carta, poi il path procede da solo; quando si ferma si sceglie un'altra carta da cui ripartire.
- **Cicli come risorsa**: i cicli possono dare più punti (moltiplicatori) o proteggere dal re. Le carte potrebbero non essere mescolate a caso ma secondo dei pattern (es. uno shift di n posti).

La prima POC (2026-07-01) implementa il solitario dei 4 re di base.

## Perché

Le altre due idee non piacevano e non portavano da nessuna parte.

## Alternative scartate

- **Idea 1, base del 31**: via di mezzo tra Slay the Spire e Balatro, gameplay automatico o manuale, briscola che attiva gli effetti del suo seme, "joker" come passive su attacco/armatura e semi — scartata il 2026-10-07: non piaceva e non portava da nessuna parte (fonte: raw/compiled/study-initialize-llm-wiki/2026-10-07-chiarimenti-gameplay.md)
- **Idea 2, autobattle a doppio punteggio 31 e 7 e mezzo**: carte raccolte a cinquine per il 31 e a coppie per seme per il 7 e mezzo, con l'asimmetria tra i due punteggi quando si modifica il mazzo — scartata il 2026-10-07: non piaceva e non portava da nessuna parte (fonte: raw/compiled/study-initialize-llm-wiki/2026-10-07-chiarimenti-gameplay.md)

## Domande aperte

- Regole esatte del tavolo: come si assegna il posto di una carta (fila = seme, colonna = valore?).
- Quando si ferma un path: tornando alla carta iniziale (chiarimenti del 2026-10-07), trovando un re o girando una carta già al suo posto (idea 3)? Che ruolo ha il re nel ciclo?
- Quando si vince e quando si perde un incontro; contro cosa si gioca (punteggio, HP, un altro mazzo).
- Quali eventi genera ogni carta nella risoluzione (es. effetti per seme: denari → soldi, coppe → cure, bastoni → armatura, spade → danno, tra le idee embrionali).
