using System.Collections.Generic;

/// <summary>
/// Contains an with a slot for each item type, the value in the 
/// slot indicates the quantity held of that item
/// </summary>
public class Inventory {

	public static int NUM_ITEMS =
		System.Enum.GetNames(typeof(ItemType)).Length;

	//public int[] items = new int[NUM_ITEMS];
	public int[] items = new int[50];	//makes it easy to expand the enum w/o messing up save file
	private int numUniqueItems = 0;

	public bool HasItem(ItemType type)
	{
		return NumItem(type) > 0;
	}

	public int NumItem(ItemType type)
	{
		return items[(int)type];
	}

	public void AddItem(ItemType type)
	{
		ShiftItemCount(type, 1);
	}

	public void RemoveItem(ItemType type)
	{
		ShiftItemCount(type, -1);
	}

	public void ShiftItemCount(ItemType type, int num)
	{
		if (items[(int)type]==0 && num > 0)
			numUniqueItems++;
		else if (items[(int)type] > 0 && items[(int)type] + num <= 0)
			numUniqueItems--;

		items[(int)type] += num;
		if (items[(int)type] < 0)
			items[(int)type] = 0;
	}

	public void SetItemCount(ItemType type, int num)
	{
		if (items[(int)type] == 0 && num > 0)
			numUniqueItems++;
		else if (items[(int)type] > 0 && num == 0)
			numUniqueItems--;

		if (num >= 0)
			items[(int)type] = num;
	}

	public List<int> ToList()
	{
		return new List<int>(items);
	}
	
	public void Load(List<int> list)
	{
		items = list.ToArray();
		foreach (int i in items) {
			if (i > 0)
				numUniqueItems++;
		}
	}

	public int GetNumUniqueItems()
	{
		return numUniqueItems;
	}
}
