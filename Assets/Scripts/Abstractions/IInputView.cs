namespace CrazyPawn
{
    public interface IInputView<T>
    {
        public delegate void Click(T view);
        public delegate void PointerDown(T view);
        public delegate void PointerDrag(T view);
        public delegate void PointerUp(T view);

        public event Click OnClick;
        public event PointerDown OnPointerDown;
        public event PointerDrag OnPointerDrag;
        public event PointerUp OnPointerUp;
    }
}