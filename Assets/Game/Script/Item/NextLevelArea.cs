using UnityEngine;

namespace Game.Script
{
    public class NextLevelArea : MonoBehaviour
    {
        public string sceneName;
        
        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.gameObject.TryGetComponent(out PlayerController player))
            {
                if (sceneName is { Length: > 0 })
                {
                    GameManager.Instance.LoadScene(sceneName);
                }
                else
                {
                    GameManager.Instance.Win();
                }
            }
        }
    }
}