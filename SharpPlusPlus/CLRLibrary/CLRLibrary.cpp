// Dies ist die Haupt-DLL.

#include "stdafx.h"

#include "CLRLibrary.h"
#include "October/String.h"
#include "October/ArrayList.h"
#include "October/Exception.h"
#include <vcclr.h>
#pragma comment(lib,"October.lib")

namespace Seasons
{
}

namespace clim
{
	enum Encoding
	{
		eUNICODE,
		eANSI,
		eUTF8,
	};
};

class LexerImpl
{
public:
	class InternalException
	{
	public:
	};
	enum MainState
	{
		Outside = 1,
		Inside = 2,
	};
	enum SubState
	{
		None = 0,
		LeftSide = 3,
		RightSide = 4,
		EOL = 5,
	};

	class CToken
	{
	public:
		enum Type
		{
			None = 0,
			Operator = 1,
			Begin = 2,
			End = 3,
			Range = 4,
			Exclusion = 5,
			EscapeSeq = 6,
			Production = 7,
			Assignment = 8,
			EOL = 9,
			WhiteSpace = 10,
			Identifier = 11,
			SyntaxError = 12,
			String = 13,
			Line = 14,
		};
		Type type = None;
		MainState state;
		SubState subState;
		int pos = 0;
		int count = 0;
		CToken()
		{

		}
		CToken(MainState state, SubState subState, Type type, int pos, int count) :
			state(state), subState(subState), type(type), pos(pos), count(count)
		{

		}
	};
	Seasons::String str;
	Seasons::ArrayList<CToken> tokens;
	MainState state = Outside;
	SubState subState = LeftSide;
	CToken * currentIdentifier = nullptr;
	CToken * currentSyntaxError = nullptr;
	CToken * currentString = nullptr;
	CToken * currentLine = nullptr;
	int posWhite = 0;
	int countWhite = 0;

