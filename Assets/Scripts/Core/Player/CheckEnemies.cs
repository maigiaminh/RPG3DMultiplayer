using System.Collections.Generic;
using UnityEngine;

public class CheckEnemies : MonoBehaviour
{
    private HashSet<EnemyVisibleControl> activeEnemies = new HashSet<EnemyVisibleControl>();

    private void Update()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, 25f, LayerMask.GetMask("Enemy"));
        HashSet<EnemyVisibleControl> currentEnemies = new HashSet<EnemyVisibleControl>();

        foreach (var collider in colliders)
        {
            if (!collider.TryGetComponent(out EnemyVisibleControl enemy)) continue;
            Debug.Log(enemy.name);
            currentEnemies.Add(enemy);

            if (!activeEnemies.Contains(enemy))
            {
                enemy.EnableLogic(true);
                activeEnemies.Add(enemy);
            }
        }

        // Tắt logic của enemy rời khỏi vùng kiểm tra
        foreach (var enemy in activeEnemies)
        {
            if (!currentEnemies.Contains(enemy))
            {
                enemy.EnableLogic(false);
            }
        }

        // Cập nhật danh sách activeEnemies
        activeEnemies = currentEnemies;
    }
}
