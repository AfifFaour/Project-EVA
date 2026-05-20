using System.Collections.Generic;
using UnityEngine;
using ProjectEva.Data;

namespace ProjectEva.Player
{
    public class ConsumableInventory : MonoBehaviour
    {
        public List<ConsumableSlot> items = new List<ConsumableSlot>();

        public void Add(ConsumableSO item, int amount = 1)
        {
            var slot = items.Find(s => s.item == item);
            if (slot != null) slot.amount += amount;
            else items.Add(new ConsumableSlot { item = item, amount = amount });
        }

        public void Remove(ConsumableSO item)
        {
            var slot = items.Find(s => s.item == item);
            if (slot == null) return;
            slot.amount--;
            if (slot.amount <= 0) items.Remove(slot);
        }
    }

    [System.Serializable]
    public class ConsumableSlot
    {
        public ConsumableSO item;
        public int amount;
    }
}