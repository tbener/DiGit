using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using DiGit.Configuration;
using DiGit.Helpers;
using DiGit.View;
using DiGit.ViewModel;

namespace DiGit.Model
{
    public static class BubblesManager
    {
        private static readonly List<Window> Views;

        private const double RIGHT_MARGIN = 140;
        private const double HORISONTAL_MARGIN = 40;

        private static bool _showBubbles = true;

        static BubblesManager()
        {
            Views = new List<Window>();

            OwnerWindow = new Window();
            OwnerWindow.WindowStyle = WindowStyle.ToolWindow;
            OwnerWindow.Left = 10000;
            OwnerWindow.Top = 1000;
            OwnerWindow.Width = 1;
            OwnerWindow.Show();
            OwnerWindow.Hide();
        }

        public static Window OwnerWindow { get; private set; }

        public static void Refresh(bool hard=false)
        {
            if (hard) Views.Clear();
            RepositoriesManager.Repos.ForEach(Add);
        }

        internal static BubbleView CreateBubble()
        {
            BubbleView bubble = new BubbleView();
            bubble.Owner = OwnerWindow;
            return bubble;
        }

        #region Add

        private static void Add(DiGitConfigRepository repo)
        {
            if (repo.View == null)
            {
                repo.View = CreateBubble();
                BubbleViewModel vm = new BubbleViewModel(repo.Repository);
                repo.View.DataContext = vm;
                Add(repo.View, repo.isActive);
                vm.Start(repo.View);
            }
            else
            {
                Add(repo.View, repo.isActive);
            }
        }

        private static void Add(Window view, bool active)
        {

            if (active) Position(view);
            if (Views.Contains(view)) return;

            Views.Add(view);
            view.LocationChanged += new EventHandler(view_LocationChanged);
        }

        #endregion

        #region Position

        private static void view_LocationChanged(object sender, EventArgs e)
        {
            Window view = sender as Window;
            if (view != null)
            {
                if (view.Top < 10)
                    view.Top = 0;
            }
        }

        private static void Position(Window view)
        {
            double x = SystemParameters.PrimaryScreenWidth - RIGHT_MARGIN;
            Rect rect = new Rect(new Point(x - view.Width, 0), new Point(x, view.Height));
            Window otherView;

            while (IsRectInWindow(rect, view, out otherView))
            {
                rect.Offset(otherView.Left - rect.Left - HORISONTAL_MARGIN - view.Width, 0);
                if (rect.X < 0) return; // position failed
            }

            view.Left = rect.Left;
            view.Top = rect.Top;

        }

        /// <summary>
        /// Check if the rect intersects with one of the existing windows.
        /// </summary>
        /// <param name="rect"></param>
        /// <param name="view"></param>
        /// <param name="otherView">The window that intersects with the rect</param>
        /// <returns>true if intersection found</returns>
        private static bool IsRectInWindow(Rect rect, Window view, out Window otherView)
        {

            otherView = Views.FirstOrDefault(v => !v.Equals(view) && v.Visibility == Visibility.Visible && rect.IntersectsWith(WindowToRect(v)));
            if (otherView != null) return true;

            return false;
        }

        private static Rect WindowToRect(Point pt, Window view)
        {
            return new Rect(pt, new Point(pt.X + view.Width, pt.Y + view.Height));
        }

        private static Rect WindowToRect(Window view)
        {
            return new Rect(new Point(view.Left, view.Top), new Point(view.Left + view.Width, view.Top + view.Height));
        }
        
        #endregion

        #region Show / Hide
        
        internal static void ShowView(Window view, bool show, bool updateActive=true)
        {
            if (!view.CheckAccess())
            {
                var action = new Action(() => ShowView(view, show, updateActive));
                view.Dispatcher.Invoke(action);
                return;
            }

            if (updateActive)
                ConfigurationHelper.Configuration.Repositories.FirstOrDefault(r => r.View == view).isActive = show;

            if (show)
            {
                if (view.Visibility == Visibility.Hidden) Position(view);
                view.Show();
            }
            else 
                view.Hide();
        }

        internal static void ShowAll(bool show)
        {
            _showBubbles = show;
            Views.ForEach(v => ShowView(v, show, show));   // updateActive = show because if the user shows all,
            // they all should become Active, but if he hides them all, the Active property should not changed.
        }

        internal static void ToggleShowHide()
        {
            _showBubbles = !_showBubbles;

            ConfigurationHelper.Configuration.Repositories.Where(r => r.isActive).ToList().ForEach(r => ShowView(r.View, _showBubbles, false));
        }

        #endregion

        internal static void CloseAll()
        {
            Views.ForEach(v => v.Close());
            Views.Clear();
        }

       
    }
}
