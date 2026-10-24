using UnityEngine;
using PlayerInput;
using System.Collections;

public class BowController : RangedWeapon
{
    public readonly static int BowIsFiringHash = Animator.StringToHash("IsFiring");

    [Header("Bow Settings")]
    [SerializeField] private GameObject arrowPrefab;
    [SerializeField] private Transform arrowSpawnPoint;
    [SerializeField] private GameObject viewArrowModel;

    [SerializeField] private float minForce = 5f;
    [SerializeField] private float maxForce = 40f;
    [SerializeField] private float maxDrawTime = 2f;

    private float drawStartTime;
    private bool isDrawing = false;

    private float _force = 0;

    protected override void Update()
    {
        base.Update();
        weaponAnimator.SetBool(BowIsFiringHash, InputManager.Instance.IsFiring && _ammo > 0);
        viewArrowModel.SetActive(_ammo > 0);

        if (InputManager.Instance.IsFiring && !isDrawing && CanShoot())
        {
            isDrawing = true;
            drawStartTime = Time.time;
        }

        if (!InputManager.Instance.IsFiring && isDrawing)
        {
            float drawTime = Mathf.Clamp(Time.time - drawStartTime, 0, maxDrawTime);
            float t = drawTime / maxDrawTime;
            _force = Mathf.Lerp(minForce, maxForce, t);

            Debug.Log($"Draw Time: {drawTime}, T: {t}, Force: {_force}");

            Shoot();
            isDrawing = false;
        }
    }

    public override void Shoot()
    {
        _ammo -= 1; maxAmmo -= 1;

        GameObject arrow = Instantiate(arrowPrefab, arrowSpawnPoint.position, playerCamera.transform.rotation);
        Rigidbody rb = arrow.GetComponent<Rigidbody>();
        rb.AddForce(playerCamera.transform.forward * _force, ForceMode.VelocityChange);
    }
}
