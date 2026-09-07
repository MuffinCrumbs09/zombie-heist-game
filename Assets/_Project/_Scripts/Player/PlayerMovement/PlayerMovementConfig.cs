using UnityEngine;

namespace Player
{
    [CreateAssetMenu(fileName = "New PlayerMovementConfig", menuName = "ZHG/Player Movement Config")]
    public class PlayerMovementConfig : ScriptableObject
    {
        [Header("Ground Speeds")]
        [Tooltip("Base walking speed")]
        public float walkSpeed = 4.0f;

        [Tooltip("Sprint speed")]
        public float sprintSpeed = 7.0f;

        [Tooltip("Crouch movement speed")]
        public float crouchSpeed = 2.0f;

        [Tooltip("Multiplier applied to current ground speed while aiming down sights")]
        [Range(0.1f, 1f)]
        public float adsSpeedMultiplier = 0.5f;

        [Header("Acceleration / Deceleration")]
        [Tooltip("Time in seconds to reach target speed when speeding up")]
        public float accelerationTime = 0.2f;

        [Tooltip("Time in seconds to reach target speed when slowing down/stopping")]
        public float decelerationTime = 0.1f;

        [Tooltip("Separate, slower acceleration time used while in ADS state")]
        public float adsAccelerationTime = 0.3f;

        [Header("Jump / Gravity")]
        [Tooltip("Peak jump height")]
        public float jumpHeight = 1.1f;

        [Tooltip("Gravity magnitude applied while airborne (positive value, subtracted from vertical velocity)")]
        public float gravity = 18f;

        [Tooltip("Grace period in seconds after leaving ground where a jump is still allowed")]
        public float coyoteTime = 0.12f;

        [Header("Slide")]
        [Tooltip("Initial velocity impulse applied when a slide starts")]
        public float slideSpeedImpulse = 9f;

        [Tooltip("Duration in seconds over which slide velocity decays to zero")]
        public float slideDuration = 0.5f;

        [Tooltip("Camera/collider height while sliding")]
        public float slideCameraHeight = 0.6f;

        [Header("Crouch")]
        [Tooltip("Camera/collider height while crouching")]
        public float crouchCameraHeight = 1.1f;

        [Tooltip("Camera/collider height while standing")]
        public float standingCameraHeight = 1.8f;

        [Tooltip("Speed of the smooth height transition when entering/exiting crouch or slide.")]
        public float crouchTransitionSpeed = 8f;

        [Header("Ground Check")]
        [Tooltip("Radius of the spherecast used for manual ground detection")]
        public float groundCheckRadius = 0.25f;

        [Tooltip("Max distance below the feet to check for ground")]
        public float groundCheckDistance = 0.15f;

        [Tooltip("Layers considered walkable ground")]
        public LayerMask groundLayers;

        [Header("Landing")]
        [Tooltip("Seconds of reduced control after a landing before returning to normal movement.")]
        public float landingRecoveryDuration = 0.15f;

        [Header("Camera - Head Bob")]
        [Tooltip("Vertical bob amplitude in meters at full move speed")]
        public float headBobAmplitude = 0.05f;

        [Tooltip("Bob cycle frequency multiplier, tied to movement speed")]
        public float headBobFrequency = 1.5f;

        [Tooltip("Smoothing time used to blend bob offset back to zero when stopping")]
        public float headBobDampTime = 0.15f;

        [Header("Camera - Sway")]
        [Tooltip("How strongly mouse/look delta tilts the camera (procedural sway)")]
        public float swaySensitivity = 0.6f;

        [Tooltip("Smoothing time for sway lag (Slerp/SmoothDamp toward target tilt)")]
        public float swaySmoothTime = 0.08f;

        [Tooltip("Maximum sway tilt angle in degrees, clamps the effect")]
        public float maxSwayAngle = 4f;

        [Header("Camera - FOV")]
        [Tooltip("Default hip-fire field of view")]
        public float fovBase = 90f;

        [Tooltip("Degrees added to base FOV while sprinting")]
        public float fovSprintKick = 8f;

        [Tooltip("Target FOV while aiming down sights")]
        public float fovAds = 55f;

        [Tooltip("Lerp speed for all FOV transitions")]
        public float fovLerpSpeed = 8f;

        [Header("Camera - Landing Dip")]
        [Tooltip("Fall distance (meters) below which no landing dip/noise is triggered")]
        public float minFallDistanceForDip = 1.0f;

        [Tooltip("Fall distance (meters) at or above which the landing is treated as loud")]
        public float loudLandingFallDistance = 3.5f;

        [Tooltip("Max downward camera offset on landing, meters, scaled by fall distance")]
        public float landingDipStrength = 0.15f;

        [Tooltip("Speed at which the landing dip recovers back to neutral")]
        public float landingDipRecoverySpeed = 6f;

        [Header("Noise Profiles (Heat System Hooks)")]
        [Tooltip("Noise radius/heat weight while walking or crouched")]
        public NoiseProfile walkNoiseProfile = new NoiseProfile(NoiseRadius.Minimal, HeatWeight.None, false);

        [Tooltip("Noise radius/heat weight while sprinting")]
        public NoiseProfile sprintNoiseProfile = new NoiseProfile(NoiseRadius.Medium, HeatWeight.Moderate, true);

        [Tooltip("Noise radius/heat weight while sliding")]
        public NoiseProfile slideNoiseProfile = new NoiseProfile(NoiseRadius.Small, HeatWeight.Low, true);

        [Tooltip("Noise radius/heat weight for a loud landing (fall distance >= loudLandingFallDistance)")]
        public NoiseProfile loudLandingNoiseProfile = new NoiseProfile(NoiseRadius.Large, HeatWeight.High, false);
    }

    /// <summary>
    /// Radius bucket for a noise event, consumed by the Heat/Noise system.
    /// </summary>
    public enum NoiseRadius
    {
        Minimal,
        Small,
        Medium,
        Large
    }

    /// <summary>
    /// How much a noise event contributes to Local/Zone Heat.
    /// </summary>
    public enum HeatWeight
    {
        None,
        Low,
        Moderate,
        High
    }

    /// <summary>
    /// Describes the noise a given movement state emits, and whether it
    /// leaves a scent trail for Tracker-type enemies to follow.
    /// </summary>
    [System.Serializable]
    public struct NoiseProfile
    {
        public NoiseRadius radius;
        public HeatWeight heatWeight;
        public bool leavesScentTrail;

        public NoiseProfile(NoiseRadius radius, HeatWeight heatWeight, bool leavesScentTrail)
        {
            this.radius = radius;
            this.heatWeight = heatWeight;
            this.leavesScentTrail = leavesScentTrail;
        }

        // <summary> Static property for a "silent" noise profile, used for states that don't emit any noise or scent trail. </summary>
        public static NoiseProfile None => new NoiseProfile(NoiseRadius.Minimal, HeatWeight.None, false);
        public bool IsSilent => radius == NoiseRadius.Minimal && heatWeight == HeatWeight.None && !leavesScentTrail;
    }
}