using System;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace SillyWidgets
{
    public class SillyController
    {
        public SillyController()
        {
        }

        public T LoadViewAsync<T>() where T : ISillyView
        {
            return(default(T));
        }

        public ISillyView LoadViewAsync(SillyResource viewFile, List<SillyResource> widgetFiles)
        {
            return(null);
        }

        public void LoadData()
        {
            
        }
    }
}