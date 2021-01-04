using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace BasInventory
{
    public class Inventory : MonoBehaviour
    {
        private const int SLOTS = 9;
        private List<IInventoryItem> inventoryItems = new List<IInventoryItem>();

        public event EventHandler<InventoryEventArgs> ItemAdded;

        public event EventHandler<InventoryEventArgs> ItemRemoved;

        public void AddItem(IInventoryItem item)
        {
            if(inventoryItems.Count < SLOTS)
            {
                Collider collider = (item as MonoBehaviour).GetComponent<Collider>();
                if(collider.enabled)
                {
                    collider.enabled = false;

                    inventoryItems.Add(item);
                    item.OnPickup();

                    if(ItemAdded !=null)
                    {
                        ItemAdded(this, new InventoryEventArgs(item));
                    }
                }
            }
        }

        public void RemoveItem(IInventoryItem item)
        {
            if (inventoryItems.Count < 1) return;

            inventoryItems.Remove(item);

            ItemRemoved(this, new InventoryEventArgs(item));
        }
    }
}
