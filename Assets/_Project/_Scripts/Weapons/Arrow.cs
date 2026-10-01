using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Arrow : MonoBehaviour
{
    private Rigidbody _rb;
    public float rotationSpeed = 10f;
    private bool hasCollided = false;

    private void Start()
    {
        _rb = GetComponent<Rigidbody>();
        _rb.useGravity = true;
        _rb.linearDamping = 0f;
    }

    private void FixedUpdate()
    {
        if (hasCollided) return;

        // Rotate the arrow to face the direction of its velocity
        if (_rb.linearVelocity.sqrMagnitude > 0.1f)
        {
            Quaternion toRotation = Quaternion.LookRotation(_rb.linearVelocity, Vector3.up);
            transform.rotation = Quaternion.Slerp(transform.rotation, toRotation, rotationSpeed * Time.fixedDeltaTime);
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.CompareTag("Player")) return; // don't stick to player
        
        hasCollided = true;
        _rb.isKinematic = true; // stop it dead, stick in place
        transform.SetParent(collision.transform); // parents to the object it hit, so it moves with it
        GetComponent<Collider>().enabled = false; // disable the collider so it doesn't interfere with other physics
    }
}