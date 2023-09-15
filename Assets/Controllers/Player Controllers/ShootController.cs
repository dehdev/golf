using System.Collections;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(AudioSource))]

public class ShootController : MonoBehaviour
{
    private Rigidbody rb;

    [SerializeField] private float shotMultiplier;
    [SerializeField] private float stopDuration = 5;
    [SerializeField] private float stopVelocity = .05f; //The velocity below which the rigidbody will be considered as stopped
    [SerializeField] private float MaxDragDistance = 30f;
    [SerializeField] private float iddleEffectDistance;
    private float time;

    private int shots;

    private string timeText;
    [SerializeField] private UnityEvent<string> shotEvent;
    [SerializeField] private UnityEvent<string> timerEvent;

    [SerializeField] private LineRenderer lineRenderer;

    [SerializeField] private GameObject idleParticles;
    [SerializeField] private GameObject arrow;

    private Vector3 lastPos;

    private SoundController soundController;

    private bool isIdle;
    private bool isAiming;
    private bool readyToShoot;
    private bool hasChangedToIdle;

    private void Awake()
    {
        readyToShoot = false;
        isAiming = false;
        lineRenderer.enabled = false;
        hasChangedToIdle = false;
        idleParticles.SetActive(false);
        arrow.SetActive(false);
    }

    private void Start()
    {
        soundController = GetComponent<SoundController>();
        rb = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        idleParticles.transform.position = new Vector3(transform.position.x, transform.position.y - iddleEffectDistance, transform.position.z);
        if (rb.velocity.magnitude < stopVelocity)
        {
            if (!isAiming)
            {
                if (!hasChangedToIdle)
                {
                    hasChangedToIdle = true;
                    soundController.PlayIdle();
                    idleParticles.SetActive(true);
                }
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
            arrow.transform.position = transform.position;
            arrow.SetActive(true);
            idleParticles.SetActive(false);
            UnityEngine.Cursor.visible = false;
            isAiming = true;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        soundController.PlayCollisionSound(rb.velocity.magnitude / 10);
        if (collision.gameObject.CompareTag("Terrain"))
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
            arrow.SetActive(false);
            readyToShoot = false;
            hasChangedToIdle = false;
            UnityEngine.Cursor.visible = true;
            Shoot(worldPoint.Value);
        }
    }

    private void Shoot(Vector3 worldPoint)
    {
        soundController.PlayBallHit();
        lastPos = transform.position;
        shots++;
        shotEvent.Invoke("Shots: " + shots.ToString());
        isAiming = false;
        lineRenderer.enabled = false;

        Vector3 horizontalWorldPoint = new(worldPoint.x, transform.position.y, worldPoint.z);

        Vector3 direction = (horizontalWorldPoint - transform.position).normalized;

        float strength = Mathf.Clamp(Vector3.Distance(transform.position, horizontalWorldPoint), 0, MaxDragDistance);

        rb.AddForce(shotMultiplier * strength * -direction);
        isIdle = false;

        Debug.Log("Force: " + (shotMultiplier * strength * -direction).magnitude);
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

        float arrowAngle = Mathf.Atan2(clampedDirection.x, clampedDirection.z) * Mathf.Rad2Deg;
        Quaternion arrowRotation = Quaternion.Euler(0f, arrowAngle, 0f);
        arrow.transform.rotation = arrowRotation;

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
        Vector3 screenMousePosFar = new(
            Input.mousePosition.x,
            Input.mousePosition.y,
            Camera.main.farClipPlane);
        Vector3 screenMousePosNear = new(
            Input.mousePosition.x,
            Input.mousePosition.y,
            Camera.main.nearClipPlane);
        Vector3 worldMousePosFar = Camera.main.ScreenToWorldPoint(screenMousePosFar);
        Vector3 worldMousePosNear = Camera.main.ScreenToWorldPoint(screenMousePosNear);
        if (Physics.Raycast(worldMousePosNear, worldMousePosFar - worldMousePosNear, out RaycastHit hit, float.PositiveInfinity))
        {
            return hit.point;
        }
        else
        {
            return null;
        }
    }
}
