using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Page
{
	/* Variables */
	public string name;
	public List<DrawInstruction> drawList;

	/* Functions */
	public Page(string _name)
	{
		name = _name;
		drawList = new List<DrawInstruction>();
	}

	public void replaceWith(string replace, string with)
	{
		for (int i = 0; i < drawList.Count; i++ )
			drawList[i].replaceWith(replace, with);
	}

	public void copyTo(Page p)
	{
		if (p != null)
		{
			p.name = name;
			p.drawList.Clear();
			for (int i = 0; i < drawList.Count; i++)
			{
				DrawInstruction tempD = new DrawInstruction(false,"","","",0,0);
				drawList[i].copyTo(tempD);
				p.drawList.Add(tempD);
			}
		}
	}
}
