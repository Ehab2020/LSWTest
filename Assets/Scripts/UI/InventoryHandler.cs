using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using LSW.Player;

namespace LSW.UI
{
    public class InventoryHandler : MonoBehaviour
    {
        [SerializeField]
        private GameObject inventory;
        [SerializeField]
        private GameObject player;
        
        public delegate void OnEquipItem(string itemName);
        public static OnEquipItem onEquipItem;

        // Start is called before the first frame update
        void Start()
        {
            player = GameObject.FindGameObjectWithTag("Player");
        }

        public void ControlInventory()
        {
            if (inventory.activeSelf)
            {
                CloseInventory();
            }
            else
            {
                ShowInventory();
            }
        }

        public void ShowInventory()
        {
            inventory.SetActive(true);
            ClearInventory();

            var ownedItems = player.GetComponent<PlayerController>().ownedItems;
            for (int i = 0; i < ownedItems.Count; i++)
            {
                string itemName = ownedItems[i];
                Sprite spr = Resources.Load<Sprite>("ItemsImages/" + itemName);
                inventory.transform.GetChild(i).GetChild(0).GetComponent<Image>().enabled = true;
                inventory.transform.GetChild(i).GetChild(0).GetComponent<Image>().sprite = spr;

                inventory.transform.GetChild(i).GetComponent<Button>().onClick.AddListener(delegate { onEquipItem(itemName); });
            }
        }

        public void ClearInventory()
        {
            foreach (Transform item in inventory.transform)
            {
                item.GetChild(0).GetComponent<Image>().enabled = false;
                item.GetComponent<Button>().onClick.RemoveAllListeners();
            }
        }

        public void CloseInventory()
        {
            inventory.SetActive(false);
        }
    }
}
