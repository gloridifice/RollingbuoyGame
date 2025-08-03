using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

namespace Game.Script
{
    public class Handle : MonoBehaviour
    {
        private Vector2 _accumulatedImpulse = Vector2.zero;

        public bool toggled = false;
        public float openHandleImpulse = 200f;

        public UnityEvent onToggled = new();
        public UnityEvent onToggledLeft = new();
        public UnityEvent onToggledRight = new();

        [Header("Animation")] 
        
        public float duration = 0.2f;
        public Ease ease = Ease.InOutCubic;

        private void FixedUpdate()
        {
            if (!toggled && Mathf.Abs(_accumulatedImpulse.x) > openHandleImpulse)
            {
                Toggle(true, _accumulatedImpulse.x > 0f);
            }
        }

        void Toggle(bool toggle, bool isLeft)
        {
            if (toggled == toggle) return;

            float deg = 60f;
            if (isLeft) deg = -60f;
            
            var twn = transform.DORotate(new Vector3(0f, 0f, deg), duration).SetEase(ease);
            
            twn.OnComplete(() =>
            {
                onToggled.Invoke();
                if (isLeft)
                    onToggledLeft.Invoke();
                else 
                    onToggledRight.Invoke();
            });
            
            toggled = toggle;
        }


        private void OnCollisionStay2D(Collision2D other)
        {
            if (other.gameObject.TryGetComponent(out PlayerController player))
            {
                foreach (var pnt in other.contacts)
                    _accumulatedImpulse += pnt.normal * pnt.normalImpulse;
            }
        }

        private void OnCollisionExit2D(Collision2D other)
        {
            if (other.gameObject.TryGetComponent(out PlayerController player))
            {
                _accumulatedImpulse = Vector2.zero;
            }
        }
    }
}