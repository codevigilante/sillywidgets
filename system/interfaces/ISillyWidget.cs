using System;
using SillyWidgets.Gizmos;

namespace SillyWidgets
{
    public interface ISillyWidget
    {
        bool Attach(TreeNodeGizmo node);
        string Render(); 
    }
}