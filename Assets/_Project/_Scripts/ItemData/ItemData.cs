using UnityEngine;

namespace Items
{
    [CreateAssetMenu(fileName = "NewItemData", menuName = "ZHG/Item Data")]
    public class ItemData : ScriptableObject
    {
        public enum ItemCatergory
        {
            Food,
            Meds,
            Materials,
            Valuables,
            Misc
        }

        [Header("Identity")]
        public string itemName = "New Item";
        public ItemCatergory catergory = ItemCatergory.Misc;
        [TextArea] public string description;

        [Header("Physical")]
        [Tooltip("Weight in kg, usaed against the player's carry capacity")]
        public float weight = 1f;

        [Header("Visuals")]
        public Sprite icon;
    }
}
