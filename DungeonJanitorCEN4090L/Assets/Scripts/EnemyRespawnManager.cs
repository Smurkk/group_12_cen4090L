using UnityEngine;
using System.Collections.Generic;

public class EnemyRespawnManager : MonoBehaviour
{
    [System.Serializable]
    public class EnemySpawnData
    {
        public GameObject enemyPrefab;
        public Vector2 spawnPosition;
        public float respawnTime = 30f;

        [HideInInspector]
        public float deathTime = -1f;
        [HideInInspector]
        public bool isDead = false;
    }

    [Header("Spawn Settings")]
    public List<EnemySpawnData> enemySpawns = new List<EnemySpawnData>();
    public bool respawnEnabled = true;

    [Header("Room-Based Respawn (Optional)")]
    public bool respawnOnRoomExit = false;
    private bool playerInRoom = true;

    private void Update()
    {
        if (!respawnEnabled) return;

        // Check each dead enemy for respawn
        foreach (EnemySpawnData spawnData in enemySpawns)
        {
            if (spawnData.isDead)
            {
                float timeSinceDeath = Time.time - spawnData.deathTime;

                if (timeSinceDeath >= spawnData.respawnTime)
                {
                    RespawnEnemy(spawnData);
                }
            }
        }
    }

    public void OnEnemyDied(Enemy enemy)
    {
        // Find the spawn data for this enemy
        foreach (EnemySpawnData spawnData in enemySpawns)
        {
            // Check if this enemy matches the spawn
            if (Vector2.Distance(enemy.transform.position, spawnData.spawnPosition) < 0.5f)
            {
                spawnData.isDead = true;
                spawnData.deathTime = Time.time;
                return;
            }
        }
    }

    private void RespawnEnemy(EnemySpawnData spawnData)
    {
        // Don't respawn if player is in the room and that setting is enabled
        if (respawnOnRoomExit && playerInRoom)
        {
            return;
        }

        GameObject newEnemy = Instantiate(spawnData.enemyPrefab, spawnData.spawnPosition, Quaternion.identity);
        spawnData.isDead = false;
        spawnData.deathTime = -1f;
    }

    // Call this when player enters/exits room
    public void SetPlayerInRoom(bool inRoom)
    {
        playerInRoom = inRoom;

        // If player left and respawnOnRoomExit is true, respawn all dead enemies
        if (!inRoom && respawnOnRoomExit)
        {
            RespawnAllEnemies();
        }
    }

    public void RespawnAllEnemies()
    {
        foreach (EnemySpawnData spawnData in enemySpawns)
        {
            if (spawnData.isDead)
            {
                RespawnEnemy(spawnData);
            }
        }
    }

    [ContextMenu("Create Spawn Data From Scene Enemies")]
    public void CreateSpawnDataFromScene()
    {
        enemySpawns.Clear();

        Enemy[] enemies = FindObjectsByType<Enemy>(FindObjectsSortMode.None);
        
        foreach (Enemy enemy in enemies)
        {
            EnemySpawnData data = new EnemySpawnData
            {
                enemyPrefab = enemy.gameObject, 
                spawnPosition = enemy.transform.position,
                respawnTime = 30f
            };
            enemySpawns.Add(data);
        }

        Debug.Log($"Created {enemySpawns.Count} spawn points from scene enemies");
    }
}