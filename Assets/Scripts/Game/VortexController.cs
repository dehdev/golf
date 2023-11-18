using UnityEngine;

public class VortexController : MonoBehaviour
{

    private Collider vortexCollider;
    [SerializeField] private float VortexForce;
    void Awake()
    {
        vortexCollider = GetComponent<Collider>();
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Vector3 normal = other.transform.position - vortexCollider.bounds.center;
            other.attachedRigidbody.AddForce(normal * -VortexForce);
        }
    }

}
