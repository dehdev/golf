using Cinemachine;
using System;
using System.Collections;
using Unity.Netcode;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]

public class PlayerController : NetworkBehaviour
{
    public static PlayerController LocalInstance { get; private set; }

    private Rigidbody rb;
    private SphereCollider sphereCollider;
    [SerializeField] private MeshRenderer meshRenderer;
    [SerializeField] private GameObject areaOfEffect;

    public static event EventHandler OnBallHit;
    public static event EventHandler<float> OnCollisionHit;
    public static event EventHandler OnIdleEvent;
    public static event EventHandler<float> OnShootingStartEvent;
    public static event EventHandler OnPlayerResetPosition;

    [SerializeField] private float shotMultiplier;
    [SerializeField] private float stopVelocity = .05f; //The velocity below which the rigidbody will be considered as stopped
    [SerializeField] private float MaxDragDistance = 30f;
    [SerializeField] private float playerContactForce = 20f;

    [SerializeField] private LineRenderer lineRenderer;
    [SerializeField] private TrailRenderer trailRenderer;
    [SerializeField] private GameObject idleParticles;
    [SerializeField] private GameObject arrow;
    [SerializeField] private PlayerVisual playerVisual;

    private bool isIdle;
    private bool isAiming;
    private bool readyToShoot;
    private bool hasChangedToIdle;

    private int localPlayerShots = 0;

    private Vector3 lastPos;
    private Vector3 spawnPos;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        sphereCollider = GetComponent<SphereCollider>();

        readyToShoot = false;
        isAiming = false;
        hasChangedToIdle = false;

