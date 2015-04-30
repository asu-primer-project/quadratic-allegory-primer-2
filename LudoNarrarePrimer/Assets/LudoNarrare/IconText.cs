using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


public class IconText
{
	/* Variables */
	public string text;
	public int red;
	public int green;
	public int blue;

	/* Functions */
	public IconText(string _text)
	{
		text = _text;
		red = 255;
		green = 255;
		blue = 255;
	}

	//false = white, true = black
	public bool getTextColor()
	{
		float avg = (float)(red + green + blue)/3f;

		if (avg <= 255f/2f)
			return false;
		else
			return true;
	}

	public void copyTo(IconText it)
	{
		if (it != null)
		{
			it.text = text;
			it.red = red;
			it.green = green;
			it.blue = blue;
		}
	}
}
