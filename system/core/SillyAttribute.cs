using System;
using SillyWidgets.Gizmos;

namespace SillyWidgets
{
    public class SillyAttribute
    {
        public enum SillyAttrType { Text, View, Unsupported }
        public SillyAttrType Type { get; private set; }
        public string Key { get; set; }

        public SillyAttribute(string name)
        {
            Key = name;
        }

        protected SillyAttribute(SillyAttrType type)
        {            
            Type = type;
        }

        public static bool TryCreateSillyAttribute(string name, out SillyAttribute attribute)
        {
            attribute = null;

            if (name == null ||
                name.Length == 0)
            {
                return(false);
            }

            string[] parts = name.Split(new char[] { ':' });

            if (parts == null ||
                parts.Length <= 1)
            {
                return(false);
            }

            if (String.Compare(parts[0], "silly", false) == 0)
            {
                if (parts[1].Contains("text"))
                {
                    attribute = new SillyAttribute(SillyAttrType.Text);
                }
                else if (parts[1].Contains("view"))
                {
                    attribute = new SillyAttribute(SillyAttrType.View);
                }
                else
                {
                    attribute = new SillyAttribute(SillyAttrType.Unsupported);
                }      

                return(true);
            }

            return(false);
        }
    }
}