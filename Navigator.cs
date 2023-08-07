using System;
using System.Windows;
using System.Windows.Navigation;

namespace SBWordListMaker
{
    internal static class Navigator
    {
        private static NavigationService NavigationService => (Application.Current.MainWindow as MainWindow)!.MainFrame.NavigationService;

        public static void Navigate(string fileName, object? param = null) => NavigationService.Navigate(new Uri($"/Views/{fileName}.xaml", UriKind.RelativeOrAbsolute), param);

        public static void GoBack() => NavigationService.GoBack();

        public static void GoForward() => NavigationService.GoForward();
    }
}
