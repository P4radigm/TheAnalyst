using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TouchBehaviours
{
    public enum ItemType
    {
        Act1_DoorKey,
        Act1_Note1,
    }

    public interface ICollectable
    {
        void Collect();
        ItemType GetItemType();
    }
}
