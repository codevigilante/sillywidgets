using System;
using System.Collections.Generic;
using SillyWidgets.Gizmos;

namespace SillyWidgets
{
    public class SillyAttribute
    {
        public enum SillyAttrType { Text, Widget, Unsupported }
        public SillyAttrType Type { get; private set; }
        public string Name { get; private set; }

        public SillyAttribute(string name, SillyAttrType type)
        {            
            Type = type;
            Name = name;
        }

        public virtual List<TreeNodeGizmo> BoundValues()
        {
            return(new List<TreeNodeGizmo>());
        }

        public static bool IsSillyAttribute(string attr)
        {
            if (attr == null ||
                attr.Length == 0)
            {
                return(false);
            }

            string[] parts = attr.Split(new char[] { ':' });

            if (parts == null ||
                parts.Length <= 1)
            {
                return(false);
            }

            if (String.Compare(parts[0], "silly", false) == 0)
            {
                return(true);
            }

            return(false);
        }
    }

    public class SillyTextAttribute : SillyAttribute
    {
        private TreeNodeGizmo Value = null;

        public SillyTextAttribute(string name, TreeNodeGizmo value)
            : base(name, SillyAttrType.Text)
        {
            Value = value;
        }

        public override List<TreeNodeGizmo> BoundValues()
        {
            return(new List<TreeNodeGizmo>() { Value });
        }
    }

    public class SillyWidgetAttribute : SillyAttribute
    {
        private List<TreeNodeGizmo> HtmlNodes = null;

        public SillyWidgetAttribute(string name, List<TreeNodeGizmo> htmlNodes)
            : base(name, SillyAttrType.Widget)
        {
            HtmlNodes = htmlNodes;
        }

        public override List<TreeNodeGizmo> BoundValues()
        {
            if (HtmlNodes == null)
            {
                return(new List<TreeNodeGizmo>() { new TextNode(base.Name + ": no HTML to bind") });
            }

            return(HtmlNodes);
        }
    }
}