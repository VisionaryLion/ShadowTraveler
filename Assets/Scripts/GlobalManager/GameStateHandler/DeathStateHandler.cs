using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

namespace Manager
{
    public class DeathStateHandler : MonoBehaviour, IGameState
    {
        public GameObject deathUIRoot;

        GameStateManager stateMan;

        void Awake()
        {
            stateMan = GameStateManager.GetInstance();
        }

        public void OnStateActive()
        {
            if (Input.anyKeyDown)
            {
                stateMan.EndCurrentState();
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex, LoadSceneMode.Single);
            }
        }

        public void OnStateStart()
        {
            //some death fx
            deathUIRoot.SetActive(true);
            Time.timeScale = 0;
        }

        public void OnStateEnd()
        {
            //nothing
            deathUIRoot.SetActive(false);
            Time.timeScale = 1;
        }

        public override string ToString()
        {
            return "DeathState";
        }
    }
}
