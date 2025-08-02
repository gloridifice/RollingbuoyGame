using System;
using UnityEngine;

namespace Game.Script
{
    /// <summary>
    /// 检测可以捕获物体是否进入区域的组件。自身必须有 Trigger 才能生效。
    /// </summary>
    public class CatcherArea : MonoBehaviour
    {
        private Catchable _catchableObject;

        public readonly Action<Catchable> OnDetectCatch = ((catchable => {}));
        public readonly Action<Catchable> OnExitCatch = ((catchable => {}));

        public Catchable GetCatchableObject()
        {
            return _catchableObject;
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.gameObject.TryGetComponent(out Catchable catchable))
            {
                _catchableObject = catchable;
                OnDetectCatch.Invoke(catchable);
            }
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            if (other.gameObject.TryGetComponent(out Catchable catchable) && catchable == _catchableObject)
            {
                OnExitCatch.Invoke(_catchableObject);
            }
        }
    }
}