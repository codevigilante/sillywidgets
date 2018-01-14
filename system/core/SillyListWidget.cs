using System;
using System.Collections.Generic;
using SillyWidgets.Gizmos;

namespace SillyWidgets
{
    public class SillyListItem : SillyView
    {        
    }

    public class SillyListWidget : SillyView
    {
        public List<SillyListItem> Items { get; private set; }

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
            return(string.Empty);
        }

        public override bool Attach(TreeNodeGizmo node)
        {
            return(true);
        }
    }
}