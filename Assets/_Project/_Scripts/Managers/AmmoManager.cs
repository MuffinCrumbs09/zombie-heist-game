using UnityEngine;
using TMPro;

// Will be continued to handle picking up ammo and adding it to the current weapon, using IAmmo.
public class AmmoManager : MonoBehaviour
{
    public static AmmoManager Instance { get; private set; }

    [Header("References")]
    [SerializeField] private TMP_Text ammoText;
    [SerializeField] private TMP_Text reloadingText;

    [Header("Settings")]
    [SerializeField] private float flashSpeed = 1f;

    public bool _isReloading { get; private set; }

    [HideInInspector] public IAmmo _curWeapon { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
            Destroy(this);

        Instance = this;
    }

    private void Update()
    {
        ammoText.text = _curWeapon.GetAmmoText();
        reloadingText.alpha = _isReloading ? Mathf.PingPong(Time.time * flashSpeed, 1f) : 0;
    }

    public void IsReloading(bool toggle)
    {
        _isReloading = toggle;
    }

    public void SetCurrentWeapon(IAmmo newWeapon) => _curWeapon = newWeapon;
}