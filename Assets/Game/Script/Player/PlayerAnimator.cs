using UnityEngine;

namespace Game.Script.Player
{
    /// <summary>
    /// VERY primitive animator example.
    /// </summary>
    public class PlayerAnimator : MonoBehaviour
    {
        [Header("Particles")] [SerializeField] private ParticleSystem _jumpParticles;
        [SerializeField] private ParticleSystem _launchParticles;
        [SerializeField] private ParticleSystem _moveParticles;
        [SerializeField] private ParticleSystem _landParticles;

        private AudioSource _source;
        private IPlayerController _player;
        private RingController _rc;
        private bool _grounded;
        private ParticleSystem.MinMaxGradient _currentGradient;

        [Header("Audio")]
        
        public AudioClip jumpAudio;
        public AudioClip catchAudio;
        public AudioClip enterWaterAudio;
        public float waterAudioPlayInterval = 1.0f;

        private float _waterAudioPlayTimer;

        private void Awake()
        {
            _source = GetComponent<AudioSource>();
            _player = GetComponentInParent<IPlayerController>();
            _rc = GetComponentInParent<RingController>();
        }
        
        private void OnEnable()
        {
            _player.Jumped += OnJumped;
            _player.GroundedChanged += OnGroundedChanged;
            _rc.onCatchInnerItem.AddListener(OnCatch);
            _rc.onCatchOuterItem.AddListener(OnCatch);
            _rc.onPutInnerItem.AddListener(OnPut);
            _rc.onPutOuterItem.AddListener(OnPut);
            _rc.onEnterWater.AddListener(OnEnterWater);

            _moveParticles.Play();
        }

        private void OnDisable()
        {
            _player.Jumped -= OnJumped;
            _player.GroundedChanged -= OnGroundedChanged;
            _rc.onCatchInnerItem.RemoveListener(OnCatch);
            _rc.onCatchOuterItem.RemoveListener(OnCatch);
            _rc.onPutInnerItem.RemoveListener(OnPut);
            _rc.onPutOuterItem.RemoveListener(OnPut);
            _rc.onEnterWater.RemoveListener(OnEnterWater);

            _moveParticles.Stop();
        }
        
        void OnCatch(Catchable catchable)
        {
            PlayAudioClipInRandom(catchAudio, 0.8f, 1.5f, 0.8f, 1.2f);
        }
        
        void OnPut(Catchable catchable)
        {
            PlayAudioClipInRandom(catchAudio, 0.4f, 0.7f, 0.8f, 1.0f);
        }

        void OnEnterWater()
        {
            if (_waterAudioPlayTimer == 0f)
            {
                PlayAudioClipInRandom(enterWaterAudio, 0.5f, 1.0f, 0.4f, 0.6f);
            }
            _waterAudioPlayTimer = waterAudioPlayInterval;
        }

        private void Update()
        {
            if (_player == null) return;

            _waterAudioPlayTimer = Mathf.Max(_waterAudioPlayTimer - Time.deltaTime, 0f);

            DetectGroundColor();

            HandleIdleSpeed();
        }

        private void HandleIdleSpeed()
        {
            var inputStrength = Mathf.Abs(_player.FrameInput.x);
            _moveParticles.transform.localScale = Vector3.MoveTowards(_moveParticles.transform.localScale, Vector3.one * inputStrength, 2 * Time.deltaTime);
        }

        private void OnJumped()
        {
            if (_grounded) // Avoid coyote
            {
                SetColor(_jumpParticles);
                SetColor(_launchParticles);
                _jumpParticles.Play();
                
                PlayAudioClipInRandom(jumpAudio, 0.9f, 1.6f, 0.7f, 1.3f);
            }
        }

        private void PlayAudioClip(AudioClip clip, float pitch = 1f, float volume = 1f)
        {
            _source.clip = clip;
            _source.pitch = pitch;
            _source.volume = volume;
            _source.Play();
        }

        private void PlayAudioClipInRandom(AudioClip clip, float pitchMin, float pitchMax, float volumeMin, float volumeMax)
        {
            PlayAudioClip(clip, Random.Range(pitchMin, pitchMax), Random.Range(volumeMin, volumeMax));
        }

        private void OnGroundedChanged(bool grounded, float impact)
        {
            _grounded = grounded;
            
            if (grounded)
            {
                DetectGroundColor();
                SetColor(_landParticles);

                _moveParticles.Play();

                _landParticles.transform.localScale = Vector3.one * Mathf.InverseLerp(0, 40, impact);
                _landParticles.Play();
            }
            else
            {
                _moveParticles.Stop();
            }
        }

        private void DetectGroundColor()
        {
            var hit = Physics2D.Raycast(transform.position, Vector3.down, 2);

            if (!hit || hit.collider.isTrigger || !hit.transform.TryGetComponent(out SpriteRenderer r)) return;
            var color = r.color;
            _currentGradient = new ParticleSystem.MinMaxGradient(color * 0.9f, color * 1.2f);
            SetColor(_moveParticles);
        }

        private void SetColor(ParticleSystem ps)
        {
            var main = ps.main;
            main.startColor = _currentGradient;
        }

        private static readonly int GroundedKey = Animator.StringToHash("Grounded");
        private static readonly int IdleSpeedKey = Animator.StringToHash("IdleSpeed");
        private static readonly int JumpKey = Animator.StringToHash("Jump");
    }
}