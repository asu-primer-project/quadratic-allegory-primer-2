using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
//using System.Xml;

public class LudoNarrare : MonoBehaviour
{
    enum LoadState { loadFile, loadStoryWorld, loadEntity, loadEntityGoal, loadEndCondition, loadVerb, loadVerbCondition, loadVerbOperator, loadVerbGoal };

    private StoryWorld sw;
    private Engine engine;
    private string swFileLoc;
    //private List<ComboBox> verbBoxes = new List<ComboBox>(7);
	private bool inputReady;
	public TextAsset storyWorldFile;
	private int loadStatus;
	private bool done = false;

    //Constructer/Setup
	/*
    public LudoNarrare()
    {
        //InitializeComponent();
        //verbBoxes.Add(comboBox0);
        //verbBoxes.Add(comboBox1);
        //verbBoxes.Add(comboBox2);
        //verbBoxes.Add(comboBox3);
        //verbBoxes.Add(comboBox4);
        //verbBoxes.Add(comboBox5);
        //verbBoxes.Add(comboBox6);
    }*/

	//Access functions
	public StoryWorld getStoryWorld()
	{
		return sw;
	}

	public Engine getEngine()
	{
		return engine;
	}

	public bool getDone()
	{
		return done;
	}

	//On start, initialize
	void Start() 
	{
		StoryWorldLoader swLoader = new StoryWorldLoader();
		loadStatus = swLoader.readStoryWorld(storyWorldFile);
		sw = swLoader.storyWorld;

		if (loadStatus == -1)
		{
			engine = new Engine(sw);
			engine.createBeginning();
		}
		else
		{
			print("Error in LNScript file on line " + loadStatus);
			Application.Quit();
		}
		done = true;
	}
	
