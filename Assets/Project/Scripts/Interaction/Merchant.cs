using System.Collections.Generic;
using UnityEngine;
using ProjectEva.Player;
using ProjectEva.Data;

namespace ProjectEva.Interaction
{
    public class Merchant : MonoBehaviour, IInteractable
    {
        [SerializeField] private List<ShopItem> shopItems = new List<ShopItem>();
        public void Interact(PlayerController player) => ShopUI.Instance?.Show(shopItems, player.GetComponent<Inventory>());
        public string GetInteractionPrompt() => "Shop";
    }

    [System.Serializable]
    public class ShopItem { public ItemSO item; public int cost; }
}