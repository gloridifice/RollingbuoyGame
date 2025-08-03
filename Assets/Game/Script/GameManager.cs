using System;
using UnityEngine;
using UnityEngine.Audio;
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

        private AudioSource _audioSource;
        public GameObject pWinUi;
        public bool isGameFinished;

        private void Awake()
        {
            _audioSource = gameObject.AddComponent<AudioSource>();
            _audioSource.clip = Resources.Load<AudioClip>("Audio/A_Bgm0");
            _audioSource.loop = true;
            _audioSource.volume = 0.4f;
            _audioSource.Play();

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