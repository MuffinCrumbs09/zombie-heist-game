using UnityEngine;
using PlayerInput;

namespace Player
{
    public class PlayerSprintState : IMovementState
    {
        PlayerMovementConfig config;

        public void EnterState(PlayerMovementController playerMovement)
        {
            config = playerMovement.Config;
        }

        public void UpdateState(PlayerMovementController playerMovement, float deltaTime)
        {
            // State Checks

            if (!InputManager.Instance.IsSprinting)
                playerMovement.SwitchState(new PlayerWalkState());
            if (InputManager.Instance.MoveInput.magnitude == 0)
                playerMovement.SwitchState(new PlayerIdleState());
            if (InputManager.Instance.IsCrouching)
                playerMovement.SwitchState(new PlayerCrouchState());
            if(InputManager.Instance.IsSliding && !playerMovement.SlideCooldown)
                playerMovement.SwitchState(new PlayerSlideState());
        }

        public void ExitState(IMovementState newState, PlayerMovementController playerMovement)
        {

        }

        public float GetTargetSpeed()
        {
            return config.sprintSpeed;
        }

        public NoiseProfile GetNoiseProfile()
        {
            return config.sprintNoiseProfile;
        }

        public float GetTargetFOV()
        {
            return config.fovBase + config.fovSprintKick;
        }
    }
}