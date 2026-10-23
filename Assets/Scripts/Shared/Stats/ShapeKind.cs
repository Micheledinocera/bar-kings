// Shapes of holes on the base table grid (rows = suits, columns = values), measured by the simulation for the
// tribuna bonus being studied (wiki business/modifica-del-mazzo.md, decision in analysis).
public enum ShapeKind
{
    Column, // the 4 suits of one value
    Diagonal, // one cell per row, on consecutive values, going down or up
    Square2,
    Square3,
    Square4,
}
