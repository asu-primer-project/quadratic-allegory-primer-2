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
	public List<Page> story; //Story so far; the history is made to make decisions

	public Mind()
	{
		name = "mind";
	}

	//Set up the initial state of the mind
	public virtual void initalize(List<Page> beginning)
	{
		story = beginning;
	}

	//Given a choice of actions, decide on one to take
	public virtual Verb decide(List<Verb> choices)
	{
		return choices[0];
	}

	//Read the story up the current moment to make an up to date decision
	public virtual void read(List<Page> newStory)
	{
		story = newStory;
	}
}
