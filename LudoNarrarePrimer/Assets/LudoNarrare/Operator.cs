using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class Operator
{
    /* Variables */
    public string name;
    public string operatorSubject;
	//0 - add, 1 - remove, 2 - change
	public int op; 
    public string tagRef;
    public string relateRef;
	public string relateObj;
	public string numRef;
	public Expression num;
	public string stringRef;
	public string stringValue;
	public string imageRef;
	public string image;

    /* Functions */
    public Operator()
    {
        name = "";
        operatorSubject = "";
		op = 0;
        tagRef = "";
        relateRef = "";
		relateObj = "";
		numRef = "";
		num = null;
		stringRef = "";
		stringValue = "";
		imageRef = "";
		image = "";
	}

    public void replaceWith(string replace, string with)
    {
        if (operatorSubject == replace)
            operatorSubject = with;
		if (relateObj == replace)
			relateObj = with;
		if (num != null)
			num.replaceWith(replace, with);
    }

	//Completely broke again
	public int getType()
	{
		/*
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
			return -1;*/
	}

    public void copyTo(Operator o)
    {
        if (o != null)
        {
            o.name = name;
            o.operatorSubject = operatorSubject;
			o.op = op;
			o.tagRef = tagRef;
			o.relateRef = relateRef;
			o.relateObj = relateObj;
			o.numRef = numRef;
			if (num != null)
			{
				o.num = new Expression(num.type);
				num.copyTo(o.num);
			}
			o.stringRef = stringRef;
			o.stringValue = stringValue;
			o.imageRef = imageRef;
			o.image = image;
        }
    }
}
