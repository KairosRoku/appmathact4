using UnityEngine;
using System.Collections;
using TMPro;

public class Spawner : MonoBehaviour
{
    public GameObject creaturePrefab;
    public Transform startPoint;
    public Transform endPoint;
    public Transform controlPoint1;
    public Transform controlPoint2;
    public MovementType moveType;
    
    [Header("Wave Settings")]
    public int totalWaves = 10;
    public float timeBetweenWaves = 5f;
    public float spawnInterval = 1f;
    public TextMeshProUGUI waveText;

    private int currentWave = 0;

    void Start()
    {
        StartCoroutine(WaveRoutine());
    }

    IEnumerator WaveRoutine()
    {
        while (currentWave < totalWaves)
        {
            currentWave++;
            UpdateWaveUI();
            
            int enemiesToSpawn = currentWave * 5; // 5, 10, 15, 20...
            
            yield return StartCoroutine(SpawnWave(enemiesToSpawn));
            
            if (currentWave < totalWaves)
            {
                yield return new WaitForSeconds(timeBetweenWaves);
            }
        }
    }

    IEnumerator SpawnWave(int count)
    {
        for (int i = 0; i < count; i++)
        {
            Spawn();
            yield return new WaitForSeconds(spawnInterval);
        }
    }

    void Spawn()
    {
        if (creaturePrefab == null) return;
        
        GameObject obj = Instantiate(creaturePrefab, startPoint.position, Quaternion.identity);
        Creature c = obj.GetComponent<Creature>();
        if (c != null)
        {
            c.movementType = moveType;
            c.startPoint = startPoint;
            c.endPoint = endPoint;
            c.controlPoint1 = controlPoint1;
            c.controlPoint2 = controlPoint2;
        }
    }

    void UpdateWaveUI()
    {
        if (waveText != null)
        {
            waveText.text = "Wave: " + currentWave + "/" + totalWaves;
        }
    }
}
