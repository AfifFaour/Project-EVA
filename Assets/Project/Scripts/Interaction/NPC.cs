using UnityEngine;
using ProjectEva.Player;

namespace ProjectEva.Interaction
{
    public class NPC : MonoBehaviour, IInteractable
    {
        public void Interact(PlayerController player) { }
        public string GetInteractionPrompt() => "Talk";
    }
}