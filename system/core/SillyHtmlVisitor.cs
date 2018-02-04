using System;
using System.Text;
using System.Collections.Generic;
using SillyWidgets.Gizmos;

namespace SillyWidgets
{
    public class SillyHtmlVisitor : ITreeNodeVisitor
    {
        public StringBuilder Payload { get; private set; }
    
        private Dictionary<string, SillyAttribute> BindVals = null;
        private Stack<TreeNodeGizmo> NodeStack = new Stack<TreeNodeGizmo>();

        public SillyHtmlVisitor(Dictionary<string, SillyAttribute> bindVals)
        {
            Payload = new StringBuilder();
            BindVals = bindVals;
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
            if (node.Visited)
            {
                if (ForceCloseTag || node.HasCloseTag)
                {
                    Payload.Append("</");
                    Payload.Append(node.Name);
                    Payload.Append(">");
                }
                else if (node.SelfCloseTag)
                {
                    Payload.Append(" />");
                }

                node.Visited = false;

                return;
            }

            Payload.Append("<");
            Payload.Append(node.Name);

            ISillyWidget widget = null;

            if (node.Attributes.Count > 0)
            {
                foreach(KeyValuePair<string, string> attr in node.Attributes)
                {
                    if (BindVals != null)
                    {
                        if (SillyAttribute.IsSillyAttribute(attr.Key))
                        {
                            SillyAttribute boundAttr = null;

                            if (BindVals.TryGetValue(attr.Value, out boundAttr))
                            {
                                if (boundAttr.Widget == null)
                                {
                                    widget = new SillyTextWidget("Error rendering " + attr.Value + ": trying to bind null node");
                                }
                                else
                                {
                                    widget = boundAttr.Widget;
                                }                     

                                continue;
                            }
                        }
                    }

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

            if (widget != null)
            {
                RenderChildren = widget.Attach(node);
                Payload.Append(">");
                Payload.Append(widget.Render());
                ForceCloseTag = true;
            }
            else if (!node.SelfCloseTag)
            {
                Payload.Append(">");
            }

            node.Visited = true;
        }

        public void VisitText(TextNode node)
        {
            if (node.Visited)
            {
                node.Visited = false;

                return;
            }

            Payload.Append(node.Text);
            node.Visited = true;
        }
    }
}