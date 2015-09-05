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

	//Given a choice of actions, decide on one to take
	public virtual Verb decide(List<Verb> choices)
	{
		return choices[0];
	}
}
