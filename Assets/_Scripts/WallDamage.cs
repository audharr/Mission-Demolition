using UnityEngine;
public class WallDamage : MonoBehaviour
{
    private Rigidbody2D rb;
    public float maxHealth = 100f;
    public float currentHealth;
    private SpriteRenderer spriteRenderer;
    public float fallDamageThreshold = 1f; // Minimum impact speed to take damage
    public float fallDamageMultiplier = 2f; // Adjust damage scaling


    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        currentHealth = maxHealth; // Initialize health
        spriteRenderer = GetComponent<SpriteRenderer>(); // Get the SpriteRenderer
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
        if (spriteRenderer != null)
        {
            float healthPercentage = currentHealth / maxHealth; // Normalize health to 0-1
            Color healthColor = Color.Lerp(Color.red, Color.green, healthPercentage); // Interpolate between red and green
            spriteRenderer.color = healthColor; // Apply color to the SpriteRenderer
        }
    }

    private void Die()
    {
        Debug.Log($"{gameObject.name} has died!");
        Destroy(gameObject); // Destroy the object when health reaches zero
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        float impactVelocity = Mathf.Abs(rb.linearVelocity.y); // Get downward velocity
        print("Impact velocity: " + impactVelocity);
        if (impactVelocity > fallDamageThreshold) // Check if velocity exceeds threshold
        {
            float damage = (impactVelocity - fallDamageThreshold) * fallDamageMultiplier; // Calculate damage
            TakeDamage(damage);
        }
    }

}