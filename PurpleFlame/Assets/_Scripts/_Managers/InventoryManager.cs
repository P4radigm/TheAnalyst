using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace TouchBehaviours
{
    [System.Serializable]
    public class InventoryItem
    {
        public GameObject ItemCanvasPlaceHolder;
        public Sprite ItemSprite;
        public GameObject Collectable;
        public ItemType ItemType;
        public GameObject ReceivableItemVersion;
        public bool Collected = false;
    }

    public class InventoryManager : MonoBehaviour
    {
        [SerializeField] private GameObject InventoryCanvas;
        [SerializeField] private List<InventoryItem> Items;
        public List<ICollectable> Collectables = new List<ICollectable>();
        public Dictionary<Image, ICollectable> InventoryItems = new Dictionary<Image, ICollectable>();

        public void CollectGameObject(ICollectable item)
        {
            if (item.GetItemType() == ItemType.Act1_DoorKey)
                EventManager<bool>.BroadCast(EVENT.Act1_Brievenbus_DONE, true);
            if(!Collectables.Contains(item))
                Collectables.Add(item);

            Items.Find(i => i.Collectable.GetComponent<ICollectable>().Equals(item)).Collected = true;
            
        }

        public void OpenInventory(ItemType item)
        {
            //InventoryCanvas
        }

        public void GetItem(ItemType item)
        {
            /*
            switch(item)
            {
                case ItemType.Act1_DoorKey:
                    Items.Find(i => i.ItemType.Equals(item)).ReceivableItemVersion

            }*/
        }

        public void OpenCloseInventory()
        {
            if (InventoryCanvas.activeSelf)
                InventoryCanvas.SetActive(false);
            else
                InventoryCanvas.SetActive(true);

            foreach(InventoryItem ii in Items)
            {
                if (ii.Collected)
                    ii.ItemCanvasPlaceHolder.SetActive(true);
                else
                    ii.ItemCanvasPlaceHolder.SetActive(false);
            }
        }
    }
}