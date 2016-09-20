using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

namespace Manager
{
    public class ButtonHandlers : MonoBehaviour
    {

        public void Resume()
        {
            GameStateManager.GetInstance().EndCurrentState();
        }

        public void Menu()
        {
            SceneManager.LoadScene("Main Menu");
        }

        public void Quit()
        {
            Application.Quit();
        }

        public void Restart()
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
    }
}