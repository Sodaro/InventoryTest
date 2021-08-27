using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(fileName = "New Item Data", menuName = "ScriptableObjects/New Item Data", order = 1)]
public class ItemData : ScriptableObject
{
	[SerializeField] private Sprite _itemIcon;
	[SerializeField] private string _itemName;
	[SerializeField] private bool _isStackable;
	[SerializeField] private int _maxStackSize;

	public Sprite ItemIcon { get => _itemIcon;}
	public string ItemName { get => _itemName;}
	public bool IsStackable { get => _isStackable;}
	public int MaxStackSize { get => _maxStackSize; }
}
