using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using ProjectEva.Data;

namespace ProjectEva.Combat
{
    public class CombatSummaryUI : MonoBehaviour
    {
        [SerializeField] private CanvasGroup canvasGroup;
        [SerializeField] private Text titleText, damageDealtText, damageTakenText, healingText, dropsText;
        private bool hasConfirmed;
        public bool HasConfirmed => hasConfirmed;

        private void Awake() => gameObject.SetActive(false);

        public void Show(bool victory, float dealt, float taken, float healing, List<MaterialDrop> drops)
        {
            hasConfirmed = false;
            gameObject.SetActive(true);
            if (canvasGroup) canvasGroup.alpha = 0;

            titleText.text = victory ? "Victory!" : "Defeat...";
            damageDealtText.text = $"Damage Dealt: {dealt:F0}";
            damageTakenText.text = $"Damage Taken: {taken:F0}";
            healingText.text = $"Healing Done: {healing:F0}";

            string dropStr = "Drops:\n";
            if (drops != null && drops.Count > 0)
                foreach (var d in drops) dropStr += $"{d.material.itemName} x{d.minAmount}\n";
            else dropStr += "None";

            if (dropsText != null) dropsText.text = dropStr;
            else
            {
                var fallback = GetComponentInChildren<Text>();
                if (fallback) fallback.text = dropStr;
            }

            StartCoroutine(AutoClose());
        }

        private IEnumerator AutoClose()
        {
            float dur = 0.4f, elapsed = 0f;
            while (elapsed < dur)
            {
                if (canvasGroup) canvasGroup.alpha = Mathf.Lerp(0, 1, elapsed / dur);
                elapsed += Time.deltaTime;
                yield return null;
            }
            if (canvasGroup) canvasGroup.alpha = 1;
            yield return new WaitForSeconds(5f);
            hasConfirmed = true;
        }
    }
}