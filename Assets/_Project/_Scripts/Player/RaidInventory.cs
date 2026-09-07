using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using PlayerInput;

namespace Items
{
    /// <summary>
    /// Tracks everything the player is carryining during a raid
    /// </summary>
    public class RaidInventory : MonoBehaviour
    {
        [Header("Capacity")]
        [Tooltip("Max Carry Weight in KG"), SerializeField] private float maxWeight = 30f;

        private readonly List<ItemData> carriedItems= new();

        public float MaxWeight => maxWeight;
        public float CurrentWeight => carriedItems.Sum(i => i.weight);
        public float RemainingWeight => Mathf.Max(0f, maxWeight - CurrentWeight);
        public IReadOnlyList<ItemData> CarriedItems => carriedItems;

        public GameObject InventoryUI;
        private bool _isInventoryOpen = false;
        private bool hasStarted = false;
        // Fired when an inventory contetns changes, passes (currentWeight, maxWeight)
        public UnityEvent<float, float> OnInventoryChanged;

        private void Start()
        {
            InputManager.Instance.OnInventoryPressed += ToggleInventory;
            _isInventoryOpen = InventoryUI.activeSelf;
            hasStarted = true;
        }

        private void OnEnable()
        {
            if (hasStarted)
            {
                InputManager.Instance.OnInventoryPressed += ToggleInventory;
            }
        }

        private void OnDisable()
        {
            InputManager.Instance.OnInventoryPressed -= ToggleInventory;
        }

        /// <summary>
        /// Returns true if the item would fit within the capacity
        /// </summary>
        public bool CanFit(ItemData item)
        {
            return true; // temp for testing
            
            if(item == null) return false;
            return CurrentWeight + item.weight <= maxWeight;
        }

        /// <summary>
        /// Attempts to add an item to the inventory
        /// </summary>
        public bool TryAddItem(ItemData item)
        {
            if(item == null) return false;

            if(!CanFit(item))
                return false;

                carriedItems.Add(item);
                OnInventoryChanged?.Invoke(CurrentWeight, maxWeight);
                return true;
        }

        public bool RemoveItemAt(int index, bool spawnWorldPickup = true)
        {
            if(index < 0 || index >= carriedItems.Count) return false;

            ItemData item = carriedItems[index];
            carriedItems.RemoveAt(index);
            OnInventoryChanged?.Invoke(CurrentWeight, maxWeight);

            // Spawn World Pickup Here

            return true;
        }
    
        private void ToggleInventory()
        {
            _isInventoryOpen = !_isInventoryOpen;
            Debug.Log($"Inventory Open: {_isInventoryOpen}");
            InventoryUI.SetActive(_isInventoryOpen);
            InputManager.Instance.ToggleCursor(_isInventoryOpen);
        }
    }
}