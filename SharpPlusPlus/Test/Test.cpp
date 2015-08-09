// Dies ist die Haupt-DLL.

#include "stdafx.h"

#include "Test.h"

#include <vcclr.h>
#include "October/ArrayList.h"
#include "October/SortedArrayList.h"
#include "October/Stack.h"
#include "October/Debug.h"

using namespace System::Collections;
using namespace System::Diagnostics;
namespace Seasons
{
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
		InProduction = 6,
	};
	class Production;
	enum Modifier
	{
		mNone,
		Star,
		Plus
	};
}

class TokenImpl
{
public:
	enum Type
	{
		None = 0,
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
		//			ParenthesisOpened = 15,
		//			ParenthesisClosed = 16,
		BracketsOpened = 17,
		BracketsClosed = 18,
		EOS = 19,
		OperatorOr = 21,
		OperatorStar = 22,
		OperatorPlus = 23,
		Anonymous = 30,
	};
	Type type = None;
	Seasons::MainState state;
	Seasons::SubState subState;
	int pos = 0;
	int count = 0;
	
	Seasons::Production * production = nullptr;
	TokenImpl()
	{

	}
	TokenImpl(Seasons::MainState state, Seasons::SubState subState, Type type, int pos, int count) :
		state(state), subState(subState), type(type), pos(pos), count(count)
	{

	}
};

namespace Seasons
{
	class Alternatives;
	class SeqElement;
	class BaseSequence
	{
	public:
	};
	class Sequence : public BaseSequence
	{
	public:
		int value = 11;
		Modifier modifier = mNone;
		Seasons::ArrayList<SeqElement> sequence;
		int compare(const Sequence&other) const
		{
			return value < other.value ? -1 : value > other.value ? 1 : 0;
		}
		void clear()
		{
			modifier = mNone;
			sequence.clear();
			value = 11;
		}
		void add(const TokenImpl&token);
			
		void add(const Alternatives&seq);
		bool isEmpty() const
		{
			return sequence.isEmpty();
		}
		void modifyLast(Modifier mode);
	};
	bool operator < (const Sequence&seqLeft, const Sequence&seqRight)
	{
		return seqLeft.compare(seqRight) < 0;
	}
	bool operator > (const Sequence&seqLeft, const Sequence&seqRight)
	{
		return seqLeft.compare(seqRight) > 0;
	}
	bool operator == (const Sequence&seqLeft, const Sequence&seqRight)
	{
		return seqLeft.compare(seqRight) == 0;
	}
	bool operator != (const Sequence&seqLeft, const Sequence&seqRight)
	{
		return seqLeft.compare(seqRight) != 0;
	}
	class Alternatives
	{
	public:
		
		Seasons::SortedArrayList<Sequence> alternatives;
		void clear()
		{
			alternatives.clear();
		}
		void insert(const Sequence& seq)
		{
			alternatives.insert(seq);
		}
	};
	class Production
	{
	public:
		TokenImpl identifier;
		Alternatives alternatives;
		void clear()
		{
			identifier = TokenImpl();
			alternatives.clear();
		}
		void insert(const Sequence&seq)
		{
			alternatives.insert(seq);
		}
	};
	class Assignment
	{
	public:
		TokenImpl identifier;
		TokenImpl type;
		TokenImpl string;
		
		void clear()
		{
			identifier = TokenImpl();
			type = TokenImpl();
			string = TokenImpl();
		}
	};
	class SeqElement
	{
	public:
		bool isAlternative = false;
		bool isToken = false;
		TokenImpl	token;
		Alternatives sequence;
		Modifier	modifier = mNone;
		SeqElement()
		{

		}
		SeqElement(const TokenImpl &token) :token(token),isToken(true)
		{

		}
		SeqElement(const Alternatives&seq);
	};

	SeqElement::SeqElement(const Alternatives&seq) :isAlternative(true), sequence(seq)
	{

	}
	void Sequence::modifyLast(Modifier mode)
	{
		if (!isEmpty())
		{
			sequence.getLast().modifier = mode;
		}
	}
	void Sequence::add(const TokenImpl&token)
	{
		sequence.add(SeqElement(token));
	}
	void Sequence::add(const Alternatives&seq)
	{
		sequence.add(SeqElement(seq));
	}

}

