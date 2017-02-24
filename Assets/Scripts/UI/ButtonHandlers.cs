using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

namespace Manager

{
    public class ButtonHandlers : MonoBehaviour

    {

        public void Resume()

        {
            GameStateManager.Instance.EndCurrentState();
        }

        public void LoadScene(string sceneIndex)

        {
            SceneManager.LoadScene(sceneIndex);
        }

        public void Quit()

        {
            #if UNITY_EDITOR
          UnityEditor.EditorApplication.isPlaying = false;
            #else
            Application.Quit();
            #endif
        }

        public void Restart()

        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);

        }
    }
}
