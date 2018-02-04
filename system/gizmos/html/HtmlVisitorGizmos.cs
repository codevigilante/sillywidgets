using System;
using System.Text;
using System.Collections.Generic;

namespace SillyWidgets.Gizmos
{
    public interface ITreeNodeVisitor
    {
        void VisitElement(ElementNode node);
        void VisitText(TextNode node);
        void Go(TreeNodeGizmo node);
    }

    public class HtmlAssembleVisitor : ITreeNodeVisitor
    {
        private StringBuilder Payload = new StringBuilder();
        private Stack<TreeNodeGizmo> NodeStack = new Stack<TreeNodeGizmo>();
        private bool Exiting = false;

        public string Get()
        {
            return(Payload.ToString());
        }

        public void Go(TreeNodeGizmo node)
        {
            if (node == null)
            {
                return;
            }

            NodeStack.Push(node);

            while(NodeStack.Count > 0)
            {
                TreeNodeGizmo current = NodeStack.Pop();

                current.Accept(this);

                if (current.Visited)
                {
                    NodeStack.Push(current);
                
                    List<TreeNodeGizmo> children = current.GetChildren();

                    for(int i = children.Count - 1; i >= 0; --i)
                    {
                        NodeStack.Push(children[i]);
                    }
                }
            }
        }

        public void VisitElement(ElementNode node)
        {
            if(node.Visited)
            {
                if (node.HasCloseTag)
                {
                    Payload.Append("</");
                    Payload.Append(node.Name);
                    Payload.Append(">");
                }

                node.Visited = false;

                return;
            }

            Payload.Append("<");
            Payload.Append(node.Name);

            if (node.Attributes.Count > 0)
            {
                foreach(KeyValuePair<string, string> attr in node.Attributes)
                {
                    Payload.Append(" ");
                    Payload.Append(attr.Key);
                    
                    if (!String.IsNullOrEmpty(attr.Value))
                    {
                        Payload.Append("=\"");
                        Payload.Append(attr.Value);
                        Payload.Append("\"");
                    }
                }
            }

            if (node.SelfCloseTag)
            {
                Payload.Append(" ");
                Payload.Append("/>");
            }
            else
            {
                Payload.Append(">");
            }

            node.Visited = true;
        }

        public void VisitText(TextNode node)
        {
            if(node.Visited)
            {
                node.Visited = false;

                return;
            }

            Payload.Append(node.Text);
            node.Visited = true;
        }
    }
}