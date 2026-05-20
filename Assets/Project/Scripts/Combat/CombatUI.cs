using UnityEngine;
using UnityEngine.UI;
using ProjectEva.Player;

namespace ProjectEva.Combat
{
    public class CombatUI : MonoBehaviour
    {
        [SerializeField] private Slider playerHPBar, enemyHPBar;
        [SerializeField] private Image playerHPFill, enemyHPFill;
        [SerializeField] private Text playerHPText, enemyHPText, turnText, parryPromptText, actionPromptText;
        private PlayerStats playerStats;
        private EnemyUnit currentTarget;

        public void Initialize(PlayerStats stats)
        {
            playerStats = stats;
            playerStats.OnHealthChanged += UpdatePlayerHP;
            UpdatePlayerHP(playerStats.CurrentHP, playerStats.MaxHP);
        }

        private void OnDestroy() { if (playerStats) playerStats.OnHealthChanged -= UpdatePlayerHP; }

        public void UpdatePlayerHP(float current, float max) => UpdateBar(playerHPBar, playerHPFill, playerHPText, current, max);

        public void ShowEnemyHP(EnemyUnit enemy)
        {
            currentTarget = enemy;
            if (enemy) UpdateBar(enemyHPBar, enemyHPFill, enemyHPText, enemy.currentHP, enemy.maxHP);
        }

        public void UpdateEnemyHP() { if (currentTarget) UpdateBar(enemyHPBar, enemyHPFill, enemyHPText, currentTarget.currentHP, currentTarget.maxHP); }

        private void UpdateBar(Slider slider, Image fill, Text text, float current, float max)
        {
            if (slider) { slider.maxValue = max; slider.value = current; }
            if (fill) fill.color = current / max > 0.6f ? Color.green : (current / max > 0.3f ? Color.yellow : Color.red);
            if (text) text.text = $"{current:F0} / {max:F0}";
        }

        public void ShowParryPrompt(bool show) { if (parryPromptText) parryPromptText.gameObject.SetActive(show); }
        public void SetTurnText(string t) { if (turnText) turnText.text = t; }
        public void ShowActionPrompts(bool show) { if (actionPromptText) actionPromptText.gameObject.SetActive(show); }
    }
}