using UnityEngine;

namespace Player
{
    public class PlayerLandingState : IMovementState
    {
        PlayerMovementConfig config;
        private float _fallDistance;
        private float _timer;
        private bool _isLoudLanding;

        public void EnterState(PlayerMovementController playerMovement)
        {
            config = playerMovement.Config;

            _fallDistance = playerMovement.AirborneStartHeight - playerMovement.transform.position.y;
            _isLoudLanding = _fallDistance > config.loudLandingFallDistance;
            _timer = 0f;

            // Reset to 0, instead of ~0, so no fall speed is carried to the next state
            playerMovement.VerticalVelocity = 0f;
        }

        public void UpdateState(PlayerMovementController playerMovement, float deltaTime)
        {
            _timer += deltaTime;

            if (_timer >= config.landingRecoveryDuration)
            {
                Vector3 moveInput = PlayerInput.InputManager.Instance.MoveInput;
                playerMovement.SwitchState(
                    moveInput.sqrMagnitude > 0.01f ? new PlayerWalkState() : new PlayerIdleState()
                );
            }
        }

        public void ExitState(IMovementState newState, PlayerMovementController playerMovement)
        {

        }

        public float GetTargetSpeed()
        {
            return config.walkSpeed * 0.5f;
        }

        public NoiseProfile GetNoiseProfile()
        {
            // Only report a noise event on the landing frame
            if (_timer > 0f) return NoiseProfile.None;

            // return _isLoudLanding ? _config.loudLandingNoiseProfile : NoiseProfile.None;
            // Temporary null return since noise effects arent implemented yet
            return NoiseProfile.None;
        }

        public float GetTargetFOV()
        {
            return config.fovBase - 10;
        }
    }
}