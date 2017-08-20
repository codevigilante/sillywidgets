using System;
using System.IO;
using System.Text;
using System.Collections.Generic;

namespace SillyWidgets.Gizmos
{
    public enum TokenType { Invalid, Text };
    public enum State { Invalid, START, TEXT, END };
    public delegate void TokenAvailable(Token token);

    public class Token
    {
        public TokenType Type { get; set; }
        public string Value { get; set; }

        public Token(TokenType type = TokenType.Invalid, string value = "")
        {
            Type = type;
            Value = value;
        }
    }

    public class HtmlLexerGizmo : IStateMachineGizmo<Token, State, char>
    {
        public TokenAvailable Collect { get; set; } 

        private LexerState Current = null;

        private static Dictionary<State, LexerState> States = new Dictionary<State, LexerState>()
        {
            { State.START, new START() },
            { State.TEXT, new TEXT() },
            { State.END, new END() }
        };
        
        public HtmlLexerGizmo(TokenAvailable tokenCallback)
        {
            Collect = tokenCallback;
        }

        public void Deposit(Token artifact)
        {
            if (Collect != null)
            {
                Collect(artifact);
            }
        }

        public void Transition(State toState, char input)
        {            
            if (toState == State.Invalid)
            {
                throw new Exception("Don't understand character '" + input + "'");
            }

            Current = States[toState];

            Accept(input);
        }

        public void Accept(char input)
        {
            //Console.WriteLine(Current.GetType().Name + ":" + (int)input);
            Current.Accept(input, this);
        }

        public void End()
        {
            Current.End(this);
        }

        public void Lex(TextReader stream)
        {
            if (stream == null)
            {
                return;
            }

            int value = -1;

            Current = States[State.START];

            while((value = stream.Read()) >= 0)
            {
                char input = (char)value;

                Accept(input);
            }

            End();
        }
    }

    public abstract class LexerState : IStateGizmo<char, HtmlLexerGizmo>
    {
        public LexerState()
        {
        }

        public abstract void Accept(char input, HtmlLexerGizmo context);

        public virtual void End(HtmlLexerGizmo context)
        {

        }
    }

    public class START : LexerState
    {
        public START()
            : base()
        {
        }

        public override void Accept(char input, HtmlLexerGizmo context)
        {                
            if (!char.IsWhiteSpace(input))
            {
                context.Transition(State.TEXT, input);

                return;
            }
        }
    }

    public class TEXT : LexerState
    {
        private StringBuilder token = new StringBuilder();

        public TEXT()
            : base()
        {
        }

        public override void Accept(char input, HtmlLexerGizmo context)
        {
            if (char.IsWhiteSpace(input))
            {
                DepositToken(context);
                context.Transition(State.START, input);

                return;
            }

            token.Append(input);
        }

        public override void End(HtmlLexerGizmo context)
        {
            DepositToken(context);
        }

        private void DepositToken(HtmlLexerGizmo context)
        {
            Token artifact = new Token(TokenType.Text, token.ToString());

            context.Deposit(artifact);

            token.Clear();
        }
    }

    public class END : LexerState
    {
        public END()
            : base()
        {

        }

        public override void Accept(char input, HtmlLexerGizmo context)
        {            
        }
    }
}