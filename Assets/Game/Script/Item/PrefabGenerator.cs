using UnityEngine;

namespace Game.Script
{
    public class PrefabGenerator : MonoBehaviour
    {
        public GameObject prefab;
        
        public void Generate()
        {
            Instantiate(prefab, transform.position, Quaternion.identity);
        }
    }
}