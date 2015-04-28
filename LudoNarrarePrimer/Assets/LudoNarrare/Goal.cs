using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class Goal
{
	/* Variables */
	public string name;
	public string operatorSubject;
	//0 - add, 1 - remove, 2 - +, 3 - -, 4 - *, 5 - /
	public int op; 
	public Tag tag;
	public Relationship relationship;
	public Number num;
	public LNString lnString;
	public Obligation obligation;
	public Behavior behavior;
	
	/* Functions */
	public Goal(string _name, string _operatorSubject, int _op)
	{
		name = _name;
		operatorSubject = _operatorSubject;
		op = _op;
		tag = null;
		relationship = null;
		num = null;
		lnString = null;
		obligation = null;
		behavior = null;
	}

    public bool matchWith(Operator o)
    {
        if ((o.operatorSubject == operatorSubject || o.operatorSubject == "?me") && o.op == op) //Is o.operatorSubject == "?me" correct?
        {
			switch(o.getType())
            {
            case 0:
                if (o.tag.name == tag.name)
                    return true;
                else
                    return false;
            case 1:
                if (o.relationship.name == relationship.name)
                {
                    if (o.relationship.other == relationship.other)
                        return true;
                    else
                        return false;
                }
                else
                    return false;
			case 2:
				if (o.num.name == num.name)
				{
					if (o.num.value == num.value)
						return true;
					else
						return false;
				}
				else
					return false;
			case 3:
				if (o.lnString.name == lnString.name)
				{
					if (o.lnString.text == lnString.text)
						return true;
					else
						return false;
				}
				else
					return false;
            case 4:
                if (o.obligation.name == obligation.name)
                    return true;
                else
                    return false;
            case 6:
                if (o.behavior.name == behavior.name)
                    return true;
                else
                    return false;
            default:
                return false;
            }
        }
        else
            return false;
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
		else if (behavior != null)
			return 6;
		else
			return -1;
	}

	public void copyTo(Goal g)
	{
		if (g != null)
		{
			g.name = name;
			g.operatorSubject = operatorSubject;
			g.op = op;
			g.tag = new Tag("");
			if (tag != null)
				tag.copyTo(g.tag);
			g.relationship = new Relationship("", "");
			if (relationship != null)
				relationship.copyTo(g.relationship);
			g.num = new Number("",0);
			if (num != null)
				num.copyTo(g.num);
			g.lnString = new LNString("","");
			if (lnString != null)
				lnString.copyTo(g.lnString);
			g.obligation = new Obligation("", "");
			if (obligation != null)
				obligation.copyTo(g.obligation);
			g.behavior = new Behavior("", "", 0);
			if (behavior != null)
				behavior.copyTo(g.behavior);
		}
	}
}
