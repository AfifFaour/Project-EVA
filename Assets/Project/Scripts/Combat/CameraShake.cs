using System.Collections;
using UnityEngine;

namespace ProjectEva.Combat
{
    public class CameraShake : MonoBehaviour
    {
        private Vector3 origPos;
        private void Awake() => origPos = transform.localPosition;
        public void Shake(float duration, float magnitude) { StopAllCoroutines(); StartCoroutine(DoShake(duration, magnitude)); }
        private IEnumerator DoShake(float duration, float magnitude)
        {
            float elapsed = 0f;
            while (elapsed < duration) { transform.localPosition = origPos + Random.insideUnitSphere * magnitude; elapsed += Time.deltaTime; yield return null; }
            transform.localPosition = origPos;
        }
    }
}