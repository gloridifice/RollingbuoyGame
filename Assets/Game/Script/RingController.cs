using System;
using DG.Tweening;
using UnityEngine;

namespace Game.Script
{
    enum RingMode
    {
        Sit,
        Stand
    }

    public class RingController : MonoBehaviour
    {
        private RingMode _ringMode = RingMode.Sit;

        public GameObject visual;
        public GameObject standCollision;

        public CatcherArea catcherArea;
        public GameObject catchPointMarker;

        public Catchable catchableItem;
        
        private void Start()
        {
            standCollision.SetActive(false);
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
                    {
                        CatchItem(catcherArea.GetCatchableObject());        
                    }
                }
                else
                {
                    if (_ringMode == RingMode.Sit)
                    {
                        PutItem();
                    }
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
            }

            if (catchable.gameObject.TryGetComponent(out Collider2D collider))
            {
                collider.enabled = false;
            }

            catchableItem = catchable;
        }

        void PutItem()
        {
            if (catchableItem.gameObject.TryGetComponent(out Rigidbody2D rgRigidbody2D))
            {
                rgRigidbody2D.bodyType = RigidbodyType2D.Dynamic;
            }

            if (catchableItem.gameObject.TryGetComponent(out Collider2D collider))
            {
                collider.enabled = true;
            }
            
            catchableItem = null;
        }

        #region Ring Mode

        void ChangeRingMode()
        {
            if (_ringMode == RingMode.Sit)
                TryStand();
            else
                TrySit();
        }

        bool TryStand()
        {
            _ringMode = RingMode.Stand;

            var tween = visual.transform.DOLocalRotate(new Vector3(90f, 0f, 0f), 0.5f);
            tween.SetEase(Ease.InOutCubic);
            standCollision.SetActive(true);
            
            return true;
        }

        bool TrySit()
        {
            _ringMode = RingMode.Sit;
            
            var tween = visual.transform.DOLocalRotate(new Vector3(0f, 0f, 0f), 0.5f);
            tween.SetEase(Ease.InOutCubic);
            standCollision.SetActive(false);
            
            return true;
        }

        #endregion

    }
}