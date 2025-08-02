using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Serialization;

namespace Game.Script
{
    public class MovableGate : MonoBehaviour
    {
        private float _defaultY;

        public float yOffset = -5f;
        public float duration = 1f;
        public Ease ease = Ease.InOutCubic;

        private void Start()
        {
            _defaultY = transform.position.y;
        }

        public void Open()
        {
            transform.DOMoveY(_defaultY + yOffset, duration).SetEase(ease);
        }

        public void Close()
        {
            transform.DOMoveY(_defaultY, duration).SetEase(ease);
        }
    }
}