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
    [SerializeField] private WeaponPositions weaponPositions;
    [SerializeField] protected Camera playerCamera;
    [SerializeField] protected Animator weaponAnimator;

    [SerializeField] private float positionSmoothSpeed = 12f;
    [SerializeField] private float rotationSmoothSpeed = 12f;

    private bool hasTarget;

    private WeaponStates state = WeaponStates.Idle;
    public WeaponStates State => state; // Getter

    // Ammunition
    protected int _ammo;
    [SerializeField] protected int maxAmmo = 8;
    [SerializeField] protected int maxAmmoPerClip = 1;

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

        if (PlayerMovementController.Instance.CurrentState is PlayerSprintState || PlayerMovementController.Instance.CurrentState is PlayerSlideState)
            UpdateState(WeaponStates.Idle);
    }

    Vector3 targetPosition;
    Vector3 targetRotation;
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
                    break;
                case WeaponStates.Aiming:
                    targetPosition = weaponPositions.AimPosition;
                    targetRotation = weaponPositions.AimRotation;
                    break;
                case WeaponStates.Crouching:
                    targetPosition = weaponPositions.CrouchPosition;
                    targetRotation = weaponPositions.CrouchRotation;
                    break;
                case WeaponStates.CrouchAiming:
                    targetPosition = weaponPositions.CrouchAimPosition;
                    targetRotation = weaponPositions.CrouchAimRotation;
                    break;
            }
            oldState = state;
            hasTarget = true;
        }

        if (targetPosition == Vector3.zero || targetRotation == Vector3.zero)
            return;

        float positionBlend = 1f - Mathf.Exp(-positionSmoothSpeed * Time.deltaTime);
        float rotationBlend = 1f - Mathf.Exp(-rotationSmoothSpeed * Time.deltaTime);

        transform.localPosition = Vector3.Lerp(
            transform.localPosition,
            targetPosition,
            positionBlend);

        transform.localRotation = Quaternion.Slerp(
            transform.localRotation,
            Quaternion.Euler(targetRotation),
            rotationBlend);

    }

    public abstract void Shoot();

    #region IAmmo Interface
    public bool CanShoot() => _ammo > 0;

    public int GetAmmo() => _ammo;

    public int GetMaxAmmo() => maxAmmo;

    public int GetMaxAmmoPerClip() => maxAmmoPerClip;
    #endregion
}
