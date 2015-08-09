// CLRLibrary.h

#pragma once


using namespace System;
using namespace System::Collections;

class LexerImpl;
namespace CLRLibrary {
	public ref class Token : System::Object
	{
	public: 
		int type;
		int state;
		int subState;
		int pos;
		int count;
		Token(int type, int state, int subState, int pos, int count):
			state(state), subState(subState), type(type), pos(pos), count(count)
		{

		}
	};
	public ref class Lexer
	{
		LexerImpl*impl;
	public:

		Lexer();
		~Lexer();
	public : void read(::String^ pch);
	public: void fill(::ArrayList^ pch);

	};
}
