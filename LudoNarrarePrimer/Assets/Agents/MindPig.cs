using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MindPig : Mind 
{
	private bool timeToEnterHouse;

	public MindPig()
	{
		name = "pig";
		timeToEnterHouse = false;
	}

	public override Verb decide (List<Verb> choices)
	{
		Verb choice;

		//No options?
		if (choices.Count == 1)
			return choices[0];

		//Is the pig safe or not?
		if (hasTag("insideHouse"))
		{
			//If the pig is safe in a house and can taunt, they have 33% chance of taunting
			if (canDoVerb(choices, "Taunt"))
			{
				if (rand.Next(0, 3) == 0)
				{
					choice = getVerb(choices, "Taunt");
					chooseRandomArguments(choice);
					return choice;
				}
				else
				{
					choice = getVerb(choices, "Wait");
					return choice;
				}
			}
		}
		else
		{
			//If fazed, become unfazed
			if (canDoVerb(choices, "BecomeUnfazed"))
				return getVerb(choices, "BecomeUnfazed");

			//If distracted, become undistracted
			if (canDoVerb(choices, "BecomeUndistracted"))
				return getVerb(choices, "BecomeUndistracted");

			//If can look for money, do so %50 of the time
			if (canDoVerb(choices, "LookForMoney"))
				if (rand.Next(0, 2) == 1)
					return getVerb(choices, "LookForMoney");

			//If house built, enter house
			if (timeToEnterHouse)
			{
				timeToEnterHouse = false;
				if (canDoVerb(choices, "EnterHouse"))
					return getVerb(choices, "EnterHouse");
			}

			//Boolean state variables
			bool hasMaterials = hasRelationship("owns");
			bool canLookForMoney = !hasTag("foundMoney");
			List<Entity> buyableSupplies = sw.entities.FindAll(x => x.tags.Exists(y => y.name == "supplies") && !x.tags.Exists(y => y.name == "bought"));
			int lowestPrice = 10000000;
			foreach (Entity e in buyableSupplies)
				if (e.numbers.Find(x => x.name == "cost").value < lowestPrice)
					lowestPrice = e.numbers.Find(x => x.name == "cost").value;
			bool hasEnoughMoney = false;
			if (getNumber("money") >= lowestPrice)
				hasEnoughMoney = true;

			string location = getRelationship("at")[0];

			//If no materials, not enough money, cannot look for money, cower
			if (!hasMaterials && !hasEnoughMoney && !canLookForMoney)
			{
				if (canDoVerb(choices, "Cower"))
					return getVerb(choices, "Cower");
				else if (canDoVerb(choices, "Run"))
				{
					//If at store, run to place where cowering is possible
					choice = getVerb(choices, "Run");
					List<Entity> runTo = choice.arguments[0].values.FindAll(x => x.name != "Store");
					chooseSpecificArgument(choice, 0, runTo[rand.Next(0, runTo.Count)].name);
					return choice;
				}
			}

			//If no materials, and not enough money, look for money.
			if (!hasMaterials && !hasEnoughMoney && canLookForMoney)
			{
				//If not at fields, run there; else look for money
				if (location != "Fields")
				{
					if (canDoVerb(choices, "Run"))
					{
						choice = getVerb(choices, "Run");
						chooseSpecificArgument(choice, 0, "Fields");
						return choice;
					}
				}
				else
					if (canDoVerb(choices, "LookForMoney"))
						return getVerb(choices, "LookForMoney");
			}

			//If no materials, go to store and buy material
			if (!hasMaterials && hasEnoughMoney)
			{
				//If not at store, run there; else, buy supplies
				if (location != "Store")
				{
					if (canDoVerb(choices, "Run"))
					{
						choice = getVerb(choices, "Run");
						chooseSpecificArgument(choice, 0, "Store");
						return choice;
					}
				}
				else
				{
					if (canDoVerb(choices, "Buy"))
					{
						choice = getVerb(choices, "Buy");
						List<Entity> materials = choice.arguments[0].values.FindAll(x => x.tags.Exists(y => y.name == "supplies"));
						chooseSpecificArgument(choice, 0, materials[rand.Next(0, materials.Count)].name);
						return choice;
					}
				}
			}

			//If have materials, go to empty land; if on empty land, build house
			if (hasMaterials)
			{
				//Find empty land
				List<string> emptyLandNames = new List<string>();
				foreach (Entity e in sw.entities.FindAll(x => x.tags.Exists(y => y.name == "home") && !x.relationships.Exists(y => y.name == "hasHouse")))
					emptyLandNames.Add(e.name);

				//On that land? Build house; else, go to random empty plot.
				if (emptyLandNames.Exists(x => x == location))
				{
					if (canDoVerb(choices, "BuildHouse"))
					{
						timeToEnterHouse = true;
						choice = getVerb(choices, "BuildHouse");
						chooseRandomArguments(choice);
						return choice;
					}
				}
				else
				{
					if (canDoVerb(choices, "Run") && emptyLandNames.Count != 0)
					{
						choice = getVerb(choices, "Run");
						chooseSpecificArgument(choice, 0, emptyLandNames[rand.Next(0, emptyLandNames.Count)]);
						return choice;
					}
				}
			}
		}

		//Random if nothing else
		//If there are more options than waiting, don't wait
		if (choices.Count > 1)
			choices.RemoveAll(x => x.name == "Wait");
		
		choice = choices[rand.Next(0, choices.Count)];
		chooseRandomArguments(choice);

		return choice;
	}
}