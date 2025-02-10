using System;
using System.Collections.Generic;
using UnityEngine;

public class DungeonSpecialUIPanel : MonoBehaviour
{
    public List<BossStateItem> _bossStateItems = new List<BossStateItem>();
    public Sprite defaultBossPortrait;
    private void Awake()
    {
        DungeonSpecialUIManager.Instance.DungeonSpecialUIPanel = this;
    }


    public void UpdateBossStateItemUI(List<Enemy> enemies)
    {
        for (int i = 0; i < enemies.Count; i++)
        {
            // _bossStateItems[i].gameObject.SetActive(i < enemies.Count);
            // if (i >= enemies.Count) continue;
            // _bossStateItems[i].BossPortrait.sprite = enemies[i].enemyData.Portrait == null ? defaultBossPortrait : enemies[i].enemyData.Portrait;
            // _bossStateItems[i].BossHPBar.fillAmount = (float)enemies[i].Health / enemies[i].MaxHealth;
            // _bossStateItems[i].BossHPText.text = $"{enemies[i].Health} / {enemies[i].MaxHealth}";

            Optional<Enemy> optionalEnemy = i < enemies.Count ? Optional<Enemy>.Some(enemies[i]) : Optional<Enemy>.None();

            _bossStateItems[i].gameObject.SetActive(optionalEnemy.HasValue);

            optionalEnemy.Match(
                enemy =>
                {
                    var portrait = enemy.enemyData.Portrait == null ? defaultBossPortrait : enemy.enemyData.Portrait;
                    var currentHealth = enemy.Health;
                    var maxHealth = enemy.MaxHealth;
                    
                    _bossStateItems[i].UpdateItem(portrait, currentHealth, maxHealth);
                },
                () =>
                {
                    _bossStateItems[i].UpdateItem(defaultBossPortrait, 0, 0);
                }
            );
        }
    }




}
