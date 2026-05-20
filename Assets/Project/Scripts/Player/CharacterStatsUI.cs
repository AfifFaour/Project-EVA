using UnityEngine;
using UnityEngine.UI;
using ProjectEva.Player;

namespace ProjectEva.UI
{
    public class CharacterStatsUI : MonoBehaviour
    {
        [SerializeField] private Text levelText, xpText, pointsText, strText, agiText, intText;
        [SerializeField] private Button addStrBtn, addAgiBtn, addIntBtn, closeBtn;
        private PlayerStats playerStats;

        private void Awake()
        {
            closeBtn.onClick.AddListener(() => { gameObject.SetActive(false); UIManager.Instance?.UpdateCursor(); });
            addStrBtn.onClick.AddListener(() => Allocate(StatType.Strength));
            addAgiBtn.onClick.AddListener(() => Allocate(StatType.Agility));
            addIntBtn.onClick.AddListener(() => Allocate(StatType.Intelligence));
            gameObject.SetActive(false);
        }

        public void Open(PlayerStats stats)
        {
            if (playerStats) Unsubscribe();
            playerStats = stats;
            if (playerStats)
            {
                playerStats.OnXPChanged += OnXPChanged;
                playerStats.OnLevelUp += OnLevelUp;
                playerStats.OnStatsChanged += Refresh;
            }
            Refresh();
            gameObject.SetActive(true);
        }

        private void Unsubscribe()
        {
            if (!playerStats) return;
            playerStats.OnXPChanged -= OnXPChanged;
            playerStats.OnLevelUp -= OnLevelUp;
            playerStats.OnStatsChanged -= Refresh;
        }
        private void OnDestroy() => Unsubscribe();

        private void OnXPChanged(int cur, int next) => Refresh();
        private void OnLevelUp(int lvl) => Refresh();
        private void Refresh()
        {
            if (!playerStats) return;
            levelText.text = $"Level: {playerStats.Level}";
            xpText.text = $"XP: {playerStats.currentXP}/{playerStats.xpToNextLevel}";
            pointsText.text = $"Points: {playerStats.AvailablePoints}";
            strText.text = $"STR: {playerStats.Strength}";
            agiText.text = $"AGI: {playerStats.Agility}";
            intText.text = $"INT: {playerStats.Intelligence}";
        }

        private void Allocate(StatType s) { if (playerStats && playerStats.AllocatePoint(s)) Refresh(); }
    }
}