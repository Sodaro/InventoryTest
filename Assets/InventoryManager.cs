using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventoryManager : MonoBehaviour
{
	const int NUM_INVENTORY_SLOTS_WIDTH = 9;
	const int NUM_INVENTORY_SLOTS_HEIGHT = 6;
	const int OFFSET_X = 38;
	const int OFFSET_Y = -38;
	const int BUTTON_WIDTH = 72;
	const int BUTTON_HEIGHT = 72;
	const int INVENTORY_PADDING = 2;

	[SerializeField] GameObject _buttonPrefab;
    List<InventoryItem> _inventoryItems;

	[SerializeField] ItemData itemdata1;
	[SerializeField] ItemData itemdata2;
	[SerializeField] ItemData itemdata3;

	[SerializeField] Transform _itemContainer;

	[SerializeField] private GameObject _draggedItemDisplay;
	private Image _draggedItemImage;

	private InventoryItem _splitItem;
	private ItemData _draggedItemData;
	private int _draggedItemStackSize;

	private RectTransform _draggedRectTransform;

	private void Awake()
	{
		_inventoryItems = new List<InventoryItem>();
		for (int y = 0; y < NUM_INVENTORY_SLOTS_HEIGHT; y++)
		{
			for (int x = 0; x < NUM_INVENTORY_SLOTS_WIDTH; x++)
			{
				Vector3 pos = new Vector3(OFFSET_X + (INVENTORY_PADDING*x) + (BUTTON_WIDTH * x), OFFSET_Y - (INVENTORY_PADDING*y) - (y * BUTTON_HEIGHT), 0);
				GameObject itemObject = Instantiate(_buttonPrefab, _itemContainer);
				itemObject.transform.localPosition = pos;
				InventoryItem item = itemObject.GetComponentInChildren<InventoryItem>();
				item.itemBeginDragEvent.AddListener(OnItemBeginDrag);
				item.itemEndDragEvent.AddListener(OnItemEndDrag);
				item.itemSplitEvent.AddListener(delegate { OnItemSplit(ref item); });
				item.itemClickEvent.AddListener(delegate { OnItemClick(ref item); });
				//item.itemSwapEvent.AddListener(OnItemSwap);
				//button.OnDrop.
				_inventoryItems.Add(item);
			}
		}
		_inventoryItems[0].SetItemData(itemdata1);
		_inventoryItems[0].StackSize = 2;

		_inventoryItems[20].SetItemData(itemdata1);
		_inventoryItems[20].StackSize = 4;

		_inventoryItems[1].SetItemData(itemdata2);
		_inventoryItems[1].StackSize = 3;

		_inventoryItems[5].SetItemData(itemdata2);
		_inventoryItems[5].StackSize = 7;

		_inventoryItems[8].SetItemData(itemdata3);
		_inventoryItems[8].StackSize = 1;

		_inventoryItems[10].SetItemData(itemdata3);
		_inventoryItems[10].StackSize = 1;

		_inventoryItems[12].SetItemData(itemdata2);
		_inventoryItems[12].StackSize = 5;

		_draggedItemImage = _draggedItemDisplay.GetComponent<Image>();
		_draggedRectTransform = _draggedItemDisplay.GetComponent<RectTransform>();

		_splitItem = null;
		//_draggedItemImage.sprite = itemdata.ItemIcon;

	}

	private void OnItemBeginDrag(ItemData itemData)
	{
		if (itemData == null)
			return;

		if (_splitItem != null)
			_splitItem.StackSize += _draggedItemStackSize;
			

		_draggedItemImage.sprite = itemData.ItemIcon;
		_draggedItemImage.color = Color.white;
	}
	private void OnItemEndDrag(ItemData itemData)
	{
		ClearDraggedItem();
	}
	private void OnItemSplit(ref InventoryItem item)
	{
		if (_splitItem != null)
		{
			_splitItem.StackSize += _draggedItemStackSize;
			ClearDraggedItem();
			return;
		}
		_splitItem = item;
		_draggedItemData = _splitItem.GetItemData();
		_draggedItemImage.sprite = _draggedItemData.ItemIcon;
		_draggedItemImage.color = Color.white;

		
		_draggedItemStackSize = item.StackSize / 2;

		_splitItem.StackSize = Mathf.CeilToInt((float)item.StackSize / 2);
	}

	private void ClearDraggedItem()
	{
		_splitItem = null;
		_draggedItemImage.sprite = null;
		_draggedItemImage.color = Color.clear;
		_draggedItemStackSize = 0;
	}

	private void OnItemClick(ref InventoryItem item)
	{
		ItemData itemData = item.GetItemData();
		if (_splitItem != null)
		{
			if (itemData == null)
			{
				item.StackSize = _draggedItemStackSize;
				item.SetItemData(_draggedItemData);
				ClearDraggedItem();
			}
			else if (itemData.ItemName == _draggedItemData.ItemName)
			{
				if (itemData.IsStackable)
				{
					//if splitting item and clicking one that is stackable, add to stack and keep extra in hand

					int newStackSize = item.StackSize + _draggedItemStackSize;
					if (newStackSize > itemData.MaxStackSize)
					{

						int overflow = newStackSize % itemData.MaxStackSize;
						newStackSize = itemData.MaxStackSize;
						_draggedItemStackSize = overflow;
						item.StackSize = newStackSize;
					}
					else
					{
						item.StackSize = newStackSize;
						ClearDraggedItem();
					}
				}
				else
					ClearDraggedItem();
			}
			else
			{
				//not same name, restore stack
				_splitItem.StackSize += _draggedItemStackSize;
				ClearDraggedItem();
			}
		}
	}

	//private void OnItemSwap(ref InventoryItem item1, ref InventoryItem item2)
	//{
	//	InventoryItem tmpItem = item1;
	//	item1 = item2;
	//	item2 = tmpItem;
	//	//int tempStackSize = draggedItem.StackSize;
	//	//draggedItem.StackSize = _stackSize;
	//	//draggedItem.SetItemData(_itemData);
	//	//_stackSize = tempStackSize;
	//	//SetItemData(draggedItemData);
	//}

	private void Update()
	{
		_draggedRectTransform.anchoredPosition = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
		
	}
}
