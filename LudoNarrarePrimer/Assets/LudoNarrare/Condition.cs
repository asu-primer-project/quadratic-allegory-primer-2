using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class Condition
{
    /* Variables */
    public string name;
    public string conditionSubject;
	public string conditionObject;
	public bool allCS; //false - one
	public bool allCO; //false - one
	//0 - has, 1 - missing, 2 - =, 3 - !=, 4 - <, 5 - >, 
	//6 - <=, 7 - >=, 8 - empty, 9 - not empty, 10 - same, 11 - not same
	//12 - matches, 13 - not matches
	public int comparison;
    public string tagRef;
    public string relateRef;
	public string numRef;
	public string stringRef;
	public Expression numCompare;
	public string stringCompare;
	public string stringRef2;

    /* Functions */
    public Condition()
    {
        name = "";
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

    public void replaceWith(string replace, string with)
    {
        if (conditionSubject == replace)
            conditionSubject = with;
		if (conditionObject == replace)
            conditionObject = with;
		if (numCompare != null)
			numCompare.replaceWith(replace, with);
    }

	//Probably completely wrong
	public int getType()
	{
		/*
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
			return -1;*/
		return 0;
	}

    public void copyTo(Condition c)
    {
        if (c != null)
        {
            c.name = name;
            c.conditionSubject = conditionSubject;
			c.conditionObject = conditionObject;
			c.allCS = allCS;
			c.allCO = allCO;
			c.comparison = comparison;
			c.tagRef = tagRef;
			c.relateRef = relateRef;
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
			c.stringRef2 = stringRef2;
        }
    }
}
