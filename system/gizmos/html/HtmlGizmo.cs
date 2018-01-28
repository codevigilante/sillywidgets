using System;
using System.IO;
using System.Collections.Generic;
using System.Text;

namespace SillyWidgets.Gizmos
{
    public class HtmlGizmo
    {
        public string ParseError { get; private set; }
        public virtual List<TreeNodeGizmo> Root
        {
            get
            {
                if (DocRoot == null)
                {
                    return(null);
                }

                return(DocRoot.GetChildren());
            }
        }

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

        public void ExecuteHtmlVisitor(ITreeNodeVisitor visitor)
        {
            if (Root == null || visitor == null)
            {
                return;
            }

            foreach(TreeNodeGizmo child in Root)
            {
                visitor.Go(child);
            }
        }

        private void ProcessToken(Token token)
        {
            StateMachine.Accept(token);
        }
    }
}