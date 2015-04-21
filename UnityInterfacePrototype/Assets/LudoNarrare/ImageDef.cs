using UnityEngine;
using System.Collections;

public class ImageDef 
{
	/* Variables */
	public string name;
	public string image;
	
	/* Functions */
	public ImageDef(string _name, string _image)
	{
		name = _name;
		image = _image;
	}
	
	public void copyTo(ImageDef id)
	{
		if (id != null)
		{
			id.name = name;
			id.image = image;
		}
	}
}
