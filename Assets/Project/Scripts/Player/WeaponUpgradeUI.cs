using UnityEngine;
using UnityEngine.UI;
using ProjectEva.Player;
using ProjectEva.Data;

namespace ProjectEva.UI
{
    public class WeaponUpgradeUI : MonoBehaviour
    {
        [SerializeField] private Text weaponNameText, weaponStatsText, requiredMaterialsText;
        [SerializeField] private Button upgradeButton, closeButton;
        private WeaponHandler weaponHandler;
        private Inventory inventory;

        private void Awake()
        {
            closeButton.onClick.AddListener(() => { gameObject.SetActive(false); UIManager.Instance?.UpdateCursor(); });
            upgradeButton.onClick.AddListener(OnUpgradeClicked);
            gameObject.SetActive(false);
        }

        public void Open(WeaponHandler handler)
        {
            weaponHandler = handler;
            inventory = handler?.GetComponent<Inventory>();
            Refresh();
            gameObject.SetActive(true);
        }

        private void Refresh()
        {
            var w = weaponHandler?.CurrentWeapon;
            if (!w) { weaponNameText.text = "No weapon"; upgradeButton.interactable = false; return; }
            weaponNameText.text = $"{w.itemName} (Lv.{w.currentUpgradeLevel})";
            weaponStatsText.text = $"Damage: {w.baseDamage:F0}";
            if (w.currentUpgradeLevel < w.upgradeCosts.Length)
            {
                var cost = w.upgradeCosts[w.currentUpgradeLevel];
                int owned = inventory ? inventory.GetMaterialCount(cost.requiredMaterial) : 0;
                requiredMaterialsText.text = $"Cost: {owned}/{cost.amount} {cost.requiredMaterial.itemName}";
                upgradeButton.interactable = owned >= cost.amount;
            }
            else { requiredMaterialsText.text = "Max Level"; upgradeButton.interactable = false; }
        }

        private void OnUpgradeClicked() { if (weaponHandler?.UpgradeWeapon() == true) Refresh(); }
    }
}