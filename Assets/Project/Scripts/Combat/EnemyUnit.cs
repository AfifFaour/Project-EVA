using UnityEngine;
using ProjectEva.Data;
using ProjectEva.Managers;

namespace ProjectEva.Combat
{
    public class EnemyUnit : MonoBehaviour
    {
        public string enemyName;
        public float maxHP, currentHP, baseDamage;
        public EnemyDataSO Data { get; private set; }

        public void Initialize(EnemyEntry entry)
        {
            Data = entry.enemyData;
            enemyName = Data.enemyName;
            maxHP = Data.baseMaxHP * WorldManager.Instance.GetEnemyHPMultiplier();
            currentHP = maxHP;
            baseDamage = Data.baseDamage * WorldManager.Instance.GetEnemyDamageMultiplier();
        }

        public void TakeDamage(float amount)
        {
            currentHP -= amount;
            if (currentHP <= 0) { CombatManager.Instance.EnemyDefeated(this); Destroy(gameObject); }
        }

        public float GetDamage() => baseDamage;
    }
}