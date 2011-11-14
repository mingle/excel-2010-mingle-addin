using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;

namespace WpfControls.MultiSelectList
{
    public static class MultiSelect
    {
        static MultiSelect()
        {
            ItemsControl.ItemsSourceProperty.OverrideMetadata(typeof(Selector), new FrameworkPropertyMetadata(ItemsSourceChanged));
        }

        public static bool GetIsEnabled(Selector target)
        {
            return (bool)target.GetValue(IsEnabledProperty);
        }

        public static void SetIsEnabled(Selector target, bool value)
        {
            target.SetValue(IsEnabledProperty, value);
        }

        public static readonly DependencyProperty IsEnabledProperty =
            DependencyProperty.RegisterAttached("IsEnabled", typeof(bool), typeof(MultiSelect), 
                new UIPropertyMetadata(IsEnabledChanged));

        static void IsEnabledChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            var selector = sender as Selector;
            var collectionView = selector.ItemsSource as IMultiSelectCollectionView;

            if (collectionView != null)
            {
                if ((bool)e.NewValue)
                {
                    collectionView.AddControl(selector);
                }
                else
                {
                    collectionView.RemoveControl(selector);
                }
            }
        }

        static void ItemsSourceChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            var selector = sender as Selector;

            if (GetIsEnabled(selector))
            {
                var oldCollectionView = e.OldValue as IMultiSelectCollectionView;
                var newCollectionView = e.NewValue as IMultiSelectCollectionView;

                if (oldCollectionView != null)
                {
                    oldCollectionView.RemoveControl(selector);
                }

                if (newCollectionView != null)
                {
                    newCollectionView.AddControl(selector);
                }
            }
        }
    }
}
