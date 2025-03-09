using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class myProjectile : MonoBehaviour
{
    static List<myProjectile> PROJECTILES = new List<myProjectile>();

    [SerializeField]
    private bool _awake = true;
    public bool awake
    {
        get { return _awake; }
        private set { _awake = value; }
    }

    private Rigidbody rigid;
    
    // **Adjustable thresholds**
    private float velocityThreshold = 0.01f;  // **Lowered to detect even slow rolling**
    private float angularVelocityThreshold = 0.05f;  // **Detects rolling motion**
    private float lowSpeedDuration = 1.5f;  // **Time before destruction**
    
    void Start()
    {
        rigid = GetComponent<Rigidbody>();
        awake = true;
        PROJECTILES.Add(this);

        StartCoroutine(CheckForDestruction());
    }

    void FixedUpdate()
    {
        if (rigid.isKinematic || !awake)
        {
            return;
        }

        // Check if velocity AND angular velocity are both very low
        if (rigid.linearVelocity.sqrMagnitude < velocityThreshold * velocityThreshold &&
            rigid.angularVelocity.sqrMagnitude < angularVelocityThreshold * angularVelocityThreshold)
        {
            awake = false;
            StartCoroutine(ForceSleepAndDestroy());  // **Ensures Rigidbody stops moving before destroying**
        }
    }

    private IEnumerator CheckForDestruction()
    {
        while (true)
        {
            yield return new WaitForSeconds(0.5f);  // Check every 0.5s

            if (rigid.linearVelocity.sqrMagnitude < velocityThreshold * velocityThreshold &&
                rigid.angularVelocity.sqrMagnitude < angularVelocityThreshold * angularVelocityThreshold)
            {
                yield return new WaitForSeconds(lowSpeedDuration);  // Wait before destroying

                if (rigid.linearVelocity.sqrMagnitude < velocityThreshold * velocityThreshold &&
                    rigid.angularVelocity.sqrMagnitude < angularVelocityThreshold * angularVelocityThreshold)
                {
                    DestroyProjectile();
                }
            }
        }
    }

    private IEnumerator ForceSleepAndDestroy()
    {
        yield return new WaitForSeconds(lowSpeedDuration);
        
        // **Force Rigidbody to sleep**
        rigid.linearVelocity = Vector3.zero;
        rigid.angularVelocity = Vector3.zero;
        rigid.Sleep();

        DestroyProjectile();
    }

    private void DestroyProjectile()
    {
        PROJECTILES.Remove(this);
        Destroy(gameObject);
    }

    private void OnDestroy()
    {
        PROJECTILES.Remove(this);
    }

    static public void DESTROY_PROJECTILES()
    {
        foreach (myProjectile p in PROJECTILES)
        {
            Destroy(p.gameObject);
        }
        PROJECTILES.Clear();
    }
}
