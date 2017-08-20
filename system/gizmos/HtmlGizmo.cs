using System;
using System.IO;

namespace SillyWidgets.Gizmos
{
    public class HtmlGizmo
    {
        public HtmlGizmo()
        {
        }

        public bool Load(TextReader htmlData)
        {
            if (htmlData == null)
            {
                return(false);
            }

            if (htmlData.Peek() < 0)
            {
                // this is fine, just create an empty doc tree
                return(true);
            }

            HtmlLexerGizmo lexer = new HtmlLexerGizmo(ProcessToken);

            try
            {   
                Console.WriteLine("Lexing...");
                lexer.Lex(htmlData);
            }
            catch(Exception ex)
            {
                Console.WriteLine("Error parsing HTML: " + ex.Message);

                return(false);
            }

            return(true);
        }

        private void ProcessToken(Token token)
        {
            Console.WriteLine(token.Type + ":" + token.Value);
        }
    }
}