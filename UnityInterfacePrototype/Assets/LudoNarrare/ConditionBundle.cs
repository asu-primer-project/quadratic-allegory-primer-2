using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class ConditionBundle 
{
	/* Variables */
	public string name;
	public List<Condition> conditions;
	
	/* Functions */
	public ConditionBundle(string _name)
	{
		name = _name;
		conditions = new List<Condition>();
	}
	
	public void copyTo(ConditionBundle cb)
	{
		if (cb != null)
		{
			cb.name = name;
			cb.conditions.Clear();
			for (int i = 0; i < conditions.Count; i++)
			{
				Condition tempC = new Condition();
				conditions[i].copyTo(tempC);
				cb.conditions.Add(tempC);
			}
		}
	}
}
