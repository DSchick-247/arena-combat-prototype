using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;

public class GameManager : MonoBehaviour
{
    public PlayerHealth playerHealth;
    public TextMeshProUGUI healthText;
    public TextMeshProUGUI waveText;
    public TextMeshProUGUI enemiesText;

    public GameObject winText;
    public GameObject loseText;

    public Image damageFlash;
    public float flashAlpha = 0.4f;
    public float flashDuration = 0.15f;

    public GameObject[] enemyPrefabs;
    public Transform[] spawnPoints;

    public int currentWave = 0;
    public int totalWaves = 3;

    private int enemiesRemaining;
    private bool gameOver = false;

    void Start()
    {
        Time.timeScale = 1f;
        gameOver = false;

        if (winText != null) winText.SetActive(false);
        if (loseText != null) loseText.SetActive(false);

        if (damageFlash != null)
        {
            Color c = damageFlash.color;
            c.a = 0f;
            damageFlash.color = c;
        }

        StartWave();
    }

    void Update()
    {
        if (playerHealth != null && healthText != null)
        {
            healthText.text = "Health: " + playerHealth.CurrentHealth;
        }

        if (Input.GetKeyDown(KeyCode.K))
        {
            TriggerDamageFlash();
        }

        if (gameOver && Input.GetKeyDown(KeyCode.R))
        {
            Time.timeScale = 1f;
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
    }

    void UpdateWaveUI()
    {
        if (waveText != null)
        {
            waveText.text = "Wave: " + currentWave + " / " + totalWaves;
        }

        if (enemiesText != null)
        {
            enemiesText.text = "Enemies: " + enemiesRemaining;
        }
    }

    void StartWave()
    {
        if (enemyPrefabs == null || enemyPrefabs.Length == 0)
        {
            Debug.LogError("Enemy Prefabs array is empty.");
            return;
        }

        if (spawnPoints == null || spawnPoints.Length == 0)
        {
            Debug.LogError("Spawn Points array is empty.");
            return;
        }

        currentWave++;

        if (currentWave > totalWaves)
        {
            ShowWin();
            return;
        }

        int enemiesToSpawn = currentWave + 1;
        enemiesRemaining = enemiesToSpawn;

        Debug.Log("Starting Wave " + currentWave + " with " + enemiesToSpawn + " enemies.");

        for (int i = 0; i < enemiesToSpawn; i++)
        {
            GameObject enemyPrefab = enemyPrefabs[Random.Range(0, enemyPrefabs.Length)];
            Transform spawnPoint = spawnPoints[Random.Range(0, spawnPoints.Length)];

            if (enemyPrefab == null)
            {
                Debug.LogError("Enemy prefab slot is empty.");
                return;
            }

            if (spawnPoint == null)
            {
                Debug.LogError("Spawn point slot is empty.");
                return;
            }

            Vector3 spawnPosition = spawnPoint.position;
            spawnPosition.y = 1f;

            Instantiate(enemyPrefab, spawnPosition, spawnPoint.rotation);
        }

        UpdateWaveUI();
    }

    public void EnemyDefeated()
    {
        if (gameOver) return;

        enemiesRemaining--;
        UpdateWaveUI();

        Debug.Log("Enemy defeated. Remaining: " + enemiesRemaining);

        if (enemiesRemaining <= 0)
        {
            StartWave();
        }
    }

    public void ShowWin()
    {
        if (gameOver) return;

        gameOver = true;

        if (winText != null)
        {
            winText.SetActive(true);
        }

        Debug.Log("YOU WIN");
        Time.timeScale = 0f;
    }

    public void ShowLose()
    {
        if (gameOver) return;

        gameOver = true;

        if (loseText != null)
        {
            loseText.SetActive(true);
        }

        Debug.Log("YOU LOSE");
        Time.timeScale = 0f;
    }

    public void TriggerDamageFlash()
    {
        if (damageFlash != null)
        {
            StopAllCoroutines();
            StartCoroutine(FlashRoutine());
        }
    }

    IEnumerator FlashRoutine()
    {
        Color c = damageFlash.color;
        c.a = flashAlpha;
        damageFlash.color = c;

        yield return new WaitForSeconds(flashDuration);

        c.a = 0f;
        damageFlash.color = c;
    }
}