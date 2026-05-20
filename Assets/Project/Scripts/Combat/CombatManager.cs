using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ProjectEva.Managers;
using ProjectEva.Data;
using ProjectEva.Player;

namespace ProjectEva.Combat
{
    public enum CombatTurn { PlayerTurn, EnemyTurn, Victory, Defeat }

    public class CombatManager : MonoBehaviour
    {
        public static CombatManager Instance { get; private set; }

        [Header("UI Prefabs")]
        [SerializeField] private GameObject combatCanvasPrefab;
        [SerializeField] private GameObject combatSummaryPrefab;

        [Header("Item Panel (Scene Instance)")]
        [SerializeField] private ItemSelectUI itemSelectPanelInstance;

        [Header("Battle Arena")]
        [SerializeField] private Transform battleArena;
        [SerializeField] private Camera battleCamera;
        [SerializeField] private CameraShake battleCameraShake;

        [Header("VFX")]
        [SerializeField] private GameObject hitSparksPrefab;
        [SerializeField] private GameObject parryFlashPrefab;
        [SerializeField] private GameObject deathEffectPrefab;

        private GameObject currentCanvas;
        private CombatUI combatUI;
        private List<EnemyUnit> aliveEnemies = new List<EnemyUnit>();
        private PlayerStats playerStats;
        private WeaponHandler weaponHandler;
        private ConsumableInventory consumableInventory;
        private EncounterData currentEncounter;

        private Vector3 playerOriginalPosition;
        private Quaternion playerOriginalRotation;
        private Camera mainCamera;

        private float totalDamageDealt, totalDamageTaken, totalHealingDone;
        private List<MaterialDrop> collectedDrops = new List<MaterialDrop>();

        public CombatTurn CurrentTurn { get; private set; }
        private enum PlayerAction { None, Attack, Skill, Item }
        private PlayerAction selectedAction;
        private SkillSO selectedSkill;
        private bool actionConfirmed;

        private void Awake()
        {
            if (Instance != null && Instance != this) { Destroy(gameObject); return; }
            Instance = this;
            DontDestroyOnLoad(gameObject);
            mainCamera = Camera.main;
            if (battleCamera) battleCamera.enabled = false;
        }
        private void OnDestroy() { if (Instance == this) Instance = null; }

        // ========== START COMBAT ==========
        public void StartCombat(EncounterData encounter)
        {
            if (this == null || gameObject == null) return;
            currentEncounter = encounter;
            playerStats = FindFirstObjectByType<PlayerStats>();
            if (playerStats == null) return;
            weaponHandler = playerStats.GetComponent<WeaponHandler>();
            consumableInventory = playerStats.GetComponent<ConsumableInventory>();

            Transform playerTransform = playerStats.transform;
            playerOriginalPosition = playerTransform.position;
            playerOriginalRotation = playerTransform.rotation;

            if (battleArena == null) return;

            Transform playerSpot = battleArena.Find("PlayerSpot");
            List<Transform> enemySpots = new List<Transform>();
            foreach (Transform child in battleArena)
                if (child.name.StartsWith("EnemySpot"))
                    enemySpots.Add(child);

            if (playerSpot) { playerTransform.position = playerSpot.position; playerTransform.rotation = playerSpot.rotation; }

            if (battleCamera) { mainCamera.enabled = false; battleCamera.enabled = true; }
            if (combatCanvasPrefab) { currentCanvas = Instantiate(combatCanvasPrefab); combatUI = currentCanvas.GetComponent<CombatUI>(); }

            totalDamageDealt = totalDamageTaken = totalHealingDone = 0f;
            collectedDrops.Clear();
            aliveEnemies.Clear();

            for (int i = 0; i < encounter.enemies.Count; i++)
            {
                EnemyEntry entry = encounter.enemies[i];
                if (entry.enemyData == null || entry.enemyData.enemyPrefab == null) continue;
                Vector3 spawnPos = i < enemySpots.Count ? enemySpots[i].position : battleArena.position + new Vector3(3 + i * 2, 0, 5);
                GameObject go = Instantiate(entry.enemyData.enemyPrefab, spawnPos, Quaternion.identity);
                EnemyUnit unit = go.GetComponent<EnemyUnit>();
                if (unit) { unit.Initialize(entry); aliveEnemies.Add(unit); }
            }

            if (combatUI)
            {
                combatUI.Initialize(playerStats);
                if (aliveEnemies.Count > 0) combatUI.ShowEnemyHP(aliveEnemies[0]);
                combatUI.SetTurnText("Your Turn");
                combatUI.ShowActionPrompts(true);
            }
            if (aliveEnemies.Count == 0) { EndCombat(true); return; }
            CurrentTurn = CombatTurn.PlayerTurn;
            StartCoroutine(PlayerTurn());
        }

