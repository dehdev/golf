using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using UnityEngine;
using static UnityEditor.PlayerSettings;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(Collider))]

public class ImpulseController : MonoBehaviour
{

    private Rigidbody rb;
    public Camera cam;
    private Vector3 mouseReleasePos;
    [SerializeField] private float forceMultiplier = 3.0f;
    [SerializeField] private float MaxDragDistance = 30f;
    [SerializeField] private AudioSource hitSound;
    private Vector3 startPoint;
    private Vector3 endPoint;
    private LineRenderer shootLine;
    [SerializeField] private float stopVelocity = 0.2f;

    private bool isIdle;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        hitSound = GetComponent<AudioSource>();
        shootLine = GetComponent<LineRenderer>();
        hitSound = GetComponent<AudioSource>();
    }

    private void OnMouseDrag()
    {
        Cursor.visible = false;
        if (isIdle)
        {
            Ray ray = cam.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit))
            {
                float distance = Vector3.Distance(this.transform.position, hit.point);
                startPoint = transform.position;
                Vector3 direction = (hit.point - startPoint).normalized * -1; // Calculate normalized direction vector
                float newDistance = Mathf.Min((hit.point - startPoint).magnitude, MaxDragDistance); // Calculate the smaller of actual distance and MaxDragDistance
                endPoint = startPoint + direction * newDistance; // Calculate the endpoint
                endPoint.y = transform.position.y;
                //Debug.DrawLine(startPoint, endPoint, Color.red);
            }
            shootLine.enabled = true;
            shootLine.SetPosition(0, Vector3.zero);
            shootLine.SetPosition(1, transform.InverseTransformPoint(endPoint));
            isIdle = false;
        }
    }

    private void OnMouseUp()
    {
        Cursor.visible = true;
        Ray ray = cam.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit))
        {
            mouseReleasePos = hit.point;
            float distance = Vector3.Distance(transform.position, mouseReleasePos);
            distance = Mathf.Clamp(distance, 1, MaxDragDistance);
            Debug.Log("Drag distance: " + distance);
            Shoot(((mouseReleasePos - transform.position).normalized) * -1, distance);
        }
        shootLine.enabled = false;
    }

    void Shoot(Vector3 dir, float power)
    {
        Vector3 forceAdded = new Vector3(dir.x, 0f, dir.z);
        forceAdded *= forceMultiplier * power;
        rb.AddForce(forceAdded, ForceMode.Impulse);
        hitSound.Play();
        Debug.Log("Force: " + forceAdded.magnitude);
    }


    // Update is called once per frame
    void FixedUpdate()
    {
        Debug.Log(isIdle);
        if(rb.velocity.magnitude < stopVelocity)
        {
            Stop();
        }
        if (Input.GetKey(KeyCode.R))
        {
            transform.position = new Vector3(6, 7, 5);
            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Terrain")
        {
            transform.position = new Vector3(6, 7, 5);
            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }
    }

    private void Stop()
    {
        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        isIdle = true;
    }
}