using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class Verb
{
    /* Variables */
    public string name;
	public List<Tag> tags;
    public List<Variable> variables;
    public List<Argument> arguments;
    public List<Condition> preconditions;
	public List<Case> cases;
	public List<Discriminator> discriminators;
	public DynamicVerbTreeNode root;
	public string icon;
	public IconText it;

    /* Functions */
    public Verb(string _name)
    {
        name = _name;
		tags = new List<Tag>();
        variables = new List<Variable>();
		arguments = new List<Argument>();
		preconditions = new List<Condition>();
		cases = new List<Case>();
		discriminators = new List<Discriminator>();
		root = null;
		icon = "";
		it = null;
    }

    public void applyPath(List<string> path)
    {
        if (path != null)
            for (int i = 0; i < path.Count; i++)
                replaceWith(arguments[i].name, path[i]);
    }

    public void replaceWith(string replace, string with)
    {
		//Add ability to replace page text entity references
		for (int i = 0; i < variables.Count; i++)
			variables[i].replaceWith(replace, with);
		for (int i = 0; i < arguments.Count; i++)
			arguments[i].replaceWith(replace, with);
		for (int i = 0; i < preconditions.Count; i++ )
			preconditions[i].replaceWith(replace, with);
		for (int i = 0; i < cases.Count; i++ )
			cases[i].replaceWith(replace, with);
		for (int i = 0; i < discriminators.Count; i++ )
			discriminators[i].replaceWith(replace, with);
    }

    public void copyTo(Verb verb)
    {
        if (verb != null)
        {
            verb.name = name;

			//Copy tags
			verb.tags.Clear();
			foreach (Tag t in tags)
				verb.tags.Add(new Tag(t.name));

			//Copy variables
			verb.variables.Clear();
			for (int i = 0; i < variables.Count; i++)
			{
				Variable temp = new Variable("");
				variables[i].copyTo(temp);
				verb.variables.Add(temp);
			}

			//Copy arguments
			verb.arguments.Clear();
			for (int i = 0; i < arguments.Count; i++)
			{
				Argument temp = new Argument("", "");
				arguments[i].copyTo(temp);
				verb.arguments.Add(temp);
			}

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
			verb.discriminators.Clear();
			for (int i = 0; i < discriminators.Count; i++)
			{
				Discriminator temp = new Discriminator("");
				discriminators[i].copyTo(temp);
				verb.discriminators.Add(temp);
			}

			verb.icon = icon;

			if (it != null)
			{
				IconText tempI = new IconText("");
				it.copyTo(tempI);
				verb.it = tempI;
			}
			else
				verb.it = null;

        }
    }
}