	bool isIdentifier(wchar_t ch)
	{
		if ((ch >= L'A' && ch <= L'Z') || (ch >= L'a' && ch <= L'z'))
		{
			return true;
		}
		return false;
	}
	bool isWhite(wchar_t ch)
	{
		if (ch == L' ' || ch == '\t' || ch == '\v')
			return true;
		return false;
	}
	bool isEOL(wchar_t ch)
	{
		if (ch == '\n' || ch == '\r')
		{
			return true;
		}
		return false;
	}
	void closeIdentifier(CToken * & currentIdentifier)
	{
		if (currentIdentifier != nullptr)
		{
			tokens.add(*currentIdentifier);
			delete currentIdentifier;
			currentIdentifier = nullptr;
		}

	}
	void closeSyntaxError(CToken * &currentSyntaxError)
	{
		if (currentSyntaxError != nullptr)
		{
			tokens.add(*currentSyntaxError);
			delete currentSyntaxError;
			currentSyntaxError = nullptr;
		}
	}
	bool treatIdentifier(const wchar_t * p, int i )
	{
		if (isIdentifier(p[i]))
		{

			if (currentIdentifier == nullptr)
			{
				closeSyntaxError(currentSyntaxError);
				currentIdentifier = new CToken(state, subState, CToken::Identifier, i, 1);
			}
			else if (currentSyntaxError == nullptr)
				currentIdentifier->count++;
			else
			{
				currentSyntaxError->count++;
			}
			return true;
		}
		return false;
	}
	bool treatWhite(const wchar_t * p, int i)
	{
		if (state == Inside && subState == RightSide && isWhite(p[i]))
			return false;
		if (isWhite(p[i]) || (state == Outside && isEOL(p[i])))
		{
			closeSyntaxError(currentSyntaxError);
			if (countWhite == 0)
			{
				posWhite = i;
				countWhite = 1;
				closeIdentifier(currentIdentifier);
			}
			else
			{
				countWhite++;
			}
			return true;
		}
		return false;
	}
	void closeAll()
	{
		closeIdentifier(currentIdentifier);
		closeSyntaxError(currentSyntaxError);
	}
	void initState(MainState newState)
	{
		state = newState;
		subState = None;
	}
	void treatAsSyntaxError(const wchar_t*p, int i)
	{
		if (currentSyntaxError != nullptr)
		{
			currentSyntaxError->count++;
		}
		else
			currentSyntaxError = new CToken(state, subState, CToken::SyntaxError, i, 1);

	}
	void finishString(const wchar_t*p, int i)
	{
		if (currentString == nullptr)
		{
			currentString = new CToken(state, subState, CToken::String, i, 0);
		}
		tokens.add(*currentString);
		delete currentString;
		currentString = nullptr;
	}
	void finishLine(const wchar_t*p, int i)
	{
		if (currentLine != nullptr)
		{
			currentLine->count = i - currentLine->pos;
			tokens.add(*currentLine);
			delete currentLine;
			currentLine = nullptr;
			return;
		}
		throw InternalException();

	}
	void treatRightSide(const wchar_t*p, int i)
	{
		if (currentString == nullptr)
		{
			currentString = new CToken(state, subState, CToken::String, i, 1);
		}
		else
		{
			currentString->count++;
		}
	}
	void read(String^ in)
	{
		
		cli::pin_ptr<const wchar_t> pinned = ::PtrToStringChars(in);
			
		const wchar_t * pch = pinned;
		str = pch;
		const wchar_t * p = pch;
		int i = 0;
		while (p[i])
		{
			if (treatWhite(p, i))
			{
				// ignore
				i++;
				continue;
			}
			else if ((state == Inside) && isEOL(p[i]))
			{
				if (subState == None)
				{
					subState = EOL;
					i++;
					continue;
				}
				else if (subState == EOL)
				{
					i++;
					continue;
				}
				else if (subState == LeftSide)
				{
					treatAsSyntaxError(p, i);
					i++;
					continue;
				}
				finishString(p,i);
				finishLine(p,i);
				subState = EOL;
			}
			else if (state == Inside && subState == EOL && !isEOL(p[i]))
			{
				subState = LeftSide;
				currentLine = new CToken(state, subState, CToken::Line, i, 0);
			}
			switch (state)
			{
			case Outside:
				switch (subState)
				{
				case LeftSide:
					switch (p[i])
					{
					case '=':
						subState = RightSide;
						closeAll();
						tokens.add(CToken(state, subState, CToken::Assignment, i, 1));
						break;
					default:
						if (!treatIdentifier(p, i))
						{
							treatAsSyntaxError(p, i);
						}
					}
					break;
				case RightSide:
					switch (p[i])
					{
					case '{':
						closeAll();
						tokens.add(CToken(state, subState, CToken::Begin, i, 1));
						initState(Inside);
						break;
					default:
						treatAsSyntaxError(p, i);
						break;
					}
					break;
				default:
					throw InternalException();
				}
			case Inside:
				switch (subState)
				{
				case LeftSide:
					if (!treatIdentifier(p, i))
					{
						switch (p[i])
						{
						case '\\':
							if (p[i + 1] == '=')
							{
								tokens.add(CToken(state, subState, CToken::EscapeSeq, i, 2));
								i += 2;
								continue;
							}
							break;
						case '[':
							if (p[i + 1] == ']' && p[i + 2] == '=')
							{
								tokens.add(CToken(state, subState, CToken::Range, i, 3));
								i += 3;
								continue;
							}
							break;
						case ':':
							if (p[i + 1] == '=')
							{
								tokens.add(CToken(state, subState, CToken::Production, i, 2));
								i += 2;
								continue;
							}
							break;
						case '^':
							if (p[i + 1] == '=')
							{
								tokens.add(CToken(state, subState, CToken::Exclusion, i, 2));
								i += 2;
								continue;
							}
							break;
						case '=':
							tokens.add(CToken(state, subState, CToken::Assignment, i, 1));
							i++;
							continue;
						case '}':
							initState(Outside);
							tokens.add(CToken(state, subState, CToken::End, i, 1));
							i++;
							continue;
						}
						treatAsSyntaxError(p, i);
					}
					i++;
					continue;
					break;
				case RightSide:
					if (isEOL(p[i]))
						throw InternalException();
					treatRightSide(p, i);
					i++;
					continue;
					break;
				default:
					throw InternalException();
				}
			default:
				throw InternalException();
			}
			throw InternalException();
		}
	}
	void fill(ArrayList^ar)
	{
		for (int i = 0; i < tokens.size(); ++i)
		{
			CToken & token = tokens[i];
			CLRLibrary::Token^ t = gcnew CLRLibrary::Token(token.type,token.state,token.subState,token.pos,token.count);
			
			ar->Add(t);

		}
	}
};
namespace CLRLibrary {

	Lexer::Lexer()
	{
		impl = new LexerImpl();
	}

	Lexer::~Lexer()
	{
		delete impl;
	}
	void Lexer::read(String^ pch)
	{
		impl->read(pch);
	}
	void Lexer::fill(ArrayList^ar)
	{
		impl->fill(ar);
	}
}
