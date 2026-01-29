using UnityEngine;

public class Coin : MonoBehaviour
{
    public float moveSpeed = 5f;
    private RectTransform targetUI;
    private Camera cam;
    private Vector3 startPos;
    private float t = 0;

    void Start()
    {
        cam = Camera.main;
        startPos = transform.position;
        if (GameManager.Instance != null)
        {
            targetUI = GameManager.Instance.coinUITarget;
        }
    }

    void Update()
    {
        if (targetUI == null || cam == null) return;

        // 1. Get the screen position of the UI element
        // targetUI.position is the screen position if the canvas is Overlay.
        Vector3 screenPos = targetUI.position;

        // 2. Convert screen position to world position
        // We need a specific depth (Z). We use the distance from the camera to the game plane (Z=0).
        float depth = Mathf.Abs(cam.transform.position.z);
        Vector3 targetWorldPos = cam.ScreenToWorldPoint(new Vector3(screenPos.x, screenPos.y, depth));
        targetWorldPos.z = 0; // Lock to game plane

        // 3. Move towards it using Lerp
        t += Time.deltaTime * moveSpeed;
        transform.position = Vector3.Lerp(startPos, targetWorldPos, t);

        // 4. Reach check
        if (t >= 1f || Vector3.Distance(transform.position, targetWorldPos) < 0.1f)
        {
            GameManager.Instance.AddCoin(10);
            Destroy(gameObject);
        }
    }
}
