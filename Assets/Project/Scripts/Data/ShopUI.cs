using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using ProjectEva.Player;
using ProjectEva.Data;
using ProjectEva.Managers;

namespace ProjectEva.Interaction
{
    public class ShopUI : MonoBehaviour
    {
        public static ShopUI Instance { get; private set; }

        [Header("UI Elements")] [SerializeField] private Text titleText; [SerializeField] private Button closeButton;
        [Header("Slot 1")] [SerializeField] private Text itemName1; [SerializeField] private Text cost1; [SerializeField] private Button buyButton1;
        [Header("Slot 2")] [SerializeField] private Text itemName2; [SerializeField] private Text cost2; [SerializeField] private Button buyButton2;
        [Header("Slot 3")] [SerializeField] private Text itemName3; [SerializeField] private Text cost3; [SerializeField] private Button buyButton3;

        private Inventory playerInventory;
        private List<ShopItem> currentItems;

        private void Awake()
        {
            if (Instance) { Destroy(gameObject); return; }
            Instance = this;
            DontDestroyOnLoad(gameObject);
            closeButton.onClick.AddListener(Close);
            gameObject.SetActive(false);
        }

        public void Show(List<ShopItem> items, Inventory inventory)
        {
            playerInventory = inventory;
            currentItems = items;
            gameObject.SetActive(true);
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;

            SetupSlot(itemName1, cost1, buyButton1, items, 0);
            SetupSlot(itemName2, cost2, buyButton2, items, 1);
            SetupSlot(itemName3, cost3, buyButton3, items, 2);
        }

        private void SetupSlot(Text nameText, Text costText, Button button, List<ShopItem> items, int index)
        {
            bool active = index < items.Count;
            nameText.transform.parent.gameObject.SetActive(active);
            if (active)
            {
                ShopItem item = items[index];
                nameText.text = item.item.itemName;
                costText.text = item.cost.ToString() + " Gold";
                button.onClick.RemoveAllListeners();
                button.onClick.AddListener(() => Buy(item));
            }
        }

        private void Buy(ShopItem item)
        {
            MaterialSO gold = GameManager.Instance?.GoldCurrency;
            if (!gold) return;
            if (!playerInventory.HasMaterials(gold, item.cost)) return;
            playerInventory.RemoveMaterials(gold, item.cost);

            if (item.item is WeaponSO w) FindFirstObjectByType<WeaponHandler>()?.AddWeapon(w);
            else if (item.item is ConsumableSO c) FindFirstObjectByType<ConsumableInventory>()?.Add(c, 1);
            Close();
        }

        public void Close()
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            gameObject.SetActive(false);
        }
    }
}