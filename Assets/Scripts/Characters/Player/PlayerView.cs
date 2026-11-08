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

        private const int BaseLayer = 0;
        private int _climbStartStateHash;
        private int _ledgeClimbStateHash;
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
        private static readonly int AnimIDClimbLedge = Animator.StringToHash("ClimbLedge");

        private void Awake()
        {
            _audioSource = GetComponent<AudioSource>();
            _playerController = GetComponent<PlayerController>();
            _climbStartStateHash = Animator.StringToHash(climbStartStateName);
            _ledgeClimbStateHash = Animator.StringToHash(ledgeClimbStateName);
        }

        private void Start()
        {
            AssignAnimationIDs();
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

        public void PlayLedgeClimb()
        {
            animator.SetTrigger(AnimIDClimbLedge);
        }

        // Entering the state counts as playing; leaving it (its exit-time transition) does not.
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
            animator.SetBool(AnimIDClimbing, false);
            // Clear any climb trigger that wasn't consumed, so it can't fire later.
            animator.ResetTrigger(AnimIDClimbStart);
            animator.ResetTrigger(AnimIDClimbGrab);
            animator.ResetTrigger(AnimIDClimbLedge);
        }

        public void SetClimbMove(Vector2 move)
        {
            animator.SetFloat(AnimIDClimbX, move.x, climbMoveDampTime, Time.deltaTime);
            animator.SetFloat(AnimIDClimbY, move.y, climbMoveDampTime, Time.deltaTime);
        }

        // Reuses the footstep dust. It's moved to the contact point on the ramp, because the feet pivot
        // ends up inside the ramp while the body lies along it.
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
