using UnityEngine;

[CreateAssetMenu(fileName = "New Weapon Positions", menuName = "Weapons/New Weapon Position")]
public class WeaponPositions : ScriptableObject
{
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
}
