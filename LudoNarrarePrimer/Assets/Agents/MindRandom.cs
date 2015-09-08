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

	public void chooseArgument(Verb v, int argueNum)
	{
		if (v.arguments[argueNum].values.Exists(x => x.numbers.Exists(y => y.name == "odds")))
		{
			Entity ac = null;
			List<Entity> thoseWithOdds = v.arguments[argueNum].values.FindAll(x => x.numbers.Exists(y => y.name == "odds"));

			int max = 0;
			foreach (Entity e in thoseWithOdds)
				max += e.numbers.Find(x => x.name == "odds").value;

			int spin = rand.Next(1, max);

			int count = 0;
			foreach (Entity e in thoseWithOdds)
			{
				count += e.numbers.Find(x => x.name == "odds").value;

				if (spin <= count)
				{
					ac = e;
					break;
				}
			}
			
			v.replaceWith(v.arguments[argueNum].name, ac.name);
		}
		else
			v.replaceWith(v.arguments[argueNum].name, v.arguments[argueNum].values[rand.Next(0, v.arguments[argueNum].values.Count())].name);
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
			chooseArgument(choice, 0);
			break;
		case 2:
			chooseArgument(choice, 0);
			chooseArgument(choice, 1);
			break;
		case 3:
			chooseArgument(choice, 0);
			chooseArgument(choice, 1);
			chooseArgument(choice, 2);
			break;
		case 4:
			chooseArgument(choice, 0);
			chooseArgument(choice, 1);
			chooseArgument(choice, 2);
			chooseArgument(choice, 3);
			break;
		}

		return choice;
	}
}
