using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class AtomicCondition
{
	/* Variables */
	public string conditionSubject;
	public string conditionObject;
	public bool allCS; //false - one
	public bool allCO; //false - one
	//0 - has, 1 - =, 2 - !=, 3 - <, 4 - >, 
	//5 - <=, 6 - >=, 7 - empty, 8 - same, 9 - matches, 10 - relates
	public int comparison;
	public string tagRef;
	public string relateRef;
	public string numRef;
	public string stringRef;
	public Expression numCompare;
	public string stringCompare;
	public string stringRef2;
	
	/* Functions */
	public AtomicCondition()
	{
		conditionSubject = "";
		conditionObject = "";
		allCS = false;
		allCO = false;
		comparison = 0;
		tagRef = "";
		relateRef = "";
		numRef = "";
		stringRef = "";
		numCompare = null;
		stringCompare = "";
		stringRef2 = "";
	}

	public bool evaluate(StoryWorld sw, Verb vc)
	{
		//Build a list of condition subjects to check
		List<Entity> csList = new List<Entity>();

		if (conditionSubject[0] == '?' && vc != null)
		{
			foreach (Entity e in vc.variables.Find(v => v.name == conditionSubject).values)
				csList.Add(e);
		}
		else
			csList.Add(sw.entities.Find(e => e.name == conditionSubject));

		//Build a list of condition objects to check
		List<Entity> coList = new List<Entity>();

		if (conditionObject[0] == '?' && vc != null)
		{
			foreach (Entity e in vc.variables.Find(v => v.name == conditionObject).values)
				coList.Add(e);
		}
		else if (conditionObject != "")
			coList.Add(sw.entities.Find(e => e.name == conditionObject));

		if (comparison == 7)
		{
			if (csList.Count == 0)
				return true;
			else
				return false;
		}
		else if (comparison == 8)
		{
			if (csList.Count == coList.Count)
			{
				foreach (Entity e in csList)
				{
					if (!coList.Exists(x => x.name == e.name))
						return false;
				}

				return true;
			}
			else 
				return false;
		}
		else
		{
			if (!allCS && !allCO)
			{
				foreach (Entity i in csList)
				{
					if (coList.Count > 0)
					{
						foreach (Entity j in coList)
						{
							if (evaluatePair(i, j, sw))
								return true;
						}
					}
					else
					{
						if (evaluatePair(i, null, sw))
							return true;
					}
				}
			}
			else if (allCS && !allCO)
			{
				int truthCount = 0;

				foreach (Entity i in csList)
				{
					if (coList.Count > 0)
					{
						foreach (Entity j in coList)
						{
							if (evaluatePair(i, j, sw))
							{
								truthCount++;
								break;
							}
						}
					}
					else
					{
						if (evaluatePair(i, null, sw))
							truthCount++;
					}
				}

				if (truthCount == csList.Count)
					return true;
				else
					return false;
			}
			else if (!allCS && allCO)
			{
				int truthCount = 0;
				
				foreach (Entity i in coList)
				{
					foreach (Entity j in csList)
					{
						if (evaluatePair(j, i, sw))
						{
							truthCount++;
							break;
						}
					}
				}
				
				if (truthCount == coList.Count)
					return true;
				else
					return false;
			}
			else if (allCS && allCO)
			{
				foreach (Entity i in csList)
				{
					foreach (Entity j in coList)
					{
						if (!evaluatePair(j, i, sw))
							return false;
					}
				}

				return true;
			}
			else
				return false;
		}
		return false;
	}

	public bool evaluatePair(Entity cs, Entity co, StoryWorld sw)
	{
		switch(comparison)
		{
		case 0:
			if (tagRef != "")
			{
				if (cs.tags.Exists(x => x.name == tagRef))
					return true;
				else
					return false;
			}
			else if (relateRef != "")
			{
				if (cs.relationships.Exists(x => x.name == relateRef))
					return true;
				else
					return false;
			}
			else if (numRef != "")
			{
				if (cs.numbers.Exists(x => x.name == numRef))
					return true;
				else
					return false;
			}
			else if (stringRef != "")
			{
				if (cs.strings.Exists(x => x.name == stringRef))
					return true;
				else
					return false;
			}
			break;
		case 1:
			if (numRef != "" && numCompare != null)
			{
				if (cs.numbers.Find(x => x.name == numRef).value == numCompare.evaluate(sw))
					return true;
				else
					return false;
			}
			break;
		case 2:
			if (numRef != "" && numCompare != null)
			{
				if (cs.numbers.Find(x => x.name == numRef).value != numCompare.evaluate(sw))
					return true;
				else
					return false;
			}
			break;
		case 3:
			if (numRef != "" && numCompare != null)
			{
				if (cs.numbers.Find(x => x.name == numRef).value < numCompare.evaluate(sw))
					return true;
				else
					return false;
			}
			break;
		case 4:
			if (numRef != "" && numCompare != null)
			{
				if (cs.numbers.Find(x => x.name == numRef).value > numCompare.evaluate(sw))
					return true;
				else
					return false;
			}
			break;
		case 5:
			if (numRef != "" && numCompare != null)
			{
				if (cs.numbers.Find(x => x.name == numRef).value <= numCompare.evaluate(sw))
					return true;
				else
					return false;
			}
			break;
		case 6:
			if (numRef != "" && numCompare != null)
			{
				if (cs.numbers.Find(x => x.name == numRef).value >= numCompare.evaluate(sw))
					return true;
				else
					return false;
			}
			break;
		case 9:
			if (stringRef != "")
			{
				if (stringCompare != "")
				{
					if (cs.strings.Find(x => x.name == stringRef).text == stringCompare)
						return true;
					else
						return false;
				}
				else if (stringRef2 != "" && co != null)
				{
					if (cs.strings.Find(x => x.name == stringRef).text == co.strings.Find(y => y.name == stringRef2).text)
						return true;
					else
						return false;
				}
			}
			break;
		case 10:
			if (relateRef != "" && co != null)
			{
				if (cs.relationships.Find(x => x.name == relateRef).other == co.name)
					return true;
				else
					return false;
			}
			break;
		}

		return false;
	}
	
	public void replaceWith(string replace, string with)
	{
		if (conditionSubject == replace)
			conditionSubject = with;
		if (conditionObject == replace)
			conditionObject = with;
		if (numCompare != null)
			numCompare.replaceWith(replace, with);
	}

	
	public void copyTo(AtomicCondition c)
	{
		if (c != null)
		{
			c.conditionSubject = conditionSubject;
			c.conditionObject = conditionObject;
			c.allCS = allCS;
			c.allCO = allCO;
			c.comparison = comparison;
			c.tagRef = tagRef;
			c.relateRef = relateRef;
			c.numRef = numRef;
			c.stringRef = stringRef;
			if (numCompare != null)
			{
				c.numCompare = new Expression(numCompare.type);
				numCompare.copyTo(c.numCompare);
			}
			else
				c.numCompare = null;
			c.stringCompare = stringCompare;
			c.stringRef2 = stringRef2;
		}
	}
}

