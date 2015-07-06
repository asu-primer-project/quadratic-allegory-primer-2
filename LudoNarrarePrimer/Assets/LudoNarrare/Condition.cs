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
	public Expression numCompare;
	public string stringCompare;
	public string variableObject;
	public string subject2;
	public string stringRef2;

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
		numCompare = null;
		stringCompare = "";
		variableObject = "";
		subject2 = "";
		stringRef2 = "";
    }

    public void replaceWith(string replace, string with)
    {
        if (conditionSubject == replace)
            conditionSubject = with;
        if (relateObject == replace)
            relateObject = with;
		if (subject2 == replace)
			subject2 = with;
		if (numCompare != null)
			numCompare.replaceWith(replace, with);
    }

	public int getType()
	{
		if (stringRef2 != "")
			return 5;
		else if (tagRef != "")
			return 0;
		else if (relateRef != "")
			return 1;
		else if (numRef != "")
			return 2;
		else if (stringRef != "")
			return 3;
		else if (variableObject != "")
			return 4;
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
			if (c.numCompare != null)
			{
				c.numCompare = new Expression(numCompare.type);
				numCompare.copyTo(c.numCompare);
			}
			else
				c.numCompare = null;
			c.stringCompare = stringCompare;
			c.variableObject = variableObject;
			c.subject2 = subject2;
			c.stringRef2 = stringRef2;
        }
    }
}
