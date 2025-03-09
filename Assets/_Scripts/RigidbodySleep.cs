using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class RigidbodySleep : MonoBehaviour
{
    private int sleepCountdown = 4;
    private Rigidbody rigid;

    private void Awake()
    {
        rigid = GetComponent<Rigidbody>();
    }

    // Start is called before the first frame update
    private void FixedUpdate()
    {
        // Only attempt to sleep when countdown is greater than 0
        if (sleepCountdown > 0)
        {
            // Decrement the countdown before sleeping the Rigidbody
            sleepCountdown--;
        }

        // Sleep Rigidbody after countdown reaches 0
        if (sleepCountdown == 0 && !rigid.IsSleeping())
        {
            rigid.Sleep();  // Put the Rigidbody to sleep
        }
    }
}