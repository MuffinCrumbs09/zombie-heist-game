using UnityEngine;

namespace Items
{
    public class ItemDatabase : MonoBehaviour
    {
        public static ItemDatabase Instance { get; private set; }

        [Header("Item Database")]
        [SerializeField] private ItemData[] items;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
        }

        public static ItemData GetRandomItem()
        {
            if (Instance.items == null || Instance.items.Length == 0)
                return null;

            int randomIndex = Random.Range(0, Instance.items.Length);
            return Instance.items[randomIndex];
        }
    }
}