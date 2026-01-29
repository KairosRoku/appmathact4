using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("Player Stats")]
    public int maxHP = 20;
    private float currentHP;
    private float ghostHP;
    public int coins = 0;
    private float displayingCoins = 0;

    [Header("UI Elements")]
    public Slider hpSlider;
    public Slider ghostHPSlider;
    public TextMeshProUGUI coinText;
    public RectTransform coinUITarget;
    public GameObject failUI;
    public GameObject winUI;
    public Transform goalPoint;
    public GameObject player;
    public float goalReachedDistance = 1.0f;

    [Header("Settings")]
    public float ghostHPEasingSpeed = 2f;
    public float coinLerpSpeed = 5f;
    public bool isGameOver = false;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    void Start()
    {
        currentHP = maxHP;
        ghostHP = maxHP;
        if (failUI != null) failUI.SetActive(false);
        if (winUI != null) winUI.SetActive(false);
        UpdateUI();
    }

    void Update()
    {
        if (isGameOver) return;

        // Smoothly lower ghost HP
        if (ghostHP > currentHP)
        {
            ghostHP = Mathf.Lerp(ghostHP, currentHP, ghostHPEasingSpeed * Time.deltaTime);
        }
        
        // Lerp coin value upward
        if (displayingCoins < coins)
        {
            displayingCoins = Mathf.MoveTowards(displayingCoins, coins, coinLerpSpeed * Time.deltaTime * 10);
        }

        UpdateUI();

        if (currentHP <= 0)
        {
            GameOver();
        }

        CheckWinCondition();
    }

    void CheckWinCondition()
    {
        if (player != null && goalPoint != null)
        {
            float dist = Vector3.Distance(player.transform.position, goalPoint.position);
            if (dist < goalReachedDistance)
            {
                Win();
            }
        }
    }

    void Win()
    {
        isGameOver = true;
        if (winUI != null) winUI.SetActive(true);
        // Turrets check Instance.isGameOver to stop firing already.
    }

    public void TakeDamage(int amount)
    {
        currentHP -= amount;
        if (currentHP < 0) currentHP = 0;
    }

    public void AddCoin(int amount)
    {
        coins += amount;
    }

    void UpdateUI()
    {
        if (hpSlider != null) hpSlider.value = (float)currentHP / maxHP;
        if (ghostHPSlider != null) ghostHPSlider.value = ghostHP / maxHP;
        if (coinText != null) coinText.text = "Coins: " + Mathf.FloorToInt(displayingCoins).ToString();
    }

    void GameOver()
    {
        isGameOver = true;
        if (failUI != null) failUI.SetActive(true);
        Time.timeScale = 0; // Pause game
    }

    public void RestartGame()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
