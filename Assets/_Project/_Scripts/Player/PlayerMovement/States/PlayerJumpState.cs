using UnityEngine;

namespace Player
{
    public class PlayerJumpState : IMovementState
    {
        PlayerMovementConfig config;
        private float _launchSpeed;

        public void EnterState(PlayerMovementController playerMovement)
        {
            config = playerMovement.Config;

            playerMovement.VerticalVelocity = Mathf.Sqrt(2f * config.gravity * config.jumpHeight);
            _launchSpeed = playerMovement.HorizontalSpeed;
            playerMovement.AirborneStartHeight = playerMovement.transform.position.y;
        }

        public void UpdateState(PlayerMovementController playerMovement, float deltaTime)
        {
            // Track peak height reached this airborne phase, so landing is more accurate
            playerMovement.AirborneStartHeight = Mathf.Max(playerMovement.AirborneStartHeight,
                playerMovement.transform.position.y);

            if (playerMovement.IsGrounded && playerMovement.VerticalVelocity <= 0f)
            {
                playerMovement.SwitchState(new PlayerLandingState());
            }
        }

        public void ExitState(IMovementState newState, PlayerMovementController playerMovement)
        {

        }

        public float GetTargetSpeed()
        {
            return _launchSpeed;
        }

        public NoiseProfile GetNoiseProfile()
        {
            // Temporary null return since noise effects arent implemented yet
            return NoiseProfile.None;
        }

        public float GetTargetFOV()
        {
            return config.fovBase;
        }
    }
}