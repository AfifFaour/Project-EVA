using UnityEngine;
using ProjectEva.Managers;
using ProjectEva.UI;
using ProjectEva.Interaction;

namespace ProjectEva.Player
{
    [RequireComponent(typeof(Rigidbody))]
    public class PlayerController : MonoBehaviour
    {
        [SerializeField] private float moveSpeed = 5f;
        [SerializeField] private float mouseSensitivity = 2f;
        [SerializeField] private float verticalClamp = 90f;
        [SerializeField] private Animator anim;
        private Rigidbody rb;
        private float verticalRotation;
        private IInteractable currentInteractable;

        private void Awake()
        {
            rb = GetComponent<Rigidbody>();
            if (!anim) anim = GetComponentInChildren<Animator>();
        }

        private void Start()
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }

        private void Update()
        {
            if (!GameManager.Instance || GameManager.Instance.CurrentState != GameState.Exploration) return;
            if (UIManager.Instance != null && UIManager.Instance.IsAnyPanelOpen) return;

            float mx = Input.GetAxis("Mouse X") * mouseSensitivity;
            float my = Input.GetAxis("Mouse Y") * mouseSensitivity;
            transform.Rotate(0f, mx, 0f);
            verticalRotation = Mathf.Clamp(verticalRotation - my, -verticalClamp, verticalClamp);
            Camera.main.transform.localRotation = Quaternion.Euler(verticalRotation, 0f, 0f);

            if (Input.GetKeyDown(KeyCode.E) && currentInteractable != null)
                currentInteractable.Interact(this);
        }

        private void FixedUpdate()
        {
            if (!GameManager.Instance || GameManager.Instance.CurrentState != GameState.Exploration) return;

            float h = Input.GetAxisRaw("Horizontal");
            float v = Input.GetAxisRaw("Vertical");
            Vector3 dir = (transform.right * h + transform.forward * v).normalized;
            Vector3 vel = dir * moveSpeed;
            vel.y = rb.linearVelocity.y;
            rb.linearVelocity = vel;

            if (anim != null)
            {
                int speedHash = Animator.StringToHash("Speed");
                if (anim.HasParameter(speedHash))
                {
                    float speed = new Vector3(rb.linearVelocity.x, 0, rb.linearVelocity.z).magnitude;
                    anim.SetFloat(speedHash, speed);
                }
            }
        }

        private void OnTriggerEnter(Collider other) => currentInteractable = other.GetComponent<IInteractable>();
        private void OnTriggerExit(Collider other)
        {
            if (other.GetComponent<IInteractable>() == currentInteractable)
                currentInteractable = null;
        }
    }

    public static class AnimatorExtensions
    {
        public static bool HasParameter(this Animator animator, int nameHash)
        {
            foreach (AnimatorControllerParameter param in animator.parameters)
                if (param.nameHash == nameHash)
                    return true;
            return false;
        }
    }
}