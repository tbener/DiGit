using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using DiGit.Configuration;
using DiGit.Helpers;
using DiGit.View;
using DiGit.ViewModel;
using System.ComponentModel;

namespace DiGit.Model
{
    public static class BubblesManager
    {

        #region Private variables

        private static List<DiGitConfigRepository> _repos;
        private static bool _showBubbles = true;
        private static bool _locationChanged = false;

        #endregion

        #region Properties
        public static Window OwnerWindow { get; private set; }

        #endregion

        #region CTOR

        static BubblesManager()
        {
            OwnerWindow = new Window();
            OwnerWindow.ShowInTaskbar = false;
            OwnerWindow.WindowStyle = WindowStyle.ToolWindow;
            OwnerWindow.Left = 10000;
            OwnerWindow.Top = 1000;
            OwnerWindow.Width = 1;
            OwnerWindow.Show();
            OwnerWindow.Hide();
        }

        #endregion

        #region Refresh \ Set View

        public static void Refresh()
        {
            _repos = ConfigurationHelper.Configuration.RepositoryList;
            if (_repos.Any())
            {
                _repos.ForEach(SetBubbleView);
                Arrange();
            }
        }

        private static void SetBubbleView(DiGitConfigRepository repo)
        {
            if (repo.View == null)
            {
                repo.View = new BubbleView(OwnerWindow);
                BubbleViewModel vm = new BubbleViewModel(repo.Repository);
                repo.View.DataContext = vm;
                vm.Start(repo.View);

                repo.View.LocationChanged += view_LocationChanged;
                repo.View.MouseUp += view_MouseUp;
            }
        }



        #endregion

        #region Position

        internal static void Arrange(Window exceptView = null)
        {
            double x = Anchor == HorizontalAlignment.Left ? FirstSpacing : SystemParameters.PrimaryScreenWidth - FirstSpacing - BubbleWidth;
            double spacing = (Spacing + BubbleWidth) * (Anchor == HorizontalAlignment.Left ? 1 : -1);

            foreach (var repo in _repos.Where(r => r.isActive))
            {
                var view = repo.View;
                if ((ConfigurationHelper.Configuration.Settings.Bubbles.autoArrange || repo.IsNew) && view != exceptView)
                {
                    view.Left = x;
                    // get the screen this view is on, to get the top y
                    view.Top = System.Windows.Forms.Screen.FromPoint(new System.Drawing.Point((int)x, (int)view.Top)).Bounds.Top;
                    //view.Top = System.Windows.Forms.Screen.FromHandle(new System.Windows.Interop.WindowInteropHelper(view).Handle).Bounds.Top;
                    view.Topmost = true;
                    view.BringIntoView();
                }

                x += spacing;
            }
        }

        private static void view_MouseUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (_locationChanged)
            {
                if (ConfigurationHelper.Configuration.Settings.Bubbles.autoArrange)
                {
                    Arrange(); 
                }
                _locationChanged = false;
                ConfigurationHelper.Save();
            }
        }

        private static void view_LocationChanged(object sender, EventArgs e)
        {
            Window view = sender as Window;
            if (!view.IsActive) return;

            _locationChanged = true;

            // default screen top (for primary screen)
            int screenTop = 0;

            if (view != null)
            {
                // get the top y of current screen
                screenTop = System.Windows.Forms.Screen.FromHandle(new System.Windows.Interop.WindowInteropHelper(view).Handle).Bounds.Top;
                if (view.Top < screenTop + 10)
                    view.Top = screenTop;
            }


            if (ConfigurationHelper.Configuration.Settings.Bubbles.autoArrange)
            {
                // If AutoArrange: change other bubbles according to this one

                var _views = GetActiveViews();
                
                if (view.Top < screenTop + 40)
                {
                    // If it's on top (user drags bubble sideway)
                    // Change the spacing between the bubbles

                    if (view == _views[0])
                    {
                        // If it's the first one - change the distance from the wall
                        FirstSpacing = Anchor == HorizontalAlignment.Left ? view.Left : SystemParameters.PrimaryScreenWidth - view.Left - view.Width;
                    }
                    else
                    {
                        // For any other one - change the spacing
                        int i = _views.IndexOf(view);
                        Spacing = ((_views[0].Left - view.Left) / i) - BubbleWidth;
                    }
                }
                else
                    // If the bubble was dragged down and then to the side,
                    // change the sort order according to the x axis value.
                    Sort();

                // while keep dragging this bubble, arrange the others
                Arrange(view);
            }
        }

