using UnityEngine;
using System.Collections;

public class LNString
{
	/* Variables */
	public string name;
	public string text;
	
	/* Functions */
	public LNString(string _name, string _text)
	{
		name = _name;
		text = _text;
	}
	
	public void copyTo(LNString l)
	{
		if (l != null)
		{
			l.name = name;
			l.text = text;
		}
	}
}
