using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class Engine : MonoBehaviour
{
    /* Variables */
    public StoryWorld storyWorld;
    public List<Page> output;
	public List<Verb> currentUserChoices;
    public bool ended;
	public bool userFoundNoAction;
	public int amountOfWaits;
	public bool standStill;
    public System.Random rand;
	public Entity userEntity;
	public List<Entity> agents;

    /* Functions */
    public Engine(StoryWorld _storyWorld)
    {
        storyWorld = _storyWorld;
		output = new List<Page>();
		userFoundNoAction = false;
		amountOfWaits = 0;
		standStill = false;
        ended = false;
		rand = new System.Random();
		userEntity = null;
		agents = null;
	}

	public Page getPagePreview(Verb v, string a1, string a2, string a3, string a4)
	{
		Verb tempV = new Verb("");
		v.copyTo(tempV);

		switch (tempV.arguments.Count)
		{
		case 0:
			break;
		case 1:
			tempV.replaceWith(tempV.arguments[0].name, a1);
			break;
		case 2:
			tempV.replaceWith(tempV.arguments[0].name, a1);
			tempV.replaceWith(tempV.arguments[1].name, a2);
			break;
		case 3:
			tempV.replaceWith(tempV.arguments[0].name, a1);
			tempV.replaceWith(tempV.arguments[1].name, a2);
			tempV.replaceWith(tempV.arguments[2].name, a3);
			break;
		case 4:
			tempV.replaceWith(tempV.arguments[0].name, a1);
			tempV.replaceWith(tempV.arguments[1].name, a2);
			tempV.replaceWith(tempV.arguments[2].name, a3);
			tempV.replaceWith(tempV.arguments[3].name, a4);
			break;
		}
		
		return generateCasePages(chooseCase(tempV), tempV, userEntity)[0];
	}

	//Shuffle the list of agents for "fair" scheduling
	public List<Entity> shuffleEntities(List<Entity> EL)
	{
		List<Entity> newEL = new List<Entity>();
		
		while (EL.Count > 0)
		{
			int e = rand.Next(0, EL.Count);
			newEL.Add(EL[e]);
			EL.RemoveAt(e);
		}
		
		return newEL;
	}

	//The loop which goes through each agent and allows them to interact with the story world
	public void handleAI()
	{
		agents = shuffleEntities(agents);
		amountOfWaits = 0;

		foreach (Entity e in agents)
		{
			Verb decision = e.agent.decide(generatePossibleVerbs(e));
			Case agentCase = chooseCase(decision);

			if (shouldShow(decision))
				output.AddRange(executeCase(agentCase, decision, e));
			else
				executeCase(agentCase, decision, e);
				      
			ended = checkEndConditions();
			if (ended)
				break;

			if (decision.name == "Wait")
				amountOfWaits++;
		}

		if (amountOfWaits == agents.Count() && userFoundNoAction && !ended)
			standStill = true;
	}

	//NEEDS TO BE REWRITTEN
	//The loop of action; each agent takes turns acting upon the world
	public void takeInputAndProcess(Verb choice)
	{
		output.RemoveAt(output.Count - 1);

		if (!ended)
		{
			//Handle user action
			Case userCase = chooseCase(choice);
			if (shouldShow(choice))
				output.AddRange(executeCase(userCase, choice, userEntity));
			else
				executeCase(userCase, choice, userEntity);

			//End at user action if end met
			ended = checkEndConditions();

			if (!ended)
			{
				//Handle AI
				handleAI();

				if (!ended)
				{
					//Now wait for input again
					currentUserChoices = generatePossibleVerbs(userEntity);
					
					while (currentUserChoices.Count <= 1 && !ended && !standStill)
					{
						userFoundNoAction = true;
						//Handle AI
						handleAI();

						if (!ended)
						{
							//Now wait for input again
							currentUserChoices = generatePossibleVerbs(userEntity);
						}
					}

					//Remove wait option
					currentUserChoices.RemoveAll(x => x.name == "Wait");
					userFoundNoAction = false;

					if (standStill)
					{
						Page p = new Page("StandstillError");
						p.drawList.Add(new DrawInstruction(true, "", "", "ERROR: Story has hit a stand still. No entity has a possible action beyond waiting."));
						output.Add(p);
					}
				}
			}
		}
		else
		{
			//Handle ended state and story restart.
		}
	}

	//Process story when no user input is required
	public void processStory()
	{
		while (!ended && !standStill)
			handleAI();

		if (standStill)
		{
			Page p = new Page("StandstillError");
			p.drawList.Add(new DrawInstruction(true, "", "", "ERROR: Story has hit a stand still. No entity has a possible action beyond waiting."));
			output.Add(p);
		}
	}

	//Start the story engine
	public void init()
	{
		output.AddRange(storyWorld.beginning);

		agents = storyWorld.entities.FindAll(x => x.agent != null);
		userEntity = agents.Find(x => x.agent.name == "user");
		agents.RemoveAll(x => x.agent.name == "user");

		//In the case the user can interact
		if (userEntity != null)
		{
			currentUserChoices = generatePossibleVerbs(userEntity);

			if (currentUserChoices.Count == 1)
				takeInputAndProcess(currentUserChoices[0]);

			//Remove wait option
			currentUserChoices.RemoveAll(x => x.name == "Wait");
		}
		else
			processStory();
	}

	//SHOULD BE ABLE TO SURVIVE LNSCRIPT CHANGES
    //If any of the story world's endings' conditions are satisfied, execute it.
    public bool checkEndConditions()
    {
		foreach(Ending e in storyWorld.endings)
		{
			bool allSatisfied = true;

			foreach(Condition c in e.conditions)
			{
				if (!c.evaluate(storyWorld, null))
				{
					allSatisfied = false;			
					break;
				}
			}

			if (allSatisfied)
			{
				output.AddRange(e.pages);
				return true;
			}
		}

		return false;
    }
	
	//SHOULD BE ABLE TO SURVIVE LNSCRIPT CHANGES
	//Given the parameters, arguments, verb context, and newly created DVT root, maps out the DVT root and returns whether there is a possible solution (null no, else yes)
	public List<DynamicVerbTreeNode> generateDVT(List<Argument> parameters, List<Entity> arguments, Verb vContext, DynamicVerbTreeNode r)
	{
		if (arguments.Count < parameters.Count)
		{
			List<DynamicVerbTreeNode> children = new List<DynamicVerbTreeNode>();

			foreach (Entity e in parameters[arguments.Count].values)
			{
				DynamicVerbTreeNode me = new DynamicVerbTreeNode(e);
				List<Entity> tempA = new List<Entity>();
				foreach (Entity a in arguments)
					tempA.Add(a);
				tempA.Add(e);
				me.children = generateDVT(parameters, tempA, vContext, r);
				if (me.children != null)
					children.Add(me);
			}

			if (arguments.Count != 0)
			{
				if (children.Count > 0)
					return children;
				else
					return null;
			}
			else
			{
				r.children = children;

				if (r.children.Count > 0)
					return children;
				else
					return null;
			}
		}
		else if (arguments.Count == parameters.Count)
		{
			if (parameters.Count > 0)
			{
				List<DynamicVerbTreeNode> bottom = new List<DynamicVerbTreeNode>();

				foreach (Entity e in parameters[arguments.Count - 1].values)
				{
					//Check if any case is valid in this arrangement
					bool anyCaseValid = false;
					
					foreach (Case c in vContext.cases)
					{
						bool invalidCase = false;
						List<Condition> caseConditions = new List<Condition>();
						caseConditions.AddRange(vContext.preconditions);
						caseConditions.AddRange(c.conditions);

						if (caseConditions.Count == 0)
							anyCaseValid = true;

						foreach (Condition k in caseConditions)
						{
							Condition tempK = new Condition();
							k.copyTo(tempK);
							for (int i = 1; i <= arguments.Count; i++)
								tempK.replaceWith(parameters[i-1].name, arguments[i-1].name);

							if (!tempK.evaluate(storyWorld, vContext))
							{
								invalidCase = true;
								break;
							}
						}
						
						if (!invalidCase)
						{
							anyCaseValid = true;
							break;			
						}
					}
					
					if (anyCaseValid)
						bottom.Add(new DynamicVerbTreeNode(e));
				}

				if (bottom.Count > 0)
					return bottom;
				else
					return null;
			}
			else
			{
				//Check if any case is valid in this arrangement
				bool anyCaseValid = false;
				
				foreach (Case c in vContext.cases)
				{
					bool invalidCase = false;
					List<Condition> caseConditions = new List<Condition>();
					caseConditions.AddRange(vContext.preconditions);
					caseConditions.AddRange(c.conditions);

					if (caseConditions.Count == 0)
						anyCaseValid = true;
					
					foreach (Condition k in caseConditions)
					{
						if (!k.evaluate(storyWorld, vContext))
						{
							invalidCase = true;
							break;
						}
					}
					
					if (!invalidCase)
					{
						anyCaseValid = true;
						break;			
					}
				}

				if (anyCaseValid)
					return new List<DynamicVerbTreeNode>();
				else
					return null;
			}
		}
		else
			return null;
	}

	//SHOULD BE ABLE TO SURVIVE LNSCRIPT CHANGES
	public List<Verb> generatePossibleVerbs(Entity e)
	{
		//Get all verbs
		List<Verb> list = new List<Verb>();

		foreach (Verb v in storyWorld.verbs)
		{
			Verb tempV = new Verb("");
			v.copyTo(tempV);
			tempV.replaceWith("?me", e.name);
			bool invalid = false;

			//Fill out variable lists
			foreach (Variable x in tempV.variables)
			{
				foreach (Entity y in storyWorld.entities)
				{
					bool allSatisfied = true;

					foreach (Condition c in x.conditions)
					{
						Condition tempC = new Condition();
						c.copyTo(tempC);
						tempC.replaceWith(x.name, y.name);

						if (!tempC.evaluate(storyWorld, tempV))
						{
							allSatisfied = false;
							break;
						}
					}

					if (allSatisfied)
						x.values.Add(y);
				}
			}

			//Fill out argument lists and check if they are null
			foreach (Argument a in tempV.arguments)
			{
				foreach (Entity y in storyWorld.entities)
				{
					bool allSatisfied = true;

					foreach (Condition c in a.conditions)
					{
						Condition tempC = new Condition();
						c.copyTo(tempC);
						tempC.replaceWith(a.name, y.name);

						if (!tempC.evaluate(storyWorld, tempV))
						{
							allSatisfied = false;
							break;
						}
					}

					if (allSatisfied)
						a.values.Add(y);
				}

				if (a.values.Count <= 0)
				{
					invalid = true;
					break;
				}
			}

			if (invalid)
				break;

			//Check all path combinations of arguments to see if at least one case is valid 
			tempV.root = new DynamicVerbTreeNode(e);
			if (generateDVT(tempV.arguments, new List<Entity>(), tempV, tempV.root) != null)
				list.Add(tempV);
		}

		return list;
	}

	//SHOULD BE ABLE TO SURVIVE LNSCRIPT CHANGES
	//Given a verb, find the correct case to execute
	public Case chooseCase(Verb v)
	{
		foreach (Case c in v.cases)
		{
			bool firstSolution = true;

			foreach (Condition k in c.conditions)
			{
				if (!k.evaluate(storyWorld, v)) 
				{
					firstSolution = false;
					break;
				}
			}

			if (firstSolution) return c;
		}

		return null;
	}

	//NEEDS A FEW TWEAKS
	//Crazy slow, needs a better solution someday
	public List<Page> generateCasePages(Case c, Verb context, Entity me)
	{
		List<Page> casePages = new List<Page>();

		foreach (Page p in c.pages)
		{
			Page temp = new Page("");
			p.copyTo(temp);

			foreach (DrawInstruction d in temp.drawList)
			{
				if (!d.isText)
				{
					if (d.entity[0] == '?')
					{
						Variable var = context.variables.Find(x => x.name == d.entity);
						d.entity = var.values[rand.Next(0, var.values.Count)].name;
					}
				}

				if (d.isText)
				{
					foreach (Variable var in context.variables)
					{
						Entity ent = var.values[rand.Next(0, var.values.Count)];
				
						foreach (LNString ln in ent.strings)
						{
							string str = (var.name + "." + ln.name);
							d.text = d.text.Replace(str, ln.text);
						}
						
						foreach (Number num in ent.numbers)
						{
							string str = (var.name + "." + num.name);
							d.text = d.text.Replace(str, num.value.ToString());
						}
					}

					foreach (Argument a in context.arguments)
					{
						Entity ent = a.values.Find(x => x.name == a.choice);
						
						foreach (LNString ln in ent.strings)
						{
							string str = (a.name + "." + ln.name);
							d.text = d.text.Replace(str, ln.text);
						}
						
						foreach (Number num in ent.numbers)
						{
							string str = (a.name + "." + num.name);
							d.text = d.text.Replace(str, num.value.ToString());
						}
					}

					foreach (Entity e in storyWorld.entities)
					{
						if (e.name == me.name)
						{
							foreach (LNString ln in e.strings)
							{
								string str = ("?me." + ln.name);
								d.text = d.text.Replace(str, ln.text);
							}
							
							foreach (Number num in e.numbers)
							{
								string str = ("?me." + num.name);
								d.text = d.text.Replace(str, num.value.ToString());
							}
						}
						else
						{
							foreach (LNString ln in e.strings)
							{
								string str = (e.name + "." + ln.name);
								d.text = d.text.Replace(str, ln.text);
							}
							
							foreach (Number num in e.numbers)
							{
								string str = (e.name + "." + num.name);
								d.text = d.text.Replace(str, num.value.ToString());
							}
						}
					}
				}
			}

			casePages.Add(temp);
		}

		return casePages;
	}

	//SHOULD BE ABLE TO SURVIVE LNSCRIPT CHANGES
	//Has side-effects...
	public List<Page> executeCase(Case c, Verb context, Entity me)
	{
		if (c != null)
		{
			foreach (Operator o in c.operators)
				o.operate(storyWorld, context);
			
			return generateCasePages(c, context, me);
		}
		else
			return null;
	}

	//SHOULD BE ABLE TO SURVIVE LNSCRIPT CHANGES
	public bool shouldShow(Verb v)
	{
		if (v.discriminators.Count == 0)
			return true;

		bool oneWorks = false;
		foreach (Discriminator d in v.discriminators)
		{
			if (d.neverShow)
				return false;

			bool show = true;
			foreach (Condition c in d.conditions)
			{
				if (!c.evaluate(storyWorld, v))
				{
					show = false;
					break;
				}
			}
			if (show)
			{
				oneWorks = true;
				break;
			}
		}

		if (oneWorks)
			return true;
		else
			return false;
	}
}
/*
	//NO LONGER NEEDED; REPLACED WITH AGENTS. SHOULD SAVE THOUGH, JUST IN CASE
    //Given a DVT and a path through that DVT, recursively find if the path goes through the entire DVT and fill in any if found, h should be 1
    public List<string> checkDVTPath(DynamicVerbTreeNode r, List<string> path, int h)
    {
		if (h != path.Count)
        {
            if (path[h] != "any")
            {
                DynamicVerbTreeNode next = r.children.Find(x => x.data.name == path[h]);

                if (next != null)
                {
                    path = checkDVTPath(next, path, h + 1);
                    return path;
                }
                else
                    return null;
            }
            else
            {
                List<int> alreadyVisited = new List<int>();
                List<string> newPath = new List<string>();

                for (int i = 0; i < r.children.Count; i++)
                    alreadyVisited.Add(i);

                while (alreadyVisited.Count != 0)
                {
                    int randIndex = rand.Next(0, alreadyVisited.Count);
                    for (int i = 0; i < path.Count; i++)
                        newPath.Add(path[i]);

                    newPath = checkDVTPath(r.children[alreadyVisited[randIndex]], newPath, h + 1);
                    
                    if (newPath != null && newPath.Count != 0)
                    {
                        newPath[h] = r.children[randIndex].data.name;
                        break;
                    }
                }

                if (newPath == null)
                    return null;
                else
                    return newPath;
            }
        }
        else
            return path;
    }

	//NO LONGER NEEDED; REPLACED WITH AGENTS. SHOULD SAVE THOUGH, JUST IN CASE
    //Returns a list of all possible paths through a DVT, allPaths and currentPath should be new path DS's, h should be 1, hMax should be getHeight(), has side effects
    public void findAllPossibleDVTPaths(DynamicVerbTreeNode r, List<List<string>> allPaths, List<string> currentPath, int h, int hMax)
    {
        if (h == hMax)
            allPaths.Add(currentPath);
        else
        {
            for (int i = 0; i < r.children.Count; i++)
            {
                List<string> branch = new List<string>();
                for (int j = 0; j < currentPath.Count; j++)
                    branch.Add(currentPath[j]);
                branch.Add(r.children[i].data.name);
                findAllPossibleDVTPaths(r.children[i], allPaths, branch, h + 1, hMax);
            }
        }
    }

	//NO LONGER NEEDED; REPLACED WITH AGENTS.
    public void AIAct(Entity e)
    {
		Verb choice = null;
        List<Verb> possibleActions = generatePossibleVerbs(e);

        //Any obligations?
        if (e.obligations.Count > 0)
        {
            List<Verb> PAO = new List<Verb>();
            for (int i = 0; i < possibleActions.Count; i++ )
                PAO.Add(possibleActions[i]);

            List<Obligation> possibleObligations = new List<Obligation>();
            for (int i = 0; i < PAO.Count; i++)
                possibleObligations.AddRange(e.obligations.FindAll(x => x.verb == PAO[i].name));
            
            if (possibleObligations.Count > 0)
            {
                List<int> indexes = new List<int>();
                for (int i = 0; i < possibleObligations.Count; i++)
                    indexes.Add(i);
                int index = rand.Next(0, indexes.Count);
                Verb tempV = PAO.Find(x => x.name == possibleObligations[indexes[index]].verb);

				if (tempV.arguments.Count > 0)
                {
                    List<string> path = checkDVTPath(tempV.root, possibleObligations[indexes[index]].arguments, 0);

                    //Check that the path constraints match the possiblities
					while (path == null && tempV.arguments.Count > 1)
                    {
                        indexes.RemoveAt(index);
                        if (indexes.Count == 0)
                            break;
                        index = rand.Next(0, indexes.Count);
                        tempV = PAO.Find(x => x.name == possibleObligations[indexes[index]].verb);
                        if (tempV.variables.Count > 0)
                            path = checkDVTPath(tempV.root, possibleObligations[indexes[index]].arguments, 0);
                    }

                    if (indexes.Count != 0 && tempV.arguments.Count > 0)
                        tempV.applyPath(path);
                }

                if (indexes.Count != 0)
                    choice = tempV;
            }        
        }

        //Any goals?
        if (choice == null && e.goals.Count > 0)
        {
            List<Verb> PAG = new List<Verb>();
            List<Verb> PAGJr = new List<Verb>();
            for (int i = 0; i < possibleActions.Count; i++)
                PAG.Add(possibleActions[i]);

            List<Verb> possibleGoalVerbs = new List<Verb>();

            //Enumerate all dynamic verbs and add to the list
            for (int i = 0; i < PAG.Count; i++)
            {
                if (PAG[i].variables.Count > 1)
                {
                    List<List<string>> pp = new List<List<string>>();
                    findAllPossibleDVTPaths(PAG[i].root, pp, new List<string>(), 0, PAG[i].root.getHeight(0));
                    
                    for (int j = 0; j < pp.Count; j++ )
                    {
                        pp[j].Insert(0, e.name);
                        Verb newV = new Verb("");
                        PAG[i].copyTo(newV);
                        newV.applyPath(pp[j]);
                        PAGJr.Add(newV);
                    }
                }
            }

            PAG.AddRange(PAGJr);

            for (int i = 0; i < e.goals.Count; i++)
                possibleGoalVerbs.AddRange(PAG.FindAll(x => x.cases.Find(z => z.operators.Find(y => e.goals[i].matchWith(y)) != null) != null));

            if (possibleGoalVerbs.Count > 0)
                choice = possibleGoalVerbs[rand.Next(0, possibleGoalVerbs.Count)];
        }

        //Any behaviors?
        if (choice == null && e.behaviors.Count > 0)
        {
            List<Behavior> possibleBehaviors = new List<Behavior>();
            for (int i = 0; i < possibleActions.Count; i++)
                possibleBehaviors.AddRange(e.behaviors.FindAll(x => x.verb == possibleActions[i].name));

            //Clear out those behaviors that don't match their paths
            for (int i = 0; i < possibleBehaviors.Count; i++ )
            {
                if (possibleBehaviors[i].arguments.Count > 0)
                {
                    Verb tempV = possibleActions.Find(x => x.name == possibleBehaviors[i].name);

                    if (tempV != null)
                        if (checkDVTPath(tempV.root, possibleBehaviors[i].arguments, 0) == null)
                            possibleBehaviors.RemoveAt(i);
                }
            }

            if (possibleBehaviors.Count > 0)
            {
                List<Behavior> ballSpinner = new List<Behavior>();

                for (int i = 0; i < possibleBehaviors.Count; i++)
                {
                    for (int j = 0; j < possibleBehaviors[i].chance; j++)
                        ballSpinner.Add(possibleBehaviors[i]);
                }
                
                int index = rand.Next(0, ballSpinner.Count);
                choice = possibleActions.Find(x => x.name == ballSpinner[index].verb);

				if (ballSpinner[index].arguments.Count > 0)
					choice.applyPath(checkDVTPath(choice.root, ballSpinner[index].arguments, 0));
            }
		}

		if (choice == null)
			choice = storyWorld.verbs.Find(x => x.name == "Wait");

        //Execute verbs
        if (choice != null)
        {
			Case AICase = chooseCase(choice);
			if (shouldShow(choice))
				output.Add(executeCase(AICase, choice, e));
			else
				executeCase(AICase, choice, e);

			if (choice.name == "Wait")
				amountOfWaits++;
        }

        ended = checkEndConditions();
    }
}*/
