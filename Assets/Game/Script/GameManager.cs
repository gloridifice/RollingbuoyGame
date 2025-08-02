using System;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Game.Script
{
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance;

        public GameObject pWinUi;
        public bool isGameFinished;

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
                SceneManager.LoadScene(SceneManager.GetActiveScene().name);
            }
        }

        public void LoadScene(string sceneName)
        {
            SceneManager.LoadScene(sceneName);
        }

        public void Win()
        {
            if (isGameFinished) return;
            
            if (pWinUi != null)
            {
                Instantiate(pWinUi);
            }
            
            isGameFinished = true;
        }
    }
}