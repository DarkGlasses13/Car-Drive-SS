using System;

namespace Assets._Project.Architecture.UI
{
    public interface IUIElement
    {
        void Hide(Action callback = null);
        void Show(Action callback = null);
    }
}