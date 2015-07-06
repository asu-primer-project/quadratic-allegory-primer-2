using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class Operator
{
    /* Variables */
    public string name;
    public string operatorSubject;
	//0 - add, 1 - remove
	public int op; 
    public Tag tag;
    public Relationship relationship;
	public string numRef;
	public Expression num;
	public LNString lnString;
	public ImageDef newImage;

    /* Functions */
    public Operator()
    {
        name = "";
        operatorSubject = "";
		op = 0;
        tag = null;
        relationship = null;
		numRef = "";
		num = null;
		lnString = null;
		newImage = null;
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
		if (num != null)
			num.replaceWith(replace, with);
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
		else if (newImage != null)
			return 4;
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
				o.num = new Expression(num.type);
				num.copyTo(o.num);
			}
			if (lnString != null)
			{
				o.lnString = new LNString("","");
				lnString.copyTo(o.lnString);
			}
			if (ImageDef != null)
			{
				o.newImage = new ImageDef("","");
				newImage.copyTo(o.newImage);
			}
        }
    }
}
