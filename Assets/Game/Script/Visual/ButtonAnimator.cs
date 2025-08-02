using System;
using DG.Tweening;
using UnityEngine;

namespace Game.Script.Visual
{
    public class ButtonAnimator : MonoBehaviour
    {
        public float moveY = -0.2f;
        public float duration = 0.2f;
        public Ease ease = Ease.InOutCubic;
        public Transform button;

        private float _defaultY;

        private void Start()
        {
            _defaultY = transform.position.y;
        }

        public void AnimateUp()
        {
            button.DOMoveY(_defaultY + moveY, duration).SetEase(ease);
        }

        public void AnimateDown()
        {
            button.DOMoveY(_defaultY, duration).SetEase(ease);
        }
    }
}