        // ========== PLAYER TURN ==========
        private IEnumerator PlayerTurn()
        {
            selectedAction = PlayerAction.None; selectedSkill = null; actionConfirmed = false;
            while (!actionConfirmed)
            {
                if (Input.GetKeyDown(KeyCode.Space)) { selectedAction = PlayerAction.Attack; actionConfirmed = true; }
                else if (Input.GetKeyDown(KeyCode.Q) || Input.GetKeyDown(KeyCode.E))
                {
                    int idx = Input.GetKeyDown(KeyCode.Q) ? 0 : 1;
                    var skills = weaponHandler?.CurrentWeapon?.skills;
                    if (skills != null && idx < skills.Count) { selectedAction = PlayerAction.Skill; selectedSkill = skills[idx]; actionConfirmed = true; }
                }
                else if (Input.GetKeyDown(KeyCode.I))
                {
                    if (itemSelectPanelInstance != null && consumableInventory != null && consumableInventory.items.Count > 0)
                    {
                        itemSelectPanelInstance.Initialize(consumableInventory, (chosenItem) =>
                        {
                            if (chosenItem != null)
                            {
                                chosenItem.Use(playerStats);
                                totalHealingDone += chosenItem.healAmount;
                                consumableInventory.Remove(chosenItem);
                                combatUI?.UpdatePlayerHP(playerStats.CurrentHP, playerStats.MaxHP);
                            }
                            selectedAction = PlayerAction.Item;
                            actionConfirmed = true;
                        });
                    }
                }
                yield return null;
            }
            if (aliveEnemies.Count == 0) { EndCombat(true); yield break; }
            if (selectedAction != PlayerAction.Item)
            {
                EnemyUnit target = aliveEnemies[0];
                float baseDmg = weaponHandler ? weaponHandler.GetWeaponDamage() : 10f;
                float dmg = (selectedAction == PlayerAction.Skill && selectedSkill) ? baseDmg * selectedSkill.damageMultiplier * playerStats.GetDamageMultiplier() : baseDmg * playerStats.GetDamageMultiplier();
                target.TakeDamage(dmg);
                totalDamageDealt += dmg;
                if (hitSparksPrefab) Instantiate(hitSparksPrefab, target.transform.position + Vector3.up, Quaternion.identity);
            }
            combatUI?.UpdateEnemyHP();
            combatUI?.ShowActionPrompts(false);
            yield return new WaitForSeconds(0.5f);
            if (aliveEnemies.Count == 0) { EndCombat(true); yield break; }
            CurrentTurn = CombatTurn.EnemyTurn;
            combatUI?.SetTurnText("Enemy Turn");
            StartCoroutine(EnemyTurn());
        }

        // ========== ENEMY TURN ==========
        private IEnumerator EnemyTurn()
        {
            foreach (var enemy in aliveEnemies.ToArray())
            {
                if (enemy == null) continue;
                yield return StartCoroutine(EnemyAttackCoroutine(enemy));
                if (playerStats.CurrentHP <= 0) { EndCombat(false); yield break; }
                if (aliveEnemies.Count == 0) { EndCombat(true); yield break; }
            }
            if (aliveEnemies.Count == 0) { EndCombat(true); yield break; }
            CurrentTurn = CombatTurn.PlayerTurn;
            combatUI?.SetTurnText("Your Turn");
            StartCoroutine(PlayerTurn());
        }

