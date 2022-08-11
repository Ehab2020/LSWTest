using System.Collections.Generic;
using UnityEngine;

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
        [SerializeField]
        public GameObject inventory;

        private Animator anim;
        private AnimationClip animationClip;
        private AnimatorOverrideController animOverrideController;
        private AnimationClipOverrides defaultAnimationClips;

        public List<string> ownedItems = new List<string>();

        public string equipedShirt = "Shirt_0";
        public string equipedPants = "Pants_0";
        public string equipedShoes = "Shoes_0";

        public int gold = 1000;

        public IInputHandler inputHandler;

        public delegate void OnNPCInteract();
        public static OnNPCInteract onNPCInteract;

        // Start is called before the first frame update
        void Start()
        {
            if (inputHandler == null)
                inputHandler = new InputHandler();

            anim = GetComponent<Animator>();
            animOverrideController = new AnimatorOverrideController(anim.runtimeAnimatorController);
            anim.runtimeAnimatorController = animOverrideController;

            defaultAnimationClips = new AnimationClipOverrides(animOverrideController.overridesCount);
            animOverrideController.GetOverrides(defaultAnimationClips);

            ownedItems.Add("Shirt_0");
            ownedItems.Add("Pants_0");
            ownedItems.Add("Shoes_0");

            UI.ShopHandler.onTrade += UpdateOwnedItems;
            UI.InventoryHandler.onEquipItem += UpdateOutfit;
        }

        // Update is called once per frame
        void Update()
        {
            HandleMovement();

            if (Input.GetKeyDown(KeyCode.T))
            {
                onNPCInteract();
            }
        }

        void HandleMovement()
        {
            float vert = inputHandler.GetVerticalAxis();
            float horiz = inputHandler.GetHorizontalAxis();

            transform.position += new Vector3(horiz, vert, 0) 
                * inputHandler.GetDeltaTime() * movementSpeed;

            anim.SetFloat("Vert", vert);
            anim.SetFloat("Horiz", horiz);

            if(vert == 1 || vert == -1 || horiz == 1 || horiz == -1)
            {
                anim.SetFloat("LastVert", vert);
                anim.SetFloat("LastHoriz", horiz);
            }
        }

        // Update Player's owned items
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

        // Update player outfit and animations with the new item
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

                    animationClip = Resources.Load<AnimationClip>("Animations/" + itemType + "/" + itemName + "/" + itemName + "_" + state + "_" + direction);

                    defaultAnimationClips[itemType + "_0_" + state + "_" + direction] = animationClip;
                }
            }

            animOverrideController.ApplyOverrides(defaultAnimationClips);
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

