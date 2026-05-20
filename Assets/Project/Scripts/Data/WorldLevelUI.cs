using UnityEngine;
using UnityEngine.UI;
using ProjectEva.Managers;

namespace ProjectEva.UI
{
    public class WorldLevelUI : MonoBehaviour
    {
        [SerializeField] private Text worldLevelText;
        private void Update() { if (WorldManager.Instance) worldLevelText.text = $"World Level: {WorldManager.Instance.CurrentWorldLevel}"; }
    }
}