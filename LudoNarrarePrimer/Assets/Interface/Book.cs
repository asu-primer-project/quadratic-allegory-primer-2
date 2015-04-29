﻿using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class Book : MonoBehaviour 
{
	public GameObject previousLeftPage;
	public GameObject previousRightPage;
	public GameObject currentLeftPage;
	public GameObject currentRightPage;
	public GameObject nextLeftPage;
	public GameObject nextRightPage;

	public GameObject previousPage;
	public GameObject currentPage;
	public GameObject nextPage;

	public Canvas previousCanvas;
	public Canvas currentCanvas;
	public Canvas nextCanvas;
	public Canvas inputCanvas;

	public GameObject previousText;
	public GameObject currentText;
	public GameObject nextText;
	public GameObject inputText;

	public GameObject verbTab;
	public GameObject argue1Tab;
	public GameObject argue2Tab;
	public GameObject argue3Tab;
	public GameObject argue4Tab;
	public GameObject executeTab;

	public GameObject verbIcon;
	public GameObject argue1Icon;
	public GameObject argue2Icon;
	public GameObject argue3Icon;
	public GameObject argue4Icon;

	public GameObject argue1Text;
	public GameObject argue2Text;
	public GameObject argue3Text;
	public GameObject argue4Text;

	public GameObject fadePanel;
	public float fadeSpeed = 3f;
	private float fadeTimer = 0f;
	private bool fadeOut = false;
	private bool fadeIn = false;

	private StoryWorld sw;
	private List<Page> story;

	private List<GameObject> previousPageItems = new List<GameObject>();
	private List<GameObject> currentPageItems = new List<GameObject>(); 
	private List<GameObject> nextPageItems = new List<GameObject>();

	public float fallSpeed = 70.0f;
	private float inputScale = 0.008333333f;

	private static bool flippingLeft = false;
	private static bool settledLeft = true;
	private static bool flippingRight = false;
	private static bool settledRight = true;
	private float mouseHingeLeft;
	private float mouseHingeRight;
	private Vector3 caughtAngleLeft;
	private Vector3 caughtAngleRight;

	private int pageIndex;

	private LudoNarrare ln;
	private bool ready = false;
	private bool waitingForInput = false;

	private int verbChoice = 0;
	private int argument1 = 0;
	private int argument2 = 0;
	private int argument3 = 0;
	private int argument4 = 0;

	//Functions for changing input UI state
	public void setUIArguments(int argueCount)
	{
		argue1Tab.SetActive(false);
		argue2Tab.SetActive(false);
		argue3Tab.SetActive(false);
		argue4Tab.SetActive(false);

		Engine eng = ln.getEngine();

		switch(argueCount)
		{
		case 0:
		{
			verbTab.GetComponent<RectTransform>().localPosition = new Vector3(650f, 100f, 0f);
			executeTab.GetComponent<RectTransform>().localPosition = new Vector3(650f, -75f, 0f);
			break;
		}
		case 1:
		{
			argue1Tab.SetActive(true);
			verbTab.GetComponent<RectTransform>().localPosition = new Vector3(650f, 187.5f, 0f);
			argue1Tab.GetComponent<RectTransform>().localPosition = new Vector3(470f, 12.5f, 0f);
			executeTab.GetComponent<RectTransform>().localPosition = new Vector3(650f, -162.5f, 0f);

			Text a1Text = argue1Text.GetComponent<Text>();
			a1Text.text = eng.currentUserChoices[verbChoice].arguments[0].text;
			break;
		}
		case 2:
		{
			argue1Tab.SetActive(true);
			argue2Tab.SetActive(true);
			verbTab.GetComponent<RectTransform>().localPosition = new Vector3(650f, 275f, 0f);
			argue1Tab.GetComponent<RectTransform>().localPosition = new Vector3(470f, 100f, 0f);
			argue2Tab.GetComponent<RectTransform>().localPosition = new Vector3(470f, -75f, 0f);
			executeTab.GetComponent<RectTransform>().localPosition = new Vector3(650f, -250f, 0f);

			Text a1Text = argue1Text.GetComponent<Text>();
			Text a2Text = argue2Text.GetComponent<Text>();
			a1Text.text = eng.currentUserChoices[verbChoice].arguments[0].text;
			a2Text.text = eng.currentUserChoices[verbChoice].arguments[1].text;
			break;
		}
		case 3:
		{
			argue1Tab.SetActive(true);
			argue2Tab.SetActive(true);
			argue3Tab.SetActive(true);
			verbTab.GetComponent<RectTransform>().localPosition = new Vector3(650f, 362.5f, 0f);
			argue1Tab.GetComponent<RectTransform>().localPosition = new Vector3(470f, 187.5f, 0f);
			argue2Tab.GetComponent<RectTransform>().localPosition = new Vector3(470f, 12.5f, 0f);
			argue3Tab.GetComponent<RectTransform>().localPosition = new Vector3(470f, -162.5f, 0f);
			executeTab.GetComponent<RectTransform>().localPosition = new Vector3(650f, -337.5f, 0f);

			Text a1Text = argue1Text.GetComponent<Text>();
			Text a2Text = argue2Text.GetComponent<Text>();
			Text a3Text = argue3Text.GetComponent<Text>();
			a1Text.text = eng.currentUserChoices[verbChoice].arguments[0].text;
			a2Text.text = eng.currentUserChoices[verbChoice].arguments[1].text;
			a3Text.text = eng.currentUserChoices[verbChoice].arguments[2].text;
			break;
		}
		case 4:
		{
			argue1Tab.SetActive(true);
			argue2Tab.SetActive(true);
			argue3Tab.SetActive(true);
			argue4Tab.SetActive(true);
			verbTab.GetComponent<RectTransform>().localPosition = new Vector3(650f, 450f, 0f);
			argue1Tab.GetComponent<RectTransform>().localPosition = new Vector3(470f, 275f, 0f);
			argue2Tab.GetComponent<RectTransform>().localPosition = new Vector3(470f, 100f, 0f);
			argue3Tab.GetComponent<RectTransform>().localPosition = new Vector3(470f, -75f, 0f);
			argue4Tab.GetComponent<RectTransform>().localPosition = new Vector3(470f, -250f, 0f);
			executeTab.GetComponent<RectTransform>().localPosition = new Vector3(650f, -425f, 0f);

			Text a1Text = argue1Text.GetComponent<Text>();
			Text a2Text = argue2Text.GetComponent<Text>();
			Text a3Text = argue3Text.GetComponent<Text>();
			Text a4Text = argue4Text.GetComponent<Text>();
			a1Text.text = eng.currentUserChoices[verbChoice].arguments[0].text;
			a2Text.text = eng.currentUserChoices[verbChoice].arguments[1].text;
			a3Text.text = eng.currentUserChoices[verbChoice].arguments[2].text;
			a4Text.text = eng.currentUserChoices[verbChoice].arguments[3].text;
			break;
		}
		}
	}

	public void redrawUI(int argueCount)
	{
		switch(argueCount)
		{
		case 0:
		{
			Image vi = verbIcon.GetComponent<Image>();
			vi.sprite = Resources.Load<Sprite>(ln.getEngine().currentUserChoices[verbChoice].icon);
			break;
		}
		case 1:
		{
			Image vi = verbIcon.GetComponent<Image>();
			Image a1i = argue1Icon.GetComponent<Image>();
			vi.sprite = Resources.Load<Sprite>(ln.getEngine().currentUserChoices[verbChoice].icon);
			a1i.sprite = Resources.Load<Sprite>(ln.getEngine().currentUserChoices[verbChoice].arguments[0].values[argument1].icon);
			break;
		}
		case 2:
		{
			Image vi = verbIcon.GetComponent<Image>();
			Image a1i = argue1Icon.GetComponent<Image>();
			Image a2i = argue2Icon.GetComponent<Image>();
			vi.sprite = Resources.Load<Sprite>(ln.getEngine().currentUserChoices[verbChoice].icon);
			a1i.sprite = Resources.Load<Sprite>(ln.getEngine().currentUserChoices[verbChoice].arguments[0].values[argument1].icon);
			a2i.sprite = Resources.Load<Sprite>(ln.getEngine().currentUserChoices[verbChoice].arguments[1].values[argument2].icon);
			break;
		}
		case 3:
		{
			Image vi = verbIcon.GetComponent<Image>();
			Image a1i = argue1Icon.GetComponent<Image>();
			Image a2i = argue2Icon.GetComponent<Image>();
			Image a3i = argue3Icon.GetComponent<Image>();
			vi.sprite = Resources.Load<Sprite>(ln.getEngine().currentUserChoices[verbChoice].icon);
			a1i.sprite = Resources.Load<Sprite>(ln.getEngine().currentUserChoices[verbChoice].arguments[0].values[argument1].icon);
			a2i.sprite = Resources.Load<Sprite>(ln.getEngine().currentUserChoices[verbChoice].arguments[1].values[argument2].icon);
			a3i.sprite = Resources.Load<Sprite>(ln.getEngine().currentUserChoices[verbChoice].arguments[2].values[argument3].icon);
			break;
		}
		case 4:
		{
			Image vi = verbIcon.GetComponent<Image>();
			Image a1i = argue1Icon.GetComponent<Image>();
			Image a2i = argue2Icon.GetComponent<Image>();
			Image a3i = argue3Icon.GetComponent<Image>();
			Image a4i = argue4Icon.GetComponent<Image>();
			vi.sprite = Resources.Load<Sprite>(ln.getEngine().currentUserChoices[verbChoice].icon);
			a1i.sprite = Resources.Load<Sprite>(ln.getEngine().currentUserChoices[verbChoice].arguments[0].values[argument1].icon);
			a2i.sprite = Resources.Load<Sprite>(ln.getEngine().currentUserChoices[verbChoice].arguments[1].values[argument2].icon);
			a3i.sprite = Resources.Load<Sprite>(ln.getEngine().currentUserChoices[verbChoice].arguments[2].values[argument3].icon);
			a4i.sprite = Resources.Load<Sprite>(ln.getEngine().currentUserChoices[verbChoice].arguments[3].values[argument4].icon);
			break;
		}
		}
	}

	//Functions that handle user verb choosing
	public void incrementVerb()
	{
		if (waitingForInput && settledLeft && settledRight)
		{
			if (verbChoice != ln.getEngine().currentUserChoices.Count - 1)
				verbChoice++;
			else
				verbChoice = 0;

			setUIArguments(ln.getEngine().currentUserChoices[verbChoice].arguments.Count);
			redrawUI(ln.getEngine().currentUserChoices[verbChoice].arguments.Count);
		}
	}

	public void decrementVerb()
	{
		if (waitingForInput && settledLeft && settledRight)
		{
			if (verbChoice != 0)
				verbChoice--;
			else
				verbChoice = ln.getEngine().currentUserChoices.Count - 1;	

			setUIArguments(ln.getEngine().currentUserChoices[verbChoice].arguments.Count);
			redrawUI(ln.getEngine().currentUserChoices[verbChoice].arguments.Count);
		}
	}

	public void incrementArgument1()
	{
		if (waitingForInput && settledLeft && settledRight)
		{
			if (argument1 != ln.getEngine().currentUserChoices[verbChoice].arguments[0].values.Count - 1)
				argument1++;
			else
				argument1 = 0;

			redrawUI(ln.getEngine().currentUserChoices[verbChoice].arguments.Count);
		}
	}
	
	public void decrementArgument1()
	{
		if (waitingForInput && settledLeft && settledRight)
		{
			if (argument1 != 0)
				argument1--;
			else
				argument1 = ln.getEngine().currentUserChoices[verbChoice].arguments[0].values.Count - 1;

			redrawUI(ln.getEngine().currentUserChoices[verbChoice].arguments.Count);
		}
	}
	
	public void incrementArgument2()
	{
		if (waitingForInput && settledLeft && settledRight)
		{
			if (argument2 != ln.getEngine().currentUserChoices[verbChoice].arguments[1].values.Count - 1)
				argument2++;
			else
				argument2 = 0;			

			redrawUI(ln.getEngine().currentUserChoices[verbChoice].arguments.Count);
		}
	}
	
	public void decrementArgument2()
	{
		if (waitingForInput && settledLeft && settledRight)
		{
			if (argument2 != 0)
				argument2--;
			else
				argument2 = ln.getEngine().currentUserChoices[verbChoice].arguments[1].values.Count - 1;	

			redrawUI(ln.getEngine().currentUserChoices[verbChoice].arguments.Count);
		}
	}
	
	public void incrementArgument3()
	{
		if (waitingForInput && settledLeft && settledRight)
		{
			if (argument3 != ln.getEngine().currentUserChoices[verbChoice].arguments[2].values.Count - 1)
				argument3++;
			else
				argument3 = 0;			

			redrawUI(ln.getEngine().currentUserChoices[verbChoice].arguments.Count);
		}
	}
	
	public void decrementArgument3()
	{
		if (waitingForInput && settledLeft && settledRight)
		{
			if (argument3 != 0)
				argument3--;
			else
				argument3 = ln.getEngine().currentUserChoices[verbChoice].arguments[2].values.Count - 1;	

			redrawUI(ln.getEngine().currentUserChoices[verbChoice].arguments.Count);
		}
	}
	
	public void incrementArgument4()
	{
		if (waitingForInput && settledLeft && settledRight)
		{
			if (argument4 != ln.getEngine().currentUserChoices[verbChoice].arguments[3].values.Count - 1)
				argument4++;
			else
				argument4 = 0;		

			redrawUI(ln.getEngine().currentUserChoices[verbChoice].arguments.Count);
		}
	}
	
	public void decrementArgument4()
	{
		if (waitingForInput && settledLeft && settledRight)
		{
			if (argument4 != 0)
				argument4--;
			else
				argument4 = ln.getEngine().currentUserChoices[verbChoice].arguments[3].values.Count - 1;	

			redrawUI(ln.getEngine().currentUserChoices[verbChoice].arguments.Count);
		}
	}

	public void processExecute()
	{
		Engine eng = ln.getEngine();
		
		//Process verb
		Verb tempV = eng.currentUserChoices[verbChoice];
		
		switch (eng.currentUserChoices[verbChoice].arguments.Count)
		{
		case 0:
			break;
		case 1:
			tempV.replaceWith(tempV.arguments[0].name, tempV.arguments[0].values[argument1].name);
			break;
		case 2:
			tempV.replaceWith(tempV.arguments[0].name, tempV.arguments[0].values[argument1].name);
			tempV.replaceWith(tempV.arguments[1].name, tempV.arguments[1].values[argument2].name);
			break;
		case 3:
			tempV.replaceWith(tempV.arguments[0].name, tempV.arguments[0].values[argument1].name);
			tempV.replaceWith(tempV.arguments[1].name, tempV.arguments[1].values[argument2].name);
			tempV.replaceWith(tempV.arguments[2].name, tempV.arguments[2].values[argument3].name);
			break;
		case 4:
			tempV.replaceWith(tempV.arguments[0].name, tempV.arguments[0].values[argument1].name);
			tempV.replaceWith(tempV.arguments[1].name, tempV.arguments[1].values[argument2].name);
			tempV.replaceWith(tempV.arguments[2].name, tempV.arguments[2].values[argument3].name);
			tempV.replaceWith(tempV.arguments[3].name, tempV.arguments[3].values[argument4].name);
			break;
		}
		
		eng.takeInputAndProcess(tempV);
		
		//Restart input UI
		verbChoice = 0;
		argument1 = 0;
		argument2 = 0;
		argument3 = 0;
		argument4 = 0;
		drawPage(pageIndex);
		setUIArguments(ln.getEngine().currentUserChoices[verbChoice].arguments.Count);
		redrawUI(ln.getEngine().currentUserChoices[verbChoice].arguments.Count);
		fadeIn = true;
	}

	public void execute()
	{
		if (waitingForInput && settledLeft && settledRight)
		{
			waitingForInput = false;
			fadeOut = true;
		}
	}
	
	//Creates a game object with the given draw instruction and page (0 - previous, 1 - current, 2 - next) 
	private void drawInstruction(DrawInstruction d, int page, int depth, bool input)
	{
		if (d.isText)
		{
			if (!input)
			{
				if (page == 0)
				{
					previousCanvas.enabled = true;
					Text t = previousText.GetComponent<Text>();
					t.text = d.text;
				}
				else if (page == 1)
				{
					currentCanvas.enabled = true;
					Text t = currentText.GetComponent<Text>();
					t.text = d.text;
				}
				else if (page == 2)
				{
					nextCanvas.enabled = true;
					Text t = nextText.GetComponent<Text>();
					t.text = d.text;
				}
			}
			else
			{
				Text t = inputText.GetComponent<Text>();
				t.text = d.text;
			}
		}
		else
		{
			GameObject temp = new GameObject();
			SpriteRenderer sr;
			sr = temp.AddComponent<SpriteRenderer>();
			Entity e = sw.entities.Find(x => x.name == d.entity);
			string img = e.images.Find(x => x.name == d.image).image;
			//sr.sprite = sl.spriteListing.Find(x => x.name == img);
			sr.sprite = Resources.Load<Sprite>(img);

			if (page == 0)
			{
				temp.transform.parent = previousPage.transform;
				previousPageItems.Add(temp);
				sr.sortingLayerName = "Previous Page";
			}
			else if (page == 1)
			{
				temp.transform.parent = currentPage.transform;
				currentPageItems.Add(temp);
				sr.sortingLayerName = "Current Page";
			}
			else if (page == 2)
			{
				temp.transform.parent = nextPage.transform;
				nextPageItems.Add(temp);
				sr.sortingLayerName = "Next Page";
			}

			sr.sortingOrder = depth;
			temp.transform.localScale += new Vector3(0.112f, 0.0f);
			temp.transform.localPosition = new Vector3((d.x * 1.112f)/100f, d.y/100f);
		}
	}

	//Given a page number, setup the graphics for the previous, current, and next page
	private void drawPage(int p)
	{
		//Clean up old pages
		foreach (GameObject go in previousPageItems)
			Destroy(go);
		previousPageItems.Clear();
		previousCanvas.enabled = false;

		foreach(GameObject go in currentPageItems)
			Destroy(go);
		currentPageItems.Clear();
		currentCanvas.enabled = false;

		foreach (GameObject go in nextPageItems)
			Destroy(go);
		nextPageItems.Clear();
		nextCanvas.enabled = false;

		inputCanvas.enabled = false;
		inputCanvas.transform.localScale = new Vector3(1.112f*inputScale, 1f*inputScale, 1f);
		inputCanvas.transform.position = new Vector3(40f, -30f, -1f);

		//Create sprite objects for new pages
		int i = 0;

		if (p != 0)
		{
			foreach(DrawInstruction d in story[p - 1].drawList)
			{
				drawInstruction(d, 0, i, false);
				i++;
			}
		}

		i = 0;
		if (story[p].isInputPage)
		{
			inputCanvas.enabled = true;
			inputCanvas.transform.position = new Vector3(0f, 0f, -1f);
			inputCanvas.transform.localScale = new Vector3(1f*inputScale, 1f*inputScale, 1f);

			foreach(DrawInstruction d in story[p].drawList)
			{
				drawInstruction(d, 1, i, true);
				i++;
			}
		}
		else
		{
			foreach(DrawInstruction d in story[p].drawList)
			{
				drawInstruction(d, 1, i, false);
				i++;
			}
		}

		i = 0;
		if (p != story.Count - 1)
		{
			if (story[p + 1].isInputPage)
			{
				inputCanvas.enabled = true;

				foreach(DrawInstruction d in story[p + 1].drawList)
				{
					drawInstruction(d, 2, i, true);
					i++;
				}
			}
			else
			{
				foreach(DrawInstruction d in story[p + 1].drawList)
				{
					drawInstruction(d, 2, i, false);
					i++;
				}
			}
		}
	}

	//Given the angle of the previous right page, turn it into current right page and rearrange book. 
	private void flipPageLeft(Vector3 startAngle)
	{
		//Reset geometry
		previousLeftPage.transform.position = new Vector3(-4.5f,0.0f,2.0f);
		previousLeftPage.transform.eulerAngles = new Vector3(0.0f,0.0f,0.0f);
		
		previousRightPage.transform.position = new Vector3(-4.5f,0.0f,0.0f);
		previousRightPage.transform.eulerAngles = new Vector3(0.0f,180.0f,0.0f);
		
		currentLeftPage.transform.position = new Vector3(-4.5f,0.0f,0.0f);
		currentLeftPage.transform.eulerAngles = new Vector3(0.0f,0.0f,0.0f);
		
		currentRightPage.transform.position = new Vector3(4.5f,0.0f,0.0f);
		
		nextLeftPage.transform.position = new Vector3(4.5f,0.0f,0.0f);
		nextLeftPage.transform.eulerAngles = new Vector3(0.0f,180.0f,0.0f);
		
		nextRightPage.transform.position = new Vector3(4.5f,0.0f,1.0f);
		nextRightPage.transform.eulerAngles = new Vector3(0.0f,0.0f,0.0f);

		//Apply carried angle and state
		currentRightPage.transform.eulerAngles = startAngle;
		flippingLeft = false;
		settledLeft = true;
		flippingRight = false;
		settledRight = false;
		currentRightPage.transform.position = new Vector3(4.5f*Mathf.Cos(Mathf.Deg2Rad*currentRightPage.transform.eulerAngles.y), 0.0f, 4.5f*Mathf.Sin(Mathf.Deg2Rad*currentRightPage.transform.eulerAngles.y));
	}

	//Given the angle of the next left page, turn it into the current left page and rearrange book
	private void flipPageRight(Vector3 startAngle)
	{
		//Reset geometry
		previousLeftPage.transform.position = new Vector3(-4.5f,0.0f,2.0f);
		previousLeftPage.transform.eulerAngles = new Vector3(0.0f,0.0f,0.0f);

		previousRightPage.transform.position = new Vector3(-4.5f,0.0f,0.0f);
		previousRightPage.transform.eulerAngles = new Vector3(0.0f,180.0f,0.0f);

		currentLeftPage.transform.position = new Vector3(-4.5f,0.0f,0.0f);

		currentRightPage.transform.position = new Vector3(4.5f,0.0f,0.0f);
		currentRightPage.transform.eulerAngles = new Vector3(0.0f,0.0f,0.0f);

		nextLeftPage.transform.position = new Vector3(4.5f,0.0f,0.0f);
		nextLeftPage.transform.eulerAngles = new Vector3(0.0f,180.0f,0.0f);

		nextRightPage.transform.position = new Vector3(4.5f,0.0f,1.0f);
		nextRightPage.transform.eulerAngles = new Vector3(0.0f,0.0f,0.0f);

		//Apply carried angle and state
		currentLeftPage.transform.eulerAngles = startAngle;
		flippingLeft = false;
		settledLeft = false;
		flippingRight = false;
		settledRight = true;
		currentLeftPage.transform.position = new Vector3(-4.5f*Mathf.Cos(Mathf.Deg2Rad*currentLeftPage.transform.eulerAngles.y), 0.0f, -4.5f*Mathf.Sin(Mathf.Deg2Rad*currentLeftPage.transform.eulerAngles.y));
	}

	// Use this for initialization
	void Start() 
	{
		ln = GetComponent<LudoNarrare>();
	}

	// Update is called once per frame
	void Update() 
	{
		if (fadeOut)
		{
			if (fadeSpeed != 0f)
				fadeTimer += Time.deltaTime / fadeSpeed;
			Image img = fadePanel.GetComponent<Image>();
			img.color = new Vector4(img.color.r, img.color.g, img.color.b, Mathf.Lerp(0f, 1f, fadeTimer));

			if (fadePanel.GetComponent<Image>().color.a == 1f)
			{
				fadeTimer = 0f;
				fadeOut = false;
				processExecute();
			}
		}

		if (fadeIn)
		{
			if (fadeSpeed != 0f)
				fadeTimer += Time.deltaTime / fadeSpeed;
			Image img = fadePanel.GetComponent<Image>();
			img.color = new Vector4(img.color.r, img.color.g, img.color.b, Mathf.Lerp(1f, 0f, fadeTimer));

			if (fadePanel.GetComponent<Image>().color.a == 0f)
			{
				fadeTimer = 0f;
				fadeIn = false;
				waitingForInput = true;
			}
		}

		if (!ready)
		{
			if (ln.getDone())
			{
				sw = ln.getStoryWorld();
				story = ln.getEngine().output;
				pageIndex = 0;
				drawPage(pageIndex);
				ready = true;
				setUIArguments(ln.getEngine().currentUserChoices[verbChoice].arguments.Count);
				redrawUI(ln.getEngine().currentUserChoices[verbChoice].arguments.Count);
				waitingForInput = true;
			}
		}
		else
		{
			if (previousLeftPage != null 
			    && previousRightPage != null 
			    && currentLeftPage != null 
			    && currentRightPage != null 
			    && nextLeftPage != null 
			    && nextRightPage != null
			    && previousPage != null
			    && currentPage != null
			    && nextPage != null
			    && story.Count != 0)
			{
				if (Input.GetMouseButton(0))
				{
					if (waitingForInput == true)
					{
						if (settledRight && !flippingLeft && !flippingRight && Input.mousePosition.x < Screen.width/2 && pageIndex != 0)
						{
							//Make sure to draw input page dummy whenever pages are moving
							if (story[pageIndex].isInputPage)
							{
								inputCanvas.transform.localScale = new Vector3(1.112f*inputScale, 1f*inputScale, 1f);
								inputCanvas.transform.position = new Vector3(0f, -30f, -1f);
							}

							//Pick up left page
							caughtAngleLeft = currentLeftPage.transform.eulerAngles;
							
							if (Mathf.Approximately(caughtAngleLeft.y, 0.0f))
								mouseHingeLeft = Input.mousePosition.x;
							else
								mouseHingeLeft = Input.mousePosition.x - ((caughtAngleLeft.y/179.9f) * (Screen.width/2.0f));
							
							flippingLeft = true; 
							settledLeft = false;
						}
						else if (settledLeft && !flippingRight && !flippingLeft && Input.mousePosition.x > Screen.width/2 && pageIndex != story.Count - 1)
						{
							//Pick up right page
							caughtAngleRight = currentRightPage.transform.eulerAngles;

							if (Mathf.Approximately(caughtAngleRight.y, 0.0f))
								mouseHingeRight = Input.mousePosition.x;
							else
								mouseHingeRight = Input.mousePosition.x + (((caughtAngleRight.y - 360.0f)/-179.9f) * (Screen.width/2.0f));

							flippingRight = true; 
							settledRight = false;
						}
						else if (flippingLeft && mouseHingeLeft - Input.mousePosition.x < 0)
						{
							//Pull back left page
							currentLeftPage.transform.eulerAngles = new Vector3(0.0f, 0.0f, 0.0f);
							previousRightPage.transform.eulerAngles = new Vector3(0.0f, 180.0f, 0.0f);
							
							float rotation = ((-(mouseHingeLeft - Input.mousePosition.x))/(Screen.width/2.0f)) * 179.9f;
							rotation = Mathf.Clamp(rotation, 0.0f, 179.9f);
							
							currentLeftPage.transform.Rotate(Vector3.up * rotation);
							previousRightPage.transform.Rotate(Vector3.up * rotation);
							
							currentLeftPage.transform.position = new Vector3(-4.5f*Mathf.Cos(Mathf.Deg2Rad*currentLeftPage.transform.eulerAngles.y), 0.0f, -4.5f*Mathf.Sin(Mathf.Deg2Rad*currentLeftPage.transform.eulerAngles.y));
							previousRightPage.transform.position = new Vector3(4.5f*Mathf.Cos(Mathf.Deg2Rad*previousRightPage.transform.eulerAngles.y), 0.0f, 4.5f*Mathf.Sin(Mathf.Deg2Rad*previousRightPage.transform.eulerAngles.y));
						}
						else if (flippingRight && mouseHingeRight - Input.mousePosition.x > 0)
						{
							//Pull back right page
							currentRightPage.transform.eulerAngles = new Vector3(0.0f, 0.0f, 0.0f);
							nextLeftPage.transform.eulerAngles = new Vector3(0.0f, 180.0f, 0.0f);

							float rotation = ((mouseHingeRight - Input.mousePosition.x)/(Screen.width/2.0f)) * -179.9f;
							rotation = Mathf.Clamp(rotation, -179.9f, 0.0f);

							currentRightPage.transform.Rotate(Vector3.up * rotation);
							nextLeftPage.transform.Rotate(Vector3.up * rotation);

							currentRightPage.transform.position = new Vector3(4.5f*Mathf.Cos(Mathf.Deg2Rad*currentRightPage.transform.eulerAngles.y), 0.0f, 4.5f*Mathf.Sin(Mathf.Deg2Rad*currentRightPage.transform.eulerAngles.y));
							nextLeftPage.transform.position = new Vector3(-4.5f*Mathf.Cos(Mathf.Deg2Rad*nextLeftPage.transform.eulerAngles.y), 0.0f, -4.5f*Mathf.Sin(Mathf.Deg2Rad*nextLeftPage.transform.eulerAngles.y));
						}
					}
				} 
				else
				{
					if (flippingLeft)
						flippingLeft = false;

					if (flippingRight)
						flippingRight = false;
				}

				if (!flippingLeft && !settledLeft)
				{
					//Let left page fall back
					if (currentLeftPage.transform.eulerAngles.y < 90.0f && !Mathf.Approximately(currentLeftPage.transform.eulerAngles.y,0.0f))
					{
						currentLeftPage.transform.Rotate(Vector3.down * fallSpeed * Time.deltaTime);
						previousRightPage.transform.Rotate(Vector3.down * fallSpeed * Time.deltaTime);
						
						currentLeftPage.transform.position = new Vector3(-4.5f*Mathf.Cos(Mathf.Deg2Rad*currentLeftPage.transform.eulerAngles.y), 0.0f, -4.5f*Mathf.Sin(Mathf.Deg2Rad*currentLeftPage.transform.eulerAngles.y));
						previousRightPage.transform.position = new Vector3(4.5f*Mathf.Cos(Mathf.Deg2Rad*previousRightPage.transform.eulerAngles.y), 0.0f, 4.5f*Mathf.Sin(Mathf.Deg2Rad*previousRightPage.transform.eulerAngles.y));
					}
					else if (currentLeftPage.transform.eulerAngles.y > 180.0f)
					{
						settledLeft = true;

						//Once settled remove input dummy
						if (story[pageIndex].isInputPage)
						{
							inputCanvas.transform.localScale = new Vector3(1f*inputScale, 1f*inputScale, 1f);
							inputCanvas.transform.position = new Vector3(0f, 0f, -1f);
						}

						currentLeftPage.transform.eulerAngles = new Vector3(0.0f, 0.0f, 0.0f);
						previousRightPage.transform.eulerAngles = new Vector3(0.0f, 180.0f, 0.0f);
						
						currentLeftPage.transform.position = new Vector3(-4.5f*Mathf.Cos(Mathf.Deg2Rad*currentLeftPage.transform.eulerAngles.y), 0.0f, -4.5f*Mathf.Sin(Mathf.Deg2Rad*currentLeftPage.transform.eulerAngles.y));
						previousRightPage.transform.position = new Vector3(4.5f*Mathf.Cos(Mathf.Deg2Rad*previousRightPage.transform.eulerAngles.y), 0.0f, 4.5f*Mathf.Sin(Mathf.Deg2Rad*previousRightPage.transform.eulerAngles.y));				
					}
					else if (currentLeftPage.transform.eulerAngles.y > 90.0f)
					{
						pageIndex--;
						drawPage(pageIndex);
						flipPageLeft(previousRightPage.transform.eulerAngles);
					}
				}

				if (!flippingRight && !settledRight)
				{	
					//Let right page fall back
					if (currentRightPage.transform.eulerAngles.y > 270.0f)
					{
						currentRightPage.transform.Rotate(Vector3.up * fallSpeed * Time.deltaTime);
						nextLeftPage.transform.Rotate(Vector3.up * fallSpeed * Time.deltaTime);

						currentRightPage.transform.position = new Vector3(4.5f*Mathf.Cos(Mathf.Deg2Rad*currentRightPage.transform.eulerAngles.y), 0.0f, 4.5f*Mathf.Sin(Mathf.Deg2Rad*currentRightPage.transform.eulerAngles.y));
						nextLeftPage.transform.position = new Vector3(-4.5f*Mathf.Cos(Mathf.Deg2Rad*nextLeftPage.transform.eulerAngles.y), 0.0f, -4.5f*Mathf.Sin(Mathf.Deg2Rad*nextLeftPage.transform.eulerAngles.y));
					}
					else if (currentRightPage.transform.eulerAngles.y > 180.0f)
					{
						pageIndex++;
						drawPage(pageIndex);
						flipPageRight(nextLeftPage.transform.eulerAngles);

						//Make sure to draw input page dummy whenever pages are moving
						if (story[pageIndex].isInputPage)
						{
							inputCanvas.transform.localScale = new Vector3(1.112f*inputScale, 1f*inputScale, 1f);
							inputCanvas.transform.position = new Vector3(0f, -30f, -1f);
						}
					}
					else if (currentRightPage.transform.eulerAngles.y < 180.0f)
					{
						settledRight = true;

						currentRightPage.transform.eulerAngles = new Vector3(0.0f, 0.0f, 0.0f);
						nextLeftPage.transform.eulerAngles = new Vector3(0.0f, 180.0f, 0.0f);

						currentRightPage.transform.position = new Vector3(4.5f*Mathf.Cos(Mathf.Deg2Rad*currentRightPage.transform.eulerAngles.y), 0.0f, 4.5f*Mathf.Sin(Mathf.Deg2Rad*currentRightPage.transform.eulerAngles.y));
						nextLeftPage.transform.position = new Vector3(-4.5f*Mathf.Cos(Mathf.Deg2Rad*nextLeftPage.transform.eulerAngles.y), 0.0f, -4.5f*Mathf.Sin(Mathf.Deg2Rad*nextLeftPage.transform.eulerAngles.y));				
					}
				}
			}
		}
	}
}