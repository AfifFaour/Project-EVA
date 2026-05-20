using System.Text;
using UnityEngine;
using UnityEngine.UI;
using ProjectEva.Data;

namespace ProjectEva.Player
{
    public class InventoryUI : MonoBehaviour
    {
        [SerializeField] private Text materialsText;
        [SerializeField] private Text consumablesText;
        [SerializeField] private Text weaponsText;
        [SerializeField] private Button closeButton;

        private void Awake()
        {
            closeButton.onClick.AddListener(Close);
            gameObject.SetActive(false);
        }

        public void Open(Inventory inv, ConsumableInventory consInv, WeaponHandler weaponHand)
        {
            Inventory inventory = Inventory.Instance;
            ConsumableInventory consumableInventory = FindFirstObjectByType<ConsumableInventory>();
            WeaponHandler weaponHandler = FindFirstObjectByType<WeaponHandler>();

            if (materialsText == null) materialsText = GetComponentInChildren<Text>();
            if (consumablesText == null) consumablesText = GetComponentInChildren<Text>();
            if (weaponsText == null) weaponsText = GetComponentInChildren<Text>();

            Refresh(inventory, consumableInventory, weaponHandler);
            gameObject.SetActive(true);
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }

        private void Refresh(Inventory inventory, ConsumableInventory consumableInventory, WeaponHandler weaponHandler)
        {
            if (materialsText != null)
            {
                StringBuilder sb = new StringBuilder(); sb.AppendLine("Materials:");
                if (inventory != null) foreach (var kvp in inventory.GetAllMaterials()) sb.AppendLine($"{kvp.Key.itemName} x{kvp.Value}");
                materialsText.text = sb.ToString();
            }
            if (consumablesText != null)
            {
                StringBuilder sb = new StringBuilder(); sb.AppendLine("Consumables:");
                if (consumableInventory != null) foreach (var slot in consumableInventory.items) if (slot.amount > 0) sb.AppendLine($"{slot.item.itemName} x{slot.amount}");
                consumablesText.text = sb.ToString();
            }
            if (weaponsText != null)
            {
                StringBuilder sb = new StringBuilder(); sb.AppendLine("Weapons:");
                if (weaponHandler != null) foreach (var w in weaponHandler.WeaponInventory) sb.AppendLine($"{w.itemName} (Lv.{w.currentUpgradeLevel})");
                weaponsText.text = sb.ToString();
            }
        }

        public void Close()
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            gameObject.SetActive(false);
        }
    }
}