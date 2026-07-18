using UnityEngine;
using PlayerInput;

namespace Player
{
    public class PlayerWalkState : IMovementState
    {
        PlayerMovementConfig config;

        public void EnterState(PlayerMovementController playerMovement)
        {
            config = playerMovement.Config;
        }

        public void UpdateState(PlayerMovementController playerMovement, float deltaTime)
        {
            // State Checks
            if (InputManager.Instance.IsSprinting)
                playerMovement.SwitchState(new PlayerSprintState());
            if (InputManager.Instance.MoveInput.magnitude == 0)
                playerMovement.SwitchState(new PlayerIdleState());
            if (InputManager.Instance.IsCrouching)
                playerMovement.SwitchState(new PlayerCrouchState());
        }

        public void ExitState(IMovementState newState, PlayerMovementController playerMovement)
        {

        }

        public float GetTargetSpeed()
        {
            return config.walkSpeed;
        }

        public NoiseProfile GetNoiseProfile()
        {
            return config.walkNoiseProfile;
        }

        public float GetTargetFOV()
        {
            return config.fovBase;
        }
    }
}