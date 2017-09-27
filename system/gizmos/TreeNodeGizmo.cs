using System;
using System.Collections.Generic;

namespace SillyWidgets.Gizmos
{
    public interface IVisitor
    {
        void VisitElement(ElementNode node);
        void VisitText(TextNode node);
    }

    public abstract class TreeNodeGizmo
    {
        public TreeNodeGizmo Parent { get; set; }
        public string Name { get; set; }

        public TreeNodeGizmo(string name)
        {
            Name = name;
            Parent = null;
        }

        public abstract void Accept(IVisitor visitor);
        public abstract void Print(string indent, bool last);
    }

    public class ElementNode : TreeNodeGizmo
    {
        private List<TreeNodeGizmo> Children = new List<TreeNodeGizmo>();
        private Dictionary<string, string> Attributes = new Dictionary<string, string>();

        public ElementNode(string name)
            : base(name)
        {            
        }

        public override void Accept(IVisitor visitor)
        {
            visitor.VisitElement(this);
        }

        public void AddChild(TreeNodeGizmo node)
        {
            node.Parent = this;
            Children.Add(node);
        }

        public List<TreeNodeGizmo> GetChildren()
        {
            return(Children);
        }

        public void SetAttribute(string name, string value)
        {
            Attributes[name.ToLower()] = value;
        }

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

        public override void Accept(IVisitor visitor)
        {
            visitor.VisitText(this);
        }

        public override void Print(string indent, bool last)
        {
            Console.WriteLine(indent + "|-" + base.Name + ":" + Text);
        }
    }
}