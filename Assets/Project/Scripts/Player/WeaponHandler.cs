using System.Collections.Generic;
using UnityEngine;
using ProjectEva.Data;

namespace ProjectEva.Player
{
    public class WeaponHandler : MonoBehaviour
    {
        [SerializeField] private List<WeaponSO> weaponInventory = new List<WeaponSO>();
        [SerializeField] private WeaponSO currentWeapon;

        public WeaponSO CurrentWeapon => currentWeapon;
        public List<WeaponSO> WeaponInventory => weaponInventory;

        public void AddWeapon(WeaponSO weapon) { if (!weaponInventory.Contains(weapon)) weaponInventory.Add(weapon); }
        public void EquipWeapon(WeaponSO weapon) { if (weaponInventory.Contains(weapon)) currentWeapon = weapon; }
        public float GetWeaponDamage() => currentWeapon ? currentWeapon.baseDamage : 10f;

        public bool UpgradeWeapon()
        {
            if (!currentWeapon) return false;
            Inventory inv = GetComponent<Inventory>();
            if (!inv) return false;
            return currentWeapon.LevelUpWeapon(inv);
        }
    }
}