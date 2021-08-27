using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class InventoryItem : MonoBehaviour, IPointerClickHandler, IBeginDragHandler, IDragHandler, IEndDragHandler, IDropHandler
{

	//TODO: swap item data objects on drop


	private ItemData _itemData;
	private Image _image;


	[HideInInspector] public ItemDataEvent itemBeginDragEvent;
	[HideInInspector] public ItemDataEvent itemEndDragEvent;
	[HideInInspector] public UnityEvent itemSplitEvent;
	[HideInInspector] public UnityEvent itemClickEvent;

	//[HideInInspector] public InventoryItemEvent itemSwapEvent;

	[SerializeField] Text _stackSizeText;

	private int _stackSize = 0;

	public int StackSize 
	{
		get => _stackSize; 
		set 
		{ 
			_stackSize = value;
			if (_itemData?.IsStackable == false)
				_stackSizeText.text = "";
			else
				_stackSizeText.text = _stackSize.ToString();
		} 
	}

	public void SetItemData(ItemData itemData)
	{
		_itemData = itemData;
		if (_itemData == null)
			_stackSize = 0;

		UpdateItemDisplay();
	}

	public ItemData GetItemData() => _itemData;


	//Image _buttonImage;
	void Awake()
	{
		_image = GetComponent<Image>();
		//_buttonImage = GetComponent<Image>();
	}

	public void OnBeginDrag(PointerEventData eventData)
	{
		itemBeginDragEvent.Invoke(_itemData);
		//_buttonImage.color = Color.blue;
	}

	public void OnDrag(PointerEventData eventData)
	{
		//throw new System.NotImplementedException();
	}

	public void OnEndDrag(PointerEventData eventData)
	{
		itemEndDragEvent.Invoke(_itemData);
		//_buttonImage.sprite = eventData..GetComponent<Image>().sprite;
		//throw new System.NotImplementedException();
	}

	public void OnPointerClick(PointerEventData eventData)
	{
		if (Input.GetKey(KeyCode.LeftShift))
		{
			if (_stackSize > 1)
				itemSplitEvent.Invoke();
			else
				itemClickEvent.Invoke();
		}
		else
		{
			itemClickEvent.Invoke();
		}
		//throw new System.NotImplementedException();
	}

	void UpdateItemDisplay()
	{
		if (_itemData == null)
		{
			_image.sprite = null;
			_image.color = Color.black;
			_stackSizeText.text = "";
		}
		else
		{
			_image.sprite = _itemData.ItemIcon;
			_image.color = Color.white;
			if (_itemData.IsStackable)
				_stackSizeText.text = _stackSize.ToString();
			else
				_stackSizeText.text = "";
		}		
	}

	public void OnDrop(PointerEventData eventData)
	{
		//check if itemdata is the same, if so update stack
		InventoryItem draggedItem = eventData.pointerDrag.GetComponent<InventoryItem>();
		ItemData draggedItemData = draggedItem?.GetItemData();

		if (draggedItemData == null || draggedItem == this)
			return;

		if (_itemData == null)
		{
			_stackSize = draggedItem.StackSize;
			SetItemData(draggedItemData);
			draggedItem.SetItemData(null);
			return;
		}


		if (draggedItemData.ItemName == _itemData.ItemName && _itemData.IsStackable)
		{
			//add together stacks, cap at max size, the item that was dropped gets the overflow (remainder) value
			int newStackSize = draggedItem.StackSize + StackSize;
			

			if (newStackSize > _itemData.MaxStackSize)
			{

				int overflow = newStackSize % _itemData.MaxStackSize;
				newStackSize = _itemData.MaxStackSize;
				draggedItem.StackSize = overflow;
				StackSize = newStackSize;

			}
			else
			{
				StackSize = newStackSize;
				draggedItem.SetItemData(null);
			}
		}
		else
		{

			int tmpStackSize = draggedItem.StackSize;

			draggedItem.StackSize = StackSize;

			draggedItem.SetItemData(_itemData);
			SetItemData(draggedItemData);

			StackSize = tmpStackSize;

			//_itemData = draggedItemData;
			//int tempStackSize = draggedItem.StackSize;
			//draggedItem.StackSize = _stackSize;
			//draggedItem.SetItemData(_itemData);
			//_stackSize = tempStackSize;
			//SetItemData(draggedItemData);
		}
	}
}
