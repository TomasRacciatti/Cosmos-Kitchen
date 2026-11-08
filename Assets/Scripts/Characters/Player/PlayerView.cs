using Audio;
using Interfaces;
using UnityEngine;
using UnityEngine.Serialization;

namespace Characters.Player
{
    public class PlayerView : MonoBehaviour, IClimbView
    {
        [Header("Climbing")]
        [Tooltip("Smoothing time for the climb blend tree parameters (s).")]
        [SerializeField] private float climbMoveDampTime = 0.1f;
        [Tooltip("Name of the Climb Start state in the Animator (Base Layer). Climb input is held while it plays.")]
        [SerializeField] private string climbStartStateName = "Player_ClimbStart";
        [Tooltip("Name of the ledge climb-up state in the Animator (Base Layer). Input is held while it plays.")]
        [SerializeField] private string ledgeClimbStateName = "Player_ClimbEdge";
        [Tooltip("Dust particles per second while sliding down a ramp (uses the footstep particles).")]
        [Min(0f)] [SerializeField] private float slideDustRate = 20f;

        [Header("Ledge Climb-Up Alignment")]
        [Tooltip("The climb idle clip (center of the ClimbMove blend tree).")]
        [SerializeField] private AnimationClip climbIdleClip;
        [Tooltip("The ledge climb-up clip (Player_ClimbEdge).")]
        [SerializeField] private AnimationClip ledgeClimbClip;
        [Tooltip("A bone that represents where the body is, e.g. the hips (Bspine).")]
        [SerializeField] private Transform bodyReferenceBone;

        private const int BaseLayer = 0;
        private int _climbStartStateHash;
        private int _ledgeClimbStateHash;
        private Vector3 _ledgeClimbOffset; // measured at start; visible in Debug inspector mode
        private bool _hasLedgeClimbOffset;
        private SkinnedMeshRenderer[] _skinnedRenderers;
        private bool[] _defaultUpdateWhenOffscreen;
        private bool _slideEffectActive;
        private Vector3 _stepsParticlesLocalPosition;

        [SerializeField] private AudioCue damagedCue;
        [SerializeField] private AudioCue footstepCue;
        [SerializeField] private AudioCue landingCue;
        
        [SerializeField] private Animator animator;
        [SerializeField] private ParticleSystem stepsParticles;
        private AudioSource _audioSource;
        private PlayerController _playerController;
        
        // animation IDs
        private int _animIDSpeed;
        private int _animIDSpeedUp;
        private int _animIDJumped;
        private int _animIDLanded;
        private int _animIDGrounded;
        private int _animIDFalling;

        // climbing animation IDs
        private static readonly int AnimIDClimbing = Animator.StringToHash("Climbing");
        private static readonly int AnimIDClimbStart = Animator.StringToHash("ClimbStart");
        private static readonly int AnimIDClimbGrab = Animator.StringToHash("ClimbGrab");
        private static readonly int AnimIDClimbX = Animator.StringToHash("ClimbX");
        private static readonly int AnimIDClimbY = Animator.StringToHash("ClimbY");

        private void Awake()
        {
            _audioSource = GetComponent<AudioSource>();
            _playerController = GetComponent<PlayerController>();
            _climbStartStateHash = Animator.StringToHash(climbStartStateName);
            _ledgeClimbStateHash = Animator.StringToHash(ledgeClimbStateName);

            _skinnedRenderers = animator.GetComponentsInChildren<SkinnedMeshRenderer>(true);
            _defaultUpdateWhenOffscreen = new bool[_skinnedRenderers.Length];
            for (int i = 0; i < _skinnedRenderers.Length; i++)
            {
                _defaultUpdateWhenOffscreen[i] = _skinnedRenderers[i].updateWhenOffscreen;
            }
        }

        private void Start()
        {
            AssignAnimationIDs();
            MeasureLedgeClimbOffset();
        }

        // The climb-up clip is authored relative to where it ends (standing on top), so its first frame draws
        // the body somewhere else than the climbing pose does. Sample both and compare the body bone, in the
        // player's local space. The Animator overwrites the sampled pose on its next update.
        private void MeasureLedgeClimbOffset()
        {
            if (climbIdleClip == null || ledgeClimbClip == null || bodyReferenceBone == null)
            {
                Debug.LogWarning($"{nameof(PlayerView)}: ledge climb-up clips or body bone not assigned; climb-up disabled.", this);
                return;
            }

            GameObject model = animator.gameObject;
            climbIdleClip.SampleAnimation(model, 0f);
            Vector3 climbingBody = transform.InverseTransformPoint(bodyReferenceBone.position);
            ledgeClimbClip.SampleAnimation(model, 0f);
            Vector3 ledgeStartBody = transform.InverseTransformPoint(bodyReferenceBone.position);

            _ledgeClimbOffset = climbingBody - ledgeStartBody;
            _hasLedgeClimbOffset = true;
        }

        public bool TryGetLedgeClimbOffset(out Vector3 localOffset)
        {
            localOffset = _ledgeClimbOffset;
            return _hasLedgeClimbOffset;
        }

