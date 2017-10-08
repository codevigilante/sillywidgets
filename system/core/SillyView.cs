using System;
using System.IO;
using SillyWidgets.Gizmos;

namespace SillyWidgets
{
    public class SillyView : ISillyView
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
        private string _content = string.Empty;

        private HtmlGizmo Html = null;

        public SillyView()
        {
            ContentType = SillyContentType.Html;
            Content = string.Empty;
        }

        public void Load(StreamReader data)
        {
            Html = new HtmlGizmo();
            bool success = Html.Load(data);

            if (!success)
            {
                throw new Exception("Parsing HTML: " + Html.ParseError);
            }
        }

        public string Render()
        {
            if (Html == null)
            {
                return(_content);
            }

            _content = Html.Payload();
            
            return(_content);
        }

    }
}