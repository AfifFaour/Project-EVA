using UnityEngine;
using ProjectEva.Player;
using ProjectEva.Data;
using ProjectEva.Managers;

namespace ProjectEva.Interaction
{
    public class LootBox : MonoBehaviour, IInteractable
    {
        public enum RewardType { Material, Consumable, Weapon, Gold }
        [SerializeField] private RewardType rewardType;
        [SerializeField] private MaterialSO materialReward; [SerializeField] private int materialAmount = 1;
        [SerializeField] private ConsumableSO consumableReward;
        [SerializeField] private WeaponSO weaponReward;
        [SerializeField] private int goldAmount = 50;
        private bool opened;

        public string GetInteractionPrompt() => opened ? "Empty" : "Open Chest";

        public void Interact(PlayerController player)
        {
            if (opened) return;
            opened = true;
            switch (rewardType)
            {
                case RewardType.Material: if (materialReward) Inventory.Instance?.AddMaterial(materialReward, materialAmount); break;
                case RewardType.Consumable: if (consumableReward) player.GetComponent<ConsumableInventory>()?.Add(consumableReward, 1); break;
                case RewardType.Weapon: if (weaponReward) player.GetComponent<WeaponHandler>()?.AddWeapon(weaponReward); break;
                case RewardType.Gold: if (GameManager.Instance?.GoldCurrency) Inventory.Instance?.AddMaterial(GameManager.Instance.GoldCurrency, goldAmount); break;
            }
            gameObject.SetActive(false);
        }
    }
}