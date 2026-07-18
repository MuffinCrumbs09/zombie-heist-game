using UnityEngine;
using TMPro;
using PlayerInput;
using System.Collections;
using System;

namespace Player
{
    [RequireComponent(typeof(CharacterController))]
    public class PlayerMovementController : MonoBehaviour
    {
        // Components
        private CharacterController _characterController;
        [SerializeField] private Transform _cameraTransform;
        [SerializeField] private PlayerMovementConfig _config;
        [SerializeField] private float _slideCooldownTime = 3.5f;

        // Runtime
        private bool _isSwitchingStates = false;
        private IMovementState _curState;
        private Vector3 _currentVelocity;
        private Vector3 _targetVelocity;
        private Vector3 _dampVelocity;
        private bool _isGrounded = true;
        private bool _canJump;
        private float _lastGroundedTime;
        private float _airborneStartHeight;
        private bool _isJumping => _curState is PlayerJumpState || _curState is PlayerLandingState;
        private float _targetHeight;
        private float _currentHeight;
        private float _heightVelocity;
        private bool _slideCooldown;

        // Getters / Setters
        public PlayerMovementConfig Config => _config;
        public bool CanJump => _canJump;
        public bool IsGrounded => _isGrounded;
        public float HorizontalSpeed => new Vector3(_currentVelocity.x, 0, _currentVelocity.z).magnitude;
        public float VerticalVelocity
        {
            get => _currentVelocity.y;
            set => _currentVelocity.y = value;
        }
        public Vector3 HorizontalVelocity => new(_currentVelocity.x, 0, _currentVelocity.z);
        public float AirborneStartHeight
        {
            get => _airborneStartHeight;
            set => _airborneStartHeight = value;
        }
        public bool SlideCooldown => _slideCooldown;

        private void OnEnable()
        {
            if (InputManager.Instance != null)
                InputManager.Instance.OnJumpPressed += TryJump;

        }

        private void OnDisable()
        {
            InputManager.Instance.OnJumpPressed -= TryJump;
        }

        private void Start()
        {
            InputManager.Instance.OnJumpPressed += TryJump;
            _characterController = GetComponent<CharacterController>();
            _curState = new PlayerIdleState();

            _currentHeight = _config.standingCameraHeight;
            _targetHeight = _config.standingCameraHeight;
            _characterController.height = _currentHeight;
            _characterController.center = new Vector3(0f, _currentHeight / 2f, 0f);
        }

        private void Update()
        {
            if (_isSwitchingStates) return;

            _curState.UpdateState(this, Time.deltaTime);

            ApplyMovement();
            UpdateGroundCheck();
            UpdateHeight(Time.deltaTime);
            UpdateCameraEffects();

#if UNITY_EDITOR
            UpdateDebugText();
#endif
        }

        private void UpdateCameraEffects()
        {
            float targetFOV = _curState.GetTargetFOV();
            Camera.main.fieldOfView = Mathf.Lerp(Camera.main.fieldOfView, targetFOV, _config.fovLerpSpeed * Time.deltaTime);
        }

        /// <summary>
        /// Switches the player to a new state, calling the exit of the old and enter of the new
        /// </summary>
        public void SwitchState(IMovementState newState)
        {
            if (_isSwitchingStates) return;
            _isSwitchingStates = true;

            _curState.ExitState(newState, this);
            newState.EnterState(this);

            _curState = newState;
            _isSwitchingStates = false;
        }

        /// <summary>
        /// Called by states to request a height change
        /// </summary>
        public void SetTargetHeight(float height)
        {
            _targetHeight = height;
        }

        public void StartSlideCooldown()
        {
            _slideCooldown = true;
            StartCoroutine(SlideCooldownCoroutine());
        }

        public void ResetVelocity()
        {
            _currentVelocity = Vector3.zero;
        }

        private IEnumerator SlideCooldownCoroutine()
        {
            yield return new WaitForSeconds(_slideCooldownTime);
            _slideCooldown = false;
        }

        /// <summary>
        /// Attempts to switch the player into the jump state, accessible from every state as long as
        /// the player is allowed to jump (grounded or within coyote time).
        /// </summary>
        private void TryJump()
        {
            Debug.Log($"Can Jump: {_canJump}, Is Grounded: {_isGrounded}, Last Grounded Time: {_lastGroundedTime}, Current Time: {Time.time}, Coyote Time: {_config.coyoteTime}");
            if (!_canJump || _isJumping) return;

            Debug.Log("Jumping!");

            SwitchState(new PlayerJumpState());
        }

        private void ApplyMovement()
        {
            Vector3 moveInput = InputManager.Instance.MoveInput;
            Vector3 targetHorizontal = transform.TransformDirection(new Vector3(moveInput.x, 0, moveInput.y)) * _curState.GetTargetSpeed();
            Vector3 currentHorizontal = new Vector3(_currentVelocity.x, 0, _currentVelocity.z);

            float smoothTime = targetHorizontal.magnitude > currentHorizontal.magnitude
                    ? _config.accelerationTime
                    : _config.decelerationTime;

            Vector3 smoothedHorizontal = Vector3.SmoothDamp(currentHorizontal, targetHorizontal, ref _dampVelocity, smoothTime);

            _currentVelocity.x = smoothedHorizontal.x;
            _currentVelocity.z = smoothedHorizontal.z;

            if (!_isGrounded)
            {
                _currentVelocity.y -= _config.gravity * Time.deltaTime;
            }

            _characterController.Move(_currentVelocity * Time.deltaTime);
        }

        private void UpdateGroundCheck()
        {
            Vector3 feetPosition = transform.position + _characterController.center
                - Vector3.up * (_characterController.height / 2f - _characterController.radius);

            bool hit = Physics.SphereCast(
                feetPosition,
                _config.groundCheckRadius,
                Vector3.down, out RaycastHit hitInfo, _config.groundCheckDistance, _config.groundLayers);

            _isGrounded = hit;

            if (_isGrounded)
                _lastGroundedTime = Time.time;

            _canJump = _isGrounded || (Time.time - _lastGroundedTime <= _config.coyoteTime);
        }

        private void UpdateHeight(float deltaTime)
        {
            _currentHeight = Mathf.SmoothDamp(_currentHeight, _targetHeight, ref _heightVelocity, 1f / _config.crouchTransitionSpeed);
            Vector3 vectorHeight = new(0f, _currentHeight / 2f, 0f);

            _characterController.height = _currentHeight;
            _characterController.center = vectorHeight;

            Vector3 camPos = _cameraTransform.localPosition;
            camPos.y = _currentHeight - 0.1f;
            _cameraTransform.localPosition = camPos;


            // ** DELETE, CHANGE OR UPDATE ASAP 
            Transform playerModel = transform.GetChild(1);
            playerModel.localPosition = new Vector3(playerModel.localPosition.x,
                _currentHeight / 2f, playerModel.localPosition.z);
        }

        // Debug
#if UNITY_EDITOR
        public TMP_Text stateText;
        private void UpdateDebugText()
        {
            if (stateText == null) return;

            stateText.text = $"State: {_curState.GetType().Name}\n" +
                            $"Speed: {_currentVelocity.magnitude}\n" +
                            $"Target Speed: {_curState.GetTargetSpeed()}\n" +
                            $"Noise Profile: {_curState.GetNoiseProfile()}\n" +
                            $"Grounded: {_isGrounded}\n" +
                            $"Can Jump: {_canJump}\n" +
                            $"Can Slide: {!_slideCooldown}\n";
        }
#endif
    }
}
