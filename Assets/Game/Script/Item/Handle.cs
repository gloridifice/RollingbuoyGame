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

        [Header("Animation")] 
        
        public float duration = 0.2f;
        public Ease ease = Ease.InOutCubic;

        private void FixedUpdate()
        {
            if (!toggled && _accumulatedImpulse.x > openHandleImpulse)
            {
                Toggle(true);
            }
        }

        void Toggle(bool toggle)
        {
            if (toggled == toggle) return;

            var twn = transform.DORotate(new Vector3(0f, 0f, -60f), duration).SetEase(ease);
            twn.OnComplete(() =>
            {
                onToggled.Invoke();
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