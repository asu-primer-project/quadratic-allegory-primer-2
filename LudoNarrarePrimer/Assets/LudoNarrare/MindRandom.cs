using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

//This is an incredibly simple mind which chooses its action randomly.
public class MindRandom : Mind 
{	
	public MindRandom()
	{
		name = "random";
	}

	//Given a choice of actions, decide on one to take
	public override Verb decide(List<Verb> choices)
	{
		System.Random rand = new System.Random();
		return choices[rand.Next(0, choices.Count)];
	}
}
