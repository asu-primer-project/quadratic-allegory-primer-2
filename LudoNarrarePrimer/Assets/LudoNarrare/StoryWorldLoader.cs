using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class StoryWorldLoader : MonoBehaviour
{
	/* Variables */
	public StoryWorld storyWorld;
	private int readLoc;
	private int endLoc;
	private string swText;
	private int type;
	private string currentToken;
	private int lineCount;
	private int tokenInt;

	/* Functions */
	//Constructor
	public StoryWorldLoader()
	{
		readLoc = 0;
		swText = "";
		endLoc = 0;
		lineCount = 1;
		tokenInt = 0;
	}

	//Lexer functions
	public bool isLetter(char c) 
	{
		char[] letters = new char[52] {'a','b','c','d','e','f','g','h','i','j','k','l','m','n','o','p','q','r','s','t','u','v','w','x','y','z',
			'A','B','C','D','E','F','G','H','I','J','K','L','M','N','O','P','Q','R','S','T','U','V','W','X','Y','Z'};
		for (int i = 0; i < letters.Length; i++) 
		{
			if (c == letters[i])
				return true;
		}
		return false;
	}
	
	public bool isNumeral(char c) 
	{
		char[] numerals = new char[10] {'0','1','2','3','4','5','6','7','8','9'};
		for (int i = 0; i < numerals.Length; i++) 
		{
			if (c == numerals[i])
				return true;
		}
		return false;
	}
	
	//Return 
	//0 - word, 1 - string, 2 - integer, 3 - variable
	//4 - left curly, 5 - right curly, 6 - colon
	//7 - semicolon, 8 - comma, 9 - equal
	//10 - not equal, 11 - less than, 12 - greater than
	//13 - less than equal to, 14 greater than equal to, 15 - plus
	//16 - minus, 17 - multiply, 18 - divide, 19 - period
	//20 - left paren, 21 - right paren
	//Return -1 - token incomplete, -2 unknown character, -3 end of file
	public int getToken() 
	{
		char c;
		currentToken = "";
		c = swText[readLoc];
		while (c == '\r' || c == '\n' || c == ' ' || c == '\t') 
		{ 
			if (c == '\n')
				lineCount++;

			readLoc++; 
			if (readLoc > endLoc)
			{
				type = -3;
				return -3;
			}
			c = swText[readLoc]; 
		}

		if (isLetter(c) || c == '_') 
		{
			currentToken += c.ToString();
			readLoc++;
			if (readLoc > endLoc)
			{
				type = 0;
				return 0;
			}
			c = swText[readLoc];

			while (isLetter(c) || c == '_' || isNumeral(c))
			{
				currentToken += c.ToString();
				readLoc++;
				if (readLoc > endLoc)
				{
					print("Read the word " + currentToken + "\n");
					type = 0;
					return 0;
				}
				c = swText[readLoc];
			}

			print("Read the word " + currentToken + "\n");
			type = 0;
			return 0;
		}
		else if (c == '"')
		{
			readLoc++;
			if (readLoc > endLoc)
			{
				type = -1;
				return -1;
			}
			c = swText[readLoc];

			while (c != '"')
			{
				currentToken += c.ToString();
				readLoc++;
				if (readLoc > endLoc)
				{
					type = -1;
					return -1;
				}
				c = swText[readLoc];
			}

			readLoc++;
			print("Read the string " + currentToken + "\n");
			type = 1;
			return 1;
		}
		else if (isNumeral(c))
		{
			currentToken += c.ToString();
			readLoc++;
			if (readLoc > endLoc)
			{
				type = 2;
				return 2;
			}
			c = swText[readLoc];

			while (isNumeral(c))
			{
				currentToken += c.ToString();
				readLoc++;
				if (readLoc > endLoc)
				{
					if (Int32.TryParse(currentToken, out tokenInt))
					{
						print("Read the integer " + tokenInt + "\n");
						type = 2;
						return 2;
					}
					else
						return -1;
				}
				c = swText[readLoc];
			}

			if (Int32.TryParse(currentToken, out tokenInt))
			{
				print("Read the integer " + tokenInt + "\n");
				type = 2;
				return 2;
			}
			else
				return -1;
		}
		else if (c == '?')
		{
			currentToken += c.ToString();
			readLoc++;
			if (readLoc > endLoc)
			{
				type = -1;
				return -1;
			}
			c = swText[readLoc];

			if (isLetter(c) || c == '_') 
			{
				currentToken += c.ToString();
				readLoc++;
				if (readLoc > endLoc)
				{
					type = 3;
					return 3;
				}
				c = swText[readLoc];
				
				while (isLetter(c) || c == '_' || isNumeral(c))
				{
					currentToken += c.ToString();
					readLoc++;
					if (readLoc > endLoc)
					{
						print("Read the variable " + currentToken + "\n");
						type = 3;
						return 3;
					}
					c = swText[readLoc];
				}

				print("Read the variable " + currentToken + "\n");
				type = 3;
				return 3;
			}

			type = -1;
			return -1;
		}
		else if (c == '{')
		{
			currentToken += c.ToString();
			readLoc++;
			if (readLoc > endLoc)
			{
				type = 4;
				return 4;
			}
			c = swText[readLoc];

			print("Read {\n");
			type = 4;
			return 4;
		}
		else if (c == '}')
		{
			currentToken += c.ToString();
			readLoc++;
			if (readLoc > endLoc)
			{
				type = 5;
				return 5;
			}
			c = swText[readLoc];

			print("Read }\n");
			type = 5;
			return 5;
		}
		else if (c == ':')
		{
			currentToken += c.ToString();
			readLoc++;
			if (readLoc > endLoc)
			{
				type = 6;
				return 6;
			}
			c = swText[readLoc];
			
			print("Read :\n");
			type = 6;
			return 6;
		}
		else if (c == ';')
		{
			currentToken += c.ToString();
			readLoc++;
			if (readLoc > endLoc)
			{
				type = 7;
				return 7;
			}
			c = swText[readLoc];
			
			print("Read ;\n");
			type = 7;
			return 7;
		}
		else if (c == ',')
		{
			currentToken += c.ToString();
			readLoc++;
			if (readLoc > endLoc)
			{
				type = 8;
				return 8;
			}
			c = swText[readLoc];
			
			print("Read ,\n");
			type = 8;
			return 8;
		}
		else if (c == '=')
		{
			currentToken += c.ToString();
			readLoc++;
			if (readLoc > endLoc)
			{
				type = 9;
				return 9;
			}
			c = swText[readLoc];
			
			print("Read =\n");
			type = 9;
			return 9;
		}
		else if (c == '!')
		{
			currentToken += c.ToString();
			readLoc++;
			if (readLoc > endLoc)
			{
				type = -1;
				return -1;
			}
			c = swText[readLoc];

			if (c == '=')
			{
				currentToken += c.ToString();
				readLoc++;
				if (readLoc > endLoc)
				{
					type = 10;
					return 10;
				}
				c = swText[readLoc];

				print("Read !=\n");
				type = 10;
				return 10;
			}

			type = -1;
			return -1;
		}
		else if (c == '<')
		{
			currentToken += swText[readLoc].ToString();
			readLoc++;
			if (readLoc > endLoc)
			{
				type = 11;
				return 11;
			}
			c = swText[readLoc];

			if (c == '=')
			{
				currentToken += c.ToString();
				readLoc++;
				if (readLoc > endLoc)
				{
					type = 13;
					return 13;
				}
				c = swText[readLoc];
				
				print("Read <=\n");
				type = 13;
				return 13;
			}

			print("Read <\n");
			type = 11;
			return 11;
		}
		else if (c == '>')
		{
			currentToken += swText[readLoc].ToString();
			readLoc++;
			if (readLoc > endLoc)
			{
				type = 12;
				return 12;
			}
			c = swText[readLoc];

			if (c == '=')
			{
				currentToken += c.ToString();
				readLoc++;
				if (readLoc > endLoc)
				{
					type = 14;
					return 14;
				}
				c = swText[readLoc];
				
				print("Read >=\n");
				type = 14;
				return 14;
			}

			print("Read >\n");
			type = 12;
			return 12;
		}
		else if (c == '+')
		{
			currentToken += swText[readLoc].ToString();
			readLoc++;
			if (readLoc > endLoc)
			{
				type = 15;
				return 15;
			}
			c = swText[readLoc];
			
			print("Read +\n");
			type = 15;
			return 15;
		}
		else if (c == '-')
		{
			currentToken += swText[readLoc].ToString();
			readLoc++;
			if (readLoc > endLoc)
			{
				type = 16;
				return 16;
			}
			c = swText[readLoc];

			if (isNumeral(c) && c != '0' )
			{
				currentToken += c.ToString();
				readLoc++;
				if (readLoc > endLoc)
				{
					type = 2;
					return 2;
				}
				c = swText[readLoc];
				
				while (isNumeral(c))
				{
					currentToken += c.ToString();
					readLoc++;
					if (readLoc > endLoc)
					{
						print("Read the integer " + currentToken + "\n");
						type = 2;
						return 2;
					}
					c = swText[readLoc];
				}
				
				print("Read the integer " + currentToken + "\n");
				type = 2;
				return 2;
			}
			
			print("Read -\n");
			type = 16;
			return 16;
		}
		else if (c == '*')
		{
			currentToken += swText[readLoc].ToString();
			readLoc++;
			if (readLoc > endLoc)
			{
				type = 17;
				return 17;
			}
			c = swText[readLoc];
			
			print("Read *\n");
			type = 17;
			return 17;
		}
		else if (c == '/')
		{
			currentToken += swText[readLoc].ToString();
			readLoc++;
			if (readLoc > endLoc)
			{
				type = 18;
				return 18;
			}
			c = swText[readLoc];

			if (c == '/')
			{
				readLoc++;
				if (readLoc > endLoc)
				{
					type = 14;
					return 14;
				}
				c = swText[readLoc];

				while (c != '\n')
				{
					readLoc++;
					if (readLoc > endLoc)
					{
						type = -3;
						return -3;
					}
					c = swText[readLoc];
				}

				print("Read comment\n");
				return getToken();
			}
			
			print("Read /\n");
			type = 18;
			return 18;
		}
		else if (c == '.')
		{
			currentToken += swText[readLoc].ToString();
			readLoc++;
			if (readLoc > endLoc)
			{
				type = 19;
				return 19;
			}
			c = swText[readLoc];
			
			print("Read .\n");
			type = 19;
			return 19;
		}
		else if (c == '(')
		{
			currentToken += swText[readLoc].ToString();
			readLoc++;
			if (readLoc > endLoc)
			{
				type = 20;
				return 20;
			}
			c = swText[readLoc];
			
			print("Read (\n");
			type = 20;
			return 20;
		}
		else if (c == ')')
		{
			currentToken += swText[readLoc].ToString();
			readLoc++;
			if (readLoc > endLoc)
			{
				type = 21;
				return 21;
			}
			c = swText[readLoc];
			
			print("Read )\n");
			type = 21;
			return 21;
		}
		else
		{
			type = -2;
			return -2;
		}
	}
			
	//Parser Functions
	//Parse expression
	public bool parseExpression(Expression e)
	{
		getToken();

		if (type == 2)
		{
			e.type = 0;
			e.number = tokenInt;
			return false;
		}
		else if (type == 20)
		{
			e.leftExp = new Expression(0);
			if (parseExpression(e.leftExp))
				return true;

			switch (getToken())
			{
			case 15: e.op = 0; break;
			case 16: e.op = 1; break;
			case 17: e.op = 2; break;
			case 18: e.op = 3; break;
			}

			e.rightExp = new Expression(0);
			if (parseExpression(e.rightExp))
				return true;

			if (getToken() == 21)
				return false;
		}
		else if (type == 0 || type == 3)
		{
			e.entRef = currentToken;

			if (getToken() == 0)
			{
				e.numRef = currentToken;
				return false;
			}
		}

		return true;
	}

	//Parse draw image def
	public bool parseDrawDef(DrawInstruction d)
	{
		if (getToken() == 6)
		{
			getToken();

			if (type == 0 || type == 3)
			{
				d.entity = currentToken;

				if (getToken() == 0)
				{
					if (currentToken == "image")
					{
						if (getToken() == 0)
						{
							d.image = currentToken;

							if (getToken() == 8)
							{
								d.x = new Expression(0);
								if (parseExpression(d.x))
									return true;

								if (getToken() == 8)
								{
									d.y = new Expression(0);
									if (parseExpression(d.y))
										return true;

									if (getToken() == 7)
										return false;
								}
							}
						}
					}
				}
			}
		}
		return true;
	}

	//Parse draw text def
	public bool parseTextDef(DrawInstruction d)
	{
		if (getToken() == 6)
		{
			if (getToken() == 1)
			{
				d.text = currentToken;

				if (getToken() == 7)
					return false;
			}
		}
		return true;
	}

	//Parse Condition
	public bool parseCondition(Condition c)
	{
		getToken();

		if (type == 3)
		{
			c.atomic = new AtomicCondition();
			return parseAtomicCondition(c.atomic);
		}
		else if (type == 0)
		{
			if (currentToken == "not")
			{
				c.notFlag = true;
				c.left = new Condition();
				if (parseCondition(c.left))
					return true;
				return false;
			}
			else
			{
				c.atomic = new AtomicCondition();
				return parseAtomicCondition(c.atomic);
			}
		}
		else if (type == 20)
		{
			c.left = new Condition();
			if (parseCondition(c.left))
				return true;

			if (getToken() == 0)
			{
				if (currentToken == "and")
				{
					c.andFlag = true;
					c.right = new Condition();
					if (parseCondition(c.right))
						return true;

					if (getToken() == 21)
						return false;
				}
				else if (currentToken == "or")
				{
					c.orFlag = true;
					c.right = new Condition();
					if (parseCondition(c.right))
						return true;

					if (getToken() == 21)
						return false;
				}
			}
		}

		return true;
	}

	//Parse Atomic Condition
	public bool parseAtomicCondition(AtomicCondition c)
	{
		string tempRef = "";

		if (type == 0 || type == 3)
		{
			if (currentToken == "one")
			{
				c.allCS = false;
				
				getToken();
				if (type == 3)
					c.conditionSubject = currentToken;
				else return true;
			}
			else if (currentToken == "all")
			{
				c.allCS = true;
				
				getToken();
				if (type == 3)
					c.conditionSubject = currentToken;
				else return true;
			}
			else
				c.conditionSubject = currentToken;
			
			if (getToken() != 0)
				return true;

			if (currentToken == "has")
			{
				if (currentToken == "has")
					c.comparison = 0;

				if (getToken() != 0)
					return true;

				if (currentToken == "tag")
				{
					if (getToken() == 0)
					{
						c.tagRef = currentToken;
						return false;
					}
				}
				if (currentToken == "relate")
				{
					if (getToken() == 0)
					{
						c.relateRef = currentToken;
						return false;
					}						
				}
				if (currentToken == "string")
				{
					if (getToken() == 0)
					{
						c.stringRef = currentToken;
						return false;
					}						
				}
				if (currentToken == "num")
				{
					if (getToken() == 0)
					{
						c.numRef = currentToken;
						return false;
					}						
				}
			}
			else if (currentToken == "empty")
			{
				c.comparison = 7;
				return false;
			}
			else if (currentToken == "same")
			{
				c.comparison = 8;
				if (getToken() == 3)
				{
					c.conditionObject = currentToken;
					return false;
				}
			}
			else
			{
				tempRef = currentToken;

				getToken();
				if (type == 0 && currentToken == "matches")
				{
					c.stringRef = tempRef;
					c.comparison = 9;

					getToken();
					if (type == 1)
					{
						c.stringCompare = currentToken;
						return false;
					}
					else if (type == 0)
					{
						if (currentToken == "one")
						{
							c.allCO = false;
							
							getToken();
							if (type == 3)
								c.conditionObject = currentToken;
							else return true;
						}
						else if (currentToken == "all")
						{
							c.allCO = true;
							
							getToken();
							if (type == 3)
								c.conditionObject = currentToken;
							else return true;
						}
						else
							c.conditionObject = currentToken;

						if (getToken() != 0)
							return true;
						c.stringRef2 = currentToken;
						return false;
					}
				}
				else if (type >= 9 && type <= 14)
				{
					c.numRef = tempRef;

					switch(type)
					{
					case 9: c.comparison = 1; break;
					case 10: c.comparison = 2; break;
					case 11: c.comparison = 3; break;
					case 12: c.comparison = 4; break;
					case 13: c.comparison = 5; break;
					case 14: c.comparison = 6; break;
					}

					c.numCompare = new Expression(0);
					if (parseExpression(c.numCompare))
						return true;
					return false;
				}
				else if (type == 0)
				{
					c.comparison = 10;
					c.relateRef = tempRef;

					if (currentToken == "one")
					{
						c.allCO = false;
						
						getToken();
						if (type == 3)
							c.conditionObject = currentToken;
						else return true;
					}
					else if (currentToken == "all")
					{
						c.allCO = true;
						
						getToken();
						if (type == 3)
							c.conditionObject = currentToken;
						else return true;
					}
					else
						c.conditionObject = currentToken;
					return false;
				}
			}
		}
		
		return true;
	}

	//Parse Operator
	public bool parseOperator(Operator o)
	{
		if (getToken() == 6)
		{
			getToken();

			if (type == 0 || type == 3)
			{
				o.operatorSubject = currentToken;

				if (getToken() != 0)
					return true;

				if (currentToken == "add")
				{
					o.op = 0;

					if (getToken() != 0)
						return true;
					if (currentToken == "tag")
					{
						if (getToken() == 0)
						{
							o.tagRef = currentToken;
							if (getToken() == 7) return false;
						}
					}
					else if (currentToken == "relate")
					{
						if (getToken() == 0)
						{
							o.relateRef = currentToken;

							if (getToken() == 8)
							{
								getToken();
								if (type == 0 || type == 3)
								{
									o.relateObj = currentToken;
									if (getToken() == 7) return false;
								}
							}
						}
					}
					else if (currentToken == "string")
					{
						if (getToken() == 0)
						{
							o.stringRef = currentToken;
							
							if (getToken() == 8)
							{
								getToken();
								if (type == 2)
								{
									o.stringValue = currentToken;
									if (getToken() == 7) return false;
								}
							}
						}
					}
					else if (currentToken == "num")
					{
						if (getToken() == 0)
						{
							o.numRef = currentToken;
							
							if (getToken() == 8)
							{
								o.num = new Expression(0);
								if (parseExpression(o.num))
									return true;
								if (getToken() == 7) return false;
							}
						}
					}
				}
				else if (currentToken == "remove")
				{
					o.op = 1;

					if (getToken() != 0)
						return true;
					if (currentToken == "tag")
					{
						if (getToken() == 0)
						{
							o.tagRef = currentToken;
							if (getToken() == 7) return false;
						}
					}
					else if (currentToken == "relate")
					{
						if (getToken() == 0)
						{
							o.relateRef = currentToken;
							
							if (getToken() == 8)
							{
								getToken();
								if (type == 0 || type == 3)
								{
									o.relateObj = currentToken;
									if (getToken() == 7) return false;
								}
							}
						}
					}
					else if (currentToken == "string")
					{
						if (getToken() == 0)
						{
							o.stringRef = currentToken;
							if (getToken() == 7) return false;
						}
					}
					else if (currentToken == "num")
					{
						if (getToken() == 0)
						{
							o.numRef = currentToken;
							if (getToken() == 7) return false;
						}
					}
				}
				else if (currentToken == "change")
				{
					o.op = 2;
					getToken();
					if (type == 0 && currentToken == "image")
					{
						if (getToken() == 0)
						{
							o.imageRef = currentToken;

							getToken();
							if (type == 0 && currentToken == "to")
							{
								o.image = currentToken;
								if (getToken() == 7) return false;
							}
						}
					}
				}
			}
		}
		return true;
	}

	//Parse Page
	public bool parsePage(Page p)
	{
		bool error = false;

		if (getToken() == 0)
		{
				p.name = currentToken;

				if (getToken() == 4)
				{
					while (getToken() != 5)
					{
						if (type == 0 && !error)
						{
							if (currentToken == "draw")
							{
								DrawInstruction temp = new DrawInstruction(false,"","","");
								p.drawList.Add(temp);
								error = parseDrawDef(temp);
							}
							else if (currentToken == "text")
							{
								DrawInstruction temp = new DrawInstruction(true,"","","");
								p.drawList.Add(temp);
								error = parseTextDef(temp);
							}
							else
								return true;
						}
						else if (type != 5 || error)
							return true;
					}
					return false;
				}
		}
		return true;
	}
	
	//Parse Beginning Pages
	public bool parseBeginning(StoryWorld sw)
	{
		bool error = false;

		if (getToken() == 4)
		{
			while (getToken() != 5)
			{
				if (type == 0 && !error)
				{
					if (currentToken == "page")
					{
						Page temp = new Page("");
						sw.beginning.Add(temp);
						error = parsePage(temp);
					}
					else
						return true;
				}
				else if (type != 5 || error)
					return true;
			}
			return false;
		}
		return true;
	}	

	//Parse Tag
	public bool parseTag(Tag t)
	{
		if (getToken() == 6)
		{
			if (getToken() == 0)
			{
				t.name = currentToken;

				if (getToken() == 7)
					return false;
			}
		}
		return true;
	}

	//Parse Relationship
	public bool parseRelationship(Relationship r)
	{
		if (getToken() == 6)
		{
			if (getToken() == 0)
			{
				r.name = currentToken;

				if (getToken() == 8)
				{
					if (getToken() == 0)
					{
						r.other = currentToken;

						if (getToken() == 7)
							return false;
					}
				}
			}
		}
		return true;
	}

	//Parse Number
	public bool parseNumber(Number n)
	{
		if (getToken() == 6)
		{
			if (getToken() == 0)
			{
				n.name = currentToken;

				if (getToken() == 8)
				{
					if (getToken() == 2)
					{
						n.value = tokenInt;

						if (getToken() == 7)
							return false;
					}
				}
			}
		}
		return true;
	}

	//Parse String
	public bool parseString(LNString s)
	{
		if (getToken() == 6)
		{
			if (getToken() == 0)
			{
				s.name = currentToken;

				if (getToken() == 8)
				{
					if (getToken() == 1)
					{
						s.text = currentToken;

						if (getToken() == 7)
							return false;
					}
				}
			}
		}
		return true;
	}

	//Parse Image
	public bool parseImage(ImageDef i)
	{
		if (getToken() == 6)
		{
			if (getToken() == 0)
			{
				i.name = currentToken;

				if (getToken() == 8)
				{
					if (getToken() == 0)
					{
						i.image = currentToken;

						if (getToken() == 7)
							return false;
					}
				}
			}
		}
		return true;
	}

	//Parse Icon text
	public bool parseIcon(Entity e, Verb v)
	{
		if (getToken() == 6)
		{
			if (getToken() == 1)
			{
				if (e != null)
				{
					e.it = new IconText(currentToken);

					if (getToken() == 8)
					{
						e.it.red = new Expression(0);
						if (parseExpression(e.it.red))
							return true;

						if (getToken() == 8)
						{
							e.it.green = new Expression(0);
							if (parseExpression(e.it.green))
								return true;

							if (getToken() == 8)
							{
								e.it.blue = new Expression(0);
								if (parseExpression(e.it.blue))
									return true;

								if (getToken() == 7)
									return false;
							}
						}
					}
				}
				else if (v != null)
				{
					v.it = new IconText(currentToken);
					
					if (getToken() == 8)
					{
						v.it.red = new Expression(0);
						if (parseExpression(v.it.red))
							return true;
						
						if (getToken() == 8)
						{
							v.it.green = new Expression(0);
							if (parseExpression(v.it.green))
								return true;
							
							if (getToken() == 8)
							{
								v.it.blue = new Expression(0);
								if (parseExpression(v.it.blue))
									return true;
								
								if (getToken() == 7)
									return false;
							}
						}
					}
				}
			}
		}
		return true;
	}

	//Parse Agent
	public bool parseAgent(Entity e)
	{
		if (getToken() == 6)
		{
			if (getToken() == 0)
			{
				//Any AI's written need to be referenced here:
				if (currentToken == "random")
					e.agent = new MindRandom();
				else if (currentToken == "user")
					e.agent = new MindUser();
				else
					return true;

				if (getToken() == 7)
					return false;
			}
		}

		return true;
	}

	//Parse Entity
	public bool parseEntity(Entity e)
	{
		bool error = false;

		if (getToken() == 0)
		{
			e.name = currentToken;

			if (getToken() == 4)
			{
				while (getToken() != 5)
				{
					if (type == 0 && !error)
					{
						if (currentToken == "tag")
						{
							Tag temp = new Tag("");
							e.tags.Add(temp);
							error = parseTag(temp);
						}
						else if (currentToken == "relate")
						{
							Relationship temp = new Relationship("","");
							e.relationships.Add(temp);
							error = parseRelationship(temp);
						}
						else if (currentToken == "num")
						{
							Number temp = new Number("",0);
							e.numbers.Add(temp);
							error = parseNumber(temp);
						}
						else if (currentToken == "string")
						{
							LNString temp = new LNString("","");
							e.strings.Add(temp);
							error = parseString(temp);
						}
						else if (currentToken == "image")
						{
							ImageDef temp = new ImageDef("","");
							e.images.Add(temp);
							error = parseImage(temp);
						}
						else if (currentToken == "icon")
							error = parseIcon(e, null);
						else if (currentToken == "agent")
							error = parseAgent(e);
						else
							return true;
					}
					else if (type != 5 || error)
						return true;
				}
				return false;
			}
		}
		return true;
	}

	//Parse Variable
	public bool parseVariable(Variable v)
	{
		if (getToken() == 3)
		{
			v.name = currentToken;

			if (getToken() == 4)
			{
				while (getToken() != 5)
				{
					if (type == 0)
					{
						if (currentToken == "where")
						{
							if (getToken() == 6)
							{
								Condition temp = new Condition();
								v.conditions.Add(temp);
								bool error = false;
								error = parseCondition(temp);
								if (error) return true;
								if (getToken() != 7) return true;
							}
							else
								return true;
						}
						else return true;
					}
					else return true;
				}
				return false;
			}
		}
		return true;
	}

	//Parse Argument
	public bool parseArgument(Argument a)
	{
		if (getToken() == 3)
		{
			a.name = currentToken;
			
			if (getToken() == 4)
			{
				while (getToken() != 5)
				{
					if (type == 0)
					{
						if (currentToken == "where")
						{
							if (getToken() == 6)
							{
								Condition temp = new Condition();
								a.conditions.Add(temp);
								bool error = false;
								error = parseCondition(temp);
								if (error) return true;
								if (getToken() != 7) return true;
							}
							else
								return true;
						}
						else if (currentToken == "text")
						{
							if (getToken() == 6)
							{
								if (getToken() == 1)
								{
									a.text = currentToken;
									
									if (getToken() != 7)
										return true;
								}
							}
						}
						else return true;
					}
					else return true;
				}
				return false;
			}
		}
		return true;
	}

	//Parse Preconditions
	public bool parsePreconditions(Verb v)
	{
		if (getToken() == 4)
		{
			while (getToken() != 5)
			{
				if (type == 0)
				{
					if (currentToken == "where")
					{
						if (getToken() == 6)
						{
							Condition temp = new Condition();
							v.preconditions.Add(temp);
							bool error = false;
							error = parseCondition(temp);
							if (error) return true;
							if (getToken() != 7) return true;
						}
						else
							return true;
					}
					else return true;
				}
				else return true;
			}
			return false;
		}
		return true;
	}

	//Parse case
	public bool parseCase(Case c)
	{
		if (getToken() == 0)
		{
			c.name = currentToken;

			if (getToken() == 4)
			{
				while (getToken() != 5)
				{
					if (type == 0)
					{
						if (currentToken == "where")
						{
							if (getToken() == 6)
							{
								Condition temp = new Condition();
								c.conditions.Add(temp);
								if (parseCondition(temp))
									return true;
								if (getToken() != 7) return true;
							}
							else
								return true;
						}
						else if (currentToken == "do")
						{
							Operator temp = new Operator();
							c.operators.Add(temp);
							if (parseOperator(temp)) 
								return true;
						}
						else if (currentToken == "page")
						{
							Page temp = new Page("");
							c.pages.Add(temp);
							if (parsePage(temp))
								return true;
						}
						else return true;
					} 
					else return true;
				}
				return false;
			}
		}
		return true;
	}

	//Parse Discriminator
	public bool parseDiscriminator(Discriminator d)
	{
		if (getToken() == 0)
		{
			if (currentToken == "never")
			{
				d.name = currentToken;

				if (getToken() == 7)
					return false;
			}
			else
			{
				d.name = currentToken;
				
				if (getToken() == 4)
				{
					while (getToken() != 5)
					{
						if (type == 0)
						{
							if (currentToken == "where")
							{
								if (getToken() == 6)
								{
									Condition temp = new Condition();
									d.conditions.Add(temp);
									if (parseCondition(temp))
										return true;
									if (getToken() != 7) return true;
								}
								else
									return true;
							}
							else return true;
						}
						else return true;
					}
					return false;
				}
			}
		}
		return true;
	}
	
	//Parse Verb
	public bool parseVerb(Verb v)
	{
		bool error = false;
		
		if (getToken() == 0)
		{
			v.name = currentToken;
			
			if (getToken() == 4)
			{
				while (getToken() != 5)
				{
					if (type == 0 && !error)
					{
						if (currentToken == "icon")
							error = parseIcon(null, v);
						else if (currentToken == "variable")
						{
							Variable temp = new Variable("");
							v.variables.Add(temp);
							error = parseVariable(temp);
						}
						else if (currentToken == "argument")
						{
							Argument temp = new Argument("", "");
							v.arguments.Add(temp);
							error = parseArgument(temp);							
						}
						else if (currentToken == "preconditions")
							error = parsePreconditions(v);		
						else if (currentToken == "case")
						{
							Case temp = new Case("");
							v.cases.Add(temp);
							error = parseCase(temp);
						}
						else if (currentToken == "discriminator")
						{
							Discriminator temp = new Discriminator("");
							v.discriminators.Add(temp);
							error = parseDiscriminator(temp);						
						}
						else
							return true;
					}
					else if (type != 5 || error)
						return true;
				}
				return false;
			}
		}
		return true;
	}

	//Parse Ending Pages
	public bool parseEnding(Ending e)
	{
		bool error = false;

		if (getToken() == 0)
		{
			e.name = currentToken;

			if (getToken() == 4)
			{
				while (getToken() != 5)
				{
					if (type == 0 && !error)
					{
						if (currentToken == "page")
						{
							Page temp = new Page("");
							e.pages.Add(temp);
							error = parsePage(temp);
						}
						else if (currentToken == "where")
						{
							if (getToken() == 6)
							{
								Condition temp = new Condition();
								e.conditions.Add(temp);
								if (parseCondition(temp)) return true;
								if (getToken() != 7) return true;
							}
							else
								return true;
						}
						else
							return true;
					}
					else if (type != 5 || error)
						return true;
				}
				return false;
			}
		}
		return true;
	}
	
	//Parse Story World
	public bool parseStoryWorld(StoryWorld sw)
	{
		bool error = false;

		if (getToken() == 0)
		{
			if (currentToken == "storyworld")
			{
				if (getToken() == 0)
				{
					sw.name = currentToken;

					if (getToken() == 4)
					{
						while (getToken() != 5)
						{
							if (type == 0 && !error)
							{
								if (currentToken == "beginning")
									error = parseBeginning(sw);
								else if (currentToken == "entity")
								{
									Entity temp = new Entity("");
									sw.entities.Add(temp);
									error = parseEntity(temp);
								}
								else if (currentToken == "verb")
								{
									Verb temp = new Verb("");
									sw.verbs.Add(temp);
									error = parseVerb(temp);
								}
								else if (currentToken == "ending")
								{
									Ending temp = new Ending("");
									sw.endings.Add(temp);
									error = parseEnding(temp);
								}
								else
									return true;
							}
							else if (type != 5 || error)
								return true;
						}
						return false;
					}
				}
			}
		}
		return true;
	}
	
	//Load story world file
	public int readStoryWorld(TextAsset swFile) 
	{
		swText = swFile.text;
		endLoc = swText.Length - 1;
		storyWorld = new StoryWorld("");

		if (!parseStoryWorld(storyWorld))
			return -1;
		else
			return lineCount;
	}
}
