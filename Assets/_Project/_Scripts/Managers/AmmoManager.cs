using UnityEngine;
using TMPro;

// Will be continued to handle picking up ammo and adding it to the current weapon, using IAmmo.
public class AmmoManager : MonoBehaviour
{
    public static AmmoManager Instance { get; private set; }
    [SerializeField] private TMP_Text ammoText;

    private IAmmo _curWeapon;

    private void Awake()
    {
        if(Instance != null && Instance != this)
            Destroy(this);
        
        Instance = this;
    }

    private void Update()
    {
        ammoText.text = _curWeapon.GetAmmoText();
    }

    public void SetCurrentWeapon(IAmmo newWeapon) => _curWeapon = newWeapon;
}