        idleParticles.SetActive(false);
        arrow.SetActive(false);
        lineRenderer.enabled = false;
        trailRenderer.enabled = false;
        meshRenderer.enabled = false;
        sphereCollider.enabled = false;
        areaOfEffect.SetActive(false);
    }

    public override void OnNetworkSpawn()
    {
        if (IsOwner)
        {
            LocalInstance = this;
            spawnPos = SpawnPointManager.Instance.GetSpawnPointsList()[(int)OwnerClientId].transform.position;
            SetPlayerPosition(spawnPos);
        }
        base.OnNetworkSpawn();
    }

    public void SetPlayerPosition(Vector3 pos)
    {
        CancelShoot();
        StartCoroutine(SetPlayerPositionCoroutine(pos));
    }

    IEnumerator SetPlayerPositionCoroutine(Vector3 pos)
    {
        yield return new WaitForFixedUpdate();
        transform.position = pos;
        Debug.Log("Player pos set to: " + transform.position);
    }

    private void Start()
    {
        GameInput.Instance.OnResetAction += GameInput_OnResetAction;
        GameInput.Instance.OnCancelShoot += GameInput_OnCancelShoot;
        PlayerData playerData = GolfGameMultiplayer.Instance.GetPlayerDataFromClientId(OwnerClientId);
        playerVisual.SetPlayerColor(GolfGameMultiplayer.Instance.GetPlayerColor(playerData.colorId));

        InitializePlayerObjectsForAll();

        if (IsOwner)
        {
            InitializePlayerObjectsForOwner();
        }
    }

    private void GameInput_OnCancelShoot(object sender, EventArgs e)
    {
        CancelShoot();
    }

    private void GameInput_OnResetAction(object sender, EventArgs e)
    {
        if (!IsOwner)
        {
            return;
        }
        SetPlayerPosition(spawnPos);
        StartCoroutine(PlayerInstantStopCoroutine());
        CancelShoot();
        OnPlayerResetPosition?.Invoke(this, EventArgs.Empty);
    }

    public void CancelShoot()
    {
        Cursor.visible = true;
        arrow.SetActive(false);
        isAiming = false;
        lineRenderer.enabled = false;
        if (isIdle)
        {
            idleParticles.SetActive(true);
        }
    }
    private void InitializePlayerObjectsForAll()
    {
        trailRenderer.enabled = true;
        meshRenderer.enabled = true;
        sphereCollider.enabled = true;
    }

    private void InitializePlayerObjectsForOwner()
    {
        areaOfEffect.SetActive(true);
        var virtualCamera = FindObjectOfType<CinemachineVirtualCamera>();
        virtualCamera.Follow = transform;
    }

    public void SetVisbility(bool isVisible)
    {
        meshRenderer.enabled = isVisible;
        trailRenderer.enabled = isVisible;
        areaOfEffect.SetActive(isVisible);
        sphereCollider.enabled = isVisible;
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
                    if (!GolfGameManager.Instance.IsLocalPlayerFinished())
                    {
                        idleParticles.SetActive(true);
                        OnIdleEvent?.Invoke(this, EventArgs.Empty);
                    }
                }
            }
            isIdle = true;
            //StartCoroutine(PlayerLerpStopCoroutine());
        }
        else
        {
            hasChangedToIdle = false;
            isIdle = false;
            idleParticles.SetActive(false);
            CancelShoot();
        }

        trailRenderer.transform.rotation = Quaternion.Euler(90, 0, 0);
    }

    private void FixedUpdate()
    {
        if (!IsOwner)
        {
            return;
        }
        ProcessAim();
    }

    private void OnMouseUp()
    {
        if (isIdle && Time.timeScale == 1)
        {
            readyToShoot = true;
        }
    }

    private void OnMouseDown()
    {
        if (!IsOwner || GolfGameManager.Instance.IsLocalPlayerFinished())
        {
            return;
        }
        readyToShoot = false;
        PreProcessAim();
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Finish"))
        {
            SetPlayerVisibilityServerRpc(false);
        }
        if (!IsOwner)
        {
            return;
        }
        float clampedVolume = Mathf.Clamp(rb.velocity.magnitude / 10 * 0.2f, 0.1f, 1);
        OnCollisionHit?.Invoke(this, clampedVolume);
        if (collision.gameObject.CompareTag("Terrain") && GolfGameManager.Instance.IsGamePlaying())
        {
            OnPlayerResetPosition?.Invoke(this, EventArgs.Empty);
            SetPlayerPosition(lastPos);
            StartCoroutine(PlayerInstantStopCoroutine());
        }
        foreach (ContactPoint contact in collision.contacts)
        {
            if (collision.gameObject.CompareTag("Player"))
            {
                Vector3 hitDirection;
                hitDirection = contact.normal;
                Vector3 appliedForce = -hitDirection * playerContactForce;
                appliedForce.y = Mathf.Max(0, appliedForce.y);
                collision.gameObject.GetComponent<PlayerController>().PlayerHitObstacle(appliedForce);
                return;
            }
        }
    }

    [ServerRpc(RequireOwnership = false)]
    private void SetPlayerVisibilityServerRpc(bool visibility, ServerRpcParams serverRpcParams = default)
    {
        SetPlayerVisibilityClientRpc(visibility, serverRpcParams.Receive.SenderClientId);
    }

    [ClientRpc]
    private void SetPlayerVisibilityClientRpc(bool visibility, ulong clientId)
    {
        if (NetworkManager.Singleton.LocalClientId == clientId)
        {
            return;
        }
        SetVisbility(visibility);
    }

    private void PreProcessAim()
    {
        if (GolfGameManager.Instance.IsGamePlaying() && isIdle)
        {
            arrow.transform.position = transform.position;
            arrow.SetActive(true);
            arrow.transform.LookAt(transform.position + Vector3.up);
            idleParticles.SetActive(false);
            isAiming = true;
            Cursor.visible = false;
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

        lineRenderer.enabled = true;
        DrawLine(worldPoint.Value);
        if (readyToShoot)
        {
            arrow.SetActive(false);
            readyToShoot = false;
            hasChangedToIdle = false;
            Cursor.visible = true;
            HandleShoot(worldPoint.Value);
            isAiming = false;
        }
    }

    private void HandleShoot(Vector3 worldPoint)
    {
        GolfGameManager.Instance.SetPlayerBallHitServerRpc();
        localPlayerShots++;
        OnBallHit?.Invoke(this, EventArgs.Empty);

        lastPos = transform.position;

        lineRenderer.enabled = false;

        Vector3 horizontalWorldPoint = new(worldPoint.x, transform.position.y, worldPoint.z);

        Vector3 direction = (horizontalWorldPoint - transform.position).normalized;

        float strength = GetClampedStrength(worldPoint);

        rb.AddForce(shotMultiplier * strength * -direction);

        isIdle = false;

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

        lineRenderer.transform.rotation = Quaternion.Euler(90, 0, 0);

        float arrowAngle = Mathf.Atan2(clampedDirection.x, clampedDirection.z) * Mathf.Rad2Deg;
        Quaternion arrowRotation = Quaternion.Euler(0f, arrowAngle, 0f);
        arrow.transform.rotation = arrowRotation;

        float distanceRatio = Vector3.Distance(transform.position, clampedWorldPoint) / MaxDragDistance;
        OnShootingStartEvent?.Invoke(this, distanceRatio);
    }

    private float GetClampedStrength(Vector3 worldPoint)
    {
        Vector3 horizontalWorldPoint = new(worldPoint.x, transform.position.y, worldPoint.z);
        return Mathf.Clamp(Vector3.Distance(transform.position, horizontalWorldPoint), 0, MaxDragDistance);
    }

    /*private IEnumerator PlayerLerpStopCoroutine()
    {
        yield return new WaitForFixedUpdate();
        rb.angularVelocity = Vector3.Lerp(rb.angularVelocity, Vector3.zero, Time.deltaTime / stopDuration);
        rb.velocity = Vector3.Lerp(rb.velocity, Vector3.zero, Time.deltaTime / stopDuration);
    }*/

    private IEnumerator PlayerInstantStopCoroutine()
    {
        yield return new WaitForFixedUpdate();
        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
    }

    private Vector3? CastMouseClickRay()
    {
        Plane plane = new(Vector3.up, transform.position);

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (plane.Raycast(ray, out float enter))
        {
            Vector3 hitPoint = ray.GetPoint(enter);
            return hitPoint;
        }
        return null;
    }

    public void PlayerHitObstacle(Vector3 hitVelocity)
    {
        rb.AddForce(hitVelocity, ForceMode.Impulse);
    }

    public int GetLocalPlayerShots()
    {
        return localPlayerShots;
    }
}
