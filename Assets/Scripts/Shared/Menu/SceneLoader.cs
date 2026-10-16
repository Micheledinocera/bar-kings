using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.SceneManagement;

// Loads a scene of the Build Settings by name: the buttons of the Menu scene and the Back button of the table
// scenes (SceneButton prefab, OnClick calls Load).
public class SceneLoader : MonoBehaviour
{
    [SerializeField, Required]
    private string sceneName;

    public void Load() => SceneManager.LoadScene(sceneName);
}
