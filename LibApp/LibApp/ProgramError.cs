using System.Windows;

namespace LibApp
{
    public static class ProgramError
    {
        public static void Show(string message)
        {
            MessageBox.Show(message, "Праграмная памылка", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }
}
