using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class Condition
{
    /* Variables */
    public string name;
    public string conditionSubject;
	public bool allCS; //false - one
	public bool allRO; //false - one
	//0 - has, 1 - missing, 2 - =, 3 - !=, 4 - <, 5 - >, 6 - <=, 7 - >=
	public int comparison;
    public string tagRef;
    public string relateRef;
	public string relateObject;
	public string numRef;
	public string stringRef;
	public string obligationRef;
	public string goalRef;
	public string behaviorRef;
	public int numCompare;
	public string stringCompare;
	public string variableObject;

    /* Functions */
    public Condition()
    {
        name = "";
		conditionSubject = "";
		allCS = false;
		allRO = false;
		comparison = 0;
		tagRef = "";
		relateRef = "";
		relateObject = "";
		numRef = "";
		stringRef = "";
		obligationRef = "";
		goalRef = "";
		behaviorRef = "";
		numCompare = 0;
		stringCompare = "";
		variableObject = "";
    }

    public void replaceWith(string replace, string with)
    {
        if (conditionSubject == replace)
            conditionSubject = with;
        if (relateObject == replace)
            relateObject = with;
    }

	public int getType()
	{
		if (tagRef != "")
			return 0;
		else if (relateRef != "")
			return 1;
		else if (numRef != "")
			return 2;
		else if (stringRef != "")
			return 3;
		else if (obligationRef != "")
			return 4;
		else if (goalRef != "")
			return 5;
		else if (behaviorRef != "")
			return 6;
		else if (variableObject != "")
			return 7;
		else
			return -1;
	}

    public void copyTo(Condition c)
    {
        if (c != null)
        {
            c.name = name;
            c.conditionSubject = conditionSubject;
			c.allCS = allCS;
			c.allRO = allRO;
			c.comparison = comparison;
			c.tagRef = tagRef;
			c.relateRef = relateRef;
			c.relateObject = relateObject;
			c.numRef = numRef;
			c.stringRef = stringRef;
			c.obligationRef = obligationRef;
			c.goalRef = goalRef;
			c.behaviorRef = behaviorRef;
			c.numCompare = numCompare;
			c.stringCompare = stringCompare;
			c.variableObject = variableObject;
        }
    }
}
