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

	/* Functions */
	//Constructor
	public StoryWorldLoader()
	{
		readLoc = 0;
		swText = "";
		endLoc = 0;
		lineCount = 1;
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
		else
		{
			type = -2;
			return -2;
		}
	}
			
	//Parser Functions
	//Parse User Entity Reference
	public bool parseUser(StoryWorld sw)
	{
		if (getToken() == 6)
		{
			if (getToken() == 0)
			{
				sw.userEntity = currentToken;

				if (getToken() == 7)
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
			if (getToken() == 0)
			{
				d.entity = currentToken;

				if (getToken() == 19)
				{
					if (getToken() == 0)
					{
						d.image = currentToken;

						if (getToken() == 8)
						{
							if (getToken() == 2)
							{
								if (Int32.TryParse(currentToken, out d.x))
								{
									if (getToken() == 8)
									{
										if (getToken() == 2)
										{
											if (Int32.TryParse(currentToken, out d.y))
											{
												if (getToken() == 7)
													return false;
											}
											else
												return true;
										}
									}
								}
								else
									return true;
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
		string tempRef = "";
		
		if (getToken() == 6)
		{
			getToken();
			bool gotAO = false;

			if (type == 0 && currentToken == "one")
			{
				c.allCS = false;
				gotAO = true;
				getToken();
			}
			else if (type == 0 && currentToken == "all")
			{
				c.allCS = true;
				gotAO = true;
				getToken();
			}

			if (type == 0 || type == 3)
			{
				c.conditionSubject = currentToken;
				int firstType = type;
				getToken();
				
				if (currentToken == "has" || currentToken == "missing")
				{
					if (currentToken == "has")
						c.comparison = 0;
					else 
						c.comparison = 1;
					getToken();
					
					if (currentToken == "tag")
					{
						if (getToken() == 0)
							c.tagRef = currentToken;
					}
					else if (currentToken == "relate")
					{
						if (getToken() == 0)
							c.relateRef = currentToken;
					}
					else if (currentToken == "num")
					{
						if (getToken() == 0)
							c.numRef = currentToken;
					}
					else if (currentToken == "string")
					{
						if (getToken() == 0)
							c.stringRef = currentToken;
					}
					else if (currentToken == "obligation")
					{
						if (getToken() == 0)
							c.obligationRef = currentToken;
					}
					else if (currentToken == "goal")
					{
						if (getToken() == 0)
							c.goalRef = currentToken;
					}
					else if (currentToken == "behavior")
					{
						if (getToken() == 0)
							c.behaviorRef = currentToken;
					}
					else
						return true;

					if (getToken() == 7)
						return false;
				}
				else if (firstType == 3 && !gotAO && (type == 9 || type == 10))
				{
					if (type == 9)
						c.comparison = 2;
					else
						c.comparison = 3;

					getToken();

					if (type == 0 && currentToken == "null")
					{
						c.variableObject = "?null";
					}
					else if (type == 3)
					{
						c.variableObject = currentToken;
					}

					if (getToken() == 7)
						return false;
				}
				else if (type == 19)
				{
					if (getToken() == 0)
					{
						tempRef = currentToken;
						getToken();
						
						if (type == 9 || type == 10)
						{
							if (type == 9)
								c.comparison = 2;
							else
								c.comparison = 3;
							
							getToken();
							
							if (type == 0)
							{
								if (currentToken == "one")
								{
									c.allRO = false;

									if (getToken() == 3)
									{
										c.relateRef = tempRef;
										c.relateObject = currentToken;
										
										if (getToken() == 7)
											return false;
									}
								}
								else if (currentToken == "all")
								{
									c.allRO = true;

									if (getToken() == 3)
									{
										c.relateRef = tempRef;
										c.relateObject = currentToken;
										
										if (getToken() == 7)
											return false;
									}
								}
								else
								{
									c.relateRef = tempRef;
									c.relateObject = currentToken;
									
									if (getToken() == 7)
										return false;
								}
							}
							else if (type == 1)
							{
								c.stringRef = tempRef;
								c.stringCompare = currentToken;
								
								if (getToken() == 7)
									return false;
							}
							else if (type == 2)
							{
								c.numRef = tempRef;
								if (Int32.TryParse(currentToken, out c.numCompare))
								{
									if (getToken() == 7)
										return false;
								}
								else
									return true;
							}
							else
								return true;
						}
						else if (getToken() == 11)
						{
							c.comparison = 4;
							
							if (getToken() == 2)
							{
								c.numRef = tempRef;
								if (Int32.TryParse(currentToken, out c.numCompare))
								{
									if (getToken() == 7)
										return false;
								}
								else
									return true;
							}
						}
						else if (getToken() == 12)
						{
							c.comparison = 5;
							
							if (getToken() == 2)
							{
								c.numRef = tempRef;
								if (Int32.TryParse(currentToken, out c.numCompare))
								{
									if (getToken() == 7)
										return false;
								}
								else
									return true;
							}
						}
						else if (getToken() == 13)
						{
							c.comparison = 6;
							
							if (getToken() == 2)
							{
								c.numRef = tempRef;
								if (Int32.TryParse(currentToken, out c.numCompare))
								{
									if (getToken() == 7)
										return false;
								}
								else
									return true;
							}
						}
						else if (getToken() == 14)
						{
							c.comparison = 7;
							
							if (getToken() == 2)
							{
								c.numRef = tempRef;
								if (Int32.TryParse(currentToken, out c.numCompare))
								{
									if (getToken() == 7)
										return false;
								}
								else
									return true;
							}
						}
						else
							return true;
					}
				}
				else
					return true;
			}
		}
		return true;
	}

	//Parse Goal Operator
	public bool parseGoalOperator(Goal g)
	{
		getToken();

		if (type == 0 || type == 3)
		{
			g.operatorSubject = currentToken;
			
			if (getToken() == 0)
			{
				if (currentToken == "add" || currentToken == "remove")
				{
					if (currentToken == "add")
						g.op = 0;
					else
						g.op = 1;
					
					if (getToken() == 0)
					{
						if (currentToken == "tag")
						{
							if (getToken() == 0)
							{
								g.tag = new Tag(currentToken);
								
								if (getToken() == 7)
									return false;
							}
						}
						else if (currentToken == "relate")
						{
							if (getToken() == 0)
							{
								g.relationship = new Relationship(currentToken, "", false);
								
								if (getToken() == 8)
								{
									getToken();

									if (type == 0 || type == 3)
									{
										g.relationship.other = currentToken;
										
										if (getToken() == 7)
											return false;
									}
								}
							}
						}
						else if (currentToken == "num")
						{
							if (getToken() == 0)
							{
								g.num = new Number(currentToken, 0);
								
								if (g.op == 0)
								{
									if (getToken() == 8)
									{
										if (getToken() == 2)
										{
											if (Int32.TryParse(currentToken, out g.num.value))
											{
												if (getToken() == 7)
													return false;
											}
										}
									}
								}
								else
								{
									if (getToken() == 7)
										return false;
								}
							}
						}
						else if (currentToken == "string")
						{
							if (getToken() == 0)
							{
								g.lnString = new LNString(currentToken, "");
								
								if (g.op == 0)
								{
									if (getToken() == 8)
									{
										if (getToken() == 1)
										{
											g.lnString.text = currentToken;
											
											if (getToken() == 7)
												return false;
										}
									}
								}
								else
								{
									if (getToken() == 7)
										return false;
								}
							}
						}
						else if (currentToken == "obligation")
						{
							if (getToken() == 0)
							{
								g.obligation = new Obligation(currentToken, "");
								
								if (g.op == 0)
								{
									if (getToken() == 8)
									{
										if (getToken() == 0)
										{
											g.obligation.verb = currentToken;
											
											while (getToken() != 7)
											{
												if (type == 8)
												{
													getToken();

													if (type == 0 || type == 3)
														g.obligation.arguments.Add(currentToken);
													else return true;
												}
												else return true;
											}
											return false;
										}
									}
								}
								else
								{
									if (getToken() == 7)
										return false;
								}
							}
						}
						else if (currentToken == "behavior")
						{
							if (getToken() == 0)
							{
								g.behavior = new Behavior(currentToken, "", 0);
								
								if (g.op == 0)
								{
									if (getToken() == 8)
									{
										if (getToken() == 0)
										{
											g.behavior.verb = currentToken;
											
											if (getToken() == 8)
											{
												if (getToken() == 2)
												{
													if (Int32.TryParse(currentToken, out g.behavior.chance))
													{
														while (getToken() != 7)
														{
															if (type == 8)
															{
																getToken();

																if (type == 0 || type == 3)
																	g.behavior.arguments.Add(currentToken);
																else return true;
															}
															else return true;
														}
														return false;
													}
												}
											}
										}
									}
								}
								else
								{
									if (getToken() == 7)
										return false;
								}
							}
						}
					}
				}
				else
				{
					g.num = new Number(currentToken, 0);
					getToken();
					
					if (type == 15)
					{
						g.op = 2;
						
						if (getToken() == 2)
						{
							if (Int32.TryParse(currentToken, out g.num.value))
							{
								if (getToken() == 7)
									return false;
							}
						}
					}
					else if (type == 16)
					{
						g.op = 3;
						
						if (getToken() == 2)
						{
							if (Int32.TryParse(currentToken, out g.num.value))
							{
								if (getToken() == 7)
									return false;
							}
						}
					}
					else if (type == 17)
					{
						g.op = 4;
						
						if (getToken() == 2)
						{
							if (Int32.TryParse(currentToken, out g.num.value))
							{
								if (getToken() == 7)
									return false;
							}
						}
					}
					else if (type == 18)
					{
						g.op = 5;
						
						if (getToken() == 2)
						{
							if (Int32.TryParse(currentToken, out g.num.value))
							{
								if (getToken() == 7)
									return false;
							}
						}
					}
					else if (type == 9)
					{
						g.op = 6;
						
						if (getToken() == 2)
						{
							if (Int32.TryParse(currentToken, out g.num.value))
							{
								if (getToken() == 7)
									return false;
							}
						}
					}
					else return true;
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

				if (getToken() == 0)
				{
					if (currentToken == "add" || currentToken == "remove")
					{
						if (currentToken == "add")
							o.op = 0;
						else
							o.op = 1;

						if (getToken() == 0)
						{
							if (currentToken == "tag")
							{
								if (getToken() == 0)
								{
									o.tag = new Tag(currentToken);

									if (getToken() == 7)
										return false;
								}
							}
							else if (currentToken == "relate")
							{
								if (getToken() == 0)
								{
									o.relationship = new Relationship(currentToken, "", false);

									if (getToken() == 8)
									{
										getToken();

										if (type == 0 || type == 3)
										{
											o.relationship.other = currentToken;

											if (getToken() == 7)
												return false;
										}
									}
								}
							}
							else if (currentToken == "num")
							{
								if (getToken() == 0)
								{
									o.num = new Number(currentToken, 0);

									if (o.op == 0)
									{
										if (getToken() == 8)
										{
											if (getToken() == 2)
											{
												if (Int32.TryParse(currentToken, out o.num.value))
												{
													if (getToken() == 7)
														return false;
												}
											}
										}
									}
									else
									{
										if (getToken() == 7)
											return false;
									}
								}
							}
							else if (currentToken == "string")
							{
								if (getToken() == 0)
								{
									o.lnString = new LNString(currentToken, "");

									if (o.op == 0)
									{
										if (getToken() == 8)
										{
											if (getToken() == 1)
											{
												o.lnString.text = currentToken;

												if (getToken() == 7)
													return false;
											}
										}
									}
									else
									{
										if (getToken() == 7)
											return false;
									}
								}
							}
							else if (currentToken == "obligation")
							{
								if (getToken() == 0)
								{
									o.obligation = new Obligation(currentToken, "");

									if (o.op == 0)
									{
										if (getToken() == 8)
										{
											if (getToken() == 0)
											{
												o.obligation.verb = currentToken;

												while (getToken() != 7)
												{
													if (type == 8)
													{
														getToken();

														if (type == 0 || type == 3)
															o.obligation.arguments.Add(currentToken);
														else return true;
													}
													else return true;
												}
												return false;
											}
										}
									}
									else
									{
										if (getToken() == 7)
											return false;
									}
								}
							}
							else if (currentToken == "goal")
							{
								if (getToken() == 0)
								{
									o.goal = new Goal(currentToken, "", 0);

									if (o.op == 0)
									{
										if (getToken() == 8)
										{
											bool error = false;
											error = parseGoalOperator(o.goal);
											if (error)
												return true;
											else
												return false;
										}
									}
									else
									{
										if (getToken() == 7)
											return false;
									}
								}
							}
							else if (currentToken == "behavior")
							{
								if (getToken() == 0)
								{
									o.behavior = new Behavior(currentToken, "", 0);

									if (o.op == 0)
									{
										if (getToken() == 8)
										{
											if (getToken() == 0)
											{
												o.behavior.verb = currentToken;

												if (getToken() == 8)
												{
													if (getToken() == 2)
													{
														if (Int32.TryParse(currentToken, out o.behavior.chance))
														{
															while (getToken() != 7)
															{
																if (type == 8)
																{
																	getToken();

																	if (type == 0 || type == 3)
																		o.behavior.arguments.Add(currentToken);
																	else return true;
																}
																else return true;
															}
															return false;
														}
													}
												}
											}
										}
									}
									else
									{
										if (getToken() == 7)
											return false;
									}
								}
							}
						}
					}
					else
					{
						o.num = new Number(currentToken, 0);
						getToken();

						if (type == 15)
						{
							o.op = 2;

							if (getToken() == 2)
							{
								if (Int32.TryParse(currentToken, out o.num.value))
								{
									if (getToken() == 7)
										return false;
								}
							}
						}
						else if (type == 16)
						{
							o.op = 3;

							if (getToken() == 2)
							{
								if (Int32.TryParse(currentToken, out o.num.value))
								{
									if (getToken() == 7)
										return false;
								}
							}
						}
						else if (type == 17)
						{
							o.op = 4;

							if (getToken() == 2)
							{
								if (Int32.TryParse(currentToken, out o.num.value))
								{
									if (getToken() == 7)
										return false;
								}
							}
						}
						else if (type == 18)
						{
							o.op = 5;

							if (getToken() == 2)
							{
								if (Int32.TryParse(currentToken, out o.num.value))
								{
									if (getToken() == 7)
										return false;
								}
							}
						}
						else if (type == 9)
						{
							o.op = 6;
							
							if (getToken() == 2)
							{
								if (Int32.TryParse(currentToken, out o.num.value))
								{
									if (getToken() == 7)
										return false;
								}
							}
						}
						else return true;
					}
				}
			}
		}
		return true;
	}

	//Parse Page
	public bool parsePage(Page p, bool isEntityPage, bool isVerbPage)
	{
		bool error = false;

		if (getToken() == 0)
		{
			if ((isEntityPage && currentToken == "entity") || (isVerbPage && currentToken == "verb") || (!isEntityPage && !isVerbPage))
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
								DrawInstruction temp = new DrawInstruction(false,"","","",0,0);
								p.drawList.Add(temp);
								error = parseDrawDef(temp);
							}
							else if (currentToken == "text")
							{
								DrawInstruction temp = new DrawInstruction(true,"","","",0,0);
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
						error = parsePage(temp,false,false);
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
						if (Int32.TryParse(currentToken, out n.value))
						{
							if (getToken() == 7)
								return false;
						}
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

	//Parse Obligation
	public bool parseObligation(Obligation o)
	{
		if (getToken() == 6)
		{
			if (getToken() == 0)
			{
				o.name = currentToken;

				if (getToken() == 8)
				{
					if (getToken() == 0)
					{
						o.verb = currentToken;

						while (getToken() != 7)
						{
							if (type == 8)
							{
								if (getToken() == 0)
									o.arguments.Add(currentToken);
								else return true;
							}
							else return true;
						}
						return false;
					}
				}
			}
		}
		return true;
	}

	//Parse Goal
	public bool parseGoal(Goal g)
	{
		if (getToken() == 6)
		{
			if (getToken() == 0)
			{
				g.name = currentToken;

				if (getToken() == 8)
				{
					bool error = false;
					error = parseGoalOperator(g);
					if (error)
						return true;
					else
						return false;
				}
			}
		}
		return true;
	}
	
	//Parse Behavior
	public bool parseBehavior(Behavior b)
	{
		if (getToken() == 6)
		{
			if (getToken() == 0)
			{
				b.name = currentToken;

				if (getToken() == 8)
				{
					if (getToken() == 0)
					{
						b.verb = currentToken;

						if (getToken() == 8)
						{
							if (getToken() == 2)
							{
								if (Int32.TryParse(currentToken, out b.chance))
								{
									while (getToken() != 7)
									{
										if (type == 8)
										{
											if (getToken() == 0)
												b.arguments.Add(currentToken);
											else return true;
										}
										else return true;
									}
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

	//Parse Icon
	public bool parseIcon(Entity e, Verb v)
	{
		if (getToken() == 6)
		{
			if (getToken() == 0)
			{
				if (e != null)
					e.icon = currentToken;
				if (v != null)
					v.icon = currentToken;
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
							Relationship temp = new Relationship("","",false);
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
						else if (currentToken == "obligation")
						{
							Obligation temp = new Obligation("","");
							e.obligations.Add(temp);
							error = parseObligation(temp);
						}
						else if (currentToken == "goal")
						{
							Goal temp = new Goal("","",0);
							e.goals.Add(temp);
							error = parseGoal(temp);
						}
						else if (currentToken == "behavior")
						{
							Behavior temp = new Behavior("","",0);
							e.behaviors.Add(temp);
							error = parseBehavior(temp);
						}
						else if (currentToken == "image")
						{
							ImageDef temp = new ImageDef("","");
							e.images.Add(temp);
							error = parseImage(temp);
						}
						else if (currentToken == "icon")
							error = parseIcon(e, null);
						else if (currentToken == "page")
						{
							Page temp = new Page("");
							error = parsePage(temp,true,false);
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

	//Parse Input Page
	public bool parseInputPage(Page p)
	{
		bool error = false;

		if (getToken() == 4)
		{
			while (getToken() != 5)
			{
				if (type == 0 && !error)
				{
					if (currentToken == "draw")
					{
						DrawInstruction temp = new DrawInstruction(false,"","","",0,0);
						p.drawList.Add(temp);
						error = parseDrawDef(temp);
					}
					else if (currentToken == "text")
					{
						DrawInstruction temp = new DrawInstruction(true,"","","",0,0);
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
		return true;
	}

	//Parse Variable
	public bool parseVariable(Variable v)
	{
		if (getToken() == 0)
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
							Condition temp = new Condition();
							v.conditions.Add(temp);
							bool error = false;
							error = parseCondition(temp);
							if (error) return true;
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
		if (getToken() == 0)
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
							Condition temp = new Condition();
							a.conditions.Add(temp);
							bool error = false;
							error = parseCondition(temp);
							if (error) return true;
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
						Condition temp = new Condition();
						v.preconditions.Add(temp);
						bool error = false;
						error = parseCondition(temp);
						if (error) return true;
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
							Condition temp = new Condition();
							c.conditions.Add(temp);
							bool error = false;
							error = parseCondition(temp);
							if (error) return true;
						}
						else if (currentToken == "do")
						{
							Operator temp = new Operator();
							c.operators.Add(temp);
							bool error = false;
							error = parseOperator(temp);
							if (error) return true;
						}
						else if (currentToken == "page")
						{
							c.page = new Page("");
							bool error = false;
							error = parsePage(c.page, false, true);
							if (error) return true;
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
				d.neverShow = true;

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
								Condition temp = new Condition();
								d.conditions.Add(temp);
								bool error = false;
								error = parseCondition(temp);
								if (error) return true;
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
							error = parsePage(temp,false,false);
						}
						else if (currentToken == "where")
						{
							Condition temp = new Condition();
							e.conditions.Add(temp);
							error = parseCondition(temp);
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
								if (currentToken == "user")
									error = parseUser(sw);
								else if (currentToken == "beginning")
									error = parseBeginning(sw);
								else if (currentToken == "entity")
								{
									Entity temp = new Entity("");
									sw.entities.Add(temp);
									error = parseEntity(temp);
								}
								else if (currentToken == "input")
								{
									Page temp = new Page("");
									temp.isInputPage = true;
									sw.input = temp;
									error = parseInputPage(temp);
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
		int i;
		if (Int32.TryParse("-345", out i))
			print(i);

		bool error = false;
		swText = swFile.text;
		endLoc = swText.Length - 1;
		storyWorld = new StoryWorld("","");

		error = parseStoryWorld(storyWorld);

		if (!error)
			return -1;
		else
			return lineCount;
	}
}
