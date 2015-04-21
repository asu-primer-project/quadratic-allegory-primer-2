using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class Verb
{
    /* Variables */
    public string name;
    public List<ConditionBundle> variables;
    public List<ConditionBundle> arguments;
	public string input;
    public List<Condition> preconditions;
	public List<Case> cases;
	public List<Condition> discriminator;
	public string icon;
	public bool neverShow;

    /* Functions */
    public Verb(string _name)
    {
        name = _name;
        variables = new List<ConditionBundle>();
		arguments = new List<ConditionBundle>();
		input = "";
		preconditions = new List<Condition>();
		cases = new List<Case>();
		discriminator = new List<Condition>();
		icon = "";
		neverShow = false;
    }

    public void applyPath(List<string> path)
    {
        if (path != null)
            for (int i = 1; i < path.Count; i++)
                replaceWith(arguments[i].name, path[i]);
    }

    public void replaceWith(string replace, string with)
    {
		//Add ability to replace page text entity references
		for (int i = 0; i < preconditions.Count; i++ )
			preconditions[i].replaceWith(replace, with);
		for (int i = 0; i < cases.Count; i++ )
			cases[i].replaceWith(replace, with);
		for (int i = 0; i < discriminator.Count; i++ )
			discriminator[i].replaceWith(replace, with);
    }

    public void copyTo(Verb verb)
    {
        if (verb != null)
        {
            verb.name = name;

			//Copy variables
			verb.variables.Clear();
			for (int i = 0; i < variables.Count; i++)
			{
				ConditionBundle temp = new ConditionBundle("");
				variables[i].copyTo(temp);
				verb.variables.Add(temp);
			}

			//Copy arguments
			verb.arguments.Clear();
			for (int i = 0; i < arguments.Count; i++)
			{
				ConditionBundle temp = new ConditionBundle("");
				arguments[i].copyTo(temp);
				verb.arguments.Add(temp);
			}

			verb.input = input;

			//Copy preconditions
			verb.preconditions.Clear();
			for (int i = 0; i < preconditions.Count; i++)
			{
				Condition temp = new Condition();
				preconditions[i].copyTo(temp);
				verb.preconditions.Add(temp);
			}

			//Copy cases
			verb.cases.Clear();
			for (int i = 0; i < cases.Count; i++)
			{
				Case temp = new Case("");
				cases[i].copyTo(temp);
				verb.cases.Add(temp);
			}

			//Copy discriminator
			verb.discriminator.Clear();
			for (int i = 0; i < discriminator.Count; i++)
			{
				Condition temp = new Condition();
				discriminator[i].copyTo(temp);
				verb.discriminator.Add(temp);
			}

			verb.icon = icon;
			verb.neverShow = neverShow;
        }
    }

    public override string ToString()
    {
		return input;
    }
}