using namespace Seasons;
class LexerImpl
{
public:
	class InternalException
	{
	public:
	};
	Seasons::ArrayList<TokenImpl> tokens;
	MainState state = Outside;
	SubState subState = LeftSide;
	TokenImpl * currentIdentifier = nullptr;
	TokenImpl * currentSyntaxError = nullptr;
	TokenImpl * currentString = nullptr;
	TokenImpl * currentLine = nullptr;
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
	void closeIdentifier(TokenImpl * & currentIdentifier)
	{
		if (currentIdentifier != nullptr)
		{
			tokens.add(*currentIdentifier);
			delete currentIdentifier;
			currentIdentifier = nullptr;
		}

	}
	void closeSyntaxError(TokenImpl * &currentSyntaxError)
	{
		if (currentSyntaxError != nullptr)
		{
			tokens.add(*currentSyntaxError);
			delete currentSyntaxError;
			currentSyntaxError = nullptr;
		}
	}
	bool treatIdentifier(const wchar_t * p, int i)
	{
		if (isIdentifier(p[i]))
		{

			if (currentIdentifier == nullptr)
			{
				closeSyntaxError(currentSyntaxError);
				currentIdentifier = new TokenImpl(state, subState, TokenImpl::Identifier, i, 1);
			}
			else if (currentSyntaxError == nullptr)
				currentIdentifier->count++;
			else
			{
				currentSyntaxError->count++;
			}
			return true;
		}
		else if (currentIdentifier != nullptr)
		{
			closeIdentifier(currentIdentifier);
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
		countWhite = 0;
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
			currentSyntaxError = new TokenImpl(state, subState, TokenImpl::SyntaxError, i, 1);

	}
	void finishString(const wchar_t*p, int i)
	{
		/*if (currentString == nullptr)
		{
			currentString = new TokenImpl(state, subState, TokenImpl::String, i, 0);
		}*/
		if (currentString != nullptr)
		{
			tokens.add(*currentString);
			delete currentString;
		}
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
		else
		{
			tokens.add(TokenImpl(state, subState, TokenImpl::EOL, i, 1));
		}

	}
	void treatRightSide(const wchar_t*p, int i)
	{
		if (currentString == nullptr)
		{
			currentString = new TokenImpl(state, subState, TokenImpl::String, i, 1);
		}
		else
		{
			currentString->count++;
		}
	}
	wchar_t * printSub(SubState sub)
	{
		switch (sub)
		{
		case LeftSide:
			return L"Left";
		case RightSide:
			return L"Right";
		case EOL:
			return L"EOL";
		case None:
			return L"None";
		case InProduction:
			return L"Prod";
		default:
			return L"Error";
		}
	}
#pragma comment(lib,"October.lib")

	void read(String^ in)
	{
		state = Outside;
		subState = LeftSide;
		countWhite = 0;
		posWhite = 0;
		tokens.clear();
		cli::pin_ptr<const wchar_t> pinned = ::PtrToStringChars(in);

		const wchar_t * pch = pinned;
		//str = pch;
		const wchar_t * p = pch;
		int i = 0;
		int counter = 0;
		while (p[i])
		{
			Seasons::debugOut(L"Round %d: %p[%i]=%c state=%s,subState=%s\n", ++counter, p, i, p[i], 
				state == Inside ? L"Inside" : state == Outside ? L"Outside" : L"Unknown" , printSub(subState));
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
					finishLine(p, i);
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
				finishString(p, i);
				closeAll();
				finishLine(p, i);
				subState = EOL;
				i++;
				continue;
			}
			else if (state == Inside && subState == EOL && !isEOL(p[i]))
			{
				subState = LeftSide;
				currentLine = new TokenImpl(state, subState, TokenImpl::Line, i, 0);
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
						tokens.add(TokenImpl(state, subState, TokenImpl::Assignment, i, 1));
						break;
					default:
						if (!treatIdentifier(p, i))
						{
							treatAsSyntaxError(p, i);
						}
					}
					i++;
					continue;
					break;
				case RightSide:
					switch (p[i])
					{
					case '{':
						closeAll();
						tokens.add(TokenImpl(state, subState, TokenImpl::Begin, i, 1));
						initState(Inside);
						break;
					default:
						treatAsSyntaxError(p, i);
						break;
					}
					i++;
					continue;
					break;
				default:
					throw InternalException();
				}
				break;
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
								tokens.add(TokenImpl(state, subState, TokenImpl::EscapeSeq, i, 2));
								i += 2;
							}
							else
							{
								treatAsSyntaxError(p, i);
								i += 2;
								continue;
							}
							break;
						case '[':
							if (p[i + 1] == ']' && p[i + 2] == '=')
							{
								tokens.add(TokenImpl(state, subState, TokenImpl::Range, i, 3));
								i += 3;
							}
							else
							{
								treatAsSyntaxError(p, i);
								i += 2;
								continue;
							}
							break;
						case ':':
							if (p[i + 1] == '=')
							{
								tokens.add(TokenImpl(state, subState, TokenImpl::Production, i, 2));
								i += 2;
								subState = InProduction;
								closeAll();
								continue;
							}
							else
							{
								treatAsSyntaxError(p, i);
								i += 2;
								continue;
							}
							break;
						case '^':
							if (p[i + 1] == '=')
							{
								tokens.add(TokenImpl(state, subState, TokenImpl::Exclusion, i, 2));
								i += 2;
							}
							else
							{
								treatAsSyntaxError(p, i);
								i += 2;
								continue;
							}
							break;
						case '=':
							tokens.add(TokenImpl(state, subState, TokenImpl::Assignment, i, 1));
							i++;
							break;
						case '}':
							initState(Outside);
							tokens.add(TokenImpl(state, subState, TokenImpl::End, i, 1));
							closeAll();
							i++;
							continue;
						default:
							treatAsSyntaxError(p, i);
							i++;
							continue;
						}
						subState = RightSide;
						closeAll();
						continue;
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
				case InProduction:
					if (!treatIdentifier(p, i))
					{
						switch (p[i])
						{
						case '+':
							closeAll();
							tokens.add(TokenImpl(state, subState, TokenImpl::OperatorPlus, i, 1));
							break;
						case '*':
							closeAll();
							tokens.add(TokenImpl(state, subState, TokenImpl::OperatorStar, i, 1));
							break;
						case '|':
							closeAll();
							tokens.add(TokenImpl(state, subState, TokenImpl::OperatorOr, i, 1));
							break;
						/*case '(':
							tokens.add(TokenImpl(state, subState, TokenImpl::ParenthesisOpened, i, 1));
							break;
						case ')':
							tokens.add(TokenImpl(state, subState, TokenImpl::ParenthesisClosed, i, 1));
							break;*/
						case '[':
							closeAll();
							tokens.add(TokenImpl(state, subState, TokenImpl::BracketsOpened, i, 1));
							break;
						case ']':
							closeAll();
							tokens.add(TokenImpl(state, subState, TokenImpl::BracketsClosed, i, 1));
							break;
						default:
							treatAsSyntaxError(p, i);
							break;
						}
					}
					i++;
					continue;
				default:
					throw InternalException();
				}
				break;
			default:
				throw InternalException();
			}
			throw InternalException();
		}
		tokens.add(TokenImpl(state, subState, TokenImpl::EOS, i, 0));
	}
	void fill(System::Collections::ArrayList^ar)
	{
		Production prod;
		Sequence sequence;
		Alternatives alternatives;
		Seasons::Stack<Sequence> stack;
		Assignment assignment;
		bool inProduction = false;
		bool inAssignment = false;
		int brackets = 0;
		
		
		for (int i = 0; i < tokens.size(); ++i)
		{
			TokenImpl & token = tokens[i];
			Test::Token^ t = gcnew Test::Token(token);
			ar->Add(t);
			if ( token.state == Inside && (
				(token.type == TokenImpl::Assignment) ||
				(token.type == TokenImpl::Range) ||
				(token.type == TokenImpl::EscapeSeq) ||
				(token.type == TokenImpl::Exclusion)))
			{
				if (i > 0 && tokens[i - 1].type == TokenImpl::Identifier)
				{
					assignment.identifier = tokens[i - 1];
					assignment.type = token;
					inAssignment = true;
				}
				else
				{
					TokenImpl syntax(token);
					syntax.type = TokenImpl::SyntaxError;
					Test::Token^ tSyntax = gcnew Test::Token(syntax);
					ar->Add(t);
					continue;
				}
			}
			else if (token.type == TokenImpl::Production)
			{
				if (i > 0 && tokens[i - 1].type == TokenImpl::Identifier)
				{
					prod.identifier = tokens[i - 1];
					inProduction = true;
				}
				else
				{
					TokenImpl syntax(token);
					syntax.type = TokenImpl::SyntaxError;
					Test::Token^ tSyntax = gcnew Test::Token(syntax);
					ar->Add(t);
					continue;
				}
			}
			else if (token.type == TokenImpl::EOL ||
				token.type == TokenImpl::Line ||
				token.type == TokenImpl::End)
			{
				if (inProduction)
				{
					prod.insert(sequence);
					productions.add(prod);
					prod.clear();
					sequence.clear();
				}
				inProduction = false;
				if (inAssignment)
				{
					assignments.add(assignment);
					assignment.clear();
				}
				inAssignment = false;
			}
			else if (inProduction)
			{
				if (token.type == TokenImpl::OperatorOr)
				{
					alternatives.insert(sequence);
					sequence.clear();
				}
				else if (token.type == TokenImpl::OperatorPlus)
				{
					sequence.modifyLast(Modifier::Plus);
				}
				else if (token.type == TokenImpl::OperatorStar)
				{
					sequence.modifyLast(Modifier::Star);
				}
				else if(token.type == TokenImpl::BracketsOpened)
				{
					stack.push(sequence);
					
					sequence.clear();
					brackets++;
				}
				else if (token.type == TokenImpl::BracketsClosed)
				{
					if (brackets < 1)
					{
						TokenImpl syntax(token);
						syntax.type = TokenImpl::SyntaxError;
						Test::Token^ tSyntax = gcnew Test::Token(syntax);
						ar->Add(t);
						continue;
					}
					alternatives.insert(sequence);
					stack.peek().add(alternatives);
					sequence = stack.pop();
					brackets--;
				}
				else if (token.type == TokenImpl::Identifier)
				{
					sequence.add(token);
				}
				else if (token.type == TokenImpl::SyntaxError)
				{
					Test::Token^ tSyntax = gcnew Test::Token(token);
					ar->Add(t);
				}
				else
				{
					throw InternalException();
				}

			}
			else if (inAssignment)
			{
				if (token.type == TokenImpl::String)
				{
					assignment.string = token;
				}
				else if (token.type == TokenImpl::SyntaxError)
				{
					Test::Token^ tSyntax = gcnew Test::Token(token);
					ar->Add(t);
				}
				else
				{
					debugOut(L"%d", token.type);
					throw InternalException();
				}

			}
			else
			{

			}


		}
	}
	void getProductions(System::Collections::ArrayList^ar)
	{
		for (int i = 0; i < productions.size(); ++i)
		{
			Test::Token^ t = gcnew Test::Token(productions[i].identifier);
			
			ar->Add(t);
		}
	}
	void getAssignments(System::Collections::ArrayList^ar)
	{
		for (int i = 0; i < assignments.size(); ++i)
		{
			Test::Assignment^ t = gcnew Test::Assignment(assignments[i]);

			ar->Add(t);
		}
	}
	Seasons::ArrayList<Production> productions;
	Seasons::ArrayList<Assignment> assignments;
};
namespace Test
{
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
	void Lexer::fill(System::Collections::ArrayList^ar)
	{
		impl->fill(ar);
	}
	Token::Token(const TokenImpl&t)
	{
		type = t.type;
		count = t.count;
		pos = t.pos;
		state = t.state;
		subState = t.subState;
		
	}
	Assignment::Assignment(const Seasons::Assignment &assign) : 
			identifier(assign.identifier),
			type(assign.type),
			string(assign.string)
	{
	}

	void Lexer::getProductions(System::Collections::ArrayList^ar)
	{
		impl->getProductions(ar);
	}
	void Lexer::getAssignments(System::Collections::ArrayList^ar)
	{
		impl->getAssignments(ar);
	}

}