using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Arrow : MonoBehaviour
{
    private Rigidbody _rb;
    [SerializeField] private float rotationSpeed = 10f;
    [SerializeField, Tooltip("Lifetime of the arrow in seconds, once landed")] private float lifeTime = 10f;
    private bool hasCollided = false;
    [SerializeField, Tooltip("Tags of objects to ignore collisions with")] private string[] ignoreTags;

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
        foreach (string tag in ignoreTags)
        {
            string collisionTag = collision.gameObject.tag.ToLower();
            if (collisionTag == tag.ToLower()) return;
        }

        hasCollided = true;
        _rb.isKinematic = true; // stop it dead, stick in place
        transform.SetParent(collision.transform); // parents to the object it hit, so it moves with it
        GetComponent<Collider>().enabled = false; // disable the collider so it doesn't interfere with other physics
        StartCoroutine(DestroyAfterTime());
    }

    private IEnumerator DestroyAfterTime()
    {
        yield return new WaitForSeconds(lifeTime);
        Destroy(gameObject);
    }
}
