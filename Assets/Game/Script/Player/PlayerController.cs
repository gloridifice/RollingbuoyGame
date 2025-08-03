using System;
using TarodevController;
using UnityEngine;
using UnityEngine.Serialization;

namespace Game.Script
{
    /// <summary>
    /// Hey!
    /// Tarodev here. I built this controller as there was a severe lack of quality & free 2D controllers out there.
    /// I have a premium version on Patreon, which has every feature you'd expect from a polished controller. Link: https://www.patreon.com/tarodev
    /// You can play and compete for best times here: https://tarodev.itch.io/extended-ultimate-2d-controller
    /// If you hve any questions or would like to brag about your score, come to discord: https://discord.gg/tarodev
    /// </summary>
    [RequireComponent(typeof(Rigidbody2D))]
    public class PlayerController : MonoBehaviour, IPlayerController
    {
        [SerializeField] private ScriptableStats _stats;
        public Rigidbody2D rb;
        private RingController _rc;
        private FrameInput _frameInput;
        private Vector2 _frameVelocity;
        private bool _cachedQueryStartInColliders;
        private float _defaultMass;
        private float _massFactor;

        public bool enableInput = true;

        public Transform groundRaycastOrigin;
        public Transform ceilRaycastOrigin;

        #region Interface

        public Vector2 FrameInput => _frameInput.Move;
        public event Action<bool, float> GroundedChanged;
        public event Action Jumped;

        #endregion

        private float _time;

        private void Awake()
        {
            var a = GameManager.Instance; // Init GameManager
            
            rb = GetComponent<Rigidbody2D>();
            _rc = GetComponent<RingController>();

            _cachedQueryStartInColliders = Physics2D.queriesStartInColliders;
            _defaultMass = rb.mass;
        }

        private void Update()
        {
            _time += Time.deltaTime;
            GatherInput();
        }

        public bool IsOnGround()
        {
            return _grounded;
        }

        private void GatherInput()
        {
            _frameInput = new FrameInput
            {
                JumpDown = Input.GetButtonDown("Jump") || Input.GetKeyDown(KeyCode.C),
                JumpHeld = Input.GetButton("Jump") || Input.GetKey(KeyCode.C),
                Move = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"))
            };

            if (_stats.SnapInput)
            {
                _frameInput.Move.x = Mathf.Abs(_frameInput.Move.x) < _stats.HorizontalDeadZoneThreshold
                    ? 0
                    : Mathf.Sign(_frameInput.Move.x);
                _frameInput.Move.y = Mathf.Abs(_frameInput.Move.y) < _stats.VerticalDeadZoneThreshold
                    ? 0
                    : Mathf.Sign(_frameInput.Move.y);
            }

            if (_frameInput.JumpDown)
            {
                _jumpToConsume = true;
                _timeJumpWasPressed = _time;
            }
        }

        private void FixedUpdate()
        {
            CheckCollisions();

            _massFactor = Mathf.Max(0f, (_rc.massThatCannotMove - rb.mass) / (_rc.massThatCannotMove - _defaultMass));

            if (_rc.ringMode == RingMode.Stand)
            {
                _massFactor = Mathf.Max(0.4f, _massFactor);
            }

            if (enableInput)
            {
                HandleJump();
                HandleDirection();
            }

            HandleGravity();
            HandleDamping();

            ApplyMovement();
        }

        #region Collisions

        private float _frameLeftGrounded = float.MinValue;
        private bool _grounded;

        private void CheckCollisions()
        {
            Physics2D.queriesStartInColliders = false;

            bool groundHit = false;
            bool ceilingHit = false;

            bool RaycastShape(Collider2D col, Vector2 direction, float distance, LayerMask layerMask)
            {
                if (col is CapsuleCollider2D capsuleCol)
                {
                    return Physics2D.CapsuleCast(col.bounds.center, capsuleCol.size, capsuleCol.direction, 0, direction,
                        distance, layerMask);
                }
                if (col is BoxCollider2D boxCol)
                {
                    var a = Physics2D.BoxCast(col.bounds.center, boxCol.size, 0, Vector2.down, _stats.GrounderDistance,
                        ~_stats.PlayerLayer);
                }

                return false;
            }

            foreach (var col in _rc.activeCapsuleCollider2D)
            {
                // Ground and Ceiling
                if (!groundHit)
                    groundHit |= RaycastShape(col, Vector2.down, _stats.GrounderDistance, ~_stats.PlayerLayer);
                if (!ceilingHit)
                    ceilingHit |= RaycastShape(col, Vector2.up, _stats.GrounderDistance, ~_stats.PlayerLayer);
            }

            // Hit a Ceiling
            if (ceilingHit) _frameVelocity.y = Mathf.Min(0, _frameVelocity.y);

            // Landed on the Ground
            if (!_grounded && groundHit)
            {
                _grounded = true;
                _coyoteUsable = true;
                _bufferedJumpUsable = true;
                _endedJumpEarly = false;
                GroundedChanged?.Invoke(true, Mathf.Abs(_frameVelocity.y));
            }
            // Left the Ground
            else if (_grounded && !groundHit)
            {
                _grounded = false;
                _frameLeftGrounded = _time;
                GroundedChanged?.Invoke(false, 0);
            }

            Physics2D.queriesStartInColliders = _cachedQueryStartInColliders;
        }

