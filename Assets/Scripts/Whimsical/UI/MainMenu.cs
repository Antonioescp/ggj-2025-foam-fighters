using UnityEngine;
using UnityEngine.SceneManagement;

namespace Whimsical.UI
{
    public class MainMenu : MonoBehaviour
    {
        public void OnStartGame()
        {
            SceneManager.LoadScene("SampleScene");
        }
    }
}
