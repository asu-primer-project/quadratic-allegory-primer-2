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
    }

    public void replaceWith(string replace, string with)
    {
        if (operatorSubject == replace)
            operatorSubject = with;
        if (relationship.other == replace)
            relationship.other = with;
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
            for (int i = 0; i < obligation.arguments.Count; i++)
            {
                if (obligation.arguments[i] == replace)
                    obligation.arguments[i] = with;
            }
        }           
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
            o.tag = new Tag("");
            if (tag != null)
                tag.copyTo(o.tag);
            o.relationship = new Relationship("", "", false);
            if (relationship != null)
                relationship.copyTo(o.relationship);
			o.num = new Number("",0);
			if (num != null)
				num.copyTo(o.num);
			o.lnString = new LNString("","");
			if (lnString != null)
				lnString.copyTo(o.lnString);
            o.obligation = new Obligation("", "");
            if (obligation != null)
                obligation.copyTo(o.obligation);
            o.goal = new Goal("", "", 0);
            if (goal != null)
                goal.copyTo(o.goal);
            o.behavior = new Behavior("", "", 0);
            if (behavior != null)
                behavior.copyTo(o.behavior);
        }
    }
}
