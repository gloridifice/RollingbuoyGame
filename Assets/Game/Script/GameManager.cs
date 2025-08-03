using System;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Game.Script
{
    public class GameManager : MonoBehaviour
    {
        private static GameManager _instance;
        public static GameManager Instance
        {
            get
            {
                if (_instance == null)
                {
                    var obj = new GameObject("Game Manager");
                    _instance = obj.AddComponent<GameManager>();
                    DontDestroyOnLoad(_instance);
                }
                
                return _instance;
            }
        }

        public GameObject pWinUi;
        public bool isGameFinished;

        private void Awake()
        {
            pWinUi = Resources.Load<GameObject>("Prefab/P_WinCanvas");
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

            FindFirstObjectByType<PlayerController>().enableInput = false;
            isGameFinished = true;
        }
    }
}