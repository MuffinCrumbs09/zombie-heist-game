using UnityEngine;
using PlayerInput;
using Unity.VisualScripting;

namespace Player
{
    public class PlayerIdleState : IMovementState
    {
        PlayerMovementConfig config;
        public void EnterState(PlayerMovementController playerMovement)
        {
            config = playerMovement.Config;
        }

        public void UpdateState(PlayerMovementController playerMovement, float deltaTime)
        {
            if(InputManager.Instance.MoveInput.magnitude > 0)
                playerMovement.SwitchState(new PlayerWalkState());
            if(InputManager.Instance.IsCrouching)
                playerMovement.SwitchState(new PlayerCrouchState());
        }

        public void ExitState(IMovementState newState, PlayerMovementController playerMovement)
        {

        }

        public float GetTargetSpeed()
        {
            return 0f;
        }

        public NoiseProfile GetNoiseProfile()
        {
            return NoiseProfile.None;
        }

        public float GetTargetFOV()
        {
            return InputManager.Instance.IsAiming ? config.fovAds : config.fovBase; 
        }
    }
}