using UnityEngine;

public class Projectile : MonoBehaviour
{
    public float speed = 12f;
    public float killDistance = 0.5f;
    public float lifeSpan = 3f;
    
    private Vector3 direction;

    public void Initialize(Vector3 shootDirection)
    {
        direction = shootDirection;
        direction.z = 0;
        direction.Normalize();

        if (direction != Vector3.zero)
        {
            transform.up = direction;
        }
        
        Destroy(gameObject, lifeSpan);
    }

    void Update()
    {
        // Math-based movement
        transform.position += direction * speed * Time.deltaTime;

        // Check for hits against all creatures
        Creature[] enemies = FindObjectsByType<Creature>(FindObjectsSortMode.None);
        foreach (Creature enemy in enemies)
        {
            if (enemy == null) continue;

            float distance = Vector3.Distance(transform.position, enemy.transform.position);
            if (distance < killDistance)
            {
                enemy.Die();
                Destroy(gameObject);
                return;
            }
        }

    }
}
