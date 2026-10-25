using System.Collections;
using Player;
using PlayerInput;
using UnityEngine;

public enum WeaponStates
{
    Idle,
    Aiming,
    Crouching,
    CrouchAiming
}

public abstract class RangedWeapon : MonoBehaviour, IAmmo
{
    [Header("Weapon Components")]
    [SerializeField] private WeaponPositions weaponPositions;
    [SerializeField] protected Camera playerCamera;
    [SerializeField] protected Animator weaponAnimator;

    [Header("IK Components")]
    [SerializeField] private Transform rightHandTarget; [SerializeField] private Transform leftHandTarget;
    [SerializeField] private Transform rightHandHint; [SerializeField] private Transform leftHandHint;

    [Header("Weapon Position Settings")]
    [SerializeField] private float positionSmoothSpeed = 12f;
    [SerializeField] private float rotationSmoothSpeed = 12f;

    [Header("Sway Settings")]
    [SerializeField] private float smooth;
    [SerializeField] private float swayMultiplier;

    private bool hasTarget;
    private bool hasIKTargets => rightHandTarget != null && leftHandTarget != null && rightHandHint != null && leftHandHint != null;

    private WeaponStates state = WeaponStates.Idle;
    public WeaponStates State => state; // Getter

    [Header("Ammunition Settings")]
    [SerializeField] protected int maxAmmo = 8;
    [SerializeField] protected int maxAmmoPerClip = 1;
    protected int _ammo;


    #region Unity Functions
    private void OnEnable()
    {
        if (InputManager.Instance != null)
            InputManager.Instance.OnReloadPressed += Reload;

    }

    private void OnDisable()
    {
        InputManager.Instance.OnReloadPressed -= Reload;
    }

    void Start()
    {
        _ammo = maxAmmoPerClip;
        AmmoManager.Instance.SetCurrentWeapon(this);
        InputManager.Instance.OnReloadPressed += Reload;
    }

    protected virtual void Update()
    {
        UpdateWeaponPosition();
        WeaponSway();
        AimManager();
    }
    #endregion

    private void Reload()
    {
        if (_ammo == maxAmmoPerClip || maxAmmo == 0 || AmmoManager.Instance._isReloading)
            return;

        int toAdd = maxAmmoPerClip - _ammo;
        StartCoroutine(ReloadCoroutine(toAdd));
    }

    private IEnumerator ReloadCoroutine(int amountToAdd)
    {
        AmmoManager.Instance.IsReloading(true);
        yield return new WaitForSeconds(1.5f);
        _ammo += amountToAdd;
        AmmoManager.Instance.IsReloading(false);
    }

    public void UpdateState(WeaponStates newState)
    {
        state = newState;
    }

    private void AimManager()
    {
        if (InputManager.Instance.IsAiming)
        {
            if (PlayerMovementController.Instance.CurrentState is PlayerCrouchState || PlayerMovementController.Instance.CurrentState is PlayerWalkState || PlayerMovementController.Instance.CurrentState is PlayerIdleState)
                UpdateState(PlayerMovementController.Instance.IsCrouching ? WeaponStates.CrouchAiming : WeaponStates.Aiming);
        }
        else
            UpdateState(PlayerMovementController.Instance.IsCrouching ? WeaponStates.Crouching : WeaponStates.Idle);

        if (PlayerMovementController.Instance.CurrentState is PlayerSprintState)
            UpdateState(WeaponStates.Idle);
        else if (PlayerMovementController.Instance.CurrentState is PlayerSlideState)
            UpdateState(WeaponStates.Crouching);
    }

