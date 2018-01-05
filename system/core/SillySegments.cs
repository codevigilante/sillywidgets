using System;
using System.Collections.Generic;


namespace SillyWidgets
{
    public class SillySegment
    {
        public ISillyView View { get; set; }
        public string SegmentID { get; private set; }
        public SillySegment Parent { get; private set; }

        private Dictionary<string, SillySegment> Children = new Dictionary<string, SillySegment>();

        public SillySegment(string segmentID, ISillyView view = null)
        {
            SegmentID = segmentID.ToLower();
            Parent = null;
            View = view;
        }

        public bool AddChild(SillySegment child)
        {
            if (child == null ||
                String.IsNullOrEmpty(child.SegmentID))
            {
                return(false);
            }

            if (Children.ContainsKey(child.SegmentID))
            {
                return(false);
            }

            Children.Add(child.SegmentID, child);

            return(true);
        }

        public bool FindPath(string segmentID, out SillySegment segment)
        {
            segment = null;

            if (Children.TryGetValue(segmentID.ToLower(), out segment))
            {
                return(true);
            }

            return(false);
        }
    }
}