using System;
using UnityEngine.Events;

[Serializable]
public class ItemDataEvent : UnityEvent<ItemData> { }

[Serializable]
public class InventoryItemEvent : UnityEvent<InventoryItem> { }