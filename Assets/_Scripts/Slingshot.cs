using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Slingshot : MonoBehaviour
{
    [Header("Inscribed")]
    public GameObject projectilePrefab; // Prefab for the projectile
    public float velocityMult = 10f;    // Multiplier for velocity
    public GameObject projLinePrefab;   // Prefab for the trajectory line
    public AudioSource src;             // AudioSource for sound effects
    public AudioClip sfx1;              // Sound effect for shooting (Rubber Band Snap)

    [Header("Slingshot Points")]
    public Transform leftPoint;         // Left rubber band anchor
    public Transform rightPoint;        // Right rubber band anchor
    private LineRenderer lineRenderer;   // LineRenderer for rubber band

    [Header("Dynamic")]
    public GameObject launchPoint;      // The point where the projectile is launched
    public Vector3 launchPos;           // Position of the launch point
    private GameObject projectile;      // The projectile instance
    private bool aimingMode = false;    // Whether the player is aiming
    private bool isMouseOver = false;   // Track if the mouse is over the slingshot

    private void Awake()
    {
        // Finding and initializing the launch point
        Transform launchPointTrans = transform.Find("LaunchPoint");
        if (launchPointTrans != null)
        {
            launchPoint = launchPointTrans.gameObject;
            launchPoint.SetActive(false); // Hide the launch point initially
            launchPos = launchPointTrans.position;
        }
        else
        {
            Debug.LogError("LaunchPoint not found in Slingshot.");
        }

        // Setup LineRenderer for rubber band
        lineRenderer = gameObject.GetComponent<LineRenderer>();
        lineRenderer.positionCount = 3; // LeftPoint, Projectile, RightPoint
        lineRenderer.startWidth = 0.1f;
        lineRenderer.endWidth = 0.1f;
        lineRenderer.material = new Material(Shader.Find("Sprites/Default")); // Default material
        lineRenderer.startColor = Color.yellow;
        lineRenderer.endColor = Color.black;
        lineRenderer.enabled = false; // Hide initially
    }

    private void Update()
    {
        HandleMouseHover(); // Check if the mouse is over the slingshot
        HandleMouseClick(); // Check for mouse click events

        if (!aimingMode || projectile == null) return;

        Vector3 mousePos3D = GetMouseWorldPosition();

        // Calculate drag vector
        Vector3 mouseDelta = mousePos3D - launchPos;

        // Limit the drag distance
        float maxMagnitude = GetComponent<SphereCollider>().radius;
        if (mouseDelta.magnitude > maxMagnitude)
        {
            mouseDelta = mouseDelta.normalized * maxMagnitude;
        }

        // Update projectile position
        projectile.transform.position = launchPos + mouseDelta;

        // Update rubber band positions
        UpdateRubberBand();

        // Handle release of the projectile
        if (Mouse.current.leftButton.wasReleasedThisFrame)
        {
            ReleaseProjectile(mouseDelta);
        }
    }

    private void HandleMouseHover()
    {
        Ray ray = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit))
        {
            if (hit.collider.gameObject == gameObject)
            {
                if (!isMouseOver)
                {
                    isMouseOver = true;
                    launchPoint.SetActive(true); // Show launch point when hovering
                }
                return;
            }
        }

        if (isMouseOver)
        {
            isMouseOver = false;
            launchPoint.SetActive(false); // Hide launch point when not hovering
        }
    }

    private void HandleMouseClick()
    {
        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            Ray ray = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit) && hit.collider.gameObject == gameObject)
            {
                StartAiming();
            }
        }
    }

    private void StartAiming()
    {
        if (projectilePrefab == null)
        {
            Debug.LogError("Projectile Prefab is not assigned in Slingshot.");
            return;
        }

        // Instantiate projectile at launch position
        projectile = Instantiate(projectilePrefab, launchPos, Quaternion.identity);
        if (projectile == null)
        {
            Debug.LogError("Projectile failed to instantiate!");
            return;
        }

        Rigidbody projRB = projectile.GetComponent<Rigidbody>();
        if (projRB != null)
        {
            projRB.isKinematic = true; // Prevent movement before release
        }
        else
        {
            Debug.LogError("Projectile has no Rigidbody component!");
        }

        aimingMode = true;
        lineRenderer.enabled = true; // Show rubber band
        UpdateRubberBand();
    }

    private void ReleaseProjectile(Vector3 launchVector)
    {
        aimingMode = false;

        Rigidbody projRB = projectile.GetComponent<Rigidbody>();
        if (projRB != null)
        {
            projRB.isKinematic = false;
            projRB.collisionDetectionMode = CollisionDetectionMode.Continuous;
            projRB.linearVelocity = -launchVector * velocityMult;
        }

        // Switch camera to follow the projectile
        FollowCam.SWITCH_VIEW(FollowCam.eView.slingshot);
        FollowCam.POI = projectile;

        // Instantiate trajectory visualization
        if (projLinePrefab != null)
        {
            GameObject projLine = Instantiate(projLinePrefab);
            projLine.transform.SetParent(projectile.transform, false);
        }

        // Notify game about the shot
        MissionDemolition.SHOT_FIRED();

        // Play shooting sound effect (Rubber Band Snap)
        if (src != null && sfx1 != null)
        {
            src.PlayOneShot(sfx1); // Play sound effect
        }

        // Hide rubber band
        Invoke("ResetRubberBand", 0.5f); // Slight delay ensures it disappears after the shot

        // Reset projectile reference
        projectile = null;
    }

    // ✅ New function to fully reset the rubber band
    private void ResetRubberBand()
    {
        lineRenderer.enabled = false;
        lineRenderer.positionCount = 3;
        lineRenderer.SetPositions(new Vector3[] { Vector3.zero, Vector3.zero, Vector3.zero });
    }

    private void UpdateRubberBand()
    {
        if (!aimingMode || projectile == null || leftPoint == null || rightPoint == null) return;

        lineRenderer.SetPosition(0, leftPoint.position);
        lineRenderer.SetPosition(1, projectile.transform.position);
        lineRenderer.SetPosition(2, rightPoint.position);
    }

    private Vector3 GetMouseWorldPosition()
    {
        if (Mouse.current == null)
        {
            Debug.LogError("Mouse.current is null! Ensure the new Input System is enabled.");
            return Vector3.zero;
        }

        Vector2 mousePos = Mouse.current.position.ReadValue();
        Vector3 mousePos2D = new Vector3(mousePos.x, mousePos.y, -Camera.main.transform.position.z);
        return Camera.main.ScreenToWorldPoint(mousePos2D);
    }
}