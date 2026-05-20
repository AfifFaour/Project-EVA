using System;
using UnityEngine;
using ProjectEva.Combat;
using ProjectEva.Managers;

namespace ProjectEva.Player
{
    public class PlayerStats : MonoBehaviour
    {
        public int Level { get; private set; } = 1;
        public int AvailablePoints { get; private set; }
        public int Strength { get; private set; } = 5;
        public int Agility { get; private set; } = 5;
        public int Intelligence { get; private set; } = 5;
        private const int PointsPerLevel = 3;

        [SerializeField] private float maxBaseHP = 100f;
        public float MaxHP { get; private set; }
        public float CurrentHP { get; private set; }

        public int currentXP;
        public int xpToNextLevel = 50;
        public int totalXP { get; private set; }

        public event Action<float, float> OnHealthChanged;
        public event Action<int, int> OnXPChanged;
        public event Action<int> OnLevelUp;
        public event Action OnStatsChanged;

        private void Awake()
        {
            MaxHP = maxBaseHP;
            CurrentHP = MaxHP;
        }

        public void GainXP(int amount)
        {
            currentXP += amount;
            totalXP += amount;
            while (currentXP >= xpToNextLevel) GainLevel();
            OnXPChanged?.Invoke(currentXP, xpToNextLevel);
            WorldManager.Instance?.CheckWorldLevelUp(totalXP);
        }

        private void GainLevel()
        {
            Level++;
            AvailablePoints += PointsPerLevel;
            currentXP -= xpToNextLevel;
            xpToNextLevel = Mathf.RoundToInt(xpToNextLevel * 1.2f);
            MaxHP += 10f;
            CurrentHP = MaxHP;
            OnLevelUp?.Invoke(Level);
            OnHealthChanged?.Invoke(CurrentHP, MaxHP);
            OnXPChanged?.Invoke(currentXP, xpToNextLevel);
        }

        public bool AllocatePoint(StatType stat)
        {
            if (AvailablePoints <= 0) return false;
            switch (stat)
            {
                case StatType.Strength: Strength++; break;
                case StatType.Agility: Agility++; break;
                case StatType.Intelligence: Intelligence++; break;
                default: return false;
            }
            AvailablePoints--;
            OnStatsChanged?.Invoke();
            return true;
        }

        public float GetDamageMultiplier() => 1f + (Strength - 5) * 0.05f;
        public float GetParryWindowDuration() => 0.3f + (Agility - 5) * 0.02f;

        public void Heal(float amount)
        {
            CurrentHP = Mathf.Min(CurrentHP + amount, MaxHP);
            OnHealthChanged?.Invoke(CurrentHP, MaxHP);
        }

        public void TakeDamage(float amount)
        {
            CurrentHP -= amount;
            if (CurrentHP <= 0) { CurrentHP = 0; Die(); }
            OnHealthChanged?.Invoke(CurrentHP, MaxHP);
        }

        private void Die() => CombatManager.Instance?.EndCombat(false);
    }

    public enum StatType { Strength, Agility, Intelligence }
}