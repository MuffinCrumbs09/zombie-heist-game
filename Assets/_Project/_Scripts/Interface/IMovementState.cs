using UnityEngine;

namespace Player
{
    public interface IMovementState
    {
        void EnterState(PlayerMovementController playerMovement);
        void UpdateState(PlayerMovementController playerMovement, float deltaTime);
        void ExitState(IMovementState newState, PlayerMovementController playerMovement);

        float GetTargetSpeed();
        float GetTargetFOV();
        NoiseProfile GetNoiseProfile();
    }
}