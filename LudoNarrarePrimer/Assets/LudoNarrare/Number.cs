using UnityEngine;
using System.Collections;

public class Number
{
	/* Variables */
	public string name;
	public int value;
	
	/* Functions */
	public Number(string _name, int _value)
	{
		name = _name;
		value = _value;
	}
	
	public void copyTo(Number n)
	{
		if (n != null)
		{
			n.name = name;
			n.value = value;
		}
	}
}
