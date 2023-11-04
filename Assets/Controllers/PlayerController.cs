using System;
using System.Collections;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(Rigidbody))]

public class PlayerController : NetworkBehaviour
{
    public static PlayerController Instance { get; private set; }

    private Rigidbody rb;

    private int shots = 0;

    public static event EventHandler OnBallHit;
    public static event EventHandler<float> OnCollisionHit;
    public static event EventHandler OnIdleEvent;
    public static event EventHandler spawnPlayer;


    [SerializeField] private float shotMultiplier;
    [SerializeField] private float stopDuration = 5;
    [SerializeField] private float stopVelocity = .05f; //The velocity below which the rigidbody will be considered as stopped
    [SerializeField] private float MaxDragDistance = 30f;

    [SerializeField] private LineRenderer lineRenderer;

    [SerializeField] private GameObject idleParticles;
    [SerializeField] private GameObject arrow;

    private Vector3 lastPos;

    private bool isIdle;
    private bool isAiming;
    private bool readyToShoot;
    private bool hasChangedToIdle;

    [SerializeField] private Cinemachine.CinemachineVirtualCamera virtualCamera;

    private void Awake()
    {
        if (Instance != null)
        {
            Debug.LogError("More than one instance of PlayerController found!");
        }
        Instance = this;
        readyToShoot = false;
        isAiming = false;
        lineRenderer.enabled = false;
        hasChangedToIdle = false;
        idleParticles.SetActive(false);
        arrow.SetActive(false);
    }

    private void Start()
    {
        if (IsLocalPlayer)
        {
            virtualCamera = Cinemachine.CinemachineVirtualCamera.FindObjectOfType<Cinemachine.CinemachineVirtualCamera>();
            virtualCamera.Follow = transform;
            rb = GetComponent<Rigidbody>();
            spawnPlayer?.Invoke(this, EventArgs.Empty);
        }
    }

    private void Update()
    {
        if (!IsOwner)
        {
            return;
        }
        if (rb.velocity.magnitude < stopVelocity)
        {
            if (!isAiming)
            {
                if (!hasChangedToIdle)
                {
                    hasChangedToIdle = true;
                    OnIdleEvent?.Invoke(this, EventArgs.Empty);
                    idleParticles.SetActive(true);
                }
            }
            isIdle = true;
            Stop();
        }
        else {
            isIdle = false;
        }
    }
    private void FixedUpdate()
    {
        if (!IsOwner)
        {
            return;
        }
        ProcessAim();
    }

    public void StopRbRotation()
    {
        if (!IsOwner)
        {
            return;
        }
        rb.angularVelocity = Vector3.zero;
        rb.velocity = Vector3.zero;
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
        PreProcessAim();
    }

    private void OnCollisionEnter(Collision collision)
    {
        float clampedVolume = Mathf.Clamp(rb.velocity.magnitude / 10 * 0.2f, 0.1f, 1);
        OnCollisionHit?.Invoke(this, clampedVolume);
    }

    private void PreProcessAim()
    {
        if (GolfGameManager.Instance.isGamePlaying() && isIdle)
        {
            arrow.transform.position = transform.position;
            arrow.SetActive(true);
            arrow.transform.LookAt(transform.position + Vector3.up);
            idleParticles.SetActive(false);
            isAiming = true; ;
        }
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
            isAiming = false;
        }
    }

    private void Shoot(Vector3 worldPoint)
    {
        shots++;
        OnBallHit?.Invoke(this, EventArgs.Empty);
        lastPos = transform.position;
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

    public void Stop()
    {
        //Debug.Log("STOP STARTED");
        //while (rb.angularVelocity.magnitude > 0.01f && rb.velocity.magnitude > 0.01f) // Adjust the threshold as needed
        //{
            rb.angularVelocity = Vector3.Lerp(rb.angularVelocity, Vector3.zero, Time.deltaTime / stopDuration);
            rb.velocity = Vector3.Lerp(rb.velocity, Vector3.zero, Time.deltaTime / stopDuration);
        //    Debug.Log("STOPPING");
        //}
        //Debug.Log("STOP ENDED");
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

    public void MoveToPos(Vector3 pos)
    {
        rb.MovePosition(pos);
    }

    public int getShots()
    {
        return shots;
    }
}
