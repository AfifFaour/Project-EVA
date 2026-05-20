using System.Collections.Generic;
using UnityEngine;
using ProjectEva.Player;

namespace ProjectEva.Data
{
    public abstract class ItemSO : ScriptableObject { public string itemName; public Sprite icon; public string description; }

    [CreateAssetMenu(menuName = "Project Eva/Material")]
    public class MaterialSO : ItemSO { public MaterialRarity rarity; public int maxStack = 99; }
    public enum MaterialRarity { Common, Rare, Epic, Legendary }

    [CreateAssetMenu(menuName = "Project Eva/Consumable")]
    public class ConsumableSO : ItemSO { public float healAmount = 30f; public void Use(PlayerStats stats) => stats.Heal(healAmount); }

    [CreateAssetMenu(menuName = "Project Eva/Weapon")]
    public class WeaponSO : ItemSO
    {
        public float baseDamage = 10f, agilityBonus;
        public int requiredLevel = 1, currentUpgradeLevel;
        public WeaponUpgradeCost[] upgradeCosts;
        public List<SkillSO> skills = new List<SkillSO>();
        public bool LevelUpWeapon(Inventory inv)
        {
            if (currentUpgradeLevel >= upgradeCosts.Length) return false;
            var cost = upgradeCosts[currentUpgradeLevel];
            if (!inv.HasMaterials(cost.requiredMaterial, cost.amount)) return false;
            inv.RemoveMaterials(cost.requiredMaterial, cost.amount);
            currentUpgradeLevel++; baseDamage *= 1.1f; agilityBonus += 0.02f; return true;
        }
    }
    [System.Serializable] public struct WeaponUpgradeCost { public MaterialSO requiredMaterial; public int amount; }

    [CreateAssetMenu(menuName = "Project Eva/Skill")]
    public class SkillSO : ScriptableObject { public string skillName; public float damageMultiplier = 1.5f; }

    [CreateAssetMenu(menuName = "Project Eva/Enemy Data")]
    public class EnemyDataSO : ScriptableObject
    {
        public string enemyName;
        public float baseMaxHP = 30f, baseDamage = 8f;
        public int xpReward = 20;
        public GameObject enemyPrefab;
        public List<MaterialDrop> possibleDrops = new List<MaterialDrop>();
    }
    [System.Serializable] public struct MaterialDrop { public MaterialSO material; public int minAmount, maxAmount; [Range(0,1)] public float dropChance; }

    [CreateAssetMenu(menuName = "Project Eva/Encounter")]
    public class EncounterData : ScriptableObject { public string encounterName; public List<EnemyEntry> enemies = new List<EnemyEntry>(); }
    [System.Serializable] public struct EnemyEntry { public EnemyDataSO enemyData; public Vector3 spawnPosition; }
}