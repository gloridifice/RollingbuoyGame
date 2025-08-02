using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

namespace Game.Script
{
    /// <summary>
    /// 检测可以捕获物体是否进入区域的组件。自身必须有 Trigger 才能生效。
    /// </summary>
    public class CatcherArea : MonoBehaviour
    {
        private Catchable _catchableObject;

        public UnityEvent<Catchable> onDetectCatch = new ();
        public UnityEvent<Catchable> onExitCatch = new ();

        public string tagsFilter = "";

        public Catchable GetCatchableObject()
        {
            return _catchableObject;
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            var rb = other.attachedRigidbody;
            if (rb == null) return;
            
            bool tagFilterValid = tagsFilter.Length == 0 || (rb.gameObject.TryGetComponent(out Item item) && item.tags.Contains(tagsFilter));
            if (rb.gameObject.TryGetComponent(out Catchable catchable) && tagFilterValid)
            {
                _catchableObject = catchable;
                onDetectCatch.Invoke(catchable);
            }
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            var rb = other.attachedRigidbody;
            if (rb == null) return;
            
            bool tagFilterValid = tagsFilter.Length == 0 || (rb.gameObject.TryGetComponent(out Item item) && item.tags.Contains(tagsFilter));
            if (rb.gameObject.TryGetComponent(out Catchable catchable) && catchable == _catchableObject && tagFilterValid)
            {
                onExitCatch.Invoke(_catchableObject);
                _catchableObject = null;
            }
        }
    }
}