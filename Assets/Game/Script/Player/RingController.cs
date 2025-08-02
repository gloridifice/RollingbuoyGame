using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;

namespace Game.Script
{
    public enum RingMode
    {
        Sit,
        Stand
    }

    public class RingController : MonoBehaviour
    {
        public RingMode ringMode = RingMode.Sit;

        private Rigidbody2D _rgBody2D;

        public GameObject visual;
        public GameObject standCollision;
        public GameObject itemCollision;

        public CatcherArea catcherArea;
        public GameObject catchPointMarker;

        public Catchable catchableItem;

        public float massThatCanSinkInWater = 10f;
        public float massThatCannotMove = 20f;

        public float inWaterDamping = 2f;
        public float inAirDamping = 1f;

        public float maxFloatSpeed = 10f;
        public float floatAcc = 10f;
        
        private void Start()
        {
            _rgBody2D = GetComponent<Rigidbody2D>();
            standCollision.SetActive(false);
            itemCollision.SetActive(false);
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.R))
            {
                ChangeRingMode();
            }

            if (Input.GetKeyDown(KeyCode.E))
            {
                if (catchableItem == null)
                {
                    if (catcherArea.GetCatchableObject() != null)
                        CatchItem(catcherArea.GetCatchableObject());        
                }
                else
                {
                    if (ringMode == RingMode.Sit)
                        PutItem();
                }
            }
        }

        void CatchItem(Catchable catchable)
        {
            catchable.transform.SetParent(catchPointMarker.transform);
            catchable.transform.localPosition = Vector3.zero;

            if (catchable.gameObject.TryGetComponent(out Rigidbody2D rgRigidbody2D))
            {
                rgRigidbody2D.bodyType = RigidbodyType2D.Kinematic;
                _rgBody2D.mass += rgRigidbody2D.mass;
            }

            if (catchable.gameObject.TryGetComponent(out Collider2D collider))
            {
                collider.enabled = false;
            }

            catchableItem = catchable;
            itemCollision.SetActive(true);
        }

        void PutItem()
        {
            itemCollision.SetActive(false);
            
            if (catchableItem.gameObject.TryGetComponent(out Rigidbody2D rgRigidbody2D))
            {
                rgRigidbody2D.bodyType = RigidbodyType2D.Dynamic;
                _rgBody2D.mass -= rgRigidbody2D.mass;
            }

            if (catchableItem.gameObject.TryGetComponent(out Collider2D collider))
            {
                collider.enabled = true;
            }
            
            catchableItem.transform.SetParent(null);
            catchableItem = null;
        }

        #region Ring Mode

        void ChangeRingMode()
        {
            if (ringMode == RingMode.Sit)
                TryStand();
            else
                TrySit();
        }

        bool TryStand()
        {
            ringMode = RingMode.Stand;

            var tween = visual.transform.DOLocalRotate(new Vector3(90f, 0f, 0f), 0.5f);
            tween.SetEase(Ease.InOutCubic);
            standCollision.SetActive(true);
            
            return true;
        }

        bool TrySit()
        {
            ringMode = RingMode.Sit;
            
            var tween = visual.transform.DOLocalRotate(new Vector3(0f, 0f, 0f), 0.5f);
            tween.SetEase(Ease.InOutCubic);
            standCollision.SetActive(false);
            
            return true;
        }

        #endregion


        public bool isInWater = false;

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.gameObject.TryGetComponent(out WaterArea water))
            {
                isInWater = true;
            }
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            if (other.gameObject.TryGetComponent(out WaterArea water))
            {
                isInWater = false;
            }
        }
    }
}