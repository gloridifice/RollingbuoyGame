using UnityEngine;
using UnityEngine.SceneManagement;

namespace Game.Script
{
    public class NextLevelArea : MonoBehaviour
    {
        public string sceneName;
        
        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.gameObject.TryGetComponent(out PlayerController player))
            {
                SceneManager.LoadScene(sceneName);
            } 
        }
    }
}