using UnityEngine;
using PlayerInput;

public class BowController : MonoBehaviour
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

    private float drawStartTime;
    private bool isDrawing = false;

    private void Update()
    {
        bowAnimator.SetBool(BowIsFiringHash, InputManager.Instance.IsFiring);
        viewArrowModel.SetActive(isDrawing);

        if(InputManager.Instance.IsFiring && !isDrawing)
        {
            isDrawing = true;
            drawStartTime = Time.time;
        }

        if(!InputManager.Instance.IsFiring && isDrawing)
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
        GameObject arrow = Instantiate(arrowPrefab, arrowSpawnPoint.position, playerCamera.transform.rotation);
        Rigidbody rb = arrow.GetComponent<Rigidbody>();
        rb.AddForce(playerCamera.transform.forward * force, ForceMode.VelocityChange);
    }
}
