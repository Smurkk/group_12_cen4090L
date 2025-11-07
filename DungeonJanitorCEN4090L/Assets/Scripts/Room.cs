using UnityEngine;
using System.Collections.Generic;

public class Room : MonoBehaviour
{
    public List<Enemy> enemiesInRoom = new List<Enemy>();
    
    private void Start()
    {
        // Auto-find enemies when game starts if list is empty
        if (enemiesInRoom.Count == 0)
        {
            FindEnemiesInRoom();
        }
    }  
        
    
    private void OnTriggerEnter2D(Collider2D collision)
    {
        
        if (collision.CompareTag("Player"))
        {
            ActivateEnemies();
        }
    }
    
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            DeactivateEnemies();
        }
    }

    private void ActivateEnemies()
    {
        foreach (Enemy enemy in enemiesInRoom)
        {
            if (enemy != null)
            {
                enemy.SetRoomActive(true);
            }
        }
    }
    
    private void DeactivateEnemies()
    {
        foreach (Enemy enemy in enemiesInRoom)
        {
            if (enemy != null)
            {
                enemy.SetRoomActive(false);
            }
        }
    }
    
 [ContextMenu("Find Enemies In Room")]
    public void FindEnemiesInRoom()
    {
        enemiesInRoom.Clear();
        
        // Find ALL enemy scripts (Skeleton, Spider, etc. all inherit from Enemy)
        Enemy[] allEnemies = FindObjectsByType<Enemy>(FindObjectsSortMode.None);        BoxCollider2D roomCollider = GetComponent<BoxCollider2D>();
                
        if (roomCollider != null)
        {
            foreach (Enemy enemy in allEnemies)
            {
                Vector2 enemyPos = enemy.transform.position;
                bool isInside = roomCollider.bounds.Contains(enemyPos);
                                
                if (isInside)
                {
                    enemiesInRoom.Add(enemy);
                }
            }
        }
    }
}