        private void AssignAnimationIDs()
        {
            _animIDSpeed = Animator.StringToHash("Speed");
            _animIDSpeedUp = Animator.StringToHash("SpeedUp");
            _animIDJumped = Animator.StringToHash("Jumped");
            _animIDLanded = Animator.StringToHash("Landed");
            _animIDGrounded = Animator.StringToHash("Grounded");
            _animIDFalling = Animator.StringToHash("Falling");
        }
        
        public void SetSpeed(float speed)
        {
            animator.SetFloat(_animIDSpeed, speed);

            var emission = stepsParticles.emission;
            
            if (!_playerController.Grounded)
            {
                emission.enabled = false;
                return;
            }
            
            //new
            //tasa base de emision
            float baseRate = 2.0f; 
    
            //si la velocidad es mayor a x(correr) aplica el multiplicador
            float rateMultiplier = (speed >= 5.0f) ? 5.0f : 1.0f;
    
            float finalRate = speed * baseRate * rateMultiplier;
            
            emission.enabled = speed > 0.1f;
            emission.rateOverTime = finalRate;
            //new
        }
        
        public void SetVerticalSpeed(float up)
        {
            animator.SetFloat(_animIDSpeedUp, up);
        }
        
        public void Jumped()
        {
            SetGrounded(false);
            animator.SetTrigger(_animIDJumped);
            //sounds
        }

        public void Landed()
        {
            SetGrounded(true);
            SetFalling(false);
            animator.SetTrigger(_animIDLanded);
            //AudioSource.PlayClipAtPoint(landingCue.GetRandomClip(), transform.position);
        }

        public void SetGrounded(bool value)
        {
            if (value) SetFalling(false);
            animator.SetBool(_animIDGrounded, value);
        }
    
        public void SetFalling(bool value)
        {
            if (value) SetGrounded(false);
            animator.SetBool(_animIDFalling, value);
        }

        public void Footstep()
        {
            //AudioSource.PlayClipAtPoint(footstepCue.GetRandomClip(), transform.position);
        }

        public void ClimbStarted(bool fromGround)
        {
            var emission = stepsParticles.emission;
            emission.enabled = false;
            animator.SetFloat(AnimIDClimbX, 0f);
            animator.SetFloat(AnimIDClimbY, 0f);
            animator.SetBool(AnimIDClimbing, true);
            animator.SetTrigger(fromGround ? AnimIDClimbStart : AnimIDClimbGrab);
        }

        public bool IsPlayingClimbStart => IsPlayingState(_climbStartStateHash);
        public bool IsPlayingLedgeClimb => IsPlayingState(_ledgeClimbStateHash);

        public float LedgeClimbProgress
        {
            get
            {
                AnimatorStateInfo state = animator.GetCurrentAnimatorStateInfo(BaseLayer);
                return state.shortNameHash == _ledgeClimbStateHash ? Mathf.Clamp01(state.normalizedTime) : 0f;
            }
        }
        
        public void PlayLedgeClimb()
        {
            SetBoundsFollowBones(true);
            animator.Play(_ledgeClimbStateHash, BaseLayer, 0f);
        }
        
        private void SetBoundsFollowBones(bool follow)
        {
            for (int i = 0; i < _skinnedRenderers.Length; i++)
            {
                _skinnedRenderers[i].updateWhenOffscreen = follow || _defaultUpdateWhenOffscreen[i];
            }
        }
        
        private bool IsPlayingState(int stateHash)
        {
            if (animator.IsInTransition(BaseLayer))
            {
                return animator.GetNextAnimatorStateInfo(BaseLayer).shortNameHash == stateHash;
            }
            return animator.GetCurrentAnimatorStateInfo(BaseLayer).shortNameHash == stateHash;
        }

        public void ClimbEnded()
        {
            SetBoundsFollowBones(false);
            animator.SetBool(AnimIDClimbing, false);
            animator.ResetTrigger(AnimIDClimbStart);
            animator.ResetTrigger(AnimIDClimbGrab);
        }

        public void SetClimbMove(Vector2 move)
        {
            animator.SetFloat(AnimIDClimbX, move.x, climbMoveDampTime, Time.deltaTime);
            animator.SetFloat(AnimIDClimbY, move.y, climbMoveDampTime, Time.deltaTime);
        }
        
        public void SetSlideEffect(bool active, Vector3 surfacePoint)
        {
            var emission = stepsParticles.emission;
            Transform particles = stepsParticles.transform;

            if (active)
            {
                if (!_slideEffectActive)
                {
                    _slideEffectActive = true;
                    _stepsParticlesLocalPosition = particles.localPosition;
                    emission.rateOverTime = slideDustRate;
                    emission.enabled = true;
                }
                particles.position = surfacePoint;
                return;
            }

            if (!_slideEffectActive) return;
            _slideEffectActive = false;
            particles.localPosition = _stepsParticlesLocalPosition;
            emission.enabled = false;
        }
    }
}
