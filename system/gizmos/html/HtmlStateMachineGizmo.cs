using System;
using System.Text;
using System.Collections.Generic;

namespace SillyWidgets.Gizmos
{
    public enum States { Begin, Start, Declaration, Doctype, Comment, Element, Name, Attributes, TagDone, Code, End }

    public class HtmlStateMachineGizmo : IStateMachineGizmo<States, Token>
    {
        private Dictionary<States, HtmlState> StateLookup = new Dictionary<States, HtmlState>()
        {
            { States.Begin, new BEGIN() },
            { States.Start, new START() },
            { States.End, new END() },
            { States.Declaration, new DECL() },
            { States.Comment, new COM() },
            { States.Doctype, new DOC() },
            { States.Element, new ELEMENT() },
            { States.Attributes, new ATTR() },
            { States.TagDone, new TAGDONE() },
            { States.Name, new NAME() },
            { States.Code, new CODE() }
        };
        private HtmlState Current = null;

        public HtmlTreeBuilder TreeBuilder = new HtmlTreeBuilder();

        public HtmlStateMachineGizmo()
        {
            Current = StateLookup[States.Begin];
        }

        public void Transition(States toState, Token token)
        {
            Current = StateLookup[toState];
            Current.Enter(token, this);
        }

        public void Accept(Token token)
        {
            Current.Accept(token, this);
        }
    }

    internal abstract class HtmlState : IStateGizmo<Token, HtmlStateMachineGizmo>
    {
        public HtmlState()
        {

        }

        public abstract void Enter(Token token, HtmlStateMachineGizmo context);
        public abstract void Accept(Token token, HtmlStateMachineGizmo context);
    }

    internal class BEGIN : HtmlState
    {
        public BEGIN()
        {

        }

        public override void Enter(Token token, HtmlStateMachineGizmo context)
        {
        }

        public override void Accept(Token token, HtmlStateMachineGizmo context)
        {
            if (token.Type == TokenType.BeginDoc)
            {
                context.TreeBuilder.CreateRoot();
                context.Transition(States.Start, token);

                return;
            }

            if (token.Type == TokenType.EndDoc)
            {
                context.Transition(States.End, token);

                return;
            }

            throw new Exception("Unexpected token: " + token.Type);
        }
    }

    internal class END : HtmlState
    {
        public END()
        {

        }

        public override void Enter(Token token, HtmlStateMachineGizmo context)
        {

        }

        public override void Accept(Token token, HtmlStateMachineGizmo context)
        {
            throw new Exception("This is the end of the document, you're supposed to be done, yet you keep trying to feed me tokens?? WTF?");
        }
    }

    internal class START : HtmlState
    {
        public START()
        {

        }

        public override void Enter(Token token, HtmlStateMachineGizmo context)
        {
        }

        public override void Accept(Token token, HtmlStateMachineGizmo context)
        {
            if (token.Type == TokenType.MarkupDecl)
            {
                context.Transition(States.Declaration, token);

                return;
            }

            if (token.Type == TokenType.OpenTag)
            {
                context.Transition(States.Element, token);

                return;
            }

            if (token.Type == TokenType.Text)
            {
                context.TreeBuilder.AddChildText(token.Value);

                return;
            }

            if (token.Type == TokenType.CloseTag)
            {
                context.Transition(States.TagDone, token);

                return;
            }

            if (token.Type == TokenType.Code)
            {
                context.Transition(States.Code, token);

                return;
            }

            if (token.Type == TokenType.EndDoc)
            {
                context.Transition(States.End, token);

                return;
            }

            throw new Exception("I was expecting either a Markup Delcaration or Open Tag, but I got: " + token.Type);
        }
    }

    internal class DECL : HtmlState
    {
        public DECL()
        {

        }

        public override void Enter(Token token, HtmlStateMachineGizmo context)
        {

        }

        public override void Accept(Token token, HtmlStateMachineGizmo context)
        {
            if (token.Type == TokenType.Doctype)
            {
                context.Transition(States.Doctype, token);

                return;
            }

            if (token.Type == TokenType.BeginComment)
            {
                context.Transition(States.Comment, token);

                return;
            }

            if (token.Type == TokenType.EndDoc)
            {
                context.Transition(States.End, token);

                return;
            }

            throw new Exception("Hmm, looks like you started declaring something cool then decided not to: " + token.Type);
        }
    }

    internal class DOC : HtmlState
    {
        public DOC()
        {

        }

        public override void Enter(Token token, HtmlStateMachineGizmo context)
        {

        }

        public override void Accept(Token token, HtmlStateMachineGizmo context)
        {
            if (token.Type == TokenType.EndTag)
            {
                context.Transition(States.Start, token);

                return;
            }

            if (token.Type == TokenType.EndDoc)
            {
                context.Transition(States.End, token);

                return;
            }

            throw new Exception("Well, you started declaring a Doctype then decided not to? " + token.Type);
        }
    }

