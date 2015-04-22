using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class Argument
{
	/* Variables */
	public string name;
	public string text;
	public List<Condition> conditions;
	
	/* Functions */
	public Argument(string _name, string _text)
	{
		name = _name;
		text = _text;
		conditions = new List<Condition>();
	}
	
	public void copyTo(Argument a)
	{
		if (a != null)
		{
			a.name = name;
			a.text = text;
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
