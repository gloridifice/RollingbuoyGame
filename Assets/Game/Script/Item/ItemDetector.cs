using UnityEngine;
using UnityEngine.Events;

namespace Game.Script
{
    public class ItemDetector : MonoBehaviour
    {
        [Tooltip("物体进入区域一段时间才会触发事件")]
        public float validDurationSec = 0.5f;
        public string validItemTag = "";
        public UnityEvent onOpen = new ();
        public UnityEvent onClose = new ();
        public bool isToggled;

        private float _accumulatedTime = 0f;
        private GameObject _itemObject;
        private bool _isItemInRange;
        

        private void Update()
        {
            if (!isToggled && _isItemInRange)
            {
                _accumulatedTime += Time.deltaTime;
                if (_accumulatedTime > validDurationSec)
                {
                    isToggled = true;
                    onOpen.Invoke();
                }
            }
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.attachedRigidbody == null) return;
            if (other.attachedRigidbody.gameObject.TryGetComponent(out Item item))
            {
                if (item.tags.Contains(validItemTag))
                {
                    _itemObject = other.attachedRigidbody.gameObject;
                    _isItemInRange = true;
                }
            }
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            if (other.attachedRigidbody == null) return;
            if (other.attachedRigidbody.gameObject == _itemObject)
            {
                _accumulatedTime = 0f;
                _isItemInRange = false;
                if (isToggled)
                {
                    onClose.Invoke();
                    isToggled = false;
                }
            }
        }
    }
}