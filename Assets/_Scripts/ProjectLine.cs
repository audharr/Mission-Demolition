using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class ProjectileLine : MonoBehaviour
{
    static List<ProjectileLine> PROJ_LINES = new List<ProjectileLine>();
    private const float DIM_MULT = 0.75f;
    private const float FADE_DURATION = 1.0f; // Time for fading effect

    private LineRenderer _line;
    private bool _drawing = true;
    public myProjectile _projectile;

    void Start()
    {
        _line = GetComponent<LineRenderer>();
        _line.positionCount = 1;
        _line.SetPosition(0, transform.position);

        // Set initial color to full opacity
        _line.startColor = _line.endColor = Color.white;

        if (_projectile != null)
        {
            ADD_LINE(this);
        }
    }

    void FixedUpdate()
    {
        if (_drawing)
        {
            _line.positionCount++;
            _line.SetPosition(_line.positionCount - 1, transform.position);

            if (_projectile != null)
            {
                if (!_projectile.awake) // Ensure _projectile has the awake property defined
                {
                    _drawing = false;
                    _projectile = null;
                }
            }
        }
    }

    private void OnDestroy()
    {
        PROJ_LINES.Remove(this);
    }

    static void ADD_LINE(ProjectileLine newLine)
    {
        // Fade all previous lines before adding a new one
        foreach (ProjectileLine pl in PROJ_LINES)
        {
            pl.StartCoroutine(pl.FadeOut()); // Start fading previous lines
        }

        PROJ_LINES.Add(newLine);
    }

    IEnumerator FadeOut()
    {
        float elapsedTime = 0f;
        Color startColor = _line.startColor;

        // Fade out the line over time
        while (elapsedTime < FADE_DURATION)
        {
            float alpha = Mathf.Lerp(1f, 0f, elapsedTime / FADE_DURATION); // Line fades from fully visible to fully transparent
            Color newColor = new Color(startColor.r, startColor.g, startColor.b, alpha);

            _line.startColor = _line.endColor = newColor;
            elapsedTime += Time.deltaTime;
            yield return null; // Wait for the next frame
        }

        // After fading, destroy the line object
        Destroy(gameObject);
    }
}
