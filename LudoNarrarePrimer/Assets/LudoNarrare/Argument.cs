using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class Argument
{
	/* Variables */
	public string name;
	public string text;
	public string choice;
	public List<Entity> values;
	public List<Condition> conditions;
	
	/* Functions */
	public Argument(string _name, string _text)
	{
		name = _name;
		text = _text;
		choice = "";
		values = new List<Entity>();
		conditions = new List<Condition>();
	}

	public void replaceWith(string replace, string with)
	{
		foreach (Condition c in conditions)
			c.replaceWith(replace, with);
		if (choice == replace)
			choice = with;
	}
	
	public void copyTo(Argument a)
	{
		if (a != null)
		{
			a.name = name;
			a.text = text;
			a.choice = choice;
			a.values.Clear();
			for (int i = 0; i < values.Count; i++)
				a.values.Add(values[i]);
			a.conditions.Clear();
			for (int i = 0; i < conditions.Count; i++)
			{
				Condition tempC = new Condition();
				conditions[i].copyTo(tempC);
				a.conditions.Add(tempC);
			}
		}
	}
}
