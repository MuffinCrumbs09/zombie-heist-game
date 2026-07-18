using UnityEngine;

namespace Player
{
    public class PlayerCrouchState : IMovementState
    {
        PlayerMovementConfig config;

        public void EnterState(PlayerMovementController playerMovement)
        {
            config = playerMovement.Config;
            playerMovement.SetTargetHeight(config.crouchCameraHeight);
        }

        public void UpdateState(PlayerMovementController playerMovement, float deltaTime)
        {
            if (!PlayerInput.InputManager.Instance.IsCrouching && CanStandUp(playerMovement))
            {
                Vector3 moveInput = PlayerInput.InputManager.Instance.MoveInput;
                playerMovement.SwitchState(moveInput.sqrMagnitude > 0.01f ?
                    new PlayerWalkState() : new PlayerIdleState());
            }
        }

        public void ExitState(IMovementState newState, PlayerMovementController playerMovement)
        {
            if (newState is PlayerIdleState || newState is PlayerWalkState || newState is PlayerSprintState)
            {
                playerMovement.SetTargetHeight(config.standingCameraHeight);
            }
        }

        public NoiseProfile GetNoiseProfile() => NoiseProfile.None;
        public float GetTargetSpeed() => config.crouchSpeed;

        private bool CanStandUp(PlayerMovementController playerMovement)
        {
            Vector3 origin = playerMovement.transform.position + Vector3.up * config.crouchCameraHeight;
            float checkDistance = config.standingCameraHeight - config.crouchCameraHeight;

            bool blocked = Physics.SphereCast(
                origin,
                playerMovement.GetComponent<CharacterController>().radius * 0.9f,
                Vector3.up,
                out _,
                checkDistance,
                config.groundLayers // Later update to check for NEW obsutruction layer AND ground layer. This will suffice for now
            );

            return !blocked;
        }

        public float GetTargetFOV()
        {
            return config.fovBase - 5;
        }
    }
}