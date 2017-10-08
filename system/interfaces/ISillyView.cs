using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SillyWidgets
{
    public enum SillyContentType { Html, Json }

    public interface ISillyView
    {
        /***** these two properties can be mothballed when everything is done, I think *****/
        SillyContentType ContentType { get; set; }
        string Content { get; set; }        
        /********************************************************************************* */
    }
}