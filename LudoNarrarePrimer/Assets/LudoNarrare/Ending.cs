using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class Ending
{
	/* Variables */
	public string name;
	public List<Condition> conditions;
	public List<Page> pages;
	
	/* Functions */
	public Ending(string _name)
	{
		name = _name;
		conditions = new List<Condition>();
		pages = new List<Page>();
	}
}
