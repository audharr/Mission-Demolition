using UnityEngine;

public class WallDamage : MonoBehaviour
{
    private Rigidbody rb;
    public float maxHealth = 100f;
    public float currentHealth;
    private Renderer objectRenderer;
    public float fallDamageThreshold = 1f; // Minimum impact speed to take damage
    public float fallDamageMultiplier = 2f; // Adjust damage scaling

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        currentHealth = maxHealth; // Initialize health
        objectRenderer = GetComponent<Renderer>(); // Get the Renderer component
        UpdateColor();
    }

    public void TakeDamage(float amount)
    {
        currentHealth -= amount;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth); // Keep health within bounds

        Debug.Log($"{gameObject.name} took {amount} damage! Current health: {currentHealth}");

        UpdateColor();

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private void UpdateColor()
    {
        if (objectRenderer != null)
        {
            float healthPercentage = currentHealth / maxHealth; // Normalize health to 0-1
            Color healthColor = Color.Lerp(Color.red, Color.green, healthPercentage); // Interpolate between red and green
            objectRenderer.material.color = healthColor; // Apply color to the material
        }
    }

    private void Die()
    {
        Debug.Log($"{gameObject.name} has been destroyed!");
        Destroy(gameObject); // Destroy the object when health reaches zero
    }

    void OnCollisionEnter(Collision collision)
    {
        float impactVelocity = collision.relativeVelocity.magnitude; // Get impact velocity
        Debug.Log($"Impact velocity: {impactVelocity}");

        if (impactVelocity > fallDamageThreshold) // Check if velocity exceeds threshold
        {
            float damage = (impactVelocity - fallDamageThreshold) * fallDamageMultiplier; // Calculate damage
            TakeDamage(damage);
        }
    }

}