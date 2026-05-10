using System;
using UnityEngine;

namespace DialogSystem.Core
{
    public class GameInput : MonoBehaviour
    {
        public static GameInput Instance { get; private set; }
        private PlayerInputActions actions;
        
        public Action InteractPerformed;

        private void Awake() {
            if (Instance != null && Instance != this) {
                Destroy(gameObject);
                return;
            } else {
                Instance = this;
            }

            actions = new PlayerInputActions();
            actions.Player.Enable();

            actions.Player.Interaction.performed += ctx => InteractPerformed?.Invoke();
        }

        public Vector2 GetMovementVector() {
            return actions.Player.Movement.ReadValue<Vector2>();
        }
    }
}
