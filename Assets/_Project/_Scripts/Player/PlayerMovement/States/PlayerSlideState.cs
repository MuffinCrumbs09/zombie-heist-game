using UnityEditorInternal;
using UnityEngine;

namespace Player
{
    public class PlayerSlideState : IMovementState
    {
        PlayerMovementConfig config;

        Vector3 slideDirection;
        float currentSlideSpeed;
        float slideTimer;
        bool hasEndedEarly;

        public void EnterState(PlayerMovementController playerMovement)
        {
            config = playerMovement.Config;
            slideDirection = playerMovement.HorizontalVelocity.normalized;
            if (slideDirection.sqrMagnitude < 0.0001f)
            {
                slideDirection = playerMovement.transform.forward;
            }

            currentSlideSpeed = config.slideSpeedImpulse;
            slideTimer = 0;
            hasEndedEarly = false;

            playerMovement.SetTargetHeight(config.slideCameraHeight);
        }
        public void UpdateState(PlayerMovementController playerMovement, float deltaTime)
        {
            slideTimer += deltaTime;

            float t = Mathf.Clamp01(slideTimer / config.slideDuration);
            currentSlideSpeed = Mathf.Lerp(config.slideSpeedImpulse, 0, t);

            bool slideHeld = PlayerInput.InputManager.Instance.IsSliding;

            if (slideTimer >= config.slideDuration)
                TransitionOut(playerMovement);
            else if (!slideHeld)
                TransitionOut(playerMovement);
        }
        public void ExitState(IMovementState newState, PlayerMovementController playerMovement)
        {
            playerMovement.StartSlideCooldown();

            if (newState is not PlayerCrouchState && newState is not PlayerSlideState)
            {
                playerMovement.SetTargetHeight(config.standingCameraHeight);
            }
        }

        public float GetTargetSpeed()
        {
            return currentSlideSpeed;
        }
        public NoiseProfile GetNoiseProfile() => NoiseProfile.None;

        private void TransitionOut(PlayerMovementController playerMovement)
        {
            bool crouchHeld = PlayerInput.InputManager.Instance.IsCrouching;
            bool canStand = CanStandUp(playerMovement);

            if (crouchHeld || !canStand)
            {
                playerMovement.SwitchState(new PlayerCrouchState());
                return;
            }

            Vector2 moveInput = PlayerInput.InputManager.Instance.MoveInput;
            if (moveInput.sqrMagnitude < 0.0001f)
            {
                playerMovement.SwitchState(new PlayerIdleState());
            }
            else
            {
                playerMovement.SwitchState(new PlayerWalkState());
            }
        }

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
            return config.fovBase + config.fovAds;
        }
    }
}