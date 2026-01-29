using UnityEngine;

public enum MovementType { Quadratic, Cubic }

public class Creature : MonoBehaviour
{
    public MovementType movementType;
    public Transform startPoint;
    public Transform endPoint;
    public Transform controlPoint1;
    public Transform controlPoint2; // Only for Cubic
    public float duration = 5f;
    public GameObject coinPrefab;

    private float elapsedTime = 0f;

    void Update()
    {
        elapsedTime += Time.deltaTime;
        float t = Mathf.Clamp01(elapsedTime / duration);

        if (movementType == MovementType.Quadratic)
        {
            transform.position = CalculateQuadraticBezierPoint(t, startPoint.position, controlPoint1.position, endPoint.position);
        }
        else
        {
            transform.position = CalculateCubicBezierPoint(t, startPoint.position, controlPoint1.position, controlPoint2.position, endPoint.position);
        }

        if (t >= 1f)
        {
            ReachEnd();
        }
    }

    Vector3 CalculateQuadraticBezierPoint(float t, Vector3 p0, Vector3 p1, Vector3 p2)
    {
        // (1-t)^2 * P0 + 2(1-t)t * P1 + t^2 * P2
        float u = 1 - t;
        float tt = t * t;
        float uu = u * u;
        Vector3 p = uu * p0;
        p += 2 * u * t * p1;
        p += tt * p2;
        return p;
    }

    Vector3 CalculateCubicBezierPoint(float t, Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3)
    {
        // (1-t)^3 * P0 + 3(1-t)^2 * t * P1 + 3(1-t) * t^2 * P2 + t^3 * P3
        float u = 1 - t;
        float tt = t * t;
        float uu = u * u;
        float uuu = uu * u;
        float ttt = tt * t;

        Vector3 p = uuu * p0;
        p += 3 * uu * t * p1;
        p += 3 * u * tt * p2;
        p += ttt * p3;
        return p;
    }

    void ReachEnd()
    {
        GameManager.Instance.TakeDamage(1);
        Destroy(gameObject);
    }

    public void Die()
    {
        if (coinPrefab != null)
        {
            Instantiate(coinPrefab, transform.position, Quaternion.identity);
        }
        Destroy(gameObject);
    }
}
