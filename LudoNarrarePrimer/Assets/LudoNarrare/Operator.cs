using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class Operator
{
    /* Variables */
    public string name;
    public string operatorSubject;
	//0 - add, 1 - remove, 2 - +, 3 - -, 4 - *, 5 - /, 6 - =
	public int op; 
    public Tag tag;
    public Relationship relationship;
	public Number num;
	public LNString lnString;
    public Obligation obligation;
    public Goal goal;
    public Behavior behavior;
	public string subject2;
	public string numRef2;

    /* Functions */
    public Operator()
    {
        name = "";
        operatorSubject = "";
		op = 0;
        tag = null;
        relationship = null;
		num = null;
		lnString = null;
        obligation = null;
        goal = null;
        behavior = null;
		subject2 = "";
		numRef2 = "";
    }

    public void replaceWith(string replace, string with)
    {
        if (operatorSubject == replace)
            operatorSubject = with;
		if (relationship != null)
		{
		    if (relationship.other == replace)
		        relationship.other = with;
		}
        if (obligation != null)
        {
            for (int i = 0; i < obligation.arguments.Count; i++ )
            {
                if (obligation.arguments[i] == replace)
                    obligation.arguments[i] = with;
            }
        }
        if (behavior != null)
        {
            for (int i = 0; i < behavior.arguments.Count; i++)
            {
                if (behavior.arguments[i] == replace)
                    behavior.arguments[i] = with;
            }
        }
		if (subject2 == replace)
			subject2 = with;
    }

	public int getType()
	{
		if (tag != null)
			return 0;
		else if (relationship != null)
			return 1;
		else if (num != null)
			return 2;
		else if (lnString != null)
			return 3;
		else if (obligation != null)
			return 4;
		else if (goal != null)
			return 5;
		else if (behavior != null)
			return 6;
		else
			return -1;
	}

    public void copyTo(Operator o)
    {
        if (o != null)
        {
            o.name = name;
            o.operatorSubject = operatorSubject;
			o.op = op;
            if (tag != null)
			{
				o.tag = new Tag("");
                tag.copyTo(o.tag);
			}
            if (relationship != null)
			{
				o.relationship = new Relationship("", "");
                relationship.copyTo(o.relationship);
			}
			if (num != null)
			{
				o.num = new Number("",0);
				num.copyTo(o.num);
			}
			if (lnString != null)
			{
				o.lnString = new LNString("","");
				lnString.copyTo(o.lnString);
			}
            if (obligation != null)
			{
				o.obligation = new Obligation("", "");
                obligation.copyTo(o.obligation);
			}
            if (goal != null)
			{
				o.goal = new Goal("", "", 0);
                goal.copyTo(o.goal);
			}
            if (behavior != null)
			{
				o.behavior = new Behavior("", "", 0);
                behavior.copyTo(o.behavior);
			}
			o.subject2 = subject2;
			o.numRef2 = numRef2;
        }
    }
}
