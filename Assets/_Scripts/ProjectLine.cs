using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class ProjectileLine : MonoBehaviour
{
    static List<ProjectileLine> PROJ_LINES = new List<ProjectileLine>();
    private const float FADE_DURATION = 2.0f; // Time taken to fade out

    private LineRenderer _line;
    private bool _drawing = true;
    private bool _fading = false; // Prevent multiple fade coroutines
    private Projectile _projectile;

    void Start()
    {
        _line = GetComponent<LineRenderer>();
        _line.positionCount = 1;
        _line.SetPosition(0, transform.position);
        _line.startColor = _line.endColor = Color.white; // Ensure lines start fully visible

        _projectile = GetComponentInParent<Projectile>();

        ADD_LINE(this);
    }

    void FixedUpdate()
    {
        if (_drawing)
        {
            _line.positionCount++;
            _line.SetPosition(_line.positionCount - 1, transform.position);

            if (_projectile != null && !_projectile.awake)
            {
                _drawing = false;
                _projectile = null;
                StartCoroutine(FadeOut()); // Start fading when the projectile stops
            }
        }
    }

    private void OnDestroy()
    {
        PROJ_LINES.Remove(this);
    }

    static void ADD_LINE(ProjectileLine newLine)
    {
        // Start fading out previous lines smoothly
        foreach (ProjectileLine pl in PROJ_LINES)
        {
            if (!pl._fading) // Prevent restarting fade effect
            {
                pl.StartCoroutine(pl.FadeOut());
            }
        }

        PROJ_LINES.Add(newLine);
    }

    IEnumerator FadeOut()
    {
        _fading = true; // Prevent multiple fade coroutines
        float elapsedTime = 0f;
        Color startColor = _line.startColor;

        while (elapsedTime < FADE_DURATION)
        {
            float alpha = Mathf.Lerp(1f, 0f, elapsedTime / FADE_DURATION); // Smoothly decrease alpha
            Color newColor = new Color(startColor.r, startColor.g, startColor.b, alpha);

            _line.startColor = _line.endColor = newColor;
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        Destroy(gameObject); // Remove the line after it fully fades
    }
}
