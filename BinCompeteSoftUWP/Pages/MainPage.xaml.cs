using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.System;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Animation;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace BinCompeteSoftUWP.Pages
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        #region Class variables
        private readonly List<(NavigationViewItem NavigationViewItem, Type Page)> JudgeMenuItems = new List<(NavigationViewItem NavigationViewItem, Type Page)>
        {
            (new NavigationViewItem
            {
                Content = "Dashboard",
                Icon = new SymbolIcon(Symbol.Home),
                Tag = "dashboard"
            }, typeof(JudgeDashboardPage)),
            (new NavigationViewItem
            {
                Content = "Add contest",
                Icon = new SymbolIcon(Symbol.Add),
                Tag = "add_contest"
            }, typeof(ContestPage)),
            (new NavigationViewItem
            {
                Content = "List contests",
                Icon = new SymbolIcon(Symbol.List),
                Tag = "list_contests"
            }, typeof(ContestsListPage))
        };
        private readonly List<(NavigationViewItem NavigationViewItem, Type Page)> AdministratorMenuItems = new List<(NavigationViewItem NavigationViewItem, Type Page)>
        { };

        private List<(string Tag, Type Page)> _pages = new List<(string Tag, Type Page)>();
        #endregion

        #region Class constructors
        public MainPage()
        {
            this.InitializeComponent();
        }
        #endregion

        #region Class event handlers
        private void NavigationViewPane_Loaded(object sender, RoutedEventArgs e)
        {
            // Check if current user is an administrator.
            if (Data.Instance.LoggedInUser.Administrator)
            {
                // Add Navigation View Items for navigation.
                foreach ((NavigationViewItem NavigationViewItem, Type Page) NavigationItem in AdministratorMenuItems)
                {
                    NavigationViewPane.MenuItems.Add(NavigationItem.NavigationViewItem);
                    _pages.Add((NavigationItem.NavigationViewItem.Tag.ToString(), NavigationItem.Page));
                }
            }
            else
            {
                // Add Navigation View Items for navigation.
                foreach ((NavigationViewItem NavigationViewItem, Type Page) NavigationItem in JudgeMenuItems)
                {
                    NavigationViewPane.MenuItems.Add(NavigationItem.NavigationViewItem);
                    _pages.Add((NavigationItem.NavigationViewItem.Tag.ToString(), NavigationItem.Page));
                }
            }

            // NavigationViewPane doesn't load any page by default, so load home page.
            NavigationViewPane.SelectedItem = NavigationViewPane.MenuItems[0];
            
            // Because we use ItemInvoked to navigate, we need to call Navigate
            // here to load the home page.
            NavigationViewPane_Navigate("dashboard", new EntranceNavigationTransitionInfo());

            // Add keyboard accelerators for backwards navigation.
            var goBack = new KeyboardAccelerator { Key = VirtualKey.GoBack };
            goBack.Invoked += BackInvoked;
            this.KeyboardAccelerators.Add(goBack);

            // ALT routes here.
            var altLeft = new KeyboardAccelerator
            {
                Key = VirtualKey.Left,
                Modifiers = VirtualKeyModifiers.Menu
            };
            altLeft.Invoked += BackInvoked;
            this.KeyboardAccelerators.Add(altLeft);
        }

        private void NavigationViewPane_ItemInvoked(NavigationView sender, NavigationViewItemInvokedEventArgs args)
        {
            if (args.IsSettingsInvoked)
            {
                NavigationViewPane_Navigate("settings", args.RecommendedNavigationTransitionInfo);
            }
            else if(args.InvokedItemContainer != null)
            {
                var navItemTag = args.InvokedItemContainer.Tag.ToString();
                NavigationViewPane_Navigate(navItemTag, args.RecommendedNavigationTransitionInfo);
            }
        }

        private void NavigationViewPane_Navigate(string navItemTag, NavigationTransitionInfo transitionInfo)
        {
            Type _page = null;
            if(navItemTag == "settings")
            {
                _page = typeof(SettingsPage);
            }
            else
            {
                var item = _pages.FirstOrDefault(p => p.Tag.Equals(navItemTag));
                _page = item.Page;
            }

            // Get the page type before navigation so you can prevent duplicate
            // entries in the backstack.
            var preNavPageType = ContentFrame.CurrentSourcePageType;

            // Only navigate if the selected page isn't currently loaded.
            if(!(_page is null) && !Type.Equals(preNavPageType, _page))
            {
                ContentFrame.Navigate(_page, null, transitionInfo);
            }
        }

        private void ContentFrame_Navigated(object sender, NavigationEventArgs e)
        {
            if(ContentFrame.SourcePageType == typeof(SettingsPage))
            {
                // SettingsItem is not part of the NavigationViewPane.MenuItems, and doesn't have a Tag.
                NavigationViewPane.SelectedItem = (NavigationViewItem)NavigationViewPane.SettingsItem;
                NavigationViewPane.Header = "Settings";
            }
            else if(ContentFrame.SourcePageType != null)
            {
                var item = _pages.FirstOrDefault(p => p.Page == e.SourcePageType);

                NavigationViewPane.SelectedItem = NavigationViewPane.MenuItems
                    .OfType<NavigationViewItem>()
                    .First(n => n.Tag.Equals(item.Tag));

                NavigationViewPane.Header = ((NavigationViewItem)NavigationViewPane.SelectedItem)?.Content?.ToString();
            }
        }

        private void BackInvoked(KeyboardAccelerator sender, KeyboardAcceleratorInvokedEventArgs args)
        {
            On_BackRequested();
            args.Handled = true;
        }

        private void NavigationViewPane_BackRequested(NavigationView sender, NavigationViewBackRequestedEventArgs args)
        {
            On_BackRequested();
        }

        private bool On_BackRequested()
        {
            if (!ContentFrame.CanGoBack)
                return false;

            // Don't go back if the nav pane is overlayed.
            if (NavigationViewPane.IsPaneOpen &&
                (NavigationViewPane.DisplayMode == NavigationViewDisplayMode.Compact ||
                NavigationViewPane.DisplayMode == NavigationViewDisplayMode.Minimal))
                return false;

            ContentFrame.GoBack();
            return true;
        }
        #endregion
    }
}
