namespace AvalonStudio.Extensibility.Shell
{
    public interface IStatusBar
    {
        bool SetText(string text);

        void ClearText();

        void SetTextPosition(int offset, int line, int column);

        void ClearTextPosition();
    }
}
