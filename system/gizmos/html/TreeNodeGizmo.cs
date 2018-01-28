using System;
using System.Collections.Generic;
using System.Text;

namespace SillyWidgets.Gizmos
{
    public interface ITreeNodeVisitor
    {
        void VisitElement(ElementNode node);
        void VisitText(TextNode node);
        void Go(TreeNodeGizmo node);
    }

    public abstract class TreeNodeGizmo
    {
        public TreeNodeGizmo Parent { get; set; }
        public string Name { get; set; }
        public bool HasCloseTag { get; set; }
        public bool SelfCloseTag { get; set; }

        public TreeNodeGizmo(string name)
        {
            Name = name;
            Parent = null;
            HasCloseTag = SelfCloseTag = false;
        }

        public abstract List<TreeNodeGizmo> GetChildren();
        public abstract void Accept(ITreeNodeVisitor visitor);
        public abstract void Print(string indent, bool last);
        //public abstract string 
    }

    public class ElementNode : TreeNodeGizmo
    {
        private List<TreeNodeGizmo> Children = new List<TreeNodeGizmo>();
        public Dictionary<string, string> Attributes { get; private set; }

        public ElementNode(string name)
            : base(name)
        {            
            Attributes = new Dictionary<string, string>();
        }

        public override void Accept(ITreeNodeVisitor visitor)
        {
            visitor.VisitElement(this);
        }

        public void AddChild(TreeNodeGizmo node)
        {
            node.Parent = this;
            Children.Add(node);
        }

        public override List<TreeNodeGizmo> GetChildren()
        {
            return(Children);
        }

        public void DeleteChildren()
        {
            Children.Clear();
        }

        public void SetAttribute(string name, string value)
        {
            Attributes[name.ToLower()] = value;
        }

        public string GetAttribute(string name)
        {
            string val = string.Empty;

            Attributes.TryGetValue(name.ToLower(), out val);

            return(val);
        }

        /*public override string ToHtmlElement()
        {
            StringBuilder builder = new StringBuilder();
            builder.Append("<");
            builder.Append(Name);
            builder.Append(">");

            return("<" + Name + ">");
        }*/

        public override void Print(string indent, bool last)
        {
            Console.Write(indent);

            if (last)
            {
                Console.Write("\\-");
                indent += "  ";
            }
            else
            {
                Console.Write("|-");
                indent += "| ";
            }

            Console.WriteLine(base.Name);

            for (int i = 0; i < Children.Count; ++i)
            {
                Children[i].Print(indent, i == Children.Count - 1);
            }
        }
    }

    public class TextNode : TreeNodeGizmo
    {
        public string Text { get; set; }

        public TextNode(string text)
            : base("text")
        {            
            Text = text;
        }

        public override List<TreeNodeGizmo> GetChildren()
        {
            return (new List<TreeNodeGizmo>());
        }

        public override void Accept(ITreeNodeVisitor visitor)
        {
            visitor.VisitText(this);
        }

        public override void Print(string indent, bool last)
        {
            Console.WriteLine(indent + "|-" + base.Name + ":" + Text);
        }
    }
}