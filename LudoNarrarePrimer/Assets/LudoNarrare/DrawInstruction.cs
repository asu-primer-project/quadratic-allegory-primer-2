using UnityEngine;
using System.Collections;

public class DrawInstruction
{
	/* Variables */
	public bool isText;
	public string entity;
	public string image;
	public string text;
	public Expression x;
	public Expression y;

	/* Functions */
	public DrawInstruction(bool _isText, string _entity, string _image, string _text)
	{
		isText = _isText;
		entity = _entity;
		image = _image;
		text = _text;
		x = new Expression(0);
		x.number = 0;
		y = new Expression(0);
		y.number = 0;
	}

	public void replaceWith(string replace, string with)
	{
		if (entity == replace)
			entity = with;
		if (text.Contains(replace))
			text.Replace(replace, with);
		if (x != null)
			x.replaceWith(replace, with);
		if (y != null)
			y.replaceWith(replace, with);
	}

	public void copyTo(DrawInstruction di)
	{
		if (di != null)
		{
			di.isText = isText;
			di.entity = entity;
			di.image = image;
			di.text = text;
			di.x = new Expression(x.type);
			x.copyTo(di.x);
			di.y = new Expression(y.type);
			y.copyTo(di.y);
		}
	}
}
