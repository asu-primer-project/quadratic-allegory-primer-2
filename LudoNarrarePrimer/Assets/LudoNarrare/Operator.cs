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
	public string agentSelector;

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
		agentSelector = "";
	}

	public void operate(StoryWorld sw, Verb vc)
	{
		//Get all subjects to operate on
		List<Entity> osList = new List<Entity>();

		if (operatorSubject[0] == '?' && vc != null)
		{
			foreach (Entity e in vc.variables.Find(v => v.name == operatorSubject).values)
				osList.Add(e);
		}
		else
			osList.Add(sw.entities.Find(x => x.name == operatorSubject));

		//Apply agent selector if needed
		if (agentSelector != "")
		{
			foreach (Entity e in osList)
			{
				if (e.agent != null)
				{
					if (e.agent.name != agentSelector)
						osList.RemoveAll(x => x.name == e.name);	
				}
				else
					osList.RemoveAll(x => x.name == e.name);	
			}
		}

		switch(op)
		{
		case 0:
			foreach (Entity e in osList)
			{
				if (tagRef != "")
				{
					if (!e.tags.Exists(x => x.name == tagRef))
						e.tags.Add(new Tag(tagRef));
				}
				else if (relateRef != "" && relateObj != "")
				{
					//Get all objects to operate on
					List<Entity> ooList = new List<Entity>();
					
					if (relateObj[0] == '?' && vc != null)
					{
						foreach (Entity o in vc.variables.Find(v => v.name == relateObj).values)
							ooList.Add(o);
					}
					else
						ooList.Add(sw.entities.Find(x => x.name == relateObj));

					foreach (Entity o in ooList)
					{
						if (!e.relationships.Exists(y => y.name == relateRef && y.other == o.name))
							e.relationships.Add(new Relationship(relateRef, o.name));
					}
				}
				else if (numRef != "" && num != null)
				{
					if (!e.numbers.Exists(x => x.name == numRef))
						e.numbers.Add(new Number(numRef, num.evaluate(sw, vc)));
					else
						e.numbers.Find(y => y.name == numRef).value = num.evaluate(sw, vc);
				}
				else if (stringRef != "" && stringValue != "")
				{
					if (!e.strings.Exists(x => x.name == stringRef))
						e.strings.Add(new LNString(stringRef, stringValue));
					else
						e.strings.Find(y => y.name == stringRef).text = stringValue;
				}
			}
			break;
		case 1:
			foreach (Entity e in osList)
			{
				if (tagRef != "")
					e.tags.RemoveAll(x => x.name == tagRef);
				else if (relateRef != "" && relateObj != "")
				{
					//Get all objects to operate on
					List<Entity> ooList = new List<Entity>();
					
					if (relateObj[0] == '?' && vc != null)
					{
						foreach (Entity o in vc.variables.Find(v => v.name == relateObj).values)
							ooList.Add(o);
					}
					else
						ooList.Add(sw.entities.Find(x => x.name == relateObj));

					foreach (Entity o in ooList)
						e.relationships.RemoveAll(y => y.name == relateRef && y.other == o.name); 
				}
				else if (numRef != "")
					e.numbers.RemoveAll(x => x.name == numRef);
				else if (stringRef != "")
					e.strings.RemoveAll(x => x.name == stringRef);
			}
			break;
		case 2:
			foreach (Entity e in osList)
				e.images.Find(x => x.name == imageRef).image = image;
			break;
		}
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
			else
				o.num = null;
			o.stringRef = stringRef;
			o.stringValue = stringValue;
			o.imageRef = imageRef;
			o.image = image;
			o.agentSelector = agentSelector;
        }
    }
}
