using System.Collections.Generic;
using UnityEngine;
using ProjectEva.Data;

namespace ProjectEva.Player
{
    public class Inventory : MonoBehaviour
    {
        public static Inventory Instance { get; private set; }
        private Dictionary<MaterialSO, int> materials = new Dictionary<MaterialSO, int>();

        private void Awake() { Instance = this; }

        public void AddMaterial(MaterialSO mat, int amount)
        {
            if (!mat) return;
            if (materials.ContainsKey(mat)) materials[mat] += amount;
            else materials[mat] = amount;
        }

        public bool HasMaterials(MaterialSO mat, int amount) =>
            mat && materials.ContainsKey(mat) && materials[mat] >= amount;

        public bool RemoveMaterials(MaterialSO mat, int amount)
        {
            if (!HasMaterials(mat, amount)) return false;
            materials[mat] -= amount;
            return true;
        }

        public int GetMaterialCount(MaterialSO mat) =>
            mat && materials.ContainsKey(mat) ? materials[mat] : 0;

        public IEnumerable<KeyValuePair<MaterialSO, int>> GetAllMaterials() => materials;
    }
}