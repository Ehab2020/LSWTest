using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace LSW.Player
{
    public class PlayerController : MonoBehaviour
    {
        [SerializeField]
        private float movementSpeed = 1f;
        [SerializeField] 
        private string[] movementStates;
        [SerializeField] 
        private string[] directions;

        private Animator _anim;
        private AnimationClip _animationClip;
        private AnimatorOverrideController _animOverrideController;
        private AnimationClipOverrides _defaultAnimationClips;

        public List<string> ownedItems = new List<string>();

        public string equipedShirt = "Shirt_0";
        public string equipedPants = "Pants_0";
        public string equipedShoes = "Shoes_0";

        public int gold = 1000;
        public GameObject inventory;
        public GameObject inventoryItemPrefab;

        public IInputHandler _inputHandler;

        // Start is called before the first frame update
        void Start()
        {
            if (_inputHandler == null)
                _inputHandler = new InputHandler();

            _anim = GetComponent<Animator>();
            _animOverrideController = new AnimatorOverrideController(_anim.runtimeAnimatorController);
            _anim.runtimeAnimatorController = _animOverrideController;

            _defaultAnimationClips = new AnimationClipOverrides(_animOverrideController.overridesCount);
            _animOverrideController.GetOverrides(_defaultAnimationClips);

            ownedItems.Add("Shirt_0");
            ownedItems.Add("Pants_0");
            ownedItems.Add("Shoes_0");

            UI.ShopHandler.onTrade += UpdateOwnedItems;
        }

        // Update is called once per frame
        void Update()
        {
            HandleMovement();

            /*if (Input.GetKeyDown(KeyCode.K))
            {
                ownedItems.Remove("Shirt_1");
            }*/
        }

        void HandleMovement()
        {
            float vert = _inputHandler.GetVerticalAxis();
            float horiz = _inputHandler.GetHorizontalAxis();

            transform.position += new Vector3(horiz, vert, 0) 
                * _inputHandler.GetDeltaTime() * movementSpeed;

            _anim.SetFloat("Vert", vert);
            _anim.SetFloat("Horiz", horiz);

            if(vert == 1 || vert == -1 || horiz == 1 || horiz == -1)
            {
                _anim.SetFloat("LastVert", vert);
                _anim.SetFloat("LastHoriz", horiz);
            }
        }

        public void UpdateOwnedItems(string itemName, int amount, string operation)
        {
            if (operation == "Buy")
            {
                gold -= amount;
                if (!ownedItems.Contains(itemName))
                    ownedItems.Add(itemName);

                UpdateOutfit(itemName);
            }
            else
            {
                gold += amount;
                if (ownedItems.Contains(itemName))
                    ownedItems.Remove(itemName);
            } 
        }

        public void UpdateOutfit(string itemName)
        {
            int itemIndex = itemName.IndexOf("_");
            string itemType = itemName.Substring(0, itemIndex);

            switch (itemType)
            {
                case "Shirt":
                    equipedShirt = itemName;
                    break;
                case "Pants":
                    equipedPants = itemName;
                    break;
                default:
                    equipedShoes = itemName;
                    break;
            }

            for (int stateIndex = 0; stateIndex < movementStates.Length; stateIndex++)
            {
                string state = movementStates[stateIndex];
                for (int directionIndex = 0; directionIndex < directions.Length; directionIndex++)
                {
                    string direction = directions[directionIndex];

                    _animationClip = Resources.Load<AnimationClip>("Animations/" + itemType + "/" + itemName + "/" + itemName + "_" + state + "_" + direction);

                    _defaultAnimationClips[itemType + "_0_" + state + "_" + direction] = _animationClip;
                }
            }

            _animOverrideController.ApplyOverrides(_defaultAnimationClips);
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
            
            for(int i = 0; i < ownedItems.Count; i++)
            {
                string itemName = ownedItems[i];
                Sprite spr = Resources.Load<Sprite>("ItemsImages/" + itemName);
                inventory.transform.GetChild(i).GetChild(0).GetComponent<Image>().enabled = true;
                inventory.transform.GetChild(i).GetChild(0).GetComponent<Image>().sprite = spr;

                inventory.transform.GetChild(i).GetComponent<Button>().onClick.AddListener(delegate { UpdateOutfit(itemName); });
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
    public class AnimationClipOverrides : List<KeyValuePair<AnimationClip, AnimationClip>>
    {
        public AnimationClipOverrides(int capacity) : base(capacity) { }

        public AnimationClip this[string name]
        {
            get { return this.Find(x => x.Key.name.Equals(name)).Value; }
            set
            {
                int index = this.FindIndex(x => x.Key.name.Equals(name));
                if (index != -1)
                    this[index] = new KeyValuePair<AnimationClip, AnimationClip>(this[index].Key, value);
            }
        }
    }
}

