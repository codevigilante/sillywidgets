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
        public virtual ISillyWidget Widget { get; set; }

        public SillyAttribute(string name, SillyAttrType type, ISillyWidget widget = null)
        {            
            Type = type;
            Name = name;
            Widget = widget;
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
}