	/*
	private void loadToolStripMenuItem_Click(object sender, EventArgs e)
    {
        DialogResult result = OpenWorldDataDialog.ShowDialog();
        bool success = false;
        
        if (result == DialogResult.OK)
        {
            swFileLoc = OpenWorldDataDialog.FileName;
            XmlTextReader reader = new XmlTextReader(swFileLoc);
            //Variables for tracking data added
            LoadState ls = LoadState.loadFile;
            Entity currentEntity = null;
            Verb currentVerb = null;
            Goal currentGoal = null;
            Condition currentCondition = null;
            Operator currentOperator = null;
            
            while (reader.Read())
            {
                switch (reader.NodeType)
                {
                    case XmlNodeType.Element:
                        //Read story world header data
                        if (reader.Name == "StoryWorld" && ls == LoadState.loadFile)
                        {
                            ls = LoadState.loadStoryWorld;
                            sw = new StoryWorld("StoryWorld", "UserEntity");

                            while (reader.MoveToNextAttribute())
                            {
                                if (reader.Name == "name")
                                    sw.name = reader.Value;
                                if (reader.Name == "userEntity")
                                    sw.userEntity = reader.Value;
                            }   
                        }
                        
                        //Read entity data
                        if (reader.Name == "Entity" && ls == LoadState.loadStoryWorld)
                        {
                            ls = LoadState.loadEntity;
                            currentEntity = new Entity("Entity");
                            sw.entities.Add(currentEntity);

                            while (reader.MoveToNextAttribute())
                            {
                                if (reader.Name == "name")
                                    currentEntity.name = reader.Value;
                            }
                        }

                        if (reader.Name == "Attribute" && ls == LoadState.loadEntity)
                        {
                            Attribute temp = new Attribute("Attribute");
                            
                            while (reader.MoveToNextAttribute())
                            {
                                if (reader.Name == "name")
                                    temp.name = reader.Value;
                            }

                            currentEntity.attributes.Add(temp);
                        }

                        if (reader.Name == "Relationship" && ls == LoadState.loadEntity)
                        {
                            Relationship temp = new Relationship("Relationship","Other",false);

                            while (reader.MoveToNextAttribute())
                            {
                                if (reader.Name == "name")
                                    temp.name = reader.Value;
                                if (reader.Name == "object")
                                    temp.other = reader.Value;
                                if (reader.Name == "exclusive")
                                {
                                    if (reader.Value == "true")
                                        temp.exclusive = true;
                                    else
                                        temp.exclusive = false;
                                }
                            }

                            currentEntity.relationships.Add(temp);
                        }

                        if (reader.Name == "Obligation" && ls == LoadState.loadEntity)
                        {
                            Obligation temp = new Obligation("Obligation", "Verb");

                            while (reader.MoveToNextAttribute())
                            {
                                if (reader.Name == "name")
                                    temp.name = reader.Value;
                                if (reader.Name == "verb")
                                    temp.verb = reader.Value;
                                if (reader.Name[0] == 'v' && reader.Name[1] == 'a' && reader.Name[2] == 'r')
                                    temp.arguments.Add(reader.Value);
                            }

                            currentEntity.obligations.Add(temp);
                        }

                        if (reader.Name == "Goal" && ls == LoadState.loadEntity)
                        {
                            ls = LoadState.loadEntityGoal;
                            currentGoal = new Goal("", "", false, 0, false);

                            while (reader.MoveToNextAttribute())
                            {
                                if (reader.Name == "name")
                                    currentGoal.name = reader.Value;
                                if (reader.Name == "subject")
                                    currentGoal.operatorSubject = reader.Value;
                                if (reader.Name == "type")
                                {
                                    if (reader.Value == "remove")
                                        currentGoal.addRemove = false;
                                    else
                                        currentGoal.addRemove = true;
                                }
                                if (reader.Name == "volatile")
                                {
                                    if (reader.Value == "true")
                                        currentGoal.volatileGoal = true;
                                    else
                                        currentGoal.volatileGoal = false;
                                }
                            }
                        }
                            
                            //Load Goal target
                            if (reader.Name == "Attribute" && ls == LoadState.loadEntityGoal)
                            {
                                while (reader.MoveToNextAttribute())
                                {
                                    if (reader.Name == "name")
                                    {
                                        currentGoal.type = 0;
                                        currentGoal.attribute = new Attribute(reader.Value);
                                    }
                                }
                            }

                            if (reader.Name == "Relationship" && ls == LoadState.loadEntityGoal)
                            {
                                while (reader.MoveToNextAttribute())
                                {
                                    if (reader.Name == "name")
                                    {
                                        currentGoal.type = 1;
                                        currentGoal.relationship = new Relationship(reader.Value, "Object", false);
                                    }
                                    if (reader.Name == "object" && currentGoal.type == 1)
                                        currentGoal.relationship.other = reader.Value;
                                    if (reader.Name == "exclusive")
                                    {
                                        if (reader.Value == "true")
                                            currentGoal.relationship.exclusive = true;
                                        else
                                            currentGoal.relationship.exclusive = false;
                                    }
                                }
                            }

                            if (reader.Name == "Obligation" && ls == LoadState.loadEntityGoal)
                            {
                                while (reader.MoveToNextAttribute())
                                {
                                    if (reader.Name == "name")
                                    {
                                        currentGoal.type = 2;
                                        currentGoal.obligation = new Obligation(reader.Value, "Verb");
                                    }
                                    if (reader.Name == "verb" && currentGoal.type == 2)
                                        currentGoal.obligation.verb = reader.Value;
                                    if (reader.Name[0] == 'v' && reader.Name[1] == 'a' && reader.Name[2] == 'r' && currentGoal.type == 2)
                                        currentGoal.obligation.arguments.Add(reader.Value);
                                }
                            }

                            if (reader.Name == "Behavior" && ls == LoadState.loadEntityGoal)
                            {
                                while (reader.MoveToNextAttribute())
                                {
                                    if (reader.Name == "name")
                                    {
                                        currentGoal.type = 4;
                                        currentGoal.behavior = new Behavior(reader.Value, "Verb", 0);
                                    }
                                    if (reader.Name == "verb" && currentGoal.type == 4)
                                        currentGoal.behavior.verb = reader.Value;
                                    if (reader.Name == "chance" && currentGoal.type == 4)
                                    {
                                        if (!Int32.TryParse(reader.Value, out currentGoal.behavior.chance))
                                            currentGoal.behavior.chance = 0;
                                    }
                                    if (reader.Name[0] == 'v' && reader.Name[1] == 'a' && reader.Name[2] == 'r' && currentGoal.type == 4)
                                        currentGoal.behavior.arguments.Add(reader.Value);
                                }
                            }

                        if (reader.Name == "Behavior" && ls == LoadState.loadEntity)
                        {
                            Behavior temp = new Behavior("Behavior", "Verb", 0);

                            while (reader.MoveToNextAttribute())
                            {
                                if (reader.Name == "name")
                                    temp.name = reader.Value;
                                if (reader.Name == "verb")
                                    temp.verb = reader.Value;
                                if (reader.Name == "chance")
                                {
                                    if (!Int32.TryParse(reader.Value, out temp.chance))
                                        temp.chance = 0;
                                }
                                if (reader.Name[0] == 'v' && reader.Name[1] == 'a' && reader.Name[2] == 'r')
                                    temp.arguments.Add(reader.Value);
                            }

                            currentEntity.behaviors.Add(temp);
                        }
                        
                        //Read end condition data
                        if (reader.Name == "EndCondition" && ls == LoadState.loadStoryWorld)
                        {
                            ls = LoadState.loadEndCondition;
                            currentCondition = new Condition("Condition", "Subject", false, 0);

                            while (reader.MoveToNextAttribute())
                            {
                                if (reader.Name == "name")
                                    currentCondition.name = reader.Value;
                                if (reader.Name == "subject")
                                    currentCondition.conditionSubject = reader.Value;
                                if (reader.Name == "negate")
                                {
                                    if (reader.Value == "true")
                                        currentCondition.negate = true;
                                    else
                                        currentCondition.negate = false;
                                }
                            }
                        }

                            //Load end condition target
                            if (reader.Name == "Attribute" && ls == LoadState.loadEndCondition)
                            {
                                while (reader.MoveToNextAttribute())
                                {
                                    if (reader.Name == "name")
                                    {
                                        currentCondition.type = 0;
                                        currentCondition.attribute = new Attribute(reader.Value);
                                    }
                                }
                            }

                            if (reader.Name == "Relationship" && ls == LoadState.loadEndCondition)
                            {
                                while (reader.MoveToNextAttribute())
                                {
                                    if (reader.Name == "name")
                                    {
                                        currentCondition.type = 1;
                                        currentCondition.relationship = new Relationship(reader.Value, "Object", false);
                                    }
                                    if (reader.Name == "object" && currentCondition.type == 1)
                                        currentCondition.relationship.other = reader.Value;
                                    if (reader.Name == "exclusive")
                                    {
                                        if (reader.Value == "true")
                                            currentCondition.relationship.exclusive = true;
                                        else
                                            currentCondition.relationship.exclusive = false;
                                    }
                                }
                            }

                        //Read verb data
                        if (reader.Name == "Verb" && ls == LoadState.loadStoryWorld)
                        {
                            ls = LoadState.loadVerb;
                            currentVerb = new Verb("Verb");
                            sw.verbs.Insert(0, currentVerb);

                            while (reader.MoveToNextAttribute())
                            {
                                if (reader.Name == "name" && reader.Value != "Wait")
                                    currentVerb.name = reader.Value;
                            }
                        }

                        if (reader.Name == "Variable" && ls == LoadState.loadVerb)
                        {
                            while (reader.MoveToNextAttribute())
                            {
                                if (reader.Name == "name" && reader.Value != "?me")
                                    currentVerb.variables.Add(reader.Value);
                            }
                        }

                        if (reader.Name == "In" && ls == LoadState.loadVerb)
                        {
                            while (reader.MoveToNextAttribute())
                            {
                                if (reader.Name == "text")
                                    currentVerb.input.Add(reader.Value);
                            }
                        }

                        if (reader.Name == "Out" && ls == LoadState.loadVerb)
                        {
                            while (reader.MoveToNextAttribute())
                            {
                                if (reader.Name == "text")
                                    currentVerb.output.Add(reader.Value);
                            }
                        }

                        if (reader.Name == "Condition" && ls == LoadState.loadVerb)
                        {
                            ls = LoadState.loadVerbCondition;
                            currentCondition = new Condition("Condition", "Subject", false, 0);

                            while (reader.MoveToNextAttribute())
                            {
                                if (reader.Name == "name")
                                    currentCondition.name = reader.Value;
                                if (reader.Name == "subject")
                                    currentCondition.conditionSubject = reader.Value;
                                if (reader.Name == "negate")
                                {
                                    if (reader.Value == "true")
                                        currentCondition.negate = true;
                                    else
                                        currentCondition.negate = false;
                                }
                            }
                        }

                            //Load verb condition target
                            if (reader.Name == "Attribute" && ls == LoadState.loadVerbCondition)
                            {
                                while (reader.MoveToNextAttribute())
                                {
                                    if (reader.Name == "name")
                                    {
                                        currentCondition.type = 0;
                                        currentCondition.attribute = new Attribute(reader.Value);
                                    }
                                }
                            }

                            if (reader.Name == "Relationship" && ls == LoadState.loadVerbCondition)
                            {
                                while (reader.MoveToNextAttribute())
                                {
                                    if (reader.Name == "name")
                                    {
                                        currentCondition.type = 1;
                                        currentCondition.relationship = new Relationship(reader.Value, "Object", false);
                                    }
                                    if (reader.Name == "object" && currentCondition.type == 1)
                                        currentCondition.relationship.other = reader.Value;
                                    if (reader.Name == "exclusive")
                                    {
                                        if (reader.Value == "true")
                                            currentCondition.relationship.exclusive = true;
                                        else
                                            currentCondition.relationship.exclusive = false;
                                    }
                                }
                            }

                        if (reader.Name == "Operator" && ls == LoadState.loadVerb)
                        {
                            ls = LoadState.loadVerbOperator;
                            currentOperator = new Operator("Operator", "Subject", true, 0);

                            while (reader.MoveToNextAttribute())
                            {
                                if (reader.Name == "name")
                                    currentOperator.name = reader.Value;
                                if (reader.Name == "subject")
                                    currentOperator.operatorSubject = reader.Value;
                                if (reader.Name == "type")
                                {
                                    if (reader.Value == "remove")
                                        currentOperator.addRemove = false;
                                    else
                                        currentOperator.addRemove = true;
                                }
                            }
                        }

                            //Load verb operator target
                            if (reader.Name == "Attribute" && ls == LoadState.loadVerbOperator)
                            {
                                while (reader.MoveToNextAttribute())
                                {
                                    if (reader.Name == "name")
                                    {
                                        currentOperator.type = 0;
                                        currentOperator.attribute = new Attribute(reader.Value);
                                    }
                                }
                            }

                            if (reader.Name == "Relationship" && ls == LoadState.loadVerbOperator)
                            {
                                while (reader.MoveToNextAttribute())
                                {
                                    if (reader.Name == "name")
                                    {
                                        currentOperator.type = 1;
                                        currentOperator.relationship = new Relationship(reader.Value, "Object", false);
                                    }
                                    if (reader.Name == "object" && currentOperator.type == 1)
                                        currentOperator.relationship.other = reader.Value;
                                    if (reader.Name == "exclusive")
                                    {
                                        if (reader.Value == "true")
                                            currentOperator.relationship.exclusive = true;
                                        else
                                            currentOperator.relationship.exclusive = false;
                                    }
                                }
                            }

                            if (reader.Name == "Obligation" && ls == LoadState.loadVerbOperator)
                            {
                                while (reader.MoveToNextAttribute())
                                {
                                    if (reader.Name == "name")
                                    {
                                        currentOperator.type = 2;
                                        currentOperator.obligation = new Obligation(reader.Value, "Verb");
                                    }
                                    if (reader.Name == "verb" && currentOperator.type == 2)
                                        currentOperator.obligation.verb = reader.Value;
                                    if (reader.Name[0] == 'v' && reader.Name[1] == 'a' && reader.Name[2] == 'r' && currentOperator.type == 2)
                                        currentOperator.obligation.arguments.Add(reader.Value);
                                }
                            }

                            if (reader.Name == "Goal" && ls == LoadState.loadVerbOperator)
                            {
                                ls = LoadState.loadVerbGoal;
                                currentOperator.type = 3;
                                currentGoal = new Goal("", "", false, 0, false);

                                while (reader.MoveToNextAttribute())
                                {
                                    if (reader.Name == "name")
                                        currentGoal.name = reader.Value;
                                    if (reader.Name == "subject")
                                        currentGoal.operatorSubject = reader.Value;
                                    if (reader.Name == "type")
                                    {
                                        if (reader.Value == "remove")
                                            currentGoal.addRemove = false;
                                        else
                                            currentGoal.addRemove = true;
                                    }
                                    if (reader.Name == "volatile")
                                    {
                                        if (reader.Value == "true")
                                            currentGoal.volatileGoal = true;
                                        else
                                            currentGoal.volatileGoal = false;
                                    }
                                }
                            }
                            
                                //Load Goal target
                                if (reader.Name == "Attribute" && ls == LoadState.loadVerbGoal)
                                {
                                    while (reader.MoveToNextAttribute())
                                    {
                                        if (reader.Name == "name")
                                        {
                                            currentGoal.type = 0;
                                            currentGoal.attribute = new Attribute(reader.Value);
                                        }
                                    }
                                }

                                if (reader.Name == "Relationship" && ls == LoadState.loadVerbGoal)
                                {
                                    while (reader.MoveToNextAttribute())
                                    {
                                        if (reader.Name == "name")
                                        {
                                            currentGoal.type = 1;
                                            currentGoal.relationship = new Relationship(reader.Value, "Object", false);
                                        }
                                        if (reader.Name == "object" && currentGoal.type == 1)
                                            currentGoal.relationship.other = reader.Value;
                                        if (reader.Name == "exclusive")
                                        {
                                            if (reader.Value == "true")
                                                currentGoal.relationship.exclusive = true;
                                            else
                                                currentGoal.relationship.exclusive = false;
                                        }
                                    }
                                }

                                if (reader.Name == "Obligation" && ls == LoadState.loadVerbGoal)
                                {
                                    while (reader.MoveToNextAttribute())
                                    {
                                        if (reader.Name == "name")
                                        {
                                            currentGoal.type = 2;
                                            currentGoal.obligation = new Obligation(reader.Value, "Verb");
                                        }
                                        if (reader.Name == "verb" && currentGoal.type == 2)
                                            currentGoal.obligation.verb = reader.Value;
                                        if (reader.Name[0] == 'v' && reader.Name[1] == 'a' && reader.Name[2] == 'r' && currentGoal.type == 2)
                                            currentGoal.obligation.arguments.Add(reader.Value);
                                    }
                                }

                                if (reader.Name == "Behavior" && ls == LoadState.loadVerbGoal)
                                {
                                    while (reader.MoveToNextAttribute())
                                    {
                                        if (reader.Name == "name")
                                        {
                                            currentGoal.type = 4;
                                            currentGoal.behavior = new Behavior(reader.Value, "Verb", 0);
                                        }
                                        if (reader.Name == "verb" && currentGoal.type == 4)
                                            currentGoal.behavior.verb = reader.Value;
                                        if (reader.Name == "chance" && currentGoal.type == 4)
                                        {
                                            if (!Int32.TryParse(reader.Value, out currentGoal.behavior.chance))
                                                currentGoal.behavior.chance = 0;
                                        }
                                        if (reader.Name[0] == 'v' && reader.Name[1] == 'a' && reader.Name[2] == 'r' && currentGoal.type == 4)
                                            currentGoal.behavior.arguments.Add(reader.Value);
                                    }
                                }

                            if (reader.Name == "Behavior" && ls == LoadState.loadVerbOperator)
                            {
                                while (reader.MoveToNextAttribute())
                                {
                                    if (reader.Name == "name")
                                    {
                                        currentOperator.type = 4;
                                        currentOperator.behavior = new Behavior(reader.Value, "Verb", 0);                                         
                                    }
                                    if (reader.Name == "verb" && currentOperator.type == 4)
                                        currentOperator.behavior.verb = reader.Value;
                                    if (reader.Name == "chance" && currentOperator.type == 4)
                                    {
                                        if (!Int32.TryParse(reader.Value, out currentGoal.behavior.chance))
                                            currentOperator.behavior.chance = 0;
                                    }
                                    if (reader.Name[0] == 'v' && reader.Name[1] == 'a' && reader.Name[2] == 'r' && currentOperator.type == 4)
                                        currentOperator.behavior.arguments.Add(reader.Value);
                                }
                            }
                        break;
                    case XmlNodeType.EndElement:
                        //Back out of story world data
                        if (reader.Name == "StoryWorld" && ls == LoadState.loadStoryWorld)
                            ls = LoadState.loadFile;
                        
                        //Back out of entity data
                        if (reader.Name == "Entity" && ls == LoadState.loadEntity)
                            ls = LoadState.loadStoryWorld;

                        //Back out of entity goal
                        if (reader.Name == "Goal" && ls == LoadState.loadEntityGoal)
                        {
                            currentEntity.goals.Add(currentGoal);
                            ls = LoadState.loadEntity;
                        }

                        //Back out of end condition
                        if (reader.Name == "EndCondition" && ls == LoadState.loadEndCondition)
                        {
                            sw.endConditions.Add(currentCondition);
                            ls = LoadState.loadStoryWorld;
                        }

                        //Back out of verbs
                        if (reader.Name == "Verb" && ls == LoadState.loadVerb)
                            ls = LoadState.loadStoryWorld;

                        //Back out of verb condition
                        if (reader.Name == "Condition" && ls == LoadState.loadVerbCondition)
                        {
                            currentVerb.conditions.Add(currentCondition);
                            ls = LoadState.loadVerb;
                        }

                        //Back out of verb operator
                        if (reader.Name == "Operator" && ls == LoadState.loadVerbOperator)
                        {
                            currentVerb.operators.Add(currentOperator);
                            ls = LoadState.loadVerb;
                        }

                        //Back out of verb goal data
                        if (reader.Name == "Goal" && ls == LoadState.loadVerbGoal)
                        {
                            currentOperator.goal = currentGoal;
                            ls = LoadState.loadVerbOperator;
                        }

                        break;
                }
            }

            //If no display verb added, add default
            if (sw.verbs.Find(x => x.name == "!Display") == null)
            {
                Verb tempD = new Verb("!Display");
                sw.verbs.Add(tempD);
            }

            success = (ls == LoadState.loadFile) ? true : false;                 
        }

        //Initiate the engine with the loaded story world data.
        if (sw != null && success)
        {
            engine = new Engine(sw);
            engine.init();
            comboBox0.DataSource = engine.currentUserChoices;

            //Hide all boxes
            for (int i = 1; i < verbBoxes.Count; i++)
            {
                verbBoxes[i].Enabled = false;
                verbBoxes[i].Visible = false;
            }

            //Unhide those that are needed
            for (int i = 1; i <= getMaxBoxIndex(); i++)
            {
                verbBoxes[i].Enabled = true;
                verbBoxes[i].Visible = true;
            }

            WorldDataOutBox.Text = sw.getPrintedWorldState();
        }
        else
            MessageBox.Show("Story World data could not be loaded correctly.", "Error", MessageBoxButtons.OK);
    }

    //Update output boxes on tab change
    private void Tabs_SelectedIndexChanged(object sender, EventArgs e)
    {
        if (sw != null)
            WorldDataOutBox.Text = sw.getPrintedWorldState();
        if (engine != null)
            OutputBox.Text = engine.output;
    }

    //Process verb chosen and then update output boxes
    private void buttonInput_Click(object sender, EventArgs e)
    {
        //Check if all values are filled
        if (inputReady)
        {
            //Get selected variables
            List<string> inputVars = new List<string>();
            for (int i = 0; i <= getMaxBoxIndex(); i++ )
                inputVars.Add(verbBoxes[i].SelectedValue.ToString());

            //Process verb
            Verb tempV = engine.currentUserChoices[comboBox0.SelectedIndex];
            if (inputVars.Count > 1)
                tempV.applyPath(inputVars);
            engine.takeInputAndProcess(tempV);

            //Update output
            if (sw != null)
                WorldDataOutBox.Text = sw.getPrintedWorldState();
            if (engine != null)
                OutputBox.Text = engine.output;

            //Scroll to end
            OutputBox.SelectionStart = OutputBox.Text.Length;
            OutputBox.ScrollToCaret();

            //Input not ready
            for (int i = 0; i < verbBoxes.Count; i++)
                verbBoxes[i].DataSource = null;
            comboBox0.DataSource = engine.currentUserChoices;
            inputReady = false;
            buttonInput.Enabled = false;
        }
    }
	
    private void onVerbComboBoxChange(object sender, EventArgs e)
    {
        ComboBox changedBox = (ComboBox)sender;
        int index = verbBoxes.IndexOf(changedBox, 0, verbBoxes.Count);
        int maxBoxIndex = getMaxBoxIndex();
        //OutputBox.Text = "Box changed! " + index;
        if (index == 0)
        {
            //Hide all boxes
            for (int i = 1; i < verbBoxes.Count; i++)
            {
                verbBoxes[i].Enabled = false;
                verbBoxes[i].Visible = false;
            }
            
            //Unhide those that are needed
            for (int i = 1; i <= maxBoxIndex; i++)
            {
                verbBoxes[i].Enabled = true;
                verbBoxes[i].Visible = true;
            }
        }
        // whenever box n is changed, reset box n+1's options, and clear all boxes > n + 1
        // ...unless it's the last box
        if (index != maxBoxIndex)
        {
            verbBoxes[index + 1].DataSource = getVerbBoxOptions(index + 1);
            for (int boxNum = index + 2; boxNum <= maxBoxIndex; boxNum++)
            {
                //OutputBox.Text += (" modifying box " + boxNum);
                verbBoxes[boxNum].DataSource = null;
                //verbBoxes[boxNum].Items.Clear();
                //verbBoxes[boxNum].ResetText();
            }
            inputReady = false;
            buttonInput.Enabled = false;
        }
        else
        {
            inputReady = true;
            buttonInput.Enabled = true;
        }
    }
    
    private List<DynamicVerbTreeNode> getVerbBoxOptions(int verbBoxIndex)
    {
        ComboBox previousBox = verbBoxes[verbBoxIndex - 1];
        if ((verbBoxIndex - 1) == 0)
        {
            List<DynamicVerbTreeNode> validPaths = new List<DynamicVerbTreeNode>();
            int currentHeight = ((Verb)previousBox.SelectedValue).root.getHeight(0);

            for (int i = 0; i < ((Verb)previousBox.SelectedValue).root.children.Count; i++)
            {
                if (((Verb)previousBox.SelectedValue).root.children[i].getHeight(0) == (currentHeight - 1))
                    validPaths.Add(((Verb)previousBox.SelectedValue).root.children[i]);
            }

            return validPaths;
        }
        else
        {
            List<DynamicVerbTreeNode> validPaths = new List<DynamicVerbTreeNode>();
            int currentHeight = ((DynamicVerbTreeNode)previousBox.SelectedValue).getHeight(0);

            for (int i = 0; i < ((DynamicVerbTreeNode)previousBox.SelectedValue).children.Count; i++)
            {
                if (((DynamicVerbTreeNode)previousBox.SelectedValue).children[i].getHeight(0) == (currentHeight - 1))
                    validPaths.Add(((DynamicVerbTreeNode)previousBox.SelectedValue).children[i]);
            }

            return validPaths;
        }
    }
    private int getMaxBoxIndex()
    {
        return (((Verb)verbBoxes[0].SelectedValue).variables.Count) - 1;
    }

    //Exit program
    private void exitToolStripMenuItem_Click(object sender, EventArgs e)
    {
        Application.Exit();
    }
	*/
}
