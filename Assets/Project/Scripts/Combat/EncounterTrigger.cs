using UnityEngine;
using ProjectEva.Managers;
using ProjectEva.Data;

namespace ProjectEva.Combat
{
    public class EncounterTrigger : MonoBehaviour
    {
        [SerializeField] private EncounterData encounter;

        private void OnTriggerEnter(Collider other)
        {
            if (!other.CompareTag("Player")) return;
            if (!GameManager.Instance || GameManager.Instance.CurrentState != GameState.Exploration) return;
            GameManager.Instance.EnterCombat(encounter);
            gameObject.SetActive(false);
        }
    }
}