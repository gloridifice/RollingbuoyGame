using UnityEngine;

namespace Game.Script
{
    public class ActiveToggle : MonoBehaviour
    {
        public void Active()
        {
            gameObject.SetActive(true);
        }

        public void DisActive()
        {
            gameObject.SetActive(false);
        }
    }
}