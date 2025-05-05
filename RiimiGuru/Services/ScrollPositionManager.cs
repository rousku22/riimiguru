using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace RiimiGuru.Services
{
    public class ScrollPositionManager
    {
        // Muistaa vierityspisteen arvon
        private double _scrollOffset;

        // Tallentaa RichTextBoxin nykyisen vierityspisteen arvon
        // Parametri:
        // richTextBox: RichTextBox-objekti, jonka vierityspiste tallennetaan.
        public void SaveScrollPosition(RichTextBox richTextBox)
        {
            var scrollViewer = GetScrollViewer(richTextBox);
            if (scrollViewer != null)
            {
                _scrollOffset = scrollViewer.VerticalOffset;
            }
        }

        // Palauttaa aiemmin tallennetun vierityspisteen RichTextBoxiin
        // Parametri:
        // richTextBox: RichTextBox-objekti, johon vierityspiste palautetaan.
        public void RestoreScrollPosition(RichTextBox richTextBox)
        {
            var scrollViewer = GetScrollViewer(richTextBox);
            if (scrollViewer != null)
            {
                scrollViewer.ScrollToVerticalOffset(_scrollOffset);
            }
        }

        // Hakee ScrollViewer-objektin annetusta DependencyObjectista
        // Parametri:
        // element: DependencyObject, josta ScrollViewer pyritään löytämään.
        private ScrollViewer GetScrollViewer(DependencyObject element)
        {
            if (element is ScrollViewer viewer)
            {
                return viewer;
            }

            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(element); i++)
            {
                var child = VisualTreeHelper.GetChild(element, i);
                var result = GetScrollViewer(child);
                if (result != null)
                {
                    return result;
                }
            }

            return null;
        }
    }
}
