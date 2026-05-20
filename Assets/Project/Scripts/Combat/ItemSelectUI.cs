using System;
using UnityEngine;
using UnityEngine.UI;
using ProjectEva.Player;
using ProjectEva.Data;

namespace ProjectEva.Combat
{
    public class ItemSelectUI : MonoBehaviour
    {
        [Header("Slot 1")] [SerializeField] private Text itemName1; [SerializeField] private Text count1; [SerializeField] private Button useButton1;
        [Header("Slot 2")] [SerializeField] private Text itemName2; [SerializeField] private Text count2; [SerializeField] private Button useButton2;
        [Header("Slot 3")] [SerializeField] private Text itemName3; [SerializeField] private Text count3; [SerializeField] private Button useButton3;
        [Header("Close")] [SerializeField] private Button closeButton;

        private Action<ConsumableSO> onItemUsed;

        private void Awake()
        {
            closeButton.onClick.AddListener(Close);
            gameObject.SetActive(false);
        }

        public void Initialize(ConsumableInventory inventory, Action<ConsumableSO> callback)
        {
            onItemUsed = callback;
            gameObject.SetActive(true);
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;

            SetupSlot(itemName1, count1, useButton1, inventory.items, 0);
            SetupSlot(itemName2, count2, useButton2, inventory.items, 1);
            SetupSlot(itemName3, count3, useButton3, inventory.items, 2);
        }

        private void SetupSlot(Text nameText, Text countText, Button button, System.Collections.Generic.List<ConsumableSlot> items, int index)
        {
            if (!nameText || !countText || !button) return;
            bool active = index < items.Count && items[index].amount > 0;
            nameText.transform.parent.gameObject.SetActive(active);
            if (active)
            {
                var slot = items[index];
                nameText.text = slot.item.itemName;
                countText.text = "x" + slot.amount;
                button.onClick.RemoveAllListeners();
                ConsumableSO item = slot.item;
                button.onClick.AddListener(() => { onItemUsed?.Invoke(item); Close(); });
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