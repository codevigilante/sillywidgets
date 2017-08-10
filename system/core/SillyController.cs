using System;
using System.Threading.Tasks;

namespace SillyWidgets
{
    public class SillyController
    {
        public SillyController()
        {
        }

        public T LoadView<T>(string filename) where T : ISillyView
        {
            return(default(T));
        }
    }
}