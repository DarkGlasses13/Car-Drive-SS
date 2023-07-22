using System;

namespace Assets._Project.Architecture.UI
{
    public interface IUIElement
    {
        void Open(Action callback);
        void Close(Action callback);
    }
}