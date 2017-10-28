using System;
using System.Text;
using System.IO;
using System.Collections.Generic;
using SillyWidgets.Gizmos;

namespace SillyWidgets
{
    public class SillyView : ISillyView
    {
        public SillyContentType ContentType { get; set; }

        private Dictionary<string, TreeNodeGizmo> BindVals = new Dictionary<string, TreeNodeGizmo>();

        public string Content
        { 
            get
            {
                if (String.IsNullOrEmpty(_content))
                {
                    return(Render());
                }

                return(_content);
            } 
            set
            {
                _content = value;
            }
        }

        private string _content = string.Empty;
        private HtmlGizmo Html = null;

        public SillyView()
        {
            ContentType = SillyContentType.Html;
            Content = string.Empty;
        }

        public void Load(StreamReader data)
        {
            Html = new HtmlGizmo();
            bool success = Html.Load(data);

            if (!success)
            {
                throw new Exception("Parsing HTML: " + Html.ParseError);
            }
        }

        public void Bind(string key, string text)
        {       
            TextNode textNode = new TextNode(text);

            BindVals[key] = new TextNode(text);
        }

        public string Render()
        {
            if (Html == null)
            {
                return(_content);
            }

            HtmlPayloadVisitor payloadCreator = new HtmlPayloadVisitor(BindVals);

            Html.ExecuteHtmlVisitor(payloadCreator);
            _content = payloadCreator.Payload.ToString();
            
            return(_content);
        }

    }

    internal class HtmlPayloadVisitor : IVisitor
    {
        public StringBuilder Payload { get; private set; }
        private bool Exiting = false;
        private Dictionary<string, TreeNodeGizmo> BindVals = null;

        public HtmlPayloadVisitor(Dictionary<string, TreeNodeGizmo> bindVals)
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

            //Console.WriteLine(node.Name);
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
                if (node.SelfCloseTag)
                {
                    Payload.Append(" />");
                }
                else if (node.HasCloseTag)
                {
                    Payload.Append("</");
                    Payload.Append(node.Name);
                    Payload.Append(">");
                }

                return;
            }

            Payload.Append("<");
            Payload.Append(node.Name);

            if (node.Attributes.Count > 0)
            {
                foreach(KeyValuePair<string, string> attr in node.Attributes)
                {
                    if (BindVals != null)
                    {
                        SillyAttribute sillyAttr = null;

                        if (SillyAttribute.TryCreateSillyAttribute(attr.Key, out sillyAttr))
                        {
                            //Console.WriteLine("Bind: " + attr.Key + " = " + attr.Value);
                            TreeNodeGizmo bindNode = null;

                            if (BindVals.TryGetValue(attr.Value, out bindNode))
                            {
                                node.DeleteChildren();
                                node.AddChild(bindNode);

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