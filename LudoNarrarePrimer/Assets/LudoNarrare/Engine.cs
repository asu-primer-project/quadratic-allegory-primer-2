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
    public List<Verb> currentAgentChoices;
    public bool ended;
	public bool agentFoundNoAction;
	public int amountOfWaits;
	public bool standStill;
    public System.Random rand;

    /* Functions */
    public Engine(StoryWorld _storyWorld)
    {
        storyWorld = _storyWorld;
		output = new List<Page>();
        currentAgentChoices = null;
		agentFoundNoAction = false;
		amountOfWaits = 0;
		standStill = false;
        ended = false;
		rand = new System.Random();
    }

	public void createBeginning()
	{
		output.AddRange(storyWorld.beginning);
		output.Add(storyWorld.input);
	}

	public void takeInputAndProcess(Verb choice)
	{
		output.RemoveAt(output.Count - 1);

		if (!ended)
		{
			//Handle user action
			Case userCase = chooseCase(choice);
			if (shouldShow(choice))
				output.Add(executeCase(userCase, choice, storyWorld.entities.Find(x => x.name == storyWorld.userEntity)));
			else
				executeCase(userCase, choice, storyWorld.entities.Find(x => x.name == storyWorld.userEntity));

			//End at user action if end met
			ended = checkEndConditions();

			if (!ended)
			{
				//Handle AI
				handleAI();

				if (!ended)
				{
					//Now wait for input again
					currentUserChoices = generatePossibleVerbs(storyWorld.entities.Find(x => x.name == storyWorld.userEntity));
					
					while (currentUserChoices.Count <= 1 && !ended && !standStill)
					{
						userFoundNoAction = true;
						//Handle AI
						handleAI();

						if (!ended)
						{
							//Now wait for input again
							currentUserChoices = generatePossibleVerbs(storyWorld.entities.Find(x => x.name == storyWorld.userEntity));
						}
					}

					userFoundNoAction = false;

					if (!ended && !standStill)
					{
						//Setup user choice interface
						output.Add(storyWorld.input);
					}

					if (standStill)
					{
						Page p = new Page("StandstillError");
						p.drawList.Add(new DrawInstruction(true, "", "", "ERROR: Story has hit a stand still. No entity has a possible action beyond waiting.", 0, 0));
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

	public void init()
	{
		//Make sure all paths match their verbs.
		for (int i = 0; i < storyWorld.entities.Count; i++ )
		{
			for (int j = 0; j < storyWorld.entities[i].obligations.Count; j++)
				checkAndFixPathSize(storyWorld.entities[i].obligations[j].arguments, storyWorld.verbs.Find(x => x.name == storyWorld.entities[i].obligations[j].verb));
			
			for (int j = 0; j < storyWorld.entities[i].behaviors.Count; j++)
				checkAndFixPathSize(storyWorld.entities[i].behaviors[j].arguments, storyWorld.verbs.Find(x => x.name == storyWorld.entities[i].behaviors[j].verb));
		}
		
		for (int i = 0; i < storyWorld.verbs.Count; i++ )
		{
			for (int j = 0; j < storyWorld.verbs[i].cases.Count; j++ )
			{
				for (int k = 0; k < storyWorld.verbs[i].cases[j].operators.Count; k++ )
				{
					if (storyWorld.verbs[i].cases[j].operators[k].getType() == 4)
						checkAndFixPathSize(storyWorld.verbs[i].cases[j].operators[k].obligation.arguments, storyWorld.verbs.Find(x => x.name == storyWorld.verbs[i].cases[j].operators[k].obligation.verb));
					if (storyWorld.verbs[i].cases[j].operators[k].getType() == 6)
						checkAndFixPathSize(storyWorld.verbs[i].cases[j].operators[k].behavior.arguments, storyWorld.verbs.Find(x => x.name == storyWorld.verbs[i].cases[j].operators[k].behavior.verb));
				}
			}
		}

		createBeginning();
		currentAgentChoices = generatePossibleVerbs(storyWorld.entities.Find(x => x.agent == "User"));
		
		if (currentAgentChoices.Count == 1)
			takeInputAndProcess(currentAgentChoices[0]);
	}
	
	//If check if path is longer enough and if not make it so, has side effects
	public void checkAndFixPathSize(List<string> path, Verb v)
	{
		if (path == null)
			return;
		if (v == null)
			return;
		else if (v.arguments == null)
			return;
		if (path.Count == v.arguments.Count)
			return;
		while (path.Count > v.arguments.Count)
			path.RemoveAt(path.Count - 1);
		while (path.Count < v.arguments.Count)
			path.Add("any");
		return;
	}

    //If any of the story world's endings' conditions are satisfied, execute it.
    public bool checkEndConditions()
    {
		foreach(Ending e in storyWorld.endings)
		{
			bool allSatisfied = true;

			foreach(Condition c in e.conditions)
			{
				if (!checkCondition(c, null))
					allSatisfied = false;			
			}

			if (allSatisfied)
			{
				output.AddRange(e.pages);
				return true;
			}
		}

		return false;
    }

	public bool checkCondition(Condition c, Verb context)
	{
		int type = c.getType();

		//Is condition subject a variable or not?
		if (c.conditionSubject[0] == '?' && context != null)
		{
			Variable varL = context.variables.Find(x => x.name == c.conditionSubject);

			//Variable comparision?
			if (type == 7)
			{
				if (c.variableObject == "?null")
				{
					if (c.comparison == 2)
						return (varL.values.Count == 0) ? true : false;
					else if (c.comparison == 3)
						return (varL.values.Count == 0) ? false : true;
					else return false;
				}
				else
				{
					if (varL != null)
					{
						if (c.comparison == 2)
						{
							Variable varR = context.variables.Find(x => x.name == c.variableObject);

							if (varL.values.Count == varR.values.Count)
							{
								foreach (Entity e in varL.values)
									if (varR.values.Find(x => x.name == e.name) == null) return false;

								return true;
							}
							else return false;
						}
						else if (c.comparison == 3)
						{
							Variable varR = context.variables.Find(x => x.name == c.variableObject);

							if (varL.values.Count == varR.values.Count)
							{
								foreach (Entity e in varL.values)
									if (varR.values.Find(x => x.name == e.name) == null) return true;
								
								return false;
							}
							else return true;
						}
						else return false;
					}
				}
			}

			//Universal or existential quantifier?
			if (varL != null)
			{
				if (c.allCS)
				{
					//Has comparison
					if (c.comparison == 0)
					{
						foreach (Entity e in varL.values)
						{
							switch (type)
							{
							case 0: if (e.tags.Find(x => x.name == c.tagRef) == null) return false; break;
							case 1: if (e.relationships.Find(x => x.name == c.relateRef) == null) return false; break;
							case 2: if (e.numbers.Find(x => x.name == c.numRef) == null) return false; break;
							case 3: if (e.strings.Find(x => x.name == c.stringRef) == null) return false; break;
							case 4: if (e.obligations.Find(x => x.name == c.obligationRef) == null) return false; break;
							case 5: if (e.goals.Find(x => x.name == c.goalRef) == null) return false; break;
							case 6: if (e.behaviors.Find(x => x.name == c.behaviorRef) == null) return false; break;
							default: return false;
							}
						}

						return true;
					}
					//Missing comparision
					else if (c.comparison == 1)
					{
						foreach (Entity e in varL.values)
						{
							switch (type)
							{
							case 0: if (e.tags.Find(x => x.name == c.tagRef) != null) return false; break;
							case 1: if (e.relationships.Find(x => x.name == c.relateRef) != null) return false; break;
							case 2: if (e.numbers.Find(x => x.name == c.numRef) != null) return false; break;
							case 3: if (e.strings.Find(x => x.name == c.stringRef) != null) return false; break;
							case 4: if (e.obligations.Find(x => x.name == c.obligationRef) != null) return false; break;
							case 5: if (e.goals.Find(x => x.name == c.goalRef) != null) return false; break;
							case 6: if (e.behaviors.Find(x => x.name == c.behaviorRef) != null) return false; break;
							default: return false;
							}
						}

						return true;
					}
					//Equal comparision
					else if (c.comparison == 2)
					{
						Entity cs2 = null;
						if (type == 8 || type == 9)
							cs2 = storyWorld.entities.Find(y => y.name == c.subject2);

						foreach (Entity e in varL.values)
						{
							switch (type)
							{
							case 1: 
								if (c.relateObject[0] == '?')
								{
									if (c.allRO)
									{
										Variable varR = context.variables.Find(x => x.name == c.relateObject);
										foreach (Entity n in varR.values)
											if (e.relationships.Find(x => x.name == c.relateRef && x.other == n.name) == null) return false;
									}
									else
									{
										bool hasOne = false;
										Variable varR = context.variables.Find(x => x.name == c.relateObject);
										foreach (Entity n in varR.values)
											if (e.relationships.Find(x => x.name == c.relateRef && x.other == n.name) != null) {hasOne = true; break;}

										if (!hasOne) return false;
									}
								}
								else if (e.relationships.Find(x => x.name == c.relateRef && x.other == c.relateObject) == null) return false; break; 
							case 2: if (e.numbers.Find(x => x.name == c.numRef && x.value == c.numCompare) == null) return false; break;
							case 3: if (e.strings.Find(x => x.name == c.stringRef && x.text == c.stringCompare) == null) return false; break;
							case 8: if (e.numbers.Find(x =>x.name == c.numRef && x.value == cs2.numbers.Find(y => y.name == c.numRef2).value) == null) return false; break;
							case 9: if (e.strings.Find(x =>x.name == c.stringRef && x.text == cs2.strings.Find(y => y.name == c.stringRef2).text) == null) return false; break;
							default: return false;
							}
						}

						return true;
					}
					//Unequal comparision
					else if (c.comparison == 3)
					{
						Entity cs2 = null;
						if (type == 8 || type == 9)
							cs2 = storyWorld.entities.Find(y => y.name == c.subject2);

						foreach (Entity e in varL.values)
						{
							switch (type)
							{
							case 1: 
								if (c.relateObject[0] == '?')
								{
									if (c.allRO)
									{
										Variable varR = context.variables.Find(x => x.name == c.relateObject);
										foreach (Entity n in varR.values)
											if (e.relationships.Find(x => x.name == c.relateRef && x.other == n.name) != null) return false;
									}
									else
									{
										bool hasAll = true;
										Variable varR = context.variables.Find(x => x.name == c.relateObject);
										foreach (Entity n in varR.values)
											if (e.relationships.Find(x => x.name == c.relateRef && x.other == n.name) == null) {hasAll = false; break;}
										
										if (hasAll) return false;
									}
								}
								else if (e.relationships.Find(x => x.name == c.relateRef && x.other == c.relateObject) != null) return false; break;
							case 2: if (e.numbers.Find(x => x.name == c.numRef && x.value == c.numCompare) != null) return false; break;
							case 3: if (e.strings.Find(x => x.name == c.stringRef && x.text == c.stringCompare) != null) return false; break;
							case 8: if (e.numbers.Find(x =>x.name == c.numRef && x.value == cs2.numbers.Find(y => y.name == c.numRef2).value) != null) return false; break;
							case 9: if (e.strings.Find(x =>x.name == c.stringRef && x.text == cs2.strings.Find(y => y.name == c.stringRef2).text) != null) return false; break;
							default: return false;
							}
						}
						
						return true;					
					}
					//Less than
					else if (c.comparison == 4)
					{
						if (type == 2)
						{
							foreach (Entity e in varL.values)
								if (e.numbers.Find(x => x.name == c.numRef && x.value < c.numCompare) == null) return false;
						}
						else return true;
					}
					//Greater than
					else if (c.comparison == 5)
					{
						if (type == 2)
						{
							foreach (Entity e in varL.values)
								if (e.numbers.Find(x => x.name == c.numRef && x.value > c.numCompare) == null) return false;
						}
						else return true;
					}
					//Less than/equal to
					else if (c.comparison == 6)
					{
						if (type == 2)
						{
							foreach (Entity e in varL.values)
								if (e.numbers.Find(x => x.name == c.numRef && x.value <= c.numCompare) == null) return false;
						}
						else return true;
					}
					//Greater than/equal to
					else if (c.comparison == 7)
					{
						if (type == 2)
						{
							foreach (Entity e in varL.values)
								if (e.numbers.Find(x => x.name == c.numRef && x.value >= c.numCompare) == null) return false;
						}
						else return true;
					}
					else return false;
				}
				else
				{
					//Has comparison
					if (c.comparison == 0)
					{
						foreach (Entity e in varL.values)
						{
							switch (type)
							{
							case 0: if (e.tags.Find(x => x.name == c.tagRef) != null) return true; break;
							case 1: if (e.relationships.Find(x => x.name == c.relateRef) != null) return true; break;
							case 2: if (e.numbers.Find(x => x.name == c.numRef) != null) return true; break;
							case 3: if (e.strings.Find(x => x.name == c.stringRef) != null) return true; break;
							case 4: if (e.obligations.Find(x => x.name == c.obligationRef) != null) return true; break;
							case 5: if (e.goals.Find(x => x.name == c.goalRef) != null) return true; break;
							case 6: if (e.behaviors.Find(x => x.name == c.behaviorRef) != null) return true; break;
							default: return false;
							}
						}
						
						return false;
					}
					//Missing comparision
					else if (c.comparison == 1)
					{
						foreach (Entity e in varL.values)
						{
							switch (type)
							{
							case 0: if (e.tags.Find(x => x.name == c.tagRef) == null) return true; break;
							case 1: if (e.relationships.Find(x => x.name == c.relateRef) == null) return true; break;
							case 2: if (e.numbers.Find(x => x.name == c.numRef) == null) return true; break;
							case 3: if (e.strings.Find(x => x.name == c.stringRef) == null) return true; break;
							case 4: if (e.obligations.Find(x => x.name == c.obligationRef) == null) return true; break;
							case 5: if (e.goals.Find(x => x.name == c.goalRef) == null) return true; break;
							case 6: if (e.behaviors.Find(x => x.name == c.behaviorRef) == null) return true; break;
							default: return false;
							}
						}
						
						return false;
					}
					//Equal comparision
					else if (c.comparison == 2)
					{
						Entity cs2 = null;
						if (type == 8 || type == 9)
							cs2 = storyWorld.entities.Find(y => y.name == c.subject2);

						foreach (Entity e in varL.values)
						{
							switch (type)
							{
							case 1: 
								if (c.relateObject[0] == '?')
								{
									if (c.allRO)
									{
										Variable varR = context.variables.Find(x => x.name == c.relateObject);
										foreach (Entity n in varR.values)
											if (e.relationships.Find(x => x.name == c.relateRef && x.other == n.name) == null) return false;
									}
									else
									{
										bool hasOne = false;
										Variable varR = context.variables.Find(x => x.name == c.relateObject);
										foreach (Entity n in varR.values)
										if (e.relationships.Find(x => x.name == c.relateRef && x.other == n.name) != null) {hasOne = true; break;}
										
										if (!hasOne) return false;
									}
								}
								else if (e.relationships.Find(x => x.name == c.relateRef && x.other == c.relateObject) != null) return true; break;
							case 2: if (e.numbers.Find(x => x.name == c.numRef && x.value == c.numCompare) != null) return true; break;
							case 3: if (e.strings.Find(x => x.name == c.stringRef && x.text == c.stringCompare) != null) return true; break;
							case 8: if (e.numbers.Find(x =>x.name == c.numRef && x.value == cs2.numbers.Find(y => y.name == c.numRef2).value) != null) return true; break;
							case 9: if (e.strings.Find(x =>x.name == c.stringRef && x.text == cs2.strings.Find(y => y.name == c.stringRef2).text) != null) return true; break;
							default: return false;
							}
						}
						
						return false;
					}
					//Unequal comparision
					else if (c.comparison == 3)
					{
						Entity cs2 = null;
						if (type == 8 || type == 9)
							cs2 = storyWorld.entities.Find(y => y.name == c.subject2);

						foreach (Entity e in varL.values)
						{
							switch (type)
							{
							case 1: 
								if (c.relateObject[0] == '?')
								{
									if (c.allRO)
									{
										Variable varR = context.variables.Find(x => x.name == c.relateObject);
										foreach (Entity n in varR.values)
											if (e.relationships.Find(x => x.name == c.relateRef && x.other == n.name) != null) return false;
									}
									else
									{
										bool hasAll = true;
										Variable varR = context.variables.Find(x => x.name == c.relateObject);
										foreach (Entity n in varR.values)
										if (e.relationships.Find(x => x.name == c.relateRef && x.other == n.name) == null) {hasAll = false; break;}
										
										if (hasAll) return false;
									}
								}
								else if (e.relationships.Find(x => x.name == c.relateRef && x.other == c.relateObject) == null) return true; break;
							case 2: if (e.numbers.Find(x => x.name == c.numRef && x.value == c.numCompare) == null) return true; break;
							case 3: if (e.strings.Find(x => x.name == c.stringRef && x.text == c.stringCompare) == null) return true; break;
							case 8: if (e.numbers.Find(x =>x.name == c.numRef && x.value == cs2.numbers.Find(y => y.name == c.numRef2).value) == null) return true; break;
							case 9: if (e.strings.Find(x =>x.name == c.stringRef && x.text == cs2.strings.Find(y => y.name == c.stringRef2).text) == null) return true; break;
							default: return false;
							}
						}
						
						return false;					
					}
					//Less than
					else if (c.comparison == 4)
					{
						if (type == 2)
						{
							foreach (Entity e in varL.values)
								if (e.numbers.Find(x => x.name == c.numRef && x.value < c.numCompare) != null) return true;
						}
						else return false;
					}
					//Greater than
					else if (c.comparison == 5)
					{
						if (type == 2)
						{
							foreach (Entity e in varL.values)
								if (e.numbers.Find(x => x.name == c.numRef && x.value > c.numCompare) != null) return true;
						}
						else return false;
					}
					//Less than/equal to
					else if (c.comparison == 6)
					{
						if (type == 2)
						{
							foreach (Entity e in varL.values)
								if (e.numbers.Find(x => x.name == c.numRef && x.value <= c.numCompare) != null) return true;
						}
						else return false;
					}
					//Greater than/equal to
					else if (c.comparison == 7)
					{
						if (type == 2)
						{
							foreach (Entity e in varL.values)
								if (e.numbers.Find(x => x.name == c.numRef && x.value >= c.numCompare) != null) return true;
						}
						else return false;
					}
					else return false;
				}
			}
		}
		else
		{
			Entity cs = storyWorld.entities.Find(x => x.name == c.conditionSubject);
		
			//Has comparision
			if (c.comparison == 0)
			{
				switch (type)
				{
				case 0: return (cs.tags.Find(x => x.name == c.tagRef) != null) ? true : false;
				case 1: return (cs.relationships.Find(x => x.name == c.relateRef) != null) ? true : false;
				case 2: return (cs.numbers.Find(x => x.name == c.numRef) != null) ? true : false;
				case 3: return (cs.strings.Find(x => x.name == c.stringRef) != null) ? true : false;
				case 4: return (cs.obligations.Find(x => x.name == c.obligationRef) != null) ? true : false;
				case 5: return (cs.goals.Find(x => x.name == c.goalRef) != null) ? true : false;
				case 6: return (cs.behaviors.Find(x => x.name == c.behaviorRef) != null) ? true : false;
				default: return false;
				}
			}
			//Missing comparision
			else if (c.comparison == 1)
			{
				switch (type)
				{
				case 0: return (cs.tags.Find(x => x.name == c.tagRef) == null) ? true : false;
				case 1: return (cs.relationships.Find(x => x.name == c.relateRef) == null) ? true : false;
				case 2: return (cs.numbers.Find(x => x.name == c.numRef) == null) ? true : false;
				case 3: return (cs.strings.Find(x => x.name == c.stringRef) == null) ? true : false;
				case 4: return (cs.obligations.Find(x => x.name == c.obligationRef) == null) ? true : false;
				case 5: return (cs.goals.Find(x => x.name == c.goalRef) == null) ? true : false;
				case 6: return (cs.behaviors.Find(x => x.name == c.behaviorRef) == null) ? true : false;
				default: return false;
				}
			}
			//Equal comparision
			else if (c.comparison == 2)
			{
				Entity cs2 = null;
				if (type == 8 || type == 9)
					cs2 = storyWorld.entities.Find(y => y.name == c.subject2);

				switch (type)
				{
				case 1: 
					if (c.relateObject[0] == '?' && context != null)
					{
						if (c.allRO)
						{
							Variable varR = context.variables.Find(x => x.name == c.relateObject);
							foreach (Entity e in varR.values)
								if (cs.relationships.Find(x => x.name == c.relateRef && x.other == e.name) == null) return false;

							return true;
						}
						else
						{
							Variable varR = context.variables.Find(x => x.name == c.relateObject);
							foreach (Entity e in varR.values)
								if (cs.relationships.Find(x => x.name == c.relateRef && x.other == e.name) != null) return true;

							return false;
						}
					}
					else return (cs.relationships.Find(x => x.name == c.relateRef && x.other == c.relateObject) != null) ? true : false;
				case 2: return (cs.numbers.Find(x => x.name == c.numRef && x.value == c.numCompare) != null) ? true : false;
				case 3: return (cs.strings.Find(x => x.name == c.stringRef && x.text == c.stringCompare) != null) ? true : false;
				case 8: return (cs.numbers.Find(x =>x.name == c.numRef && x.value == cs2.numbers.Find(y => y.name == c.numRef2).value) != null) ? true : false;
				case 9: return (cs.strings.Find(x =>x.name == c.stringRef && x.text == cs2.strings.Find(y => y.name == c.stringRef2).text) != null) ? true : false;
				default: return false;
				}
			}
			//Unequal comparision
			else if (c.comparison == 3)
			{
				Entity cs2 = null;
				if (type == 8 || type == 9)
					cs2 = storyWorld.entities.Find(y => y.name == c.subject2);

				switch (type)
				{
				case 1: 
					if (c.relateObject[0] == '?' && context != null)
					{
						if (c.allRO)
						{
							Variable varR = context.variables.Find(x => x.name == c.relateObject);
							foreach (Entity e in varR.values)
								if (cs.relationships.Find(x => x.name == c.relateRef && x.other == e.name) != null) return false;
							
							return true;
						}
						else
						{
							Variable varR = context.variables.Find(x => x.name == c.relateObject);
							foreach (Entity e in varR.values)
								if (cs.relationships.Find(x => x.name == c.relateRef && x.other == e.name) == null) return true;
							
							return false;
						}
					}
					else return (cs.relationships.Find(x => x.name == c.relateRef && x.other == c.relateObject) == null) ? true : false;
				case 2: return (cs.numbers.Find(x => x.name == c.numRef && x.value == c.numCompare) == null) ? true : false;
				case 3: return (cs.strings.Find(x => x.name == c.stringRef && x.text == c.stringCompare) == null) ? true : false;
				case 8: return (cs.numbers.Find(x =>x.name == c.numRef && x.value == cs2.numbers.Find(y => y.name == c.numRef2).value) == null) ? true : false;
				case 9: return (cs.strings.Find(x =>x.name == c.stringRef && x.text == cs2.strings.Find(y => y.name == c.stringRef2).text) == null) ? true : false;
				default: return false;
				}
			}
			//Less than
			else if (c.comparison == 4)
			{
				Entity cs2 = null;
				if (type == 8)
					cs2 = storyWorld.entities.Find(y => y.name == c.subject2);

				if (type == 2)
					return (cs.numbers.Find(x => x.name == c.numRef && x.value < c.numCompare) != null) ? true : false;
				else if (type == 8)
					return (cs.numbers.Find(x =>x.name == c.numRef && x.value < cs2.numbers.Find(y => y.name == c.numRef2).value) != null) ? true : false;
				else return false;
			}
			//Greater than
			else if (c.comparison == 5)
			{
				Entity cs2 = null;
				if (type == 8)
					cs2 = storyWorld.entities.Find(y => y.name == c.subject2);

				if (type == 2)
					return (cs.numbers.Find(x => x.name == c.numRef && x.value > c.numCompare) != null) ? true : false;
				else if (type == 8)
					return (cs.numbers.Find(x =>x.name == c.numRef && x.value > cs2.numbers.Find(y => y.name == c.numRef2).value) != null) ? true : false;
				else return false;
			}
			//Less than/equal to
			else if (c.comparison == 6)
			{
				Entity cs2 = null;
				if (type == 8)
					cs2 = storyWorld.entities.Find(y => y.name == c.subject2);

				if (type == 2)
					return (cs.numbers.Find(x => x.name == c.numRef && x.value <= c.numCompare) != null) ? true : false;
				else if (type == 8)
					return (cs.numbers.Find(x =>x.name == c.numRef && x.value <= cs2.numbers.Find(y => y.name == c.numRef2).value) != null) ? true : false;
				else return false;
			}
			//Greater than/equal to
			else if (c.comparison == 7)
			{
				Entity cs2 = null;
				if (type == 8)
					cs2 = storyWorld.entities.Find(y => y.name == c.subject2);

				if (type == 2)
					return (cs.numbers.Find(x => x.name == c.numRef && x.value >= c.numCompare) != null) ? true : false;
				else if (type == 8)
					return (cs.numbers.Find(x =>x.name == c.numRef && x.value >= cs2.numbers.Find(y => y.name == c.numRef2).value) != null) ? true : false;
				else return false;
			}
			else
				return false;
		}
		return false;
	}

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
							
							if (!checkCondition(tempK, vContext))
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
						if (!checkCondition(k, vContext))
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

						if (!checkCondition(tempC, tempV))
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

						if (!checkCondition(tempC, tempV))
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

	public void applyOperator(Operator o, Verb context)
	{
		List<Entity> subjects = new List<Entity>();

		if (o.operatorSubject[0] == '?')
			subjects.AddRange(context.variables.Find(x => x.name == o.operatorSubject).values);
		else
			subjects.Add(storyWorld.entities.Find(x => x.name == o.operatorSubject));

		if (subjects.Count > 0)
		{
			int type = o.getType();

			foreach (Entity e in subjects)
			{
				switch (o.op)
				{
				//Add
				case 0:
					switch (type)
					{
					case 0:
						if (e.tags.Find(x => x.name == o.tag.name) == null)
						{
							Tag temp = new Tag(o.tag.name);
							e.tags.Add(temp);
						}
						break;
					case 1:
						if (o.relationship.other[0] == '?')
						{
							List<Entity> var = context.variables.Find(x => x.name == o.relationship.other).values;

							foreach (Entity y in var)
							{
								if (e.relationships.Find(x => x.name == o.relationship.name && x.other == y.name) == null)
								{
									Relationship temp = new Relationship(o.relationship.name, y.name);
									e.relationships.Add(temp);
								}
							}
						}
						else
						{
							if (e.relationships.Find(x => x.name == o.relationship.name && x.other == o.relationship.other) == null)
							{
								Relationship temp = new Relationship(o.relationship.name, o.relationship.other);
								e.relationships.Add(temp);
							}
						}
						break;
					case 2:
						if (e.numbers.Find(x => x.name == o.num.name) == null)
						{
							Number temp = new Number(o.num.name, o.num.value);
							e.numbers.Add(temp);
						}
						break;
					case 3:
						if (e.strings.Find(x => x.name == o.lnString.name) == null)
						{
							LNString temp = new LNString(o.lnString.name, o.lnString.text);
							e.strings.Add(temp);
						}
						break;
					case 4:
						//Currently there is no support for obligation enumeration via variable lists; recursive solution needed.
						if (e.obligations.Find(x => x.name == o.obligation.name && x.verb == o.obligation.verb && x.arguments.SequenceEqual(o.obligation.arguments)) == null && e.name != storyWorld.userEntity)
						{
							Obligation temp = new Obligation("","");
							o.obligation.copyTo(temp);
							e.obligations.Add(temp);
						}
						break;
					case 5:
						if (e.goals.Find(x => x.name == o.goal.name) == null && e.name != storyWorld.userEntity)
						{
							Goal temp = new Goal("","",0);
							o.goal.copyTo(temp);
							e.goals.Add(temp);
						}
						break;
					case 6:
						//Currently there is no support for behavior enumeration via variable lists; recursive solution needed.
						if (e.behaviors.Find(x => x.name == o.behavior.name && x.verb == o.behavior.verb && x.chance == o.behavior.chance && x.arguments.SequenceEqual(o.behavior.arguments)) == null  && e.name != storyWorld.userEntity)
						{
							Behavior temp = new Behavior("","",0);
							o.behavior.copyTo(temp);
							e.behaviors.Add(temp);
						}
						break;
					default: return;
					}
					break;
				//Remove
				case 1:
					switch (type)
					{
					case 0:
						e.tags.RemoveAll(x => x.name == o.tag.name);
						break;
					case 1:
						if (o.relationship.other[0] == '?')
						{
							List<Entity> var = context.variables.Find(x => x.name == o.relationship.other).values;

							foreach(Entity y in var)
								e.relationships.RemoveAll(x => x.name == o.relationship.name && x.other == y.name);							
						}
						else
							e.relationships.RemoveAll(x => x.name == o.relationship.name && x.other == o.relationship.other);
						break;
					case 2:
						e.numbers.RemoveAll(x => x.name == o.num.name);
						break;
					case 3:
						e.strings.RemoveAll(x => x.name == o.lnString.name);
						break;
					case 4:
						e.obligations.RemoveAll(x => x.name == o.obligation.name);
						break;
					case 5:
						e.goals.RemoveAll(x => x.name == o.goal.name);
						break;
					case 6:
						e.behaviors.RemoveAll(x => x.name == o.behavior.name);
						break;
					default: return;
					}
					break;
				// +
				case 2:
					if (type == 2)
					{
						if (o.subject2 != "")
						{
							Number n = e.numbers.Find(x => x.name == o.num.name);

							int i = storyWorld.entities.Find(x => x.name == o.subject2).numbers.Find(y => y.name == o.numRef2).value;

							if (n != null)
								n.value = n.value + i;
							else
								e.numbers.Add(new Number(o.num.name, 0 + i));
						}
						else
						{
							Number n = e.numbers.Find(x => x.name == o.num.name);
							
							if (n != null)
								n.value = n.value + o.num.value;
							else
								e.numbers.Add(new Number(o.num.name, 0 + o.num.value));
						}
					}
					else return;
					break;
				// -
				case 3:
					if (type == 2)
					{
						if (o.subject2 != "")
						{
							Number n = e.numbers.Find(x => x.name == o.num.name);
							
							int i = storyWorld.entities.Find(x => x.name == o.subject2).numbers.Find(y => y.name == o.numRef2).value;
							
							if (n != null)
								n.value = n.value - i;
							else
								e.numbers.Add(new Number(o.num.name, 0 - i));
						}
						else
						{
							Number n = e.numbers.Find(x => x.name == o.num.name);
							
							if (n != null)
								n.value = n.value - o.num.value;
							else
								e.numbers.Add(new Number(o.num.name, 0 - o.num.value));
						}
					}
					else return;
					break;
				// *
				case 4:
					if (type == 2)
					{
						if (o.subject2 != "")
						{
							Number n = e.numbers.Find(x => x.name == o.num.name);
							
							int i = storyWorld.entities.Find(x => x.name == o.subject2).numbers.Find(y => y.name == o.numRef2).value;
							
							if (n != null)
								n.value = n.value * i;
							else
								e.numbers.Add(new Number(o.num.name, 0));
						}
						else
						{
							Number n = e.numbers.Find(x => x.name == o.num.name);
							
							if (n != null)
								n.value = n.value * o.num.value;
							else
								e.numbers.Add(new Number(o.num.name, 0));
						}
					}
					else return;
					break;
				// /
				case 5:
					if (type == 2)
					{
						if (o.subject2 != "")
						{
							Number n = e.numbers.Find(x => x.name == o.num.name);
							
							int i = storyWorld.entities.Find(x => x.name == o.subject2).numbers.Find(y => y.name == o.numRef2).value;
							
							if (n != null)
								n.value = n.value / i;
							else
								e.numbers.Add(new Number(o.num.name, 0));
						}
						else
						{
							Number n = e.numbers.Find(x => x.name == o.num.name);
							
							if (n != null)
								n.value = n.value / o.num.value;
							else
								e.numbers.Add(new Number(o.num.name, 0));
						}
					}
					else return;
					break;
				// =
				case 6:
					if (type == 2)
					{
						if (o.subject2 != "")
						{
							Number n = e.numbers.Find(x => x.name == o.num.name);
							
							int i = storyWorld.entities.Find(x => x.name == o.subject2).numbers.Find(y => y.name == o.numRef2).value;
							
							if (n != null)
								n.value = i;
							else
								e.numbers.Add(new Number(o.num.name, i));
						}
						else
						{
							Number n = e.numbers.Find(x => x.name == o.num.name);

							if (n != null)
								n.value = o.num.value;
							else
								e.numbers.Add(new Number(o.num.name, o.num.value));
						}
					}
					else return;
					break;
				default: return;
				}
			}
		}
	}

	public Case chooseCase(Verb v)
	{
		foreach (Case c in v.cases)
		{
			bool firstSolution = true;

			foreach (Condition k in c.conditions)
			{
				if (!checkCondition(k, v)) 
				{
					firstSolution = false;
					break;
				}
			}

			if (firstSolution) return c;
		}

		return null;
	}

	//Crazy slow, needs a better solution someday
	public Page generateCasePage(Case c, Verb context, Entity me)
	{
		Page p = new Page("");
		c.page.copyTo(p);

		foreach (DrawInstruction d in p.drawList)
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

		return p;
	}

	//Has side-effects...
	public Page executeCase(Case c, Verb context, Entity me)
	{
		if (c != null)
		{
			for (int i = 0; i < c.operators.Count; i++)
				applyOperator(c.operators[i], context);
			
			return generateCasePage(c, context, me);
		}
		else
			return null;
	}

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
				if (!checkCondition(c, v))
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

	public void handleAI()
	{
		List<Entity> actors = shuffleEntities(storyWorld.entities.FindAll(x => x.obligations.Count > 0 || x.goals.Count > 0 || x.behaviors.Count > 0));
		amountOfWaits = 0;

		for (int i = 0; i < actors.Count; i++)
		{
			if (ended)
				break;
			else
			{
				if (actors[i].name != storyWorld.userEntity)
					AIAct(actors[i]);
			}
		}
		if (amountOfWaits == actors.Count && userFoundNoAction)
			standStill = true;
	}
	
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
}
