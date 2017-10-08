using System;
using System.Text;
using System.Collections.Generic;

namespace SillyWidgets.Gizmos
{
    internal class HtmlPayloadVisitor : IVisitor
    {
        public StringBuilder Payload { get; private set; }
        private bool Exiting = false;

        public HtmlPayloadVisitor()
        {
            Payload = new StringBuilder();
        }

        public void Go(TreeNodeGizmo node)
        {
            if (node == null)
            {
                return;
            }

            Exiting = false;
            node.Accept(this);

            foreach(TreeNodeGizmo child in node.GetChildren())
            {
                Go(child);
            }

            Exiting = true;
            node.Accept(this);
        }

        public void VisitElement(ElementNode node)
        {
            if (Exiting)
            {
                Payload.Append("</");
                Payload.Append(node.Name);
                Payload.Append(">");

                return;
            }

            Payload.Append("<");
            Payload.Append(node.Name);

            if (node.Attributes.Count > 0)
            {
                Payload.Append(" ");
                
                foreach(KeyValuePair<string, string> attr in node.Attributes)
                {
                    Payload.Append(attr.Key);
                    
                    if (!String.IsNullOrEmpty(attr.Value))
                    {
                        Payload.Append("=\"");
                        Payload.Append(attr.Value);
                        Payload.Append("\"");
                    }

                    Payload.Append(" ");
                }
            }

            Payload.Append(">");
        }

        public void VisitText(TextNode node)
        {
            if (Exiting)
            {
                return;
            }

            Payload.Append(node.Text);
        }
    }
}