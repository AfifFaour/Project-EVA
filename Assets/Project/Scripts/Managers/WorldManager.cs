using UnityEngine;

namespace ProjectEva.Managers
{
    public class WorldManager : MonoBehaviour
    {
        public static WorldManager Instance { get; private set; }
        public int CurrentWorldLevel { get; private set; } = 1;

        private int[] levelThresholds = { 0, 100, 250, 450, 700, 1000 };

        private void Awake()
        {
            if (Instance != null) { Destroy(gameObject); return; }
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }

        public void CheckWorldLevelUp(int totalXP)
        {
            int nextLevel = CurrentWorldLevel + 1;
            if (nextLevel >= levelThresholds.Length) return;
            if (totalXP >= levelThresholds[nextLevel])
                CurrentWorldLevel = nextLevel;
        }

        public void AdvanceWorldLevel() => CurrentWorldLevel++;
        public float GetEnemyHPMultiplier() => 1f + (CurrentWorldLevel - 1) * 0.15f;
        public float GetEnemyDamageMultiplier() => 1f + (CurrentWorldLevel - 1) * 0.1f;
    }
}