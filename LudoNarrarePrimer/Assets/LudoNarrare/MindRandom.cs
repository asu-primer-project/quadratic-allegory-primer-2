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

		//If there are more options than waiting, don't wait
		if (choices.Count() > 1)
			choices.RemoveAll(x => x.name == "Wait");

		return choices[rand.Next(0, choices.Count)];
	}
}
