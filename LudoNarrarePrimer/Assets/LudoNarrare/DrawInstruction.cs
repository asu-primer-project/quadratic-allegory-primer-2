using UnityEngine;
using System.Collections;

public class DrawInstruction
{
	/* Variables */
	public bool isText;
	public string entity;
	public string image;
	public string text;
	public int x;
	public int y;

	/* Functions */
	public DrawInstruction(bool _isText, string _entity, string _image, string _text, int _x, int _y)
	{
		isText = _isText;
		entity = _entity;
		image = _image;
		text = _text;
		x = _x;
		y = _y;
	}

	public void replaceWith(string replace, string with)
	{
		if (entity == replace)
			entity = with;
		if (text.Contains(replace))
			text.Replace(replace, with);
	}

	public void copyTo(DrawInstruction di)
	{
		if (di != null)
		{
			di.isText = isText;
			di.entity = entity;
			di.image = image;
			di.text = text;
			di.x = x;
			di.y = y;
		}
	}
}