    Vector3 targetPosition;
    Vector3 targetRotation;
    Vector3 rightHandTargetPosition; Vector3 rightHandTargetRotation;
    Vector3 rightHandHintPosition;
    Vector3 leftHandTargetPosition; Vector3 leftHandTargetRotation;
    Vector3 leftHandHintPosition;
    WeaponStates oldState;
    private void UpdateWeaponPosition()
    {
        if (!hasTarget || state != oldState)
        {
            switch (state)
            {
                case WeaponStates.Idle:
                    targetPosition = weaponPositions.IdlePosition;
                    targetRotation = weaponPositions.IdleRotation;
                    IKSetup();
                    break;
                case WeaponStates.Aiming:
                    targetPosition = weaponPositions.AimPosition;
                    targetRotation = weaponPositions.AimRotation;
                    IKSetup();
                    break;
                case WeaponStates.Crouching:
                    targetPosition = weaponPositions.CrouchPosition;
                    targetRotation = weaponPositions.CrouchRotation;
                    IKSetup();
                    break;
                case WeaponStates.CrouchAiming:
                    targetPosition = weaponPositions.CrouchAimPosition;
                    targetRotation = weaponPositions.CrouchAimRotation;
                    IKSetup();
                    break;
            }
            oldState = state;
            hasTarget = true;
        }

        float positionBlend = 1f - Mathf.Exp(-positionSmoothSpeed * Time.deltaTime);
        float rotationBlend = 1f - Mathf.Exp(-rotationSmoothSpeed * Time.deltaTime);

        if (targetPosition != Vector3.zero && targetRotation != Vector3.zero)
        {
            transform.localPosition = Vector3.Lerp(
                transform.localPosition,
                targetPosition,
                positionBlend);

            transform.localRotation = Quaternion.Slerp(
                transform.localRotation,
                Quaternion.Euler(targetRotation),
                rotationBlend);
        }

        // IK is applied independently so an unset weapon target never blocks it
        IKMovement(positionBlend, rotationBlend);

        void IKSetup()
        {
            if (!hasIKTargets)
                return;

            bool isCrouched = state == WeaponStates.Crouching || state == WeaponStates.CrouchAiming;
            if (isCrouched)
            {
                rightHandTargetPosition = weaponPositions.RightHandCrouchPosition;
                rightHandTargetRotation = weaponPositions.RightHandCrouchRotation;
                rightHandHintPosition = weaponPositions.RightHandCrouchHintPosition;
                leftHandTargetPosition = weaponPositions.LeftHandCrouchPosition;
                leftHandTargetRotation = weaponPositions.LeftHandCrouchRotation;
                leftHandHintPosition = weaponPositions.LeftHandCrouchHintPosition;
            }
            else
            {
                rightHandTargetPosition = weaponPositions.RightHandStandingPosition;
                rightHandTargetRotation = weaponPositions.RightHandStandingRotation;
                rightHandHintPosition = weaponPositions.RightHandHintPosition;
                leftHandTargetPosition = weaponPositions.LeftHandStandingPosition;
                leftHandTargetRotation = weaponPositions.LeftHandStandingRotation;
                leftHandHintPosition = weaponPositions.LeftHandHintPosition;
            }
        }

        void IKMovement(float positionBlend, float rotationBlend)
        {
            if (!hasIKTargets)
                return;

            rightHandTarget.localPosition = Vector3.Lerp(
                rightHandTarget.localPosition,
                rightHandTargetPosition,
                positionBlend);

            rightHandTarget.localRotation = Quaternion.Slerp(
                rightHandTarget.localRotation,
                Quaternion.Euler(rightHandTargetRotation),
                rotationBlend);

            rightHandHint.localPosition = Vector3.Lerp(
                rightHandHint.localPosition,
                rightHandHintPosition,
                positionBlend);

            leftHandTarget.localPosition = Vector3.Lerp(
                leftHandTarget.localPosition,
                leftHandTargetPosition,
                positionBlend);

            leftHandTarget.localRotation = Quaternion.Slerp(
                leftHandTarget.localRotation,
                Quaternion.Euler(leftHandTargetRotation),
                rotationBlend);

            leftHandHint.localPosition = Vector3.Lerp(
                leftHandHint.localPosition,
                leftHandHintPosition,
                positionBlend);
        }
    }

    private void WeaponSway()
    {
        Vector2 lookInput = InputManager.Instance.LookInput;
        float mouseX = lookInput.x * swayMultiplier;
        float mouseY = lookInput.y * swayMultiplier;

        Quaternion rotationX = Quaternion.AngleAxis(-mouseY, Vector3.right);
        Quaternion rotationY = Quaternion.AngleAxis(mouseX, Vector3.up);

        Quaternion targetRotation = rotationX * rotationY;

        transform.localRotation = Quaternion.Slerp(transform.localRotation, targetRotation, smooth * Time.deltaTime);
    }

    public abstract void Shoot();

    #region IAmmo Interface
    public bool CanShoot() => _ammo > 0;

    public int GetAmmo() => _ammo;

    public int GetMaxAmmo() => maxAmmo;

    public int GetMaxAmmoPerClip() => maxAmmoPerClip;
    #endregion
}
