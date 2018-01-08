using System;

namespace SillyWidgets
{
    public class SillyTextWidget : ISillyWidget
    {
        public string Text { get; private set; }

        public SillyTextWidget(string text)
        {
            Text = text;
        }

        public string Render()
        {
            return(Text);
        }
    }
}