using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

//Entity which queries its emotions and knowledge to make decisions
public class MindPerson : Mind 
{
	public MindPerson()
	{
		name = "person";
	}

	//Given a value from -5 to 5, gambles to find whether to lean negative or positive
	public bool gamble(int i)
	{
		int compare = i + 6;
		int gamble = rand.Next(0, 12);

		if (gamble >= compare)
			return false;
		else
			return true;
	}

	//Given a choice of actions, decide on one to take
	public override Verb decide(List<Verb> choices)
	{
		//Going to take action or not?
		int init = getNumber("initiative");

		//If there are more options than waiting, don't wait
		if (choices.Count() > 1)
		{
			if (gamble(init))
			{
				List<Verb> reasonableChoices = null;

				bool social = gamble(getNumber("social"));
				bool moral = gamble(getNumber("morality"));
				bool calm = gamble(getNumber("happiness"));
				bool happy = gamble(getNumber("temperment"));

				switch (rand.Next(0,3))
				{
				case 0:
					if (social)
						reasonableChoices = choices.FindAll(x => x.tags.Exists(y => y.name == "social"));
					else
						reasonableChoices = choices.FindAll(x => x.tags.Exists(y => y.name == "notSocial"));
					break;
				case 1:
					if (moral)
						reasonableChoices = choices.FindAll(x => x.tags.Exists(y => y.name == "moral"));
					else
						reasonableChoices = choices.FindAll(x => x.tags.Exists(y => y.name == "notMoral"));
					break;
				case 2:
					if (calm)
						reasonableChoices = choices.FindAll(x => x.tags.Exists(y => y.name == "calm"));
					else
						reasonableChoices = choices.FindAll(x => x.tags.Exists(y => y.name == "notCalm"));
					break;
				case 3:
					if (happy)
						reasonableChoices = choices.FindAll(x => x.tags.Exists(y => y.name == "happy"));
					else
						reasonableChoices = choices.FindAll(x => x.tags.Exists(y => y.name == "notHappy"));
					break;
				}

				//Choose
				if (reasonableChoices.Count > 0)
				{
					Verb choice = reasonableChoices[rand.Next(0, reasonableChoices.Count)];

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
		}

		return choices.Find(x => x.name == "Wait");
	}
}
