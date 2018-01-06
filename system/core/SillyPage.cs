using System;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Collections.Generic;

namespace SillyWidgets
{
    public abstract class SillyPage : SillyView, ISillyPage
    {
        public SillyContentType ContentType { get; set; }

        public string Content
        { 
            get
            {
                if (String.IsNullOrEmpty(_content))
                {
                    return(Render());
                }

                return(_content);
            } 
            set
            {
                _content = value;
            }
        }

        public string Name
        {
            get { return(_name); }
        }

        public string UrlPrefix
        {
            get { return(_urlPrefix); }
        }

        public bool AcceptsUrlParameters
        {
            get { return(_acceptUrlParams); }
        }

        private string _content = string.Empty;
        private string _name = string.Empty;
        private string _urlPrefix = string.Empty;
        private bool _acceptUrlParams = false;

        public abstract bool Accept(ISillyContext context, string[] urlParams);

        public SillyPage(bool acceptUrlParams = false)
            : base()
        {
            ContentType = SillyContentType.Html;
            Content = string.Empty;
            _acceptUrlParams = acceptUrlParams;
        }

        public SillyPage(string name, bool acceptUrlParams = false)
            : this(acceptUrlParams)
        {
            _name = name;
        }

        public SillyPage(string name, string urlPrefix, bool acceptUrlParams = false)
            : this(name, acceptUrlParams)
        {
            _urlPrefix = urlPrefix;
        }
    }
}