using System;
using System.IO;
using System.Collections.Generic;

namespace silly
{
    public class SillyOption : CLIToken
    {
        public List<string> Parameters { get; private set; }

        protected int ParameterCount = 0;

        public SillyOption(string name, string description)
            : base(name, description, TokenTypes.Option)
        {
            Parameters = new List<string>();
        }

        public bool AddParameter(string param)
        {
            if (String.IsNullOrEmpty(param) ||
                Parameters.Count >= ParameterCount)
            {
                return(false);
            }

            Parameters.Add(param);

            return(true);
        }
    }

    public class HelpOption : SillyOption
    {
        public HelpOption(string id)
            : base(id, "Show help for this command/directive")
        {
            base.ParameterCount = 0;
        }
    }

    public class LocationOption : SillyOption
    {
        public LocationOption(string id)
            : base(id, "<path/to/silly/directory> : The silly directory to compile")
        {
            base.ParameterCount = 1;    
        }

        public DirectoryInfo GetLocation()
        {
            if (base.Parameters.Count == 0)
            {
                throw new Exception(base.Name + " requires a <path/to/directory> parameter");
            }

            DirectoryInfo location = new DirectoryInfo(base.Parameters[0]);

            if (location == null ||
                !location.Exists)
            {
                throw new Exception("location provided '" + location.FullName + "' doesn't exist");
            }

            return(location);
        }
    }

    public class SiteNameOption : SillyOption
    {
        public SiteNameOption(string id)
            : base(id, "\"name of site\" : The name of the new site")
        {
            base.ParameterCount = 1;
        }

        public string GetName()
        {
            if (base.Parameters.Count == 0)
            {
                throw new Exception(base.Name + " requires an actual \"site name\"");
            }

            return(base.Parameters[0]);
        }
    }
}