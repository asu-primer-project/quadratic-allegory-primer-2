using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class Discriminator
{
	/* Variables */
	public string name;
	public List<Condition> conditions;
	public bool neverShow;
	
	/* Functions */
	public Discriminator(string _name)
	{
		name = _name;
		conditions = new List<Condition>();
		neverShow = false;
	}

	public void replaceWith(string replace, string with)
	{
		foreach(Condition c in conditions)
			c.replaceWith(replace, with);
	}
	
	public void copyTo(Discriminator d)
	{
		if (d != null)
		{
			d.name = name;
			d.conditions.Clear();
			for (int i = 0; i < conditions.Count; i++)
			{
				Condition tempC = new Condition();
				conditions[i].copyTo(tempC);
				d.conditions.Add(tempC);
			}
			d.neverShow = neverShow;
		}
	}
}
