using System;
using System.IO;
using System.Text;
using System.Collections.Generic;

namespace SillyWidgets.Gizmos
{
    public enum TokenType { Invalid, BeginDoc, Doctype, EndDoc, MarkupDecl, OpenTag, EndTag, TagName, CloseTag, SelfCloseTag, Code, BeginComment, EndComment,
                            EndCode, Text, AttributeName, AttributeValue };
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

    public class HtmlLexerGizmo
    {
        public TokenAvailable Collect { get; set; } 

        private TextReader Stream = null;
        private static HashSet<char> LetterSet = new HashSet<char>()
        {
            'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', 'i', 'j', 'k', 'l', 'm', 'n', 'o', 'p', 'q', 'r', 's', 't', 'u', 'v', 'w', 'x', 'y', 'z',
            'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J', 'K', 'L', 'M', 'N', 'O', 'P', 'Q', 'R', 'S', 'T', 'U', 'V', 'W', 'X', 'Y', 'Z'
        };
        private static HashSet<char> NameSet = new HashSet<char>(LetterSet)
        {
            '0', '1', '2', '3', '4', '5', '6', '7', '8', '9', ':', '.', '-', '_'
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

        public void Gimme(TextReader stream)
        {
            if (stream == null)
            {
                return;
            }

            Stream = stream;

            Deposit(new Token(TokenType.BeginDoc));

            char current;
            StringBuilder content = new StringBuilder();
            bool isContent = false;

            while(GetNext(out current))
            {
                if (isContent)
                {
                    if (current != '<')
                    {
                        content.Append(current);

                        continue;
                    }

                    char next;

                    GetNext(out next, true);

                    if (char.IsWhiteSpace(next))
                    {
                        content.Append(current);

                        continue;
                    }

                    string text = content.ToString();

                    if (text.Length > 0)
                    {
                        Deposit(new Token(TokenType.Text, text));
                        content.Clear();
                    }

                    isContent = false;
                }

                if (char.IsWhiteSpace(current))
                {
                    continue;
                }

                if (current == '<')
                {
                    GetNext(out current);

                    if (current == '!')
                    {
                        MarkupDeclaration();

                        continue;
                    }

                    bool isScript;

                    if (LetterSet.Contains(current))
                    {
                        Deposit(new Token(TokenType.OpenTag));

                        char tagEnd = TagName(current, out isScript);

                        if (TagEnd(tagEnd, isScript))
                        {
                            isContent = true;

                            continue;
                        }

                        Attributes(tagEnd);

                        if (isScript)
                        {
                            Code();
                        }

                        isContent = true;

                        continue;
                    }

                    if (current == '/')
                    {
                        Deposit(new Token(TokenType.CloseTag));

                        GetNext(out current);

                        char tagEnd = TagName(current, out isScript);

                        if (tagEnd == '>')
                        {
                            Deposit(new Token(TokenType.EndTag));

                            isContent = true;
                        }
                    }
                }
            }

            Deposit(new Token(TokenType.EndDoc));
        }

        private void Attributes(char current)
        {
            if (!NameSet.Contains(current))
            {
                return;
            }

            char next = BuildName(current, TokenType.AttributeName);

            while (!TagEnd(next, false))
            {
                if (next == '=')
                {
                    next = AttributeValue(next);

                    continue;
                }

                next = BuildName(next, TokenType.AttributeName);
            }
        }

        private char AttributeValue(char current)
        {
            char next = ChewWhitespace();
            StringBuilder value = new StringBuilder();
            
            if (next == '"' || next == '\'')
            {
                char delimiter = next;
                char previous = next;

                while (GetNext(out next))
                {
                    if (next == delimiter &&
                        previous != '\\')
                    {
                        break;
                    }

                    value.Append(next);
                    previous = next;
                }

                Deposit(new Token(TokenType.AttributeValue, value.ToString()));

                return(ChewWhitespace());
            }

            if (NameSet.Contains(next))
            {
                next = BuildName(next, TokenType.AttributeValue);
            }

            return(next);
        }

        private char ChewWhitespace()
        {
            char next;

            while(GetNext(out next))
            {
                if (!char.IsWhiteSpace(next))
                {
                    break;
                }
            }

            return(next);
        }

        private char BuildName(char current, TokenType type)
        {
            StringBuilder name = new StringBuilder();

            name.Append(current);

            char next;
            
            while(GetNext(out next))
            {
                if (!NameSet.Contains(next))
                {
                    break;
                }

                name.Append(next);
            }

            Deposit(new Token(type, name.ToString()));

            if (char.IsWhiteSpace(next))
            {
                next = ChewWhitespace();
            }

            return(next);
        }

        private bool TagEnd(char end, bool isScript)
        {
            if (end == '>')
            {
                Deposit(new Token(TokenType.EndTag));

                if (isScript)
                {
                    Code();
                }

                return(true);
            }

            if (end == '/')
            {
                char next;

                GetNext(out next);

                if (next == '>')
                {
                    Deposit(new Token(TokenType.SelfCloseTag));

                    return(true);
                }
            }

            return(false);
        }

        private void Code()
        {
            StringBuilder buffer = new StringBuilder();
            StringBuilder notSureBuffer = new StringBuilder();
            char next;
            char expecting = '<';
            bool isClosing = false;

            while(GetNext(out next))
            {
                if (isClosing)
                {
                    notSureBuffer.Append(next);

                    if (next == '>')
                    {
                        Deposit(new Token(TokenType.Code, buffer.ToString()));
                        Deposit(new Token(TokenType.EndCode));

                        return;
                    }

                    if (!char.IsWhiteSpace(next))
                    {
                        isClosing = false;
                        expecting = '<';
                    }
                    else
                    {
                        continue;
                    }
                }

                if (next == expecting)
                {
                    notSureBuffer.Append(next);

                    switch(next)
                    {
                        case '<':
                            expecting = '/';
                            break;
                        case '/':
                            expecting = 's';
                            break;
                        case 's':
                            expecting = 'c';
                            break;
                        case 'c':
                            expecting = 'r';
                            break;
                        case 'r':
                            expecting = 'i';
                            break;
                        case 'i':
                            expecting = 'p';
                            break;
                        case 'p':
                            expecting = 't';
                            break;
                        case 't':
                            expecting = '>';
                            isClosing = true;
                            break;
                        default:
                            break;
                    }

                    continue;
                }

                expecting = '<';
                isClosing = false;

                if (notSureBuffer.Length > 0)
                {
                    buffer.Append(notSureBuffer.ToString());
                    notSureBuffer.Clear();
                }

                buffer.Append(next);
            }
        }

        private char TagName(char firstChar, out bool isScript)
        {
            StringBuilder buffer = new StringBuilder();
            char next;
            bool tagDone = false;

            isScript = false;
            buffer.Append(firstChar);

            while (GetNext(out next))
            {
                if (!tagDone)
                {
                    if (NameSet.Contains(next))
                    {
                        buffer.Append(next);

                        continue;
                    }

                    string tagName = buffer.ToString().Trim();

                    Deposit(new Token(TokenType.TagName, tagName));

                    if (String.Compare(tagName, "script", true) == 0)
                    {
                        isScript = true;
                    }
                }

                tagDone = true;

                if (char.IsWhiteSpace(next))
                {
                    continue;
                }

                return(next);
            }

            return(next);
        }

        private void MarkupDeclaration()
        {
            Deposit(new Token(TokenType.MarkupDecl));

            char next, afterNext;

            if (GetNext(out next) && GetNext(out afterNext))
            {
                if (next == '-' && afterNext == '-')
                {
                    Comment();
                }
                else
                {
                    Doctype();
                }
            }
        }

        private void Doctype()
        {
            char current;
            StringBuilder buffer = new StringBuilder();

            buffer.Append('d');
            buffer.Append('o');

            while(GetNext(out current))
            {
                if (current == '>')
                {
                    Deposit(new Token(TokenType.Doctype, buffer.ToString()));
                    Deposit(new Token(TokenType.EndTag));

                    return;
                }

                buffer.Append(current);
            }
        }

        private bool GetNext(out char next, bool peek = false)
        {
            next = '\0';

            if (Stream == null)
            {
                return(false);
            }

            int value = -1;

            if (peek)
            {
                value = Stream.Peek();
            }
            else
            {
                value = Stream.Read();
            }

            if (value < 0)
            {
                return(false);
            }

            next = (char)value;

            return(true);
        }

        private void Comment()
        {
            Deposit(new Token(TokenType.BeginComment));

            StringBuilder buffer = new StringBuilder();
            char current;
            int dashCount = 0;

            while(GetNext(out current))
            {
                if (current == '-')
                {
                    ++dashCount;

                    if (dashCount >= 2)
                    {
                        char peek;

                        if (GetNext(out peek, true))
                        {
                            if (peek == '>')
                            {
                                Deposit(new Token(TokenType.EndComment, buffer.ToString()));

                                GetNext(out current);

                                return;
                            }
                        }
                    }
                }
                else
                {
                    dashCount = 0;
                }

                buffer.Append(current);
            }

            Deposit(new Token(TokenType.EndComment, buffer.ToString()));
        }
    }
}