// Test.h

#pragma once

using namespace System;

class LexerImpl;
class TokenImpl;
namespace Seasons
{
	class Assignment;
}
namespace Test {
	public ref class TestClass
	{
		// TODO: Die Methoden für diese Klasse hier hinzufügen.
	public:
		void foo()
		{

		}
	};
	public ref class Token : System::Object
	{
	public:
		int type;
		int state;
		int subState;
		int pos;
		int count;
		Token()
		{

		}
		Token(int type, int state, int subState, int pos, int count) :
			state(state), subState(subState), type(type), pos(pos), count(count)
		{

		}
		Token(const TokenImpl&t);
		Token(const Token % token)
		{
			type = token.type;
			state = token.state;
			pos = token.pos;
			count = token.count;
			subState = token.subState;
		}
		Token(Token^ token)
		{
			type = token->type;
			state = token->state;
			pos = token->pos;
			count = token->count;
			subState = token->subState;
		}
	};
	public ref class Assignment : System::Object
	{
	public:
		Token identifier;
		Token type;
		Token string;
		Assignment()
		{

		}
		Assignment(const Seasons::Assignment &assign);
	public:
		void getIdentifier(Token^token)
		{
			token->pos = identifier.pos;
			token->type = identifier.type;
			token->count = identifier.count;
		}
		void getType(Token^token)
		{
			token->pos = type.pos;
			token->type = type.type;
			token->count = type.count;
		}
		void getString(Token^token)
		{
			token->pos = string.pos;
			token->type = string.type;
			token->count = string.count;
		}
	};
	public ref class Lexer
	{
		LexerImpl*impl;
	public:

		Lexer();
		~Lexer();
	public: void read(::String^ pch);
	public: void fill(System::Collections::ArrayList^ pch);
	public: void getProductions(System::Collections::ArrayList^productions);
	public: void getAssignments(System::Collections::ArrayList^assignments);
	};
}