        public static void ResetPositionsToDefault()
        {
            bool saveAutoArrange = ConfigurationHelper.Configuration.Settings.Bubbles.autoArrange;
            ConfigurationHelper.Configuration.Settings.Bubbles = new DiGitConfigSettingsBubbles();
            Arrange();
            ConfigurationHelper.Configuration.Settings.Bubbles.autoArrange = saveAutoArrange;
        }

        private static void Sort()
        {
            if (Anchor == HorizontalAlignment.Right)
                _repos.Sort((r2, r1) => r1.View.Left.CompareTo(r2.View.Left));
            else
                _repos.Sort((r1, r2) => r1.View.Left.CompareTo(r2.View.Left));
        }

        #endregion

        #region Position properties

        public static double FirstSpacing
        {
            get
            {
                return ConfigurationHelper.Configuration.Settings.Bubbles.firstSpacing;
            }
            set
            {
                ConfigurationHelper.Configuration.Settings.Bubbles.firstSpacing = value; 
            }
        }

        public static double BubbleWidth
        {
            get
            {
                return 160;
            }
        }
        public static double Spacing
        {
            get
            {
                return ConfigurationHelper.Configuration.Settings.Bubbles.spacing;
            }
            set
            {
                ConfigurationHelper.Configuration.Settings.Bubbles.spacing = Math.Max(value, -BubbleWidth + 40); // Math.Min(Math.Max(value, -BubbleWidth + 40), 250);
            }
        }

        public static HorizontalAlignment Anchor
        {
            get
            {
                return ConfigurationHelper.Configuration.Settings.Bubbles.PositionAnchor;
            }
            set
            {
                ConfigurationHelper.Configuration.Settings.Bubbles.PositionAnchor = value;
                Arrange();
            }
        }
        #endregion

        #region GetViews helpers

        public static List<Window> GetActiveViews()
        {
            return _repos.Where(r => r.isActive).Select(r => r.View).ToList();
        }

        public static List<Window> GetAllViews()
        {
            return _repos.Select(r => r.View).ToList();
        }

        #endregion

        #region Show / Hide

        internal static void ShowView(Window view, bool show, bool updateActive = true)
        {
            if (!view.CheckAccess())
            {
                var action = new Action(() => ShowView(view, show, updateActive));
                view.Dispatcher.Invoke(action);
                return;
            }
            
            if (updateActive)
                _repos.FirstOrDefault(r => r.View == view).isActive = show;

            Arrange();

            if (show)
            {
                //if (view.Visibility == Visibility.Hidden) Position(view);
                view.Show();
            }
            else
                view.Hide();
        }

        internal static void ShowAll(bool show)
        {
            _showBubbles = show;
            GetAllViews().ForEach(v => ShowView(v, show, show));   // updateActive = show because if the user shows all,
            // they all should become Active, but if he hides them all, the Active property should not changed.
        }

        // Show / hide without change the isActive property
        internal static void ToggleShowHide()
        {
            _showBubbles = !_showBubbles;

            GetActiveViews().ForEach(v => ShowView(v, _showBubbles, false));
        }

        internal static void HideAllButThis(Window view)
        {
            GetActiveViews().Where(v => v != view).ToList().ForEach(v => ShowView(v, false, true));
        }


        #endregion

        #region Close views

        internal static void Close(Window view)
        {
            view.Close();
        }

        internal static void CloseAll()
        {
            GetAllViews().ForEach(v => v.Close());
        }

        #endregion

        #region Not in use. keep for now...

        /// <summary>
        /// Check if the rect intersects with one of the existing windows.
        /// </summary>
        /// <param name="rect"></param>
        /// <param name="view"></param>
        /// <param name="otherView">The window that intersects with the rect</param>
        /// <returns>true if intersection found</returns>
        //private static bool IsRectInWindow(Rect rect, Window view, out Window otherView)
        //{

        //    otherView = Views.FirstOrDefault(v => !v.Equals(view) && v.Visibility == Visibility.Visible && rect.IntersectsWith(WindowToRect(v)));
        //    if (otherView != null) return true;

        //    return false;
        //}

        //private static Rect WindowToRect(Point pt, Window view)
        //{
        //    return new Rect(pt, new Point(pt.X + view.Width, pt.Y + view.Height));
        //}

        //private static Rect WindowToRect(Window view)
        //{
        //    return new Rect(new Point(view.Left, view.Top), new Point(view.Left + view.Width, view.Top + view.Height));
        //}

        //private static T GetDefaultValue<T>(string propName)
        //{
        //    var property = typeof(DiGitConfigSettingsBubbles).GetProperty(propName);
        //    var attribute = property.GetCustomAttributes(typeof(DefaultValueAttribute), false).FirstOrDefault() as DefaultValueAttribute;
        //    return (T)attribute.Value;
        //}

        #endregion
    }
}
