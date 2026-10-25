using UnityEngine;

[CreateAssetMenu(fileName = "New Weapon Positions", menuName = "Weapons/New Weapon Position")]
public class WeaponPositions : ScriptableObject
{
    [Header("Weapon Values")]
    [Header("Idle Positions")]
    public Vector3 IdlePosition;
    public Vector3 IdleRotation;

    [Header("Aim Positions")]
    public Vector3 AimPosition;
    public Vector3 AimRotation;

    [Header("Crouch Positions")]
    public Vector3 CrouchPosition;
    public Vector3 CrouchRotation;

    [Header("Crouch Aim Positions")]
    public Vector3 CrouchAimPosition;
    public Vector3 CrouchAimRotation;

    [Header("IK Values")]
    [Header("Standing Positions")]
    public Vector3 RightHandStandingPosition;
    public Vector3 RightHandStandingRotation;
    public Vector3 RightHandHintPosition;
    public Vector3 LeftHandStandingPosition;
    public Vector3 LeftHandStandingRotation;
    public Vector3 LeftHandHintPosition;

    [Header("Crouch Positions")]
    public Vector3 RightHandCrouchPosition;
    public Vector3 RightHandCrouchRotation;
    public Vector3 RightHandCrouchHintPosition;
    public Vector3 LeftHandCrouchPosition;
    public Vector3 LeftHandCrouchRotation;
    public Vector3 LeftHandCrouchHintPosition;
}
