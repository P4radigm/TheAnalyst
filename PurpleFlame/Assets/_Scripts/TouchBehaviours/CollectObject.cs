using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using BasInventory;

namespace TouchBehaviours
{
    public class CollectObject : MonoBehaviour, ICollectable, IInventoryItem
    {
        [SerializeField] private UnityEvent collectedEvents;
        [SerializeField]private ItemType itemType;

        public string Name { get => "Key"; }

        public Sprite Icon { get; }

        public ItemType GetItemType()
        {
            return itemType;
        }

        public void Collect()
        {
            collectedEvents.Invoke();
            //GameManager.Instance.InventoryManager.AddItem(this);
        }

        private IEnumerator DeactivateAFterTime()
        {
            yield return new WaitForSeconds(2f);
            gameObject.SetActive(false);
        }

        public void OnPickup()
        {
            StartCoroutine(DeactivateAFterTime());

        }
    }
}
