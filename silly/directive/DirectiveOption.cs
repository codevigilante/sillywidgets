using System;
using System.Collections.Generic;

namespace silly
{
    public class DirectiveOption
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public List<string> Parameters { get; private set; }

        public DirectiveOption(string name, string description, List<string> parameters = null)
        {
            Name = name;
            Description = description;

            if (parameters == null)
            {
                Parameters = new List<string>();
            }
            else
            {
                Parameters = parameters;
            }
        }

    }
}