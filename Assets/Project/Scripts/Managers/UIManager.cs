using UnityEngine;
using ProjectEva.Managers;
using ProjectEva.Player;

namespace ProjectEva.UI
{
    public class UIManager : MonoBehaviour
    {
        public static UIManager Instance { get; private set; }
        [SerializeField] private GameObject characterStatsPanel;
        [SerializeField] private GameObject weaponUpgradePanel;
        [SerializeField] private GameObject inventoryPanel;
        private PlayerStats playerStats;
        private WeaponHandler weaponHandler;
        private Inventory inventory;
        private ConsumableInventory consumableInventory;

        public bool IsAnyPanelOpen =>
            (characterStatsPanel != null && characterStatsPanel.activeSelf) ||
            (weaponUpgradePanel != null && weaponUpgradePanel.activeSelf) ||
            (inventoryPanel != null && inventoryPanel.activeSelf);

        private void Awake() => Instance = this;

        private void Start()
        {
            playerStats = FindFirstObjectByType<PlayerStats>();
            if (playerStats)
            {
                inventory = playerStats.GetComponent<Inventory>();
                consumableInventory = playerStats.GetComponent<ConsumableInventory>();
                weaponHandler = playerStats.GetComponent<WeaponHandler>();
            }
        }

        private void Update()
        {
            if (!GameManager.Instance || GameManager.Instance.CurrentState != GameState.Exploration) return;
            if (Input.GetKeyDown(KeyCode.C)) ToggleCharacterStats();
            if (Input.GetKeyDown(KeyCode.U)) ToggleWeaponUpgrade();
            if (Input.GetKeyDown(KeyCode.Tab)) ToggleInventory();
        }

        private void ToggleCharacterStats()
        {
            if (!characterStatsPanel) return;
            bool active = !characterStatsPanel.activeSelf;
            characterStatsPanel.SetActive(active);
            if (active) characterStatsPanel.GetComponent<CharacterStatsUI>()?.Open(playerStats);
            UpdateCursor();
        }

        private void ToggleWeaponUpgrade()
        {
            if (!weaponUpgradePanel) return;
            bool active = !weaponUpgradePanel.activeSelf;
            weaponUpgradePanel.SetActive(active);
            if (active) weaponUpgradePanel.GetComponent<WeaponUpgradeUI>()?.Open(weaponHandler);
            UpdateCursor();
        }

        private void ToggleInventory()
        {
            if (!inventoryPanel) return;
            bool active = !inventoryPanel.activeSelf;
            inventoryPanel.SetActive(active);
            if (active) inventoryPanel.GetComponent<InventoryUI>()?.Open(inventory, consumableInventory, weaponHandler);
            UpdateCursor();
        }

        public void UpdateCursor()
        {
            Cursor.lockState = IsAnyPanelOpen ? CursorLockMode.None : CursorLockMode.Locked;
            Cursor.visible = IsAnyPanelOpen;
        }
    }
}