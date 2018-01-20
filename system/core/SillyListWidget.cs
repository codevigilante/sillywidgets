using System;
using System.Text;
using System.Collections.Generic;
using SillyWidgets.Gizmos;

namespace SillyWidgets
{
    public class SillyListItem : SillyView
    {   
        public override List<TreeNodeGizmo> Root
        {
            get
            {
                return(_attachedNodes);
            }
        }

        private List<TreeNodeGizmo> _attachedNodes = new List<TreeNodeGizmo>();

        public override bool Attach(TreeNodeGizmo node)
        {
            if (node == null)
            {
                return(true);
            }

            foreach(TreeNodeGizmo child in node.GetChildren())
            {
                _attachedNodes.Add(child);
            }

            return(true);
        }     
    }

    public class SillyListWidget : SillyView
    {
        public List<SillyListItem> Items { get; private set; }

        private TreeNodeGizmo AttachedNode = null;

        public SillyListWidget()
            : base()
        {
            Items = new List<SillyListItem>();
        }

        public void AddItem(SillyListItem item)
        {
            Items.Add(item);
        }

        public override string Render()
        {
            if (Items.Count == 0)
            {
                return(string.Empty);
            }

            StringBuilder payload = new StringBuilder();

            foreach(SillyListItem item in Items)
            {
                payload.Append(item.Render());
            }

            return(payload.ToString());
        }

        public override bool Attach(TreeNodeGizmo node)
        {
            if (Items.Count == 0)
            {
                return(true);
            }

            foreach(SillyListItem item in Items)
            {
                item.Attach(node);
            }

            AttachedNode = node;

            return(false);
        }
    }
}