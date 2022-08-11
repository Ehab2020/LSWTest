using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LSW
{
    [CreateAssetMenu(fileName = "New Shop Item", menuName = "Shop Item")]
    public class ShopItem : ScriptableObject
    {
        public string itemName;
        public int buyPrice;
        public int sellPrice;
    }
}