    internal class COM : HtmlState
    {
        public COM()
        {

        }

        public override void Enter(Token token, HtmlStateMachineGizmo context)
        {
        }

        public override void Accept(Token token, HtmlStateMachineGizmo context)
        {
            if (token.Type == TokenType.EndComment)
            {
                context.Transition(States.Start, token);

                return;
            }

            if (token.Type == TokenType.EndDoc)
            {
                context.Transition(States.End, token);

                return;
            }

            throw new Exception("You were making a comment, some profound statement about life or whatever, but then got distracted by fart videos? " + token.Type);
        }
    }

    internal class ELEMENT : HtmlState
    {
        public ELEMENT()
        {

        }

        public override void Enter(Token token, HtmlStateMachineGizmo context)
        {
        }

        public override void Accept(Token token, HtmlStateMachineGizmo context)
        {
            if (token.Type == TokenType.TagName)
            {
                context.Transition(States.Name, token);

                return;
            }

            throw new Exception("I expected to get a Tag Name, but instead I got " + token.Type);            
        }
    }

    internal class NAME : HtmlState
    {
        public NAME()
        {
        }

        public override void Enter(Token token, HtmlStateMachineGizmo context)
        {
            context.TreeBuilder.AddChildElement(token.Value);
        }

        public override void Accept(Token token, HtmlStateMachineGizmo context)
        {
            if (token.Type == TokenType.AttributeName)
            {
                context.Transition(States.Attributes, token);

                return;
            }

            if (token.Type == TokenType.EndTag)
            {
                context.Transition(States.Start, token);

                return;
            }

            if (token.Type == TokenType.SelfCloseTag)
            {
                context.TreeBuilder.CompleteCurrentElement();
                context.Transition(States.Start, token);

                return;
            }

            throw new Exception("I'm sitting here thinking you were going to define a tag, but instead you're doing this: " + token.Type);
        }
    }

    internal class ATTR : HtmlState
    {
        private string Name = string.Empty;

        public ATTR()
        {

        }

        public override void Enter(Token token, HtmlStateMachineGizmo context)
        {
            Name = token.Value;
            context.TreeBuilder.AddAttribute(Name, string.Empty);
        }

        public override void Accept(Token token, HtmlStateMachineGizmo context)
        {
            if (token.Type == TokenType.AttributeName)
            {
                Name = token.Value;
                context.TreeBuilder.AddAttribute(Name, string.Empty);

                return;
            }

            if (token.Type == TokenType.AttributeValue)
            {
                if (Name.Length == 0)
                {
                    throw new Exception("I think you should try assigning a name to Attribute before giving me a value: " + token.Value);
                }

                context.TreeBuilder.AddAttribute(Name, token.Value);
                Name = string.Empty;

                return;
            }

            if (token.Type == TokenType.EndTag)
            {
                context.Transition(States.Start, token);

                return;
            }

            if (token.Type == TokenType.SelfCloseTag)
            {
                context.TreeBuilder.CompleteCurrentElement();
                context.Transition(States.Start, token);

                return;
            }

            throw new Exception("I was thinking I'd get some Attribute stuff and eventually an End Tag, but instead I got this: " + token.Type);
        }
    }

    internal class TAGDONE : HtmlState
    {
        private bool GotTagName = false;
        private string TagName = string.Empty;

        public TAGDONE()
        {

        }

        public override void Enter(Token token, HtmlStateMachineGizmo context)
        {
            GotTagName= false;
            TagName = string.Empty;
        }

        public override void Accept(Token token, HtmlStateMachineGizmo context)
        {
            if (!GotTagName)
            {
                if (token.Type == TokenType.TagName)
                {
                    TagName = token.Value;
                    GotTagName = true;

                    return;
                }

                throw new Exception("I was expecting a Tag Name, but I got: " + token.Type);
            }
            
            if (token.Type == TokenType.EndTag)
            {
                context.TreeBuilder.CompleteCurrentElement(TagName);
                context.Transition(States.Start, token);

                return;
            }

            throw new Exception("Are you not going to end your tags properly, with a '>', because, whatever: " + token.Type);
        }
    }

    internal class CODE : HtmlState
    {
        public CODE()
        {

        }

        public override void Enter(Token token, HtmlStateMachineGizmo context)
        {
            context.TreeBuilder.AddChildText(token.Value);
        }

        public override void Accept(Token token, HtmlStateMachineGizmo context)
        {
            if (token.Type == TokenType.Code)
            {
                context.TreeBuilder.AddChildText(token.Value);

                return;
            }

            if (token.Type == TokenType.EndCode)
            {
                context.TreeBuilder.CompleteCurrentElement("script");
                context.Transition(States.Start, token);

                return;
            }

            throw new Exception("I was waiting for some Code stuff, but got this crap: " + token.Type);            
        }
    }
}