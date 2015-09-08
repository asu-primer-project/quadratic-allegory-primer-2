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

	public Mind()
	{
		name = "mind";
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

	public bool hasNumber(string n, int v, int c)
	{
		//Equal to
		if (c == 0)
			return body.numbers.Exists(x => x.name == n && x.value == v);

		//Greater than
		if (c == 1)
			return body.numbers.Exists(x => x.name == n && x.value < v);

		//Greater than or equal to
		if (c == 2)
			return body.numbers.Exists(x => x.name == n && x.value <= v);

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

	//Given a choice of actions, decide on one to take
	public virtual Verb decide(List<Verb> choices)
	{
		return choices[0];
	}
}
