using UnityEngine;
using UnityEngine.SceneManagement;

namespace Whimsical.UI
{
    using Debug;

    public class MainMenu : MonoBehaviour
    {
        public void OnStartGame()
        {
            DebugExtensions.Log("Starting the game");
            SceneManager.LoadScene("SampleScene");
        }
    }
}
