using System;
using UnityEngine;

namespace Game.Script
{
    public class Floatable : MonoBehaviour
    {
        public bool isInWater;

        private Rigidbody2D _rgBody2D;
        private bool _hasRgBody2D;

        public float floatForce = 70f;

        private int _waterCount = 0;
        private void Awake()
        {
            if (TryGetComponent(out Rigidbody2D rigidbody2D))
            {
                _rgBody2D = rigidbody2D;
                _hasRgBody2D = true;
            }
        }

        private void FixedUpdate()
        {
            if (isInWater && _hasRgBody2D)
            {
                _rgBody2D.AddForce(floatForce * Vector2.up);
            }
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.gameObject.TryGetComponent(out WaterArea water))
            {
                _waterCount += 1;
                isInWater = true;
            }
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            if (other.gameObject.TryGetComponent(out WaterArea water))
            {
                _waterCount -= 1;
                if (_waterCount == 0)
                {
                    isInWater = false;
                }
            }
        }
    }
}