        // ========== ENEMY ATTACK + PARRY ==========
        private IEnumerator EnemyAttackCoroutine(EnemyUnit enemy)
        {
            combatUI?.ShowParryPrompt(true);
            float window = playerStats.GetParryWindowDuration();
            float timer = 0f; bool parried = false;
            while (timer < window) { if (Input.GetKeyDown(KeyCode.Space)) { parried = true; break; } timer += Time.deltaTime; yield return null; }
            combatUI?.ShowParryPrompt(false);
            if (parried)
            {
                if (battleCameraShake) battleCameraShake.Shake(0.15f, 0.2f);
                if (parryFlashPrefab && enemy) Instantiate(parryFlashPrefab, enemy.transform.position + Vector3.up, Quaternion.identity);
                float counter = (weaponHandler ? weaponHandler.GetWeaponDamage() : 10f) * playerStats.GetDamageMultiplier();
                enemy.TakeDamage(counter);
                totalDamageDealt += counter;
                combatUI?.UpdateEnemyHP();
                if (aliveEnemies.Count == 0) { yield return new WaitForSeconds(0.3f); EndCombat(true); yield break; }
            }
            else
            {
                float dmg = enemy.GetDamage();
                playerStats.TakeDamage(dmg);
                totalDamageTaken += dmg;
                combatUI?.UpdatePlayerHP(playerStats.CurrentHP, playerStats.MaxHP);
            }
            yield return new WaitForSeconds(0.5f);
        }

        public void EnemyDefeated(EnemyUnit unit)
        {
            aliveEnemies.Remove(unit);
            if (deathEffectPrefab && unit) Instantiate(deathEffectPrefab, unit.transform.position, Quaternion.identity);
            if (unit.Data) playerStats.GainXP(unit.Data.xpReward);
        }

        // ========== END COMBAT ==========
        public void EndCombat(bool victory)
        {
            StopAllCoroutines();
            CurrentTurn = victory ? CombatTurn.Victory : CombatTurn.Defeat;
            if (battleCamera) battleCamera.enabled = false;
            if (mainCamera) mainCamera.enabled = true;
            playerStats.transform.position = playerOriginalPosition;
            playerStats.transform.rotation = playerOriginalRotation;

            if (victory && currentEncounter != null && Inventory.Instance)
            {
                foreach (var entry in currentEncounter.enemies)
                {
                    if (!entry.enemyData) continue;
                    foreach (var drop in entry.enemyData.possibleDrops)
                    {
                        if (!drop.material) continue;
                        if (Random.value <= drop.dropChance)
                        {
                            int amt = Random.Range(drop.minAmount, drop.maxAmount + 1);
                            Inventory.Instance.AddMaterial(drop.material, amt);
                            collectedDrops.Add(new MaterialDrop { material = drop.material, minAmount = amt, maxAmount = amt, dropChance = 1f });
                        }
                    }
                }
            }
            if (currentCanvas) Destroy(currentCanvas);
            StartCoroutine(ShowSummary(victory));
        }

        private IEnumerator ShowSummary(bool victory)
        {
            if (!combatSummaryPrefab) { GameManager.Instance.ExitCombat(); yield break; }
            GameObject go = Instantiate(combatSummaryPrefab);
            var ui = go.GetComponent<CombatSummaryUI>();
            if (ui) { ui.Show(victory, totalDamageDealt, totalDamageTaken, totalHealingDone, collectedDrops); yield return new WaitUntil(() => ui.HasConfirmed); }
            Destroy(go);
            GameManager.Instance.ExitCombat();
        }
    }
}