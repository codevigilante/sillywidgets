using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.IO;

namespace SillyWidgets
{
    public class SillyController
    {
        public SillyController()
        {
        }

        public SillyView LoadView(string filepath)
        {
            if (filepath == null ||
                filepath.Length == 0)
            {
                throw new SillyException(SillyHttpStatusCode.NotFound, "Invalid view file specified, either NULL or empty");
            }

            if (!File.Exists(filepath))
            {
                throw new SillyException(SillyHttpStatusCode.NotFound, "View file '" + filepath + "' does not exist");
            }

            SillyView view = new SillyView();
            FileStream fileStream = new FileStream(filepath, FileMode.Open);

            using (StreamReader reader = new StreamReader(fileStream))
            {
                view.Load(reader);
            }

            return(view);
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