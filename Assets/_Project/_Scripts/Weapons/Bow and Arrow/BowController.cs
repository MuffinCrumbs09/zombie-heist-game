using UnityEngine;
using PlayerInput;
using System.Collections;

public enum WeaponStates
{
    Idle,
    Aiming,
    Crouching,
    CrouchAiming
}

public class BowController : MonoBehaviour, IAmmo
{
    public readonly static int BowIsFiringHash = Animator.StringToHash("IsFiring");

    [SerializeField] private GameObject arrowPrefab;
    [SerializeField] private Transform arrowSpawnPoint;
    [SerializeField] private Camera playerCamera;
    [SerializeField] private Animator bowAnimator;
    [SerializeField] private GameObject viewArrowModel;

    [SerializeField] private float minForce = 5f;
    [SerializeField] private float maxForce = 40f;
    [SerializeField] private float maxDrawTime = 2f;

    [SerializeField] private WeaponPositions weaponPositions;
    private WeaponStates state = WeaponStates.Idle;

    private float drawStartTime;
    private bool isDrawing = false;

    #region Ammunition
    private int _ammo;
    [SerializeField] private int maxAmmo = 8;
    [SerializeField] private int maxAmmoPerClip = 1;
    #endregion

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

    private void Update()
    {
        bowAnimator.SetBool(BowIsFiringHash, InputManager.Instance.IsFiring && _ammo > 0);
        viewArrowModel.SetActive(_ammo > 0);
        UpdateWeaponPosition();

        if (InputManager.Instance.IsFiring && !isDrawing && CanShoot())
        {
            isDrawing = true;
            drawStartTime = Time.time;
        }

        if (!InputManager.Instance.IsFiring && isDrawing)
        {
            float drawTime = Mathf.Clamp(Time.time - drawStartTime, 0, maxDrawTime);
            float t = drawTime / maxDrawTime;
            float force = Mathf.Lerp(minForce, maxForce, t);

            Debug.Log($"Draw Time: {drawTime}, T: {t}, Force: {force}");

            ShootArrow(force);
            isDrawing = false;
        }
    }

    private void ShootArrow(float force)
    {
        _ammo -= 1; maxAmmo -= 1;

        GameObject arrow = Instantiate(arrowPrefab, arrowSpawnPoint.position, playerCamera.transform.rotation);
        Rigidbody rb = arrow.GetComponent<Rigidbody>();
        rb.AddForce(playerCamera.transform.forward * force, ForceMode.VelocityChange);
    }

    private void Reload()
    {
        if (_ammo == maxAmmoPerClip || maxAmmo == 0)
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

    Vector3 targetPosition;
    Vector3 targetRotation;
    WeaponStates oldState;
    private void UpdateWeaponPosition()
    {
        if (state != oldState)
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
        }

        if(targetPosition == Vector3.zero || targetRotation == Vector3.zero)
            return;

        transform.localPosition = targetPosition;
        transform.localEulerAngles = targetRotation;
    }

    public void UpdateState(WeaponStates newState)
    {
        state= newState;
    }

    private bool CanShoot() => _ammo > 0;

    public int GetAmmo() => _ammo;

    public int GetMaxAmmo() => maxAmmo;

    public int GetMaxAmmoPerClip() => maxAmmoPerClip;
}
