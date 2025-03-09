using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Renderer))]
public class Goal : MonoBehaviour
{
    static public bool goalMet = false;

    private void Start()
    {
        // Set the initial color to green
        GetComponent<Renderer>().material.color = Color.green;
    }

    private void OnTriggerEnter(Collider other)
    {
        Projectile proj = other.GetComponent<Projectile>();

        if (proj != null)
        {
            Goal.goalMet = true;
            Material mat = GetComponent<Renderer>().material;
            mat.color = Color.red; // Change color to red when hit
        }
    }
}
