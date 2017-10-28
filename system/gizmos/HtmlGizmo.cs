using System;
using System.IO;
using System.Collections.Generic;
using System.Text;

namespace SillyWidgets.Gizmos
{
    public class HtmlGizmo
    {
        public string ParseError { get; private set; }

        private HtmlLexerGizmo Lexer = null;
        private HtmlStateMachineGizmo StateMachine = null;
        private ElementNode DocRoot = null;

        public HtmlGizmo()
        {
            ParseError = string.Empty;
        }

        public bool Load(TextReader htmlData)
        {
            if (htmlData == null)
            {
                ParseError = "HTML data is NULL";

                return(false);
            }

            if (htmlData.Peek() < 0)
            {
                DocRoot = new ElementNode("root");

                return(true);
            }

            Lexer = new HtmlLexerGizmo(ProcessToken);
            StateMachine = new HtmlStateMachineGizmo();
            bool ok = true;

            try
            {   
                Lexer.Gimme(htmlData);
            }
            catch(Exception ex)
            {
                ok = false;
                ParseError = ex.Message;
            }

            DocRoot = StateMachine.TreeBuilder.Root;

            return(ok);
        }

        public void ExecuteHtmlVisitor(IVisitor visitor)
        {
            if (DocRoot == null || visitor == null)
            {
                return;
            }

            if (DocRoot.GetChildren().Count > 0)
            {
                foreach(TreeNodeGizmo child in DocRoot.GetChildren())
                {
                    visitor.Go(child);
                }
            }
        }

        private void ProcessToken(Token token)
        {
            //Console.WriteLine(token.Type + ":" + token.Value);
            StateMachine.Accept(token);
        }
    }
}