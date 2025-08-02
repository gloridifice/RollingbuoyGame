using System;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Game.Script
{
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance;

        public GameObject pWinUi;
        public string currentSceneName;

        private void Awake()
        {
            if (Instance == null)
                Instance = this;
            
            DontDestroyOnLoad(Instance);
        }

        private void Update()
        {
            // Reload scene
            if (Input.GetKeyDown(KeyCode.R))
            {
                SceneManager.LoadScene(currentSceneName);
            }
        }

        public void LoadScene(string sceneName)
        {
            currentSceneName = sceneName;
            SceneManager.LoadScene(sceneName);
        }

        public void Win()
        {
            if (pWinUi != null)
            {
                Instantiate(pWinUi);
            }
        }
    }
}