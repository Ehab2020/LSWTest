using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using LSW.Player;

namespace LSW.UI
{
    public class ShopHandler : MonoBehaviour
    {
        [SerializeField]
        private ShopItem[] shopItems;
        [SerializeField]
        private GameObject shopWindow;
        [SerializeField]
        private GameObject contentsHolder;
        [SerializeField]
        private Text playerGold;
        [SerializeField]
        private GameObject shopInventory;
        [SerializeField]
        private GameObject equippedItemsPrompt;

        public GameObject itemPrefab;
        public GameObject player;

        public delegate void OnTrade(string itemName, int amount, string operation);
        public static OnTrade onTrade;

        public delegate void OnClose();
        public static OnClose onClose;

        // Start is called before the first frame update
        void Start()
        {
            player = GameObject.FindGameObjectWithTag("Player");
            ShopKeeper.onOpenShop += DisplayShop;
        }

        // Dispaly Shop UI
        public void DisplayShop()
        {
            shopWindow.SetActive(true);
            RefreshShopInventory();

            playerGold.text = player.GetComponent<PlayerController>().gold.ToString();
            foreach (var shopItem in shopItems)
            {
                GameObject itemBtn = Instantiate(itemPrefab);
                itemBtn.transform.SetParent(contentsHolder.transform, false);
                itemBtn.name = shopItem.name;

                itemBtn.transform.GetChild(0).GetComponent<Text>().text = shopItem.itemName;
                Sprite itemSprite = Resources.Load<Sprite>("ItemsImages/" + shopItem.itemName);
                itemBtn.transform.GetChild(1).GetComponent<Image>().sprite = itemSprite;
                itemBtn.transform.GetChild(4).GetComponent<Text>().text = shopItem.buyPrice.ToString();
                itemBtn.transform.GetChild(5).GetComponent<Text>().text = shopItem.sellPrice.ToString();

                itemBtn.GetComponent<Button>().onClick.AddListener(delegate { Trade(shopItem.itemName, shopItem.buyPrice, "Buy"); });
            }
        }

        // Close Shop UI
        public void CloseShop()
        {
            shopWindow.SetActive(false);
            onClose();
        }

        // Refresh Shop UI when Player's ownedItems change
        public void RefreshShopInventory()
        {
            ClearInventory();
            var playerItems = player.GetComponent<PlayerController>().ownedItems;
            for (int i = 0; i < playerItems.Count; i++)
            {
                Sprite spr = Resources.Load<Sprite>("ItemsImages/" + playerItems[i]);
                shopInventory.transform.GetChild(i).GetChild(0).GetComponent<Image>().enabled = true;
                shopInventory.transform.GetChild(i).GetChild(0).GetComponent<Image>().sprite = spr;

                int index = shopItems.ToList().FindIndex(x => x.itemName == playerItems[i]);

                shopInventory.transform.GetChild(i).GetComponent<Button>().onClick.AddListener(delegate { Trade(shopItems[index].itemName, shopItems[index].sellPrice, "Sell"); });
            }
        }

        // Handles Buying/Selling Operations
        public void Trade(string itemName, int itemPrice, string operation)
        {
            if (itemName == player.GetComponent<PlayerController>().equipedShirt ||
                itemName == player.GetComponent<PlayerController>().equipedPants ||
                itemName == player.GetComponent<PlayerController>().equipedShoes)
            {
                ShowEquippedItemsPrompt();
                return;
            }

            onTrade(itemName, itemPrice, operation);
            playerGold.text = player.GetComponent<PlayerController>().gold.ToString();
            RefreshShop();
            RefreshShopInventory();
        }

        // Refreshes Shop UI's sold items (Enable/Disable buying items)
        public void RefreshShop()
        {
            int goldAmount = int.Parse(playerGold.text);
            foreach (var shopItem in shopItems)
            {
                bool playerOwnsItem = player.GetComponent<PlayerController>().ownedItems.Contains(shopItem.itemName);
                contentsHolder.transform.Find(shopItem.itemName).GetComponent<Button>().interactable = 
                    goldAmount > shopItem.buyPrice && !playerOwnsItem;
            }
        }

        // Remove images and button events from inventory elements
        public void ClearInventory()
        {
            foreach(Transform item in shopInventory.transform)
            {
                item.GetChild(0).GetComponent<Image>().enabled = false;
                item.GetComponent<Button>().onClick.RemoveAllListeners();
            }
        }

        // Display cannot sell equipped items panel
        public void ShowEquippedItemsPrompt()
        {
            equippedItemsPrompt.SetActive(true);
        }

        // Hides cannot sell equipped items panel
        public void HideEquippedItemsPrompt()
        {
            equippedItemsPrompt.SetActive(false);
        }
    }
}
