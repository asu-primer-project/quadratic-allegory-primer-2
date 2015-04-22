using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class Engine
{
    /* Variables */
    public StoryWorld storyWorld;
    public int tick;
    public List<Page> output;
    public List<Verb> currentUserChoices;
    public bool ended;
    public Random rand;

    /* Functions */
    public Engine(StoryWorld _storyWorld)
    {
        storyWorld = _storyWorld;
        tick = 0;
		output = new List<Page>();
        currentUserChoices = null;
        ended = false;
        rand = new Random();
    }

	public void createBeginning()
	{
		foreach(Page p in storyWorld.beginning)
			output.Add(p);
		output.Add(storyWorld.input);
	}

	/*
    //Currently string together all end conditions as being logical OR
    public bool checkEndConditions()
    {
        for (int i = 0; i < storyWorld.endConditions.Count; i++)
        {
            if (checkCondition(storyWorld.endConditions[i]))
            {
                return true;
            }                    
        }

        return false;
    }

    public void takeInputAndProcess(Verb choice)
    {
        if (!ended)
        {
            //Handle user action
            if (choice.name != "!Wait")
                output += (tick + ": " + executeVerb(choice) + "\n");
            
            //End at user action if end met
            ended = checkEndConditions();

            //Handle AI
            handleAI();
            tick++;

            //Now wait for input again
            currentUserChoices = generatePossibleVerbs(storyWorld.entities.Find(x => x.name == storyWorld.userEntity));

            while (currentUserChoices.Count <= 1)
            {
                //Handle AI
                handleAI();
                tick++;

                //Now wait for input again
                currentUserChoices = generatePossibleVerbs(storyWorld.entities.Find(x => x.name == storyWorld.userEntity));
            }

            //waitingForInput = true;
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
            for (int j = 0; j < storyWorld.verbs[i].operators.Count; j++ )
            {
                if (storyWorld.verbs[i].operators[j].type == 2)
                    checkAndFixPathSize(storyWorld.verbs[i].operators[j].obligation.arguments, storyWorld.verbs.Find(x => x.name == storyWorld.verbs[i].operators[j].obligation.verb));
                if (storyWorld.verbs[i].operators[j].type == 4)
                    checkAndFixPathSize(storyWorld.verbs[i].operators[j].behavior.arguments, storyWorld.verbs.Find(x => x.name == storyWorld.verbs[i].operators[j].behavior.verb));
            }
        }

        currentUserChoices = generatePossibleVerbs(storyWorld.entities.Find(x => x.name == storyWorld.userEntity));

        if (currentUserChoices.Count == 1)
            takeInputAndProcess(currentUserChoices[0]);

        //waitingForInput = true;
    }

    public string printVerb(Verb v)
    {
        string s = "";
        if (v.output.Count > 0)
        {
            for (int i = 0; i < v.output.Count; i++)
                s += (v.output[i] + " ");
            s = Char.ToUpper(s[0]) + s.Substring(1);
        }
        return s;
    }

    public bool isEntitySolution(Entity e, Verb v, string var)
    {
        //Get all conditions pertaining to the variable, replace the variable with the candidate entity, and then check if the entity works as a solution.
        List<Condition> varConditions = new List<Condition>();

        for (int i = 0; i < v.conditions.Count; i++)
        {
            if (v.conditions[i].conditionSubject == var || v.conditions[i].relationship.other == var)
            {
                Condition tempC = new Condition("", "", false, 0);
                v.conditions[i].copyTo(tempC);
                tempC.replaceWith(var, e.name);
                varConditions.Add(tempC);
            }
        }

        for (int i = 0; i < varConditions.Count; i++)
        {
            if (!checkCondition(varConditions[i]))
                return false;
        }

        return true;
    }

    public void buildDVT(DynamicVerbTreeNode r, Verb v, int h, Entity e)
    {
        if (h < (v.variables.Count - 1))
        {
            for (int i = 0; i < storyWorld.entities.Count; i++)
            {
                if (isEntitySolution(storyWorld.entities[i], v, v.variables[h + 1]) && storyWorld.entities[i].name != e.name)
                {
                    DynamicVerbTreeNode child = new DynamicVerbTreeNode(storyWorld.entities[i]);
                    r.children.Add(child);

                    //Create new version of verb with child as solution, then build that tree
                    Verb newV = new Verb("");
                    v.copyTo(newV);
                    newV.replaceWith(newV.variables[h + 1], child.data.name);
                    buildDVT(child, newV, h + 1, e);
                }
            }
        }
        else
            return;
    }

    //Given a DVT and a path through that DVT, recursively find if the path goes through the entire DVT and fill in ?rand if found, h should be 1
    public List<string> checkDVTPath(DynamicVerbTreeNode r, List<string> path, int h)
    {
        if (h != path.Count)
        {
            if (path[h] != "?rand")
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

    //If check if path is longer enough and if not make it so, has side effects
    public void checkAndFixPathSize(List<string> path, Verb v)
    {
        if (path == null)
            return;
        if (v == null)
            return;
        else if (v.variables == null)
            return;
        if (path.Count == v.variables.Count)
            return;
        while (path.Count > v.variables.Count)
            path.RemoveAt(path.Count - 1);
        while (path.Count < v.variables.Count)
            path.Add("?rand");
        return;
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

    public List<Verb> generatePossibleVerbs(Entity e)
    {
        //Get all verbs
        List<Verb> list = new List<Verb>();
        for (int i = 0; i < storyWorld.verbs.Count; i++ )
        {
            if (storyWorld.verbs[i].name != "!Display")
            {
                Verb temp = new Verb("");
                storyWorld.verbs[i].copyTo(temp);
                temp.replaceWith("?me", e.name);

                list.Add(temp);
            }
        }

        List<string> thoseToRemove = new List<string>();
        
        //Select verbs that match conditions
        for (int i = 0; i < list.Count; i++ )
        {             
            for (int j = 0; j < list[i].conditions.Count; j++ )
            {
                if (!checkCondition(list[i].conditions[j]))
                {
                    thoseToRemove.Add(list[i].name);
                    break;
                }
            }

            //If there is more than one variable, create the variable tree and see if it has a possible route.
            if (list[i].variables.Count > 1 && thoseToRemove.Find(x => x == list[i].name) == null)
            {
                list[i].root = new DynamicVerbTreeNode(e);
                DynamicVerbTreeNode currentDVT = list[i].root;
                buildDVT(list[i].root, list[i], 0, e);

                if (currentDVT.getHeight(0) < (list[i].variables.Count - 1))
                    thoseToRemove.Add(list[i].name);
            }
        }

        //Remove those which don't match conditions
        for (int i = 0; i < thoseToRemove.Count; i++ )
            list.RemoveAll(x => x.name == thoseToRemove[i]);

        return list;
    }

    public bool evaluteDisplayVerb(Entity e)
    {
        Verb display = storyWorld.verbs.Find(x => x.name == "!Display");
        display.replaceWith("?me", e.name);

        //Select verbs that match conditions
        for (int j = 0; j < display.conditions.Count; j++)
        {
            if (!checkCondition(display.conditions[j]))
                return false;
        }

        //If there is more than one variable, create the variable tree and see if it has a possible route.
        if (display.variables.Count > 1)
        {
            display.root = new DynamicVerbTreeNode(e);
            buildDVT(display.root, display, 0, e);

            if (display.root.getHeight(0) < (display.variables.Count - 1))
                return false;
        }

        return true;
    }

    public bool checkCondition(Condition c)
    {
        Entity tempE = null;

        //Ignore if it is a condition for a variable, this will be handled later.
        if (c.conditionSubject[0] == '?' && c.conditionSubject != "?any")
            return true;
        
        if (c.conditionSubject == "?any")
        {
            bool anySatisfy = false;
            
            for (int i = 0; i < storyWorld.entities.Count; i++ )
            {
                if (c.type == 0)
                {
                    Attribute tempA = null;
                    tempA = storyWorld.entities[i].attributes.Find(x => x.name == c.attribute.name);

                    anySatisfy = (tempA != null) ? true : false;

                    if (anySatisfy)
                    {
                        if (!c.negate)
                            return true;
                        else
                            return false;
                    }
                }
                else if (c.type == 1)
                {
                    Relationship tempR = null;
                    //If the object is a variable, don't worry about it yet.
                    if (c.relationship.other[0] == '?')
                        tempR = storyWorld.entities[i].relationships.Find(x => x.name == c.relationship.name);
                    else
                        tempR = storyWorld.entities[i].relationships.Find(x => x.name == c.relationship.name && x.other == c.relationship.other);

                    anySatisfy = (tempR != null) ? true : false;
                    
                    if (anySatisfy)
                    {
                        if (!c.negate)
                            return true;
                        else
                            return false;
                    }
                }
            }

            if (!c.negate)
                return false;
            else
                return true;
        }
        else
        {
            tempE = storyWorld.entities.Find(x => x.name == c.conditionSubject);

            //Does the subject exist
            if (tempE != null)
            {
                if (c.type == 0)
                {
                    Attribute tempA = null;
                    tempA = tempE.attributes.Find(x => x.name == c.attribute.name);

                    return (tempA != null) ? !c.negate : c.negate;
                }
                else if (c.type == 1)
                {
                    Relationship tempR = null;
                    //If the object is a variable, don't worry about it yet.
                    if (c.relationship.other[0] == '?')
                        tempR = tempE.relationships.Find(x => x.name == c.relationship.name);                   
                    else
                        tempR = tempE.relationships.Find(x => x.name == c.relationship.name && x.other == c.relationship.other);

                    return (tempR != null) ? !c.negate : c.negate;
                }
                else
                    return false;
            }
            else
                return false;
        }
    }
    
    public void applyOperator(Operator o)
    {
        Entity subject = storyWorld.entities.Find(x => x.name == o.operatorSubject);

        if (subject != null)
        {
            if (o.addRemove)
            {
                //Add
                switch (o.type)
                {
                    case 0:
                        if (o.attribute != null)
                        {
                            Attribute tempA = new Attribute("");
                            o.attribute.copyTo(tempA);
                            if (subject.attributes.Find(x => x.name == tempA.name) == null)
                                subject.attributes.Add(tempA);
                        }
                        break;
                    case 1:
                        if (o.relationship != null)
                        {
                            Relationship tempR = new Relationship("", "", false);
                            o.relationship.copyTo(tempR);
                            if (tempR.exclusive == true)
                            {
                                subject.relationships.RemoveAll(x => x.name == tempR.name);
                                subject.relationships.Add(tempR);
                            }
                            else
                            {
                                if (subject.relationships.Find(x => x.name == tempR.name && x.other == tempR.other) == null)
                                    subject.relationships.Add(tempR);
                            }
                        }
                        break;
                    case 2:
                        if (subject.obligations.Find(x => x.name == o.obligation.name) == null && subject.name != storyWorld.userEntity)
                            subject.obligations.Add(o.obligation);
                        break;
                    case 3:
                        if (subject.goals.Find(x => x.name == o.goal.name) == null && subject.name != storyWorld.userEntity)
                            subject.goals.Add(o.goal);
                        break;
                    case 4:
                        if (o.behavior != null && subject.name != storyWorld.userEntity)
                        {
                            Behavior tempB = new Behavior("", "", 0);
                            o.behavior.copyTo(tempB);
                            if (subject.behaviors.Find(x => x.name == tempB.name) == null)
                                subject.behaviors.Add(tempB);
                            else
                                subject.behaviors.Find(x => x.name == tempB.name).chance = tempB.chance;
                        }
                        break;
                }
            }
            else
            {
                //Remove
                switch (o.type)
                {
                    case 0:
                        if (o.attribute != null)
                            subject.attributes.RemoveAll(x => x.name == o.attribute.name);
                        break;
                    case 1:
                        if (o.relationship != null)
                        {
                            if (o.relationship.exclusive == true)
                                subject.relationships.RemoveAll(x => x.name == o.relationship.name);
                            else
                                subject.relationships.RemoveAll(x => x.name == o.relationship.name && x.other == o.relationship.other);                                   
                        }
                        break;
                    case 2:
                        subject.obligations.RemoveAll(x => x.name == o.obligation.name);
                        break;
                    case 3:
                        subject.goals.RemoveAll(x => x.name == o.goal.name);
                        break;
                    case 4:
                        if (o.behavior != null)
                            subject.behaviors.RemoveAll(x => x.name == o.behavior.name);
                        break;
                }
            }
        }
    }

    //Has side-effects...
    public string executeVerb(Verb v)
    {
        if (v != null)
        {
            for (int i = 0; i < v.operators.Count; i++)
                applyOperator(v.operators[i]);

            return printVerb(v);
        }
        else
            return "";
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

        for (int i = 0; i < actors.Count; i++)
        {
            if (ended)
                break;
            else
                AIAct(actors[i]);
        }
    }

    public void AIAct(Entity e)
    {
        List<Verb> possibleActions = generatePossibleVerbs(e);
        Verb choice = null;

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

                if (tempV.variables.Count > 1)
                {
                    List<string> path = checkDVTPath(tempV.root, possibleObligations[indexes[index]].arguments, 1);

                    //Check that the path constraints match the possiblities
                    while (path == null && tempV.variables.Count > 1)
                    {
                        indexes.RemoveAt(index);
                        if (indexes.Count == 0)
                            break;
                        index = rand.Next(0, indexes.Count);
                        tempV = PAO.Find(x => x.name == possibleObligations[indexes[index]].verb);
                        if (tempV.variables.Count > 1)
                            path = checkDVTPath(tempV.root, possibleObligations[indexes[index]].arguments, 1);
                    }

                    if (indexes.Count != 0 && tempV.variables.Count > 1)
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
                possibleGoalVerbs.AddRange(PAG.FindAll(x => x.operators.Find(y => e.goals[i].matchWith(y)) != null));

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
                if (possibleBehaviors[i].arguments.Count > 1)
                {
                    Verb tempV = possibleActions.Find(x => x.name == possibleBehaviors[i].name);

                    if (tempV != null)
                        if (checkDVTPath(tempV.root, possibleBehaviors[i].arguments, 1) == null)
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

                if (ballSpinner[index].arguments.Count > 1)
                    choice.applyPath(checkDVTPath(choice.root, ballSpinner[index].arguments, 1));
            }
        }

        //Execute verbs
        if (choice != null)
        {
            if (choice.name != "!Wait")
            {

                //Check if with display verb if output is to be shown
                //if (evaluteDisplayVerb(e))
                //Stupid hardcoded temp solution to verb hiding. Need to implement AND and || functionality to conditions and make operators arguments for those conditions.
                Entity UE = storyWorld.entities.Find(x => x.name == storyWorld.userEntity);
                if (storyWorld.name == "Desert Delvers" && UE.relationships.Find(x => x.name == "at") != null)
                {
                    if (e.relationships.Find(x => x.name == "at" && x.other == UE.relationships.Find(y => y.name == "at").other) != null
                       || choice.operators.Find(x => x.type == 1 && x.relationship.name == "at" && x.relationship.other == UE.relationships.Find(y => y.name == "at").other) != null)
                        output += (tick + ": " + executeVerb(choice) + "\n");
                    else
                        executeVerb(choice);
                }
                else if (storyWorld.name == "Desert Delvers" && UE.relationships.Find(x => x.name == "in" && x.other == "Cab") != null)
                {
                    if (e.name == "Cab Driver")
                        output += (tick + ": " + executeVerb(choice) + "\n");
                    else 
                        executeVerb(choice);
                }
                else 
                    output += (tick + ": " + executeVerb(choice) + "\n");

                //else
                    //executeVerb(choice);

                //All volatile goals that are satisfied by the choice are removed
                for (int i = 0; i < choice.operators.Count; i++)
                    e.goals.RemoveAll(x => x.volatileGoal == true && x.matchWith(choice.operators[i]));
            }
        }

        ended = checkEndConditions();
    }*/
}
