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
        public GameObject model;
        public GameObject sitCollision;
        public GameObject standCollision;
        public GameObject itemCollision;

        public CatcherArea catcherArea;
        public GameObject catchPointMarker;

        public Catchable catchableItem;

        public float massThatCanSinkInWater = 10f;
        public float massThatCannotMove = 20f;

        public float inWaterDamping = 2f;
        public float inAirDamping = 1f;

        public CapsuleCollider2D[] activeCapsuleCollider2D;

        private CapsuleCollider2D[] _sitCollider2D;
        private CapsuleCollider2D[] _standCollider2D;

        private void Awake()
        {
            _sitCollider2D = sitCollision.GetComponents<CapsuleCollider2D>();
            _standCollider2D = standCollision.GetComponents<CapsuleCollider2D>();
        }

        private void Start()
        {
            _rgBody2D = GetComponent<Rigidbody2D>();
            standCollision.SetActive(false);
            itemCollision.SetActive(false);
            activeCapsuleCollider2D = _sitCollider2D;
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Q))
            {
                ChangeRingMode();
            }

            if (Input.GetKeyDown(KeyCode.E))
            {
                if (catchableItem == null)
                {
                    if (catcherArea.GetCatchableObject() != null)
                    {
                        CatchItem(catcherArea.GetCatchableObject());        
                    }
                }
                else
                {
                    if (ringMode == RingMode.Sit)
                        PutItem();
                }
            }

            if (ringMode == RingMode.Stand)
            {
                model.transform.Rotate(Vector3.down, (float)(_rgBody2D.linearVelocity.x / 1.8 / 2 * Mathf.PI) * Mathf.Rad2Deg *
                                                   Time.deltaTime, Space.Self);
            }
        }


        void CatchItem(Catchable catchable)
        {
            catchable.transform.SetParent(catchPointMarker.transform);
            catchable.transform.localPosition = catchable.catchOffset;

            if (catchable.gameObject.TryGetComponent(out Rigidbody2D rgRigidbody2D))
            {
                rgRigidbody2D.bodyType = RigidbodyType2D.Kinematic;
                _rgBody2D.mass += rgRigidbody2D.mass;
            }

            foreach (var child in catchable.GetComponentsInChildren<Collider2D>())
            {
                child.enabled = false;
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
            
            foreach (var child in catchableItem.GetComponentsInChildren<Collider2D>())
            {
                child.enabled = true;
            }
            
            catchableItem.transform.SetParent(null);
            catchableItem.transform.position += Vector3.up * 0.2f;
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

            tween.OnComplete((() =>
            {
                activeCapsuleCollider2D = _standCollider2D;
                sitCollision.SetActive(false);
                standCollision.SetActive(true);
                itemCollision.SetActive(false);
            }));
            
            return true;
        }

        bool TrySit()
        {
            ringMode = RingMode.Sit;

            var tween = visual.transform.DOLocalRotate(new Vector3(0f, 0f, 0f), 0.5f);
            tween.SetEase(Ease.InOutCubic);
            tween.OnComplete((() =>
            {
                activeCapsuleCollider2D = _sitCollider2D;
                standCollision.SetActive(false);
                sitCollision.SetActive(true);
                if (catchableItem != null)
                    itemCollision.SetActive(true);
            }));
            
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