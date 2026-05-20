using UnityEngine;
using ProjectEva.Combat;
using ProjectEva.Data;

namespace ProjectEva.Managers
{
    public enum GameState { Exploration, Combat }

    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance { get; private set; }
        public GameState CurrentState { get; private set; } = GameState.Exploration;
        public MaterialSO GoldCurrency;

        [SerializeField] private CombatManager combatManager;

        private void Awake()
        {
            if (Instance != null && Instance != this) { Destroy(gameObject); return; }
            Instance = this;
            DontDestroyOnLoad(gameObject);
            if (combatManager == null) combatManager = GetComponent<CombatManager>();
        }

        public void EnterCombat(EncounterData encounter)
        {
            if (CurrentState == GameState.Combat) return;
            if (combatManager == null || combatManager.gameObject == null) return;
            CurrentState = GameState.Combat;
            combatManager.StartCombat(encounter);
        }

        public void ExitCombat() => CurrentState = GameState.Exploration;
    }
}