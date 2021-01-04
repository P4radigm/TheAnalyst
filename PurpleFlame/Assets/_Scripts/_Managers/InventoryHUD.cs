using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace BasInventory
{
    public class InventoryHUD : MonoBehaviour
    {
        public Inventory Inventory;
        public Transform InventoryHUDPanel;

        private Dictionary<Transform, IInventoryItem> collectedHUDItems = new Dictionary<Transform, IInventoryItem>();

        private void Start()
        {
            Inventory.ItemAdded += HUDItemAdded;
            Inventory.ItemRemoved += HUDItemRemoved;
        }

        public void OpenPanel()
        {
            if (InventoryHUDPanel.gameObject.activeSelf)
                InventoryHUDPanel.gameObject.SetActive(false);
            else
                InventoryHUDPanel.gameObject.SetActive(true);
        }

        public void HUDItemAdded(object sender, InventoryEventArgs e)
        {
            foreach(Transform slot in InventoryHUDPanel)
            {
                slot.gameObject.SetActive(true);
                Image image = slot.GetComponent<Image>();

                //We found the empty slot
                if(!image.enabled)
                {
                    image.enabled = true;
                    image.sprite = e.Item.Icon;
                    collectedHUDItems.Add(slot, e.Item);

                    break;
                }
            }
        }

        public void HUDItemRemoved(object sender, InventoryEventArgs e)
        {
            foreach(KeyValuePair<Transform, IInventoryItem> keyValuePair in collectedHUDItems)
            {
                if(keyValuePair.Value.Equals(e.Item))
                {
                    keyValuePair.Key.gameObject.SetActive(false);
                    collectedHUDItems.Remove(keyValuePair.Key);
                    break;
                }
            }
        }
    }
}
