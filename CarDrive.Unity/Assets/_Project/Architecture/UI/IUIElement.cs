using System;

namespace Assets._Project.Architecture.UI
{
    public interface IUIElement
    {
        void Open(Action callback = null);
        void Close(Action callback = null);
    }
}