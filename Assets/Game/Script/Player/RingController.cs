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
        [FormerlySerializedAs("sitCollision")] public GameObject sitCollisionObject;
        [FormerlySerializedAs("standCollision")] public GameObject standCollisionObject;
        public GameObject itemCollision;

        [FormerlySerializedAs("catcherArea")] public CatcherArea innerCatcherArea;
        public CatcherArea outerCatcherArea;
        public GameObject catchPointMarker;

        public Catchable catchableItem;

        public float massThatCanSinkInWater = 10f;
        public float massThatCannotMove = 20f;

        public float inWaterDamping = 2f;
        public float inAirDamping = 1f;

        public CapsuleCollider2D[] activeCapsuleCollider2D;

        private CapsuleCollider2D[] _sitColliders;
        private CapsuleCollider2D[] _standColliders;

        private float _defaultStandRadius;

        public OuterCatchable outerCatchableItem;

        private void Awake()
        {
            _sitColliders = sitCollisionObject.GetComponents<CapsuleCollider2D>();
            _standColliders = standCollisionObject.GetComponents<CapsuleCollider2D>();
            var stand = _standColliders[0];
            _defaultStandRadius = stand.size.y / 2f;
        }

        private void Start()
        {
            _rgBody2D = GetComponent<Rigidbody2D>();
            standCollisionObject.SetActive(false);
            itemCollision.SetActive(false);
            activeCapsuleCollider2D = _sitColliders;
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
                    if (innerCatcherArea.GetCatchableObject() != null)
                    {
                        CatchInnerItem(innerCatcherArea.GetCatchableObject());
                    }
                    else if (outerCatcherArea.GetCatchableObject() != null)
                    {
                        CatchOuterItem(outerCatcherArea.GetCatchableObject());
                    }
                }
                else
                {
                    if (ringMode == RingMode.Sit)
                    {
                        if (outerCatchableItem != null)
                        {
                            PutOuterItem();
                        }
                        else if (catchableItem != null)
                        {
                            PutItem();
                        }
                    }
                }
            }

            if (ringMode == RingMode.Stand)
            {
                model.transform.Rotate(Vector3.down, (float)(_rgBody2D.linearVelocity.x / 1.8 / 2 * Mathf.PI) *
                                                     Mathf.Rad2Deg *
                                                     Time.deltaTime, Space.Self);
            }
        }

        void CatchOuterItem(Catchable catchable)
        {
            if (catchable is OuterCatchable outerCatchable)
            {
                catchable.transform.SetParent(catchPointMarker.transform);
                catchable.transform.localPosition = catchable.catchOffset;

                if (catchable.gameObject.TryGetComponent(out Rigidbody2D rgRigidbody2D))
                {
                    rgRigidbody2D.bodyType = RigidbodyType2D.Kinematic;
                    _rgBody2D.mass += rgRigidbody2D.mass;
                }

                outerCatchableItem = outerCatchable;
            }
        }

        void CatchInnerItem(Catchable catchable)
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

        void PutOuterItem()
        {
            if (outerCatchableItem.gameObject.TryGetComponent(out Rigidbody2D rgRigidbody2D))
            {
                rgRigidbody2D.bodyType = RigidbodyType2D.Dynamic;
                _rgBody2D.mass -= rgRigidbody2D.mass;
            }

            outerCatchableItem.transform.SetParent(null);
            outerCatchableItem.transform.position += Vector3.up * 0.3f;
            outerCatchableItem = null;
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
            catchableItem.transform.position += Vector3.up * 0.3f;
            var rot = catchableItem.transform.rotation.eulerAngles;
            rot.x = 0f;
            rot.y = 0f;
            catchableItem.transform.rotation = Quaternion.Euler(rot);
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
                activeCapsuleCollider2D = _standColliders;
                sitCollisionObject.SetActive(false);
                standCollisionObject.SetActive(true);
                itemCollision.SetActive(false);

                if (outerCatchableItem != null)
                {
                    SetStandColliderRadius(outerCatchableItem.outerRadius);
                    
                    foreach (var childCollider in outerCatchableItem.GetComponentsInChildren<Collider2D>())
                        childCollider.enabled = false;
                }
            }));

            return true;
        }

        void SetStandColliderRadius(float radius)
        {
            _standColliders[0].size = new Vector2(radius * 2, radius * 2);
        }
        
        bool TrySit()
        {
            ringMode = RingMode.Sit;

            var tween = visual.transform.DOLocalRotate(new Vector3(0f, 0f, 0f), 0.5f);
            tween.SetEase(Ease.InOutCubic);
            tween.OnComplete((() =>
            {
                activeCapsuleCollider2D = _sitColliders;
                standCollisionObject.SetActive(false);
                sitCollisionObject.SetActive(true);
                if (outerCatchableItem != null)
                {
                    SetStandColliderRadius(_defaultStandRadius);
                    foreach (var childCollider in outerCatchableItem.GetComponentsInChildren<Collider2D>())
                        childCollider.enabled = true;
                }
                if (catchableItem != null)
                    itemCollision.SetActive(true);
            }));

            return true;
        }

        #endregion


        public bool isInWater = false;
        private int _inWaterCount = 0;

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.gameObject.TryGetComponent(out WaterArea water))
            {
                _inWaterCount += 1;
                isInWater = true;
            }
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            if (other.gameObject.TryGetComponent(out WaterArea water))
            {
                _inWaterCount -= 1;
                if (_inWaterCount == 0)
                {
                    isInWater = false;
                }
            }
        }
    }
}