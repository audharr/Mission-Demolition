using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowCam : MonoBehaviour
{
    static private FollowCam S;     // Singleton instance
    static public GameObject POI;   // Point of Interest (target)

    public enum eView { none, slingshot, castle, both };

    [Header("Inscribed")]
    public float easing = 0.05f;         // Easing factor for smooth movement
    public Vector2 minXY = Vector2.zero; // Minimum X and Y for camera boundaries
    public GameObject viewBothGO;        // Object that combines slingshot and castle views

    [Header("Dynamic")]
    public float camZ;                   // Camera's initial Z position
    public eView nextView = eView.slingshot;

    private void Awake()
    {
        S = this;
        camZ = this.transform.position.z;  // Maintain the camera's Z position
    }

    private void Start()
    {
        // Ensure that when the game starts, the camera is set to slingshot view.
        // In your setup, slingshot view means POI is null.
        POI = null;
        // Set nextView so that the next view will be castle (or whichever you prefer)
        nextView = eView.castle;
    }

    private void FixedUpdate()
    {
        Vector3 destination = Vector3.zero;

        // If POI is not null, get its position
        if (POI != null)
        {
            // Check if POI has a Rigidbody and if it is sleeping (inactive), then nullify POI
            Rigidbody poiRigid = POI.GetComponent<Rigidbody>();
            if ((poiRigid != null) && poiRigid.IsSleeping())
            {
                POI = null;
            }
            else
            {
                destination = POI.transform.position; // Get the target's position
            }
        }

        // Ensure the camera stays within min boundary limits
        destination.x = Mathf.Max(minXY.x, destination.x);
        destination.y = Mathf.Max(minXY.y, destination.y);

        // Smooth the camera movement with easing
        destination = Vector3.Lerp(transform.position, destination, easing);

        // Maintain the Z position so the camera doesn’t move in or out of the screen
        destination.z = camZ;

        // Apply the calculated position to the camera
        transform.position = destination;

        // Adjust the orthographic size of the camera based on the Y position of the POI
        Camera.main.orthographicSize = destination.y + 10;
    }

    public void SwitchView(eView newView)
    {
        if (newView == eView.none)
        {
            newView = nextView;
        }

        switch (newView)
        {
            case eView.slingshot:
                POI = null;
                nextView = eView.castle;
                break;
            case eView.castle:
                POI = MissionDemolition.GET_CASTLE();  // Assign the castle as the POI
                nextView = eView.both;
                break;
            case eView.both:
                POI = viewBothGO;  // Assign the combined object as the POI
                nextView = eView.slingshot;
                break;
        }
    }

    public void SwitchView()
    {
        SwitchView(eView.none);
    }

    static public void SWITCH_VIEW(eView newView)
    {
        S.SwitchView(newView);
    }
}