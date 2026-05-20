namespace ProjectEva.Interaction
{
    public interface IInteractable { string GetInteractionPrompt(); void Interact(Player.PlayerController player); }
}