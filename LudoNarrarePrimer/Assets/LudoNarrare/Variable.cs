using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class Variable
{
	/* Variables */
	public string name;
	public List<Entity> values;
	public List<Condition> conditions;
	
	/* Functions */
	public Variable(string _name)
	{
		name = _name;
		values = new List<Entity>();
		conditions = new List<Condition>();
	}
	
	public void copyTo(Variable v)
	{
		if (v != null)
		{
			v.name = name;
			v.values.Clear();
			for (int i = 0; i < values.Count; i++)
				v.values.Add(values[i]);
			v.conditions.Clear();
			for (int i = 0; i < conditions.Count; i++)
			{
				Condition tempC = new Condition();
				conditions[i].copyTo(tempC);
				v.conditions.Add(tempC);
			}
		}
	}
}
