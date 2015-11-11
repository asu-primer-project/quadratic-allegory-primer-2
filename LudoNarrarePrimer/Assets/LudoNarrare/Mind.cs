using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

//All AI controllers are children of Mind and need to implement it's three main functions to interact with the story.
//Once an AI controller is written, it must be added to the list of referenceable AI in the StoryWorldLoader class.
public class Mind 
{
	public string name;
	public Entity body;
	public StoryWorld sw;	
	protected System.Random rand;

	public Mind()
	{
		name = "mind";
		rand = new System.Random();
	}

	//Query the attributes of the entity's body
	public bool hasTag(string t)
	{
		return body.tags.Exists(x => x.name == t);
	}

	public bool hasRelationship(string r, string o)
	{
		return body.relationships.Exists(x => x.name == r && x.other == o);
	}

	public bool hasRelationship(string r)
	{
		return body.relationships.Exists(x => x.name == r);
	}

	public List<string> getRelationship(string r)
	{
		List<string> others = new List<String>();
		foreach (Relationship rel in body.relationships.FindAll(x => x.name == r))
			others.Add(rel.other);

		return others;
	}

	public bool hasNumber(string n, int v, int c)
	{
		//Equal to
		if (c == 0)
			return body.numbers.Exists(x => x.name == n && x.value == v);

		//Greater than
		if (c == 1)
			return body.numbers.Exists(x => x.name == n && x.value > v);

		//Less than
		if (c == 2)
			return body.numbers.Exists(x => x.name == n && x.value < v);

		return false;
	}

	public int getNumber(string n)
	{
		return body.numbers.Find(x => x.name == n).value;
	}

	public bool hasString (string s, string v)
	{
		return body.strings.Exists(x => x.name == s && x.text == v);
	}

	public string getString(string s)
	{
		return body.strings.Find(x => x.name == s).text;
	}

	public void chooseRandomArgument(Verb v, int argueNum)
	{
		v.replaceWith(v.arguments[argueNum].name, v.arguments[argueNum].values[rand.Next(0, v.arguments[argueNum].values.Count)].name);
	}

	public void chooseRandomArguments(Verb v)
	{
		switch(v.arguments.Count)
		{
		case 0:
			break;
		case 1:
			chooseRandomArgument(v, 0);
			break;
		case 2:
			chooseRandomArgument(v, 0);
			chooseRandomArgument(v, 1);
			break;
		case 3:
			chooseRandomArgument(v, 0);
			chooseRandomArgument(v, 1);
			chooseRandomArgument(v, 2);
			break;
		case 4:
			chooseRandomArgument(v, 0);
			chooseRandomArgument(v, 1);
			chooseRandomArgument(v, 2);
			chooseRandomArgument(v, 3);
			break;
		}
	}
	
	public void chooseSpecificArgument(Verb v, int argueNum, string value)
	{
		if (hasArgument(v, argueNum, value))
			v.replaceWith(v.arguments[argueNum].name, value);
		else
			v.replaceWith(v.arguments[argueNum].name, v.arguments[argueNum].values[rand.Next(0, v.arguments[argueNum].values.Count)].name);
	}
	
	public bool hasArgument(Verb v, int argueNum, string value)
	{
		return v.arguments[argueNum].values.Exists(x => x.name == value);
	}
	
	public bool canDoVerb(List<Verb> choices, string verb)
	{
		return choices.Exists(x => x.name == verb);
	}
	
	public Verb getVerb(List<Verb> choices, string verb)
	{
		return choices.Find(x => x.name == verb);
	}

	//Given a choice of actions, decide on one to take
	public virtual Verb decide(List<Verb> choices)
	{
		return choices[0];
	}
}
