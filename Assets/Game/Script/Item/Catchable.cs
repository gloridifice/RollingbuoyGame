using UnityEngine;

namespace Game.Script
{
    /// <summary>
    /// 可以被捕获的物体需要使用该组件
    /// </summary>
    public class Catchable : MonoBehaviour
    {
        public Vector3 catchOffset = Vector3.zero;
    }
}