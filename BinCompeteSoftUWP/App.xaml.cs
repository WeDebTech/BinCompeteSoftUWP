using BinCompeteSoftUWP.Pages;
using System;
using System.Collections.Generic;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.ApplicationModel.Core;
using Windows.UI;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace BinCompeteSoftUWP
{
    /// <summary>
    /// Provides application-specific behavior to supplement the default Application class.
    /// </summary>
    sealed partial class App : Application
    {
        private static List<ContentDialogQueueItem> DialogueQueue { get; } = new List<ContentDialogQueueItem>();

        /// <summary>
        /// Initializes the singleton application object.  This is the first line of authored code
        /// executed, and as such is the logical equivalent of main() or WinMain().
        /// </summary>
        public App()
        {
            this.InitializeComponent();
            this.Suspending += OnSuspending;
        }

        /// <summary>
        /// Invoked when the application is launched normally by the end user.  Other entry points
        /// will be used such as when the application is launched to open a specific file.
        /// </summary>
        /// <param name="e">Details about the launch request and process.</param>
        protected override void OnLaunched(LaunchActivatedEventArgs e)
        {
            Frame rootFrame = Window.Current.Content as Frame;

            // Do not repeat app initialization when the Window already has content,
            // just ensure that the window is active
            if (rootFrame == null)
            {
                // Create a Frame to act as the navigation context and navigate to the first page
                rootFrame = new Frame();

                rootFrame.NavigationFailed += OnNavigationFailed;

                if (e.PreviousExecutionState == ApplicationExecutionState.Terminated)
                {
                    //TODO: Load state from previously suspended application
                }

                // Place the frame in the current Window
                Window.Current.Content = rootFrame;
            }

            if (e.PrelaunchActivated == false)
            {
                if (rootFrame.Content == null)
                {
                    // When the navigation stack isn't restored navigate to the first page,
                    // configuring the new page by passing required information as a navigation
                    // parameter
                    rootFrame.Navigate(typeof(LoginPage), e.Arguments);
                }
                // Ensure the current window is active
                Window.Current.Activate();

                CustomizeTitleBar();
            }
        }

        /// <summary>
        /// Invoked when Navigation to a certain page fails
        /// </summary>
        /// <param name="sender">The Frame which failed navigation</param>
        /// <param name="e">Details about the navigation failure</param>
        void OnNavigationFailed(object sender, NavigationFailedEventArgs e)
        {
            throw new Exception("Failed to load Page " + e.SourcePageType.FullName);
        }

        /// <summary>
        /// Invoked when application execution is being suspended.  Application state is saved
        /// without knowing whether the application will be terminated or resumed with the contents
        /// of memory still intact.
        /// </summary>
        /// <param name="sender">The source of the suspend request.</param>
        /// <param name="e">Details about the suspend request.</param>
        private void OnSuspending(object sender, SuspendingEventArgs e)
        {
            var deferral = e.SuspendingOperation.GetDeferral();
            //TODO: Save application state and stop any background activity
            deferral.Complete();
        }

        /// <summary>
        /// Sets the title bar as acrylic.
        /// </summary>
        private void CustomizeTitleBar()
        {
            CoreApplication.GetCurrentView().TitleBar.ExtendViewIntoTitleBar = false;
        }

        /// <summary>
        /// Adds a ContentDialog to a queue and shows the first to be added, and then the next one,
        /// until the queue is empty.
        /// </summary>
        /// <param name="contentDialog"></param>
        public static async void ShowContentDialog(ContentDialog contentDialog, Action<ContentDialogResult> callback)
        {
            App.DialogueQueue.Add(new ContentDialogQueueItem {
                ContentDialog = contentDialog,
                Callback = callback
            });

            // Add event handler for when dialog is closed.
            contentDialog.Closed += Dialog_Closed;

            // Check if it's the unique ContentDialog in queue.
            if(App.DialogueQueue.Count == 1)
            {
                var result = await contentDialog.ShowAsync();

                // If there's any callbacks, invoke them.
                callback?.DynamicInvoke(result);
            }
        }

        // Event handler for when ContentDialog is closed.
        private static async void Dialog_Closed(ContentDialog sender, ContentDialogClosedEventArgs e)
        {
            // Remove the ContentDialog that has just closed from the queue.
            App.DialogueQueue.RemoveAll(x => x.ContentDialog == sender);

            // Check if there's more ContentDialogs in queue.
            if (App.DialogueQueue.Count > 0)
            {
                var callback = App.DialogueQueue[0].Callback;

                var result = await App.DialogueQueue[0].ContentDialog.ShowAsync();

                // Check if there's any callbacks, and execute if so.
                callback?.DynamicInvoke(result);
            }
        }
    }

    /// <summary>
    /// This class holds a ContentDialog and a possible Action.
    /// </summary>
    class ContentDialogQueueItem
    {
        public ContentDialog ContentDialog { get; set; }
        public Action<ContentDialogResult> Callback { get; set; }
    }
}
