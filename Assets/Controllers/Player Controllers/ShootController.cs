using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class ShootController : MonoBehaviour
{
    [SerializeField] private float shotPower;
    [SerializeField] private float stopDuration = 5;
    [SerializeField] private float stopVelocity = .05f; //The velocity below which the rigidbody will be considered as stopped
    [SerializeField] private AudioSource hitSound;
    [SerializeField] private float MaxDragDistance = 30f;
    [SerializeField] private UnityEvent<string> shotEvent;
    [SerializeField] private UnityEvent<string> timerEvent;
    [SerializeField] private LineRenderer lineRenderer;
    [SerializeField] private GameObject idleParticles;
    [SerializeField] private GameObject arrow;

    [SerializeField] private float iddleEffectDistance;

    private int shots;
    private float time;
    private string timeText;
    private Vector3 lastPos;
    private bool isIdle;
    private bool isAiming;
    private bool readyToShoot;
    private Rigidbody rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        hitSound = GetComponent<AudioSource>();
        readyToShoot = false;
        isAiming = false;
        lineRenderer.enabled = false;
        idleParticles.SetActive(false);
    }

    private void Update()
    {
        arrow.transform.position = new Vector3(transform.position.x, transform.position.y, transform.position.z + 2);
        if (rb.velocity.magnitude < stopVelocity)
        {
            if (!isAiming)
            {
                idleParticles.transform.position = new Vector3(transform.position.x, transform.position.y - iddleEffectDistance, transform.position.z);
                idleParticles.SetActive(true);
            }
            isIdle = true;
            StartCoroutine(Stop());
        }
        UpdateTimer();
        if (Input.GetKey(KeyCode.R))
        {
            transform.position = new Vector3(6, 7, 5);
            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }
    }

    private void FixedUpdate()
    {
        ProcessAim();
    }

    private void OnMouseUp()
    {
        if (isIdle)
        {
            readyToShoot = true;
        }
    }

    private void OnMouseDown()
    {
        if (isIdle)
        {
            idleParticles.SetActive(false);
            Cursor.visible = false;
            isAiming = true;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Terrain")
        {
            transform.position = lastPos;
            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }
    }

    private void UpdateTimer()
    {
        time += Time.deltaTime;
        float seconds = Mathf.FloorToInt(time % 60);
        float minutes = Mathf.FloorToInt(time / 60);
        timeText = string.Format("{0:00}:{1:00}", minutes, seconds);
        timerEvent.Invoke(timeText.ToString());
    }

    private void ProcessAim()
    {

        if (!isAiming || !isIdle)
        {
            return;
        }

        Vector3? worldPoint = CastMouseClickRay();

        if (!worldPoint.HasValue)
        {
            return;
        }

        DrawLine(worldPoint.Value);
        if (readyToShoot)
        {
            readyToShoot = false;
            Cursor.visible = true;
            Shoot(worldPoint.Value);
        }
    }

    private void Shoot(Vector3 worldPoint)
    {
        lastPos = transform.position;
        hitSound.Play();
        shots++;
        shotEvent.Invoke("Shots: " + shots.ToString());
        isAiming = false;
        lineRenderer.enabled = false;

        Vector3 horizontalWorldPoint = new Vector3(worldPoint.x, transform.position.y, worldPoint.z);

        Vector3 direction = (horizontalWorldPoint - transform.position).normalized;

        float strength = Mathf.Clamp(Vector3.Distance(transform.position, horizontalWorldPoint), 0, MaxDragDistance);

        rb.AddForce(-direction * strength * shotPower);
        isIdle = false;

        Debug.Log("Force: " + (-direction * strength * shotPower).magnitude);
    }

    private void DrawLine(Vector3 worldPoint)
    {

        // Calculate the direction from the current position to the worldPoint
        Vector3 direction = worldPoint - transform.position;

        // Clamp the magnitude of the direction vector to not exceed maxDragDistance
        Vector3 clampedDirection = Vector3.ClampMagnitude(direction, MaxDragDistance);

        // Calculate the clamped worldPoint by adding the clamped direction to the current position
        Vector3 clampedWorldPoint = transform.position + clampedDirection;

        clampedWorldPoint.y = transform.position.y;

        Vector3[] positions = {
        transform.position,
        clampedWorldPoint
    };

        lineRenderer.SetPositions(positions);
        lineRenderer.enabled = true;

        arrow.transform.RotateAround(rb.transform.position, transform.parent.up, 100 * Time.deltaTime);

    }


    IEnumerator Stop()
    {
        while (rb.angularVelocity.magnitude > 0.01f) // Adjust the threshold as needed
        {
            rb.angularVelocity = Vector3.Lerp(rb.angularVelocity, Vector3.zero, Time.deltaTime / stopDuration);
            rb.velocity = Vector3.Lerp(rb.velocity, Vector3.zero, Time.deltaTime / stopDuration);
            yield return null;
        }
        rb.velocity = Vector3.zero;  // Ensure the velocity is exactly zero when done
        rb.angularVelocity = Vector3.zero; // Ensure the angular velocity is exactly zero when done
    }



    private Vector3? CastMouseClickRay()
    {
        Vector3 screenMousePosFar = new Vector3(
            Input.mousePosition.x,
            Input.mousePosition.y,
            Camera.main.farClipPlane);
        Vector3 screenMousePosNear = new Vector3(
            Input.mousePosition.x,
            Input.mousePosition.y,
            Camera.main.nearClipPlane);
        Vector3 worldMousePosFar = Camera.main.ScreenToWorldPoint(screenMousePosFar);
        Vector3 worldMousePosNear = Camera.main.ScreenToWorldPoint(screenMousePosNear);
        RaycastHit hit;
        if (Physics.Raycast(worldMousePosNear, worldMousePosFar - worldMousePosNear, out hit, float.PositiveInfinity))
        {
            return hit.point;
        }
        else
        {
            return null;
        }
    }
}
