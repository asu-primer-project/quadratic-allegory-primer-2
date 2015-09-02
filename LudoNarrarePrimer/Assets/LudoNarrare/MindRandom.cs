using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

//This is an incredibly simple mind which chooses its action randomly.
public class MindRandom : Mind 
{	
	private System.Random rand;

	public MindRandom()
	{
		name = "random";
		rand = new System.Random();
	}

	//Given a choice of actions, decide on one to take
	public override Verb decide(List<Verb> choices)
	{
		//If there are more options than waiting, don't wait
		if (choices.Count() > 1)
			choices.RemoveAll(x => x.name == "Wait");

		Verb choice = choices[rand.Next(0, choices.Count)];

		//Apply random arguments
		switch(choice.arguments.Count())
		{
		case 0:
			break;
		case 1:
			choice.replaceWith(choice.arguments[0].name, choice.arguments[0].values[rand.Next(0, choice.arguments[0].values.Count())].name);
			break;
		case 2:
			choice.replaceWith(choice.arguments[0].name, choice.arguments[0].values[rand.Next(0, choice.arguments[0].values.Count())].name);
			choice.replaceWith(choice.arguments[1].name, choice.arguments[1].values[rand.Next(0, choice.arguments[1].values.Count())].name);
			break;
		case 3:
			choice.replaceWith(choice.arguments[0].name, choice.arguments[0].values[rand.Next(0, choice.arguments[0].values.Count())].name);
			choice.replaceWith(choice.arguments[1].name, choice.arguments[1].values[rand.Next(0, choice.arguments[1].values.Count())].name);
			choice.replaceWith(choice.arguments[2].name, choice.arguments[2].values[rand.Next(0, choice.arguments[2].values.Count())].name);
			break;
		case 4:
			choice.replaceWith(choice.arguments[0].name, choice.arguments[0].values[rand.Next(0, choice.arguments[0].values.Count())].name);
			choice.replaceWith(choice.arguments[1].name, choice.arguments[1].values[rand.Next(0, choice.arguments[1].values.Count())].name);
			choice.replaceWith(choice.arguments[2].name, choice.arguments[2].values[rand.Next(0, choice.arguments[2].values.Count())].name);
			choice.replaceWith(choice.arguments[3].name, choice.arguments[3].values[rand.Next(0, choice.arguments[3].values.Count())].name);
			break;
		}

		return choice;
	}
}