        #endregion


        #region Jumping

        private bool _jumpToConsume;
        private bool _bufferedJumpUsable;
        private bool _endedJumpEarly;
        private bool _coyoteUsable;
        private float _timeJumpWasPressed;

        private bool HasBufferedJump => _bufferedJumpUsable && _time < _timeJumpWasPressed + _stats.JumpBuffer;
        private bool CanUseCoyote => _coyoteUsable && !_grounded && _time < _frameLeftGrounded + _stats.CoyoteTime;

        private void HandleJump()
        {
            if (!_endedJumpEarly && !_grounded && !_frameInput.JumpHeld && rb.linearVelocity.y > 0)
                _endedJumpEarly = true;

            if (!_jumpToConsume && !HasBufferedJump) return;

            if ((_grounded || CanUseCoyote)) ExecuteJump();

            _jumpToConsume = false;
        }

        private void ExecuteJump()
        {
            _endedJumpEarly = false;
            _timeJumpWasPressed = 0;
            _bufferedJumpUsable = false;
            _coyoteUsable = false;
            _frameVelocity.y = _stats.JumpPower * _massFactor;
            Jumped?.Invoke();
        }

        #endregion

        #region Horizontal

        private void HandleDirection()
        {
            if (_frameInput.Move.x == 0)
            {
                var deceleration = _grounded ? _stats.GroundDeceleration : _stats.AirDeceleration;
                _frameVelocity.x = Mathf.MoveTowards(_frameVelocity.x, 0, deceleration * Time.fixedDeltaTime);
            }
            else
            {
                _frameVelocity.x = Mathf.MoveTowards(_frameVelocity.x,
                    _frameInput.Move.x * _stats.MaxSpeed * _massFactor, _stats.Acceleration * Time.fixedDeltaTime);
            }
        }

        #endregion

        #region Gravity

        private void HandleGravity()
        {
            if (_grounded && _frameVelocity.y <= 0f && !_rc.isInWater)
            {
                _frameVelocity.y = _stats.GroundingForce;
            }
            else
            {
                var inAirGravity = _stats.FallAcceleration;
                var verticalAcc = inAirGravity;

                if (_rc.isInWater)
                {
                    var massFactor = Mathf.Clamp(-(_rc.massThatCanSinkInWater - rb.mass) /
                                                 (_rc.massThatCanSinkInWater - _defaultMass), -1f, 1f);

                    verticalAcc *= massFactor;
                }

                if (_endedJumpEarly && _frameVelocity.y > 0) verticalAcc *= _stats.JumpEndEarlyGravityModifier;
                _frameVelocity.y = Mathf.MoveTowards(_frameVelocity.y, -_stats.MaxFallSpeed,
                    verticalAcc * Time.fixedDeltaTime);
            }
        }

        #endregion

        #region Damping

        private void HandleDamping()
        {
            var len = _frameVelocity.magnitude;
            var dampingAcc = len * len * (_rc.isInWater ? _rc.inWaterDamping : _rc.inAirDamping) * 0.1f;
            _frameVelocity = Vector2.MoveTowards(_frameVelocity, Vector2.zero,
                dampingAcc * Time.fixedDeltaTime);
        }

        #endregion

        private void ApplyMovement() => rb.linearVelocity = _frameVelocity;

#if UNITY_EDITOR
        private void OnValidate()
        {
            if (_stats == null)
                Debug.LogWarning("Please assign a ScriptableStats asset to the Player Controller's Stats slot", this);
        }
#endif
    }

    public struct FrameInput
    {
        public bool JumpDown;
        public bool JumpHeld;
        public Vector2 Move;
    }

    public interface IPlayerController
    {
        public event Action<bool, float> GroundedChanged;

        public event Action Jumped;
        public Vector2 FrameInput { get; }
    }
}