using System;
using System.Text;
using System.Collections.Generic;

namespace SillyWidgets.Gizmos
{
    public class HtmlTreeBuilder
    {
        public ElementNode Root { get; private set; }

        private ElementNode Current = null;

        public HtmlTreeBuilder()
        {            
            Root = null;
            Current = null;
        }

        public void CreateRoot()
        {
            Root = new ElementNode("root");
            Current = Root;
        }

        public ElementNode AddChildElement(string name)
        {
            IsCurrent();

            ElementNode node = new ElementNode(name);

            Current.AddChild(node);
            Current = node;

            return(node);
        }

        public TextNode AddChildText(string text)
        {
            IsCurrent();

            TextNode node = new TextNode(text);
            
            Current.AddChild(node);

            return(node);
        }

        public void CompleteCurrentElement(string tagName = "")
        {
            IsCurrent();

            if (tagName != null && 
                tagName.Length > 0)
            {
                ElementNode Saver = Current;
                bool found = false;

                while(Current.Parent != null)
                {
                    if (!IsNameEqual(tagName))
                    {
                        Current = Current.Parent as ElementNode;
                    }
                    else
                    {
                        found = true;

                        break;
                    }
                }

                if (!found)
                {
                    // don't move Current up because this is probably a dangling close tag
                    Current = Saver;

                    return;
                }

                Current.HasCloseTag = true;
            }
            else
            {
                Current.SelfCloseTag = true;
            }

            Current = Current.Parent as ElementNode;
        }

        private bool IsNameEqual(string name)
        {
            IsCurrent();

            return(String.Compare(Current.Name, name, false) == 0);
        }

        public void AddAttribute(string name, string value)
        {
            IsCurrent();

            Current.SetAttribute(name, value);
        }

        private void IsCurrent()
        {
            if (Current == null)
            {
                throw new Exception("Invalid tree structure");
            }
        }
    }
}