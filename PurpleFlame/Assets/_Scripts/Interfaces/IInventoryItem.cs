using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BasInventory
{
    public interface IInventoryItem
    {
        string Name { get; }

        Sprite Icon { get; }

        void OnPickup();
    }

    public class InventoryEventArgs : EventArgs
    {
        public IInventoryItem Item;

        public InventoryEventArgs(IInventoryItem item)
        {
            Item = item;
        }
    }
}
