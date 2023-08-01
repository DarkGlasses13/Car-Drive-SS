using System;

namespace Assets._Project.Architecture.UI
{
    public interface IUIElement
    {
        void Show(Action callback = null);
        void Hide(Action callback = null);
    }
}