using UnityEngine;
using PlayerInput;

public class BowController : MonoBehaviour
{
    public GameObject arrowPrefab;
    public Transform arrowSpawnPoint;
    public Camera playerCamera;

    public float minForce = 5f;
    public float maxForce = 40f;
    public float maxDrawTime = 2f;

    private float drawStartTime;
    private bool isDrawing = false;

    private void Update()
    {
        if(InputManager.Instance.IsFiring && !isDrawing)
        {
            isDrawing = true;
            drawStartTime = Time.time;
        }

        if(!InputManager.Instance.IsFiring && isDrawing)
        {
            float drawTime = Mathf.Clamp(Time.time - drawStartTime, 0, maxDrawTime);
            float t = drawTime / maxDrawTime;
            float foce = Mathf.Lerp(minForce, maxForce, t);

            ShootArrow(foce);
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
