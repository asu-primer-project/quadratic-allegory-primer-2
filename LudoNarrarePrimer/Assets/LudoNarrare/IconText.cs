using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


public class IconText
{
	/* Variables */
	public string text;
	public Expression red;
	public Expression green;
	public Expression blue;

	/* Functions */
	public IconText(string _text)
	{
		text = _text;
		red = new Expression(0);
		red.number = 255;
		green = new Expression(0);
		green.number = 255;
		blue = new Expression(0);
		blue.number = 255;
	}
	
	public bool getTextColor(StoryWorld sw)
	{
		float avg = (float)(red.evaluate(sw) + green.evaluate(sw) + blue.evaluate(sw))/3f;

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
			it.red = new Expression(red.type);
			red.copyTo(it.red);
			it.green = new Expression(green.type);
			green.copyTo(it.green);
			it.blue = new Expression(blue.type);
			blue.copyTo(it.blue);
		}
	}
}
