using UnityEngine;

public enum TurretType { MachineGun, Sniper, Shotgun }

public class Turret : MonoBehaviour
{
    [Header("General Settings")]
    public TurretType turretType;
    public GameObject projectilePrefab;
    public Transform firePoint;
    public float rotationSpeed = 5f;
    public float fireRange = 10f;
    public float fireCooldown = 1.0f;
    
    [Header("Specific Settings")]
    public float fireAngleThreshold = 45f; // For cone-based triggers
    public int shotgunPellets = 5;
    public float shotgunSpread = 30f;
    public float sniperSightsTolerance = 2f; // Very narrow for sniper

    [Header("Visuals")]
    public LineRenderer rangeVisualizer;

    private float nextFireTime;
    private Transform currentTarget;

    void Start()
    {
        if (rangeVisualizer == null)
        {
            rangeVisualizer = gameObject.AddComponent<LineRenderer>();
            rangeVisualizer.startWidth = 0.05f;
            rangeVisualizer.endWidth = 0.05f;
            rangeVisualizer.positionCount = 0;
            rangeVisualizer.useWorldSpace = true;
            rangeVisualizer.material = new Material(Shader.Find("Sprites/Default"));
            rangeVisualizer.startColor = Color.cyan;
            rangeVisualizer.endColor = Color.cyan;
        }
    }

    void Update()
    {
        if (GameManager.Instance != null && GameManager.Instance.isGameOver) return;

        FindTarget();
        UpdateRangeVisualizer();

        if (currentTarget == null) return;

        // Math-calculated rotation towards the target
        Vector3 directionToTarget = currentTarget.position - transform.position;
        directionToTarget.z = 0;

        if (directionToTarget != Vector3.zero)
        {
            float targetAngle = Mathf.Atan2(directionToTarget.y, directionToTarget.x) * Mathf.Rad2Deg - 90f;
            Quaternion targetRotation = Quaternion.Euler(0, 0, targetAngle);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        }

        // Distance Check
        float distance = Vector3.Distance(transform.position, currentTarget.position);
        if (distance > fireRange) return;

        // Angle Check (Front-facing)
        Vector3 dirToTarget = (currentTarget.position - transform.position).normalized;
        float angle = Vector3.Angle(transform.up, dirToTarget);

        bool canFire = false;
        switch (turretType)
        {
            case TurretType.MachineGun:
                canFire = angle <= fireAngleThreshold;
                break;
            case TurretType.Shotgun:
                canFire = angle <= 30f; // Typical shotgun range angle
                break;
            case TurretType.Sniper:
                canFire = angle <= sniperSightsTolerance;
                break;
        }

        if (canFire && Time.time >= nextFireTime)
        {
            Shoot();
            nextFireTime = Time.time + fireCooldown;
        }
    }

    void FindTarget()
    {
        // Target creatures first (TD logic)
        Creature[] enemies = FindObjectsByType<Creature>(FindObjectsSortMode.None);
        float closestDistance = Mathf.Infinity;
        currentTarget = null;

        foreach (Creature enemy in enemies)
        {
            float dist = Vector3.Distance(transform.position, enemy.transform.position);
            if (dist < closestDistance && dist <= fireRange)
            {
                closestDistance = dist;
                currentTarget = enemy.transform;
            }
        }

        // No player targeting in TD mode
    }

    void Shoot()
    {
        if (projectilePrefab == null || firePoint == null) return;

        switch (turretType)
        {
            case TurretType.MachineGun:
                // Rapid fire single shots with a tiny bit of random spread
                float machineGunSpread = 5f;
                float mgAngle = Random.Range(-machineGunSpread, machineGunSpread);
                SpawnProjectile(transform.rotation * Quaternion.Euler(0, 0, mgAngle));
                break;

            case TurretType.Shotgun:
                for (int i = 0; i < shotgunPellets; i++)
                {
                    float randomAngle = Random.Range(-shotgunSpread / 2f, shotgunSpread / 2f);
                    SpawnProjectile(transform.rotation * Quaternion.Euler(0, 0, randomAngle));
                }
                break;

            case TurretType.Sniper:
                SpawnProjectile(transform.rotation);
                break;
        }
    }

    void SpawnProjectile(Quaternion rotation)
    {
        GameObject projObj = Instantiate(projectilePrefab, firePoint.position, rotation);
        Projectile projectile = projObj.GetComponent<Projectile>();
        if (projectile != null)
        {
            // Calculate direction based on rotation (transform.up is the direction)
            Vector3 dir = rotation * Vector3.up;
            projectile.Initialize(dir);
        }
    }

    void UpdateRangeVisualizer()
    {
        if (rangeVisualizer == null) return;

        if (turretType == TurretType.MachineGun || turretType == TurretType.Shotgun)
        {
            float currentLimit = (turretType == TurretType.MachineGun) ? fireAngleThreshold : 30f;
            int segments = 20;
            rangeVisualizer.positionCount = segments + 2;
            
            rangeVisualizer.SetPosition(0, transform.position);
            
            for (int i = 0; i <= segments; i++)
            {
                float progress = (float)i / segments;
                float angle = Mathf.Lerp(-currentLimit, currentLimit, progress);
                
                // Rotation relative to transform.up
                Vector3 dir = Quaternion.AngleAxis(angle, Vector3.forward) * transform.up;
                rangeVisualizer.SetPosition(i + 1, transform.position + dir * fireRange);
            }
            
            // Loop back to center? Or just leave as cone arc.
            // Let's make it a closed cone for better visibility
            rangeVisualizer.positionCount = segments + 3;
            rangeVisualizer.SetPosition(segments + 2, transform.position);
        }
        else if (turretType == TurretType.Sniper)
        {
            // Sniper shows a long thin line
            rangeVisualizer.positionCount = 2;
            rangeVisualizer.SetPosition(0, transform.position);
            rangeVisualizer.SetPosition(1, transform.position + transform.up * fireRange);
        }
        else
        {
            // Default circle
            int segments = 50;
            rangeVisualizer.positionCount = segments + 1;
            for (int i = 0; i <= segments; i++)
            {
                float angle = i * (360f / segments) * Mathf.Deg2Rad;
                float x = Mathf.Sin(angle) * fireRange;
                float y = Mathf.Cos(angle) * fireRange;
                rangeVisualizer.SetPosition(i, transform.position + new Vector3(x, y, 0));
            }
        }
    }
}
