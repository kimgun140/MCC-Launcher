
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows;
using Microsoft.Xaml.Behaviors; // 중요: 이 네임스페이스

namespace MCC_Launcher.Utilies
{
    public class ScrollToTopBehavior : Behavior<ScrollViewer>
    {
        public static readonly DependencyProperty TriggerProperty =
            DependencyProperty.Register(
                nameof(Trigger),
                typeof(bool),
                typeof(ScrollToTopBehavior),
                new PropertyMetadata(false, OnTriggerChanged));

        public bool Trigger
        {
            get => (bool)GetValue(TriggerProperty);
            set => SetValue(TriggerProperty, value);
        }

        private static void OnTriggerChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is ScrollToTopBehavior behavior &&
                behavior.AssociatedObject is ScrollViewer scrollViewer &&
                (bool)e.NewValue)
            {
                //scrollViewer.Dispatcher.BeginInvoke(new Action(() =>
                //{
                //    var extent = scrollViewer.ExtentHeight;
                //    var offset = scrollViewer.VerticalOffset;
                //    var maxOffset = scrollViewer.ScrollableHeight;

                //    scrollViewer.ScrollToTop();
                //    behavior.Trigger = false; // Reset은 여기서
                //}), System.Windows.Threading.DispatcherPriority.Background);

                scrollViewer.Dispatcher.BeginInvoke(new Action(() =>
                {
                    scrollViewer.ScrollToTop();

                    // Trigger를 false로 초기화 (다음에 다시 실행될 수 있도록)
                    behavior.Trigger = false;

                }), System.Windows.Threading.DispatcherPriority.Background);
            }
        }
    }
}
