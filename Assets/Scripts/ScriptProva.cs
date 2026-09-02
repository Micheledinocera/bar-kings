using UnityEngine;

public class ScriptProva : MonoBehaviour
{
    public string nome;
    public int eta;

    void Start() { }

    void Update() { }

    public void debugLog()
    {
        Debug.Log($"mi chiamo {nome} e ho {eta} anni");
    }
}
