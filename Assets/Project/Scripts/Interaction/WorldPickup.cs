using UnityEngine;
using ProjectEva.Data;
using ProjectEva.Player;
using ProjectEva.Managers;

namespace ProjectEva.Interaction
{
    public class WorldPickup : MonoBehaviour
    {
        public enum PickupType { Material, Consumable, Gold, Health }
        [SerializeField] private PickupType pickupType;
        [SerializeField] private MaterialSO material; [SerializeField] private int materialAmount = 1;
        [SerializeField] private ConsumableSO consumable;
        [SerializeField] private int goldAmount = 10;
        [SerializeField] private float healAmount = 25f;

        private void OnTriggerEnter(Collider other)
        {
            if (!other.CompareTag("Player")) return;
            switch (pickupType)
            {
                case PickupType.Material: Inventory.Instance?.AddMaterial(material, materialAmount); break;
                case PickupType.Consumable: other.GetComponent<ConsumableInventory>()?.Add(consumable, 1); break;
                case PickupType.Gold: if (GameManager.Instance?.GoldCurrency) Inventory.Instance?.AddMaterial(GameManager.Instance.GoldCurrency, goldAmount); break;
                case PickupType.Health: other.GetComponent<PlayerStats>()?.Heal(healAmount); break;
            }
            Destroy(gameObject);
        }
    }
}