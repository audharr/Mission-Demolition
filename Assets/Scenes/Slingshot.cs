using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine;

public class Slingshot : MonoBehaviour
{
    [Header("Inscribed")]
    public GameObject projectilePrefab;
    public float velocityMult = 10f;

    [Header("Dynamic")]
    public GameObject launchPoint;
    public Vector3 launchPos;
    public GameObject projectile;
    public bool aimingMode;
    private bool isMouseOver = false;

    void Awake()
    {
        Transform launchPointTrans = transform.Find("LaunchPoint");
        launchPoint = launchPointTrans.gameObject;
        launchPoint.SetActive(false);
        launchPos = launchPointTrans.position;
    }
      void OnMouseDown()
{
    Debug.Log("Mouse Down Triggered");
    aimingMode = true;
    projectile = Instantiate(projectilePrefab) as GameObject;
    projectile.transform.position = launchPos;
    projectile.GetComponent<Rigidbody>().isKinematic = true;
}

    void Update()
    {
        // Cast a ray from the mouse position to check if it's over the Slingshot
        Ray ray = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit))
        {
            Debug.Log("Raycast hit: " + hit.collider.name);
            if (hit.collider != null && hit.collider.gameObject == gameObject)
            {
                if (!isMouseOver)
                {
                    if (hit.collider.gameObject == gameObject && Mouse.current.leftButton.wasPressedThisFrame)
                    {
                        OnMouseDown();
                    }
                    isMouseOver = true;
                    launchPoint.SetActive(true);
                }
            }
            else if (isMouseOver)
            {
                isMouseOver = false;
                launchPoint.SetActive(false);
            }
        }
        else if (isMouseOver)
        {
            isMouseOver = false;
            launchPoint.SetActive(false);
        }

        // Handle aiming and launching
        if (aimingMode)
        {
            Vector3 mousePos2D = Input.mousePosition;
            mousePos2D.z = -Camera.main.transform.position.z;
            Vector3 mousePos3D = Camera.main.ScreenToWorldPoint(mousePos2D);
            Vector3 mouseDelta = mousePos3D - launchPos;

            float maxMagnitude = this.GetComponent<SphereCollider>().radius;
            if (mouseDelta.magnitude > maxMagnitude)
            {
                mouseDelta.Normalize();
                mouseDelta *= maxMagnitude;
            }

            Vector3 projPos = launchPos + mouseDelta;
            projectile.transform.position = projPos;

            if (Input.GetMouseButtonUp(0))
            {
                aimingMode = false;
                Rigidbody projRB = projectile.GetComponent<Rigidbody>();
                projRB.isKinematic = false;
                projRB.collisionDetectionMode = CollisionDetectionMode.Continuous;
                projRB.linearVelocity = -mouseDelta * velocityMult; // Fix: Use velocity instead of linearVelocity
                FollowCam.POI = projectile;
                projectile = null;
            }
        }
    }

}