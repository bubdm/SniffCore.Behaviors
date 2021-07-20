//
// Copyright (c) David Wendland. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
//

using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using SniffCore.Behaviors.Internal;

namespace SniffCore.Behaviors
{
    /// <summary>
    ///     Brings the feature to controls with a <see cref="System.Windows.Controls.GridViewColumnHeader" /> to have columns
    ///     with a dynamic width.
    /// </summary>
    /// <example>
    ///     <code lang="XAML">
    /// <![CDATA[
    /// <ListView controls:ColumnWidthBehavior.AutoSize="ByContent" />
    /// 
    /// <ListView controls:ColumnWidthBehavior.AutoSize="Proportional">
    ///     <ListView.View>
    ///         <GridView>
    ///             <GridViewColumn Header="Name" DisplayMemberBinding="{Binding Name}" controls:ColumnWidthBehavior.ProportionalWidth="60" />
    ///             <GridViewColumn Header="Size" DisplayMemberBinding="{Binding Size}" controls:ColumnWidthBehavior.ProportionalWidth="30" />
    ///             <GridViewColumn Header="Date" DisplayMemberBinding="{Binding Date}" controls:ColumnWidthBehavior.ProportionalWidth="10" />
    ///         </GridView>
    ///     </ListView.View>
    /// </ListView>
    /// ]]>
    /// </code>
    /// </example>
    public class ColumnWidthBehavior
    {
        /// <summary>
        ///     Identifies the <see cref="GetAutoSize(DependencyObject)" />
        ///     <see cref="SetAutoSize(DependencyObject, ColumnResizeKind)" /> attached property.
        /// </summary>
        public static readonly DependencyProperty AutoSizeProperty =
            DependencyProperty.RegisterAttached("AutoSize", typeof(ColumnResizeKind), typeof(ColumnWidthBehavior), new UIPropertyMetadata(OnAutoSizeChanged));

        /// <summary>
        ///     Identifies the <see cref="GetProportionalWidth(DependencyObject)" />
        ///     <see cref="SetProportionalWidth(DependencyObject, double)" /> attached property.
        /// </summary>
        public static readonly DependencyProperty ProportionalWidthProperty =
            DependencyProperty.RegisterAttached("ProportionalWidth", typeof(double), typeof(ColumnWidthBehavior), new UIPropertyMetadata(double.NaN));

        /// <summary>
        ///     Identifies the <see cref="GetTemplatePaddingWidthFix(DependencyObject)" />
        ///     <see cref="SetTemplatePaddingWidthFix(DependencyObject, double)" /> attached property.
        /// </summary>
        public static readonly DependencyProperty TemplatePaddingWidthFixProperty =
            DependencyProperty.RegisterAttached("TemplatePaddingWidthFix", typeof(double), typeof(ColumnWidthBehavior), new UIPropertyMetadata(10.0));

        private static readonly DependencyProperty OriginalWidthIsRememberedProperty =
            DependencyProperty.RegisterAttached("OriginalWidthIsRemembered", typeof(bool), typeof(ColumnWidthBehavior), new UIPropertyMetadata(false));

        private static readonly DependencyProperty OriginalWidthProperty =
            DependencyProperty.RegisterAttached("OriginalWidth", typeof(double), typeof(ColumnWidthBehavior), new UIPropertyMetadata(0.0));

        private static readonly DependencyProperty ColumnWidthBehaviorProperty =
            DependencyProperty.RegisterAttached("ColumnWidthBehavior", typeof(ColumnWidthBehavior), typeof(ColumnWidthBehavior), new UIPropertyMetadata(null));

        private bool _changedEventCatched;
        private GridViewColumnCollection _columns;

        private DependencyObject _owner;
        private ScrollContentPresenter _scrollContentPresenter;
        private List<GridViewColumn> _toResizeColumns;

        private ColumnWidthBehavior()
        {
        }

        /// <summary>
        ///     Gets resize kind for a column.
        /// </summary>
        /// <param name="obj">The element from which the property value is read.</param>
        /// <returns>The ColumnWidthBehavior.AutoSize property value for the element.</returns>
        public static ColumnResizeKind GetAutoSize(DependencyObject obj)
        {
            return (ColumnResizeKind) obj.GetValue(AutoSizeProperty);
        }

        /// <summary>
        ///     Attaches the resize kind for a column.
        /// </summary>
        /// <param name="obj">The element to which the attached property is written.</param>
        /// <param name="value">The needed ColumnWidthBehavior.AutoSize value.</param>
        public static void SetAutoSize(DependencyObject obj, ColumnResizeKind value)
        {
            obj.SetValue(AutoSizeProperty, value);
        }

        private static void OnAutoSizeChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            var behavior = GetOrSetBehavior(sender);
            var element = (FrameworkElement) sender;
            behavior._owner = sender;
            element.Loaded += behavior.Element_Loaded;
        }

        /// <summary>
        ///     Gets proportional size in percent for a column.
        /// </summary>
        /// <param name="obj">The element from which the property value is read.</param>
        /// <returns>The ColumnWidthBehavior.ProportionalWidth property value for the element.</returns>
        public static double GetProportionalWidth(DependencyObject obj)
        {
            return (double) obj.GetValue(ProportionalWidthProperty);
        }

        /// <summary>
        ///     Attaches the proportional size in percent for a column.
        /// </summary>
        /// <param name="obj">The element to which the attached property is written.</param>
        /// <param name="value">The needed ColumnWidthBehavior.ProportionalWidth value.</param>
        public static void SetProportionalWidth(DependencyObject obj, double value)
        {
            obj.SetValue(ProportionalWidthProperty, value);
        }

        /// <summary>
        ///     Gets additional space left from a column by calculating the width.
        /// </summary>
        /// <param name="obj">The element from which the property value is read.</param>
        /// <returns>The ColumnWidthBehavior.TemplatePaddingWidthFix property value for the element.</returns>
        public static double GetTemplatePaddingWidthFix(DependencyObject obj)
        {
            return (double) obj.GetValue(TemplatePaddingWidthFixProperty);
        }

        /// <summary>
        ///     Attaches the additional space left from a column by calculating the width.
        /// </summary>
        /// <param name="obj">The element to which the attached property is written.</param>
        /// <param name="value">The needed ColumnWidthBehavior.TemplatePaddingWidthFix value.</param>
        public static void SetTemplatePaddingWidthFix(DependencyObject obj, double value)
        {
            obj.SetValue(TemplatePaddingWidthFixProperty, value);
        }

        private static bool GetOriginalWidthIsRemembered(DependencyObject obj)
        {
            return (bool) obj.GetValue(OriginalWidthIsRememberedProperty);
        }

        private static void SetOriginalWidthIsRemembered(DependencyObject obj, bool value)
        {
            obj.SetValue(OriginalWidthIsRememberedProperty, value);
        }

        private static double GetOriginalWidth(DependencyObject obj)
        {
            return (double) obj.GetValue(OriginalWidthProperty);
        }

        private static void SetOriginalWidth(DependencyObject obj, double value)
        {
            obj.SetValue(OriginalWidthProperty, value);
        }

        private static ColumnWidthBehavior GetColumnWidthBehavior(DependencyObject obj)
        {
            return (ColumnWidthBehavior) obj.GetValue(ColumnWidthBehaviorProperty);
        }

        private static void SetColumnWidthBehavior(DependencyObject obj, ColumnWidthBehavior value)
        {
            obj.SetValue(ColumnWidthBehaviorProperty, value);
        }

        private static ColumnWidthBehavior GetOrSetBehavior(DependencyObject sender)
        {
            var behavior = GetColumnWidthBehavior(sender);
            if (behavior == null)
            {
                behavior = new ColumnWidthBehavior();
                SetColumnWidthBehavior(sender, behavior);
            }

            return behavior;
        }

        private void Element_Loaded(object sender, RoutedEventArgs e)
        {
            Resize();
        }

        private void ScrollContentPresenter_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            Resize();
        }

        private void Resize()
        {
            var kind = GetAutoSize(_owner);
            if (kind == ColumnResizeKind.NoResize)
                return;

            if (_columns == null)
                TryReadColumns();

            if (_scrollContentPresenter == null)
            {
                _scrollContentPresenter = FindPresenter();
                if (_scrollContentPresenter != null)
                    _scrollContentPresenter.SizeChanged += ScrollContentPresenter_SizeChanged;
            }

            if (_columns == null || _scrollContentPresenter == null)
                return;

            switch (kind)
            {
                case ColumnResizeKind.ByContent:
                    ResizeByContent();
                    break;
                case ColumnResizeKind.ByControl:
                    ResizeByControl();
                    break;
                case ColumnResizeKind.Proportional:
                    ResizeProportional();
                    break;
            }
        }

        private void TryReadColumns()
        {
            var presenter = VisualTreeAssist.FindChild<GridViewHeaderRowPresenter>(_owner);
            if (presenter != null)
                _columns = presenter.Columns;

            if (_columns == null)
                return;

            SetOriginalWidths();
            _columns.CollectionChanged += (a, b) => Reset();
        }

        private void Reset()
        {
            if (_toResizeColumns != null)
            {
                foreach (var column in _toResizeColumns.Where(GetOriginalWidthIsRemembered))
                    column.Width = GetOriginalWidth(column);
                _toResizeColumns.Clear();
                _toResizeColumns = null;
            }

            Resize();
        }

        private void SetOriginalWidths()
        {
            foreach (var column in _columns)
            {
                SetOriginalWidth(column, column.Width);
                SetOriginalWidthIsRemembered(column, true);
            }
        }

        private ScrollContentPresenter FindPresenter()
        {
            var internalScrollViewer = VisualTreeAssist.FindChild<ScrollViewer>(_owner);
            return VisualTreeAssist.FindChild<ScrollContentPresenter>(internalScrollViewer);
        }

        private void ResizeByContent()
        {
            if (!_changedEventCatched)
            {
                _changedEventCatched = true;

                var itemsControl = (ItemsControl) _owner;
                var coll = CollectionViewSource.GetDefaultView(itemsControl.Items);
                coll.CollectionChanged += (a, b) => { ResizeByContent(); };
            }

            foreach (var column in _columns)
            {
                if (!double.IsNaN(column.Width))
                    continue;

                column.Width = 0;
                column.Width = double.NaN;
            }
        }

        private void ResizeByControl()
        {
            var maxWidth = _scrollContentPresenter.ActualWidth;
            maxWidth -= GetTemplatePaddingWidthFix(_owner);

            if (maxWidth > 0)
            {
                var leftDistance = CalculateFixedDistance();
                foreach (var column in _toResizeColumns)
                {
                    var newWidth = (maxWidth - leftDistance) / _toResizeColumns.Count;
                    if (newWidth >= 0)
                        column.Width = newWidth;
                }
            }
        }

        private void ResizeProportional()
        {
            var maxWidth = _scrollContentPresenter.ActualWidth;
            maxWidth -= GetTemplatePaddingWidthFix(_owner);

            if (maxWidth > 0)
                foreach (var column in _toResizeColumns)
                {
                    var proportion = GetProportionalWidth(column);
                    if (!double.IsNaN(proportion))
                    {
                        var newWidth = maxWidth / 100.0 * proportion;
                        if (newWidth >= 0)
                            column.Width = newWidth;
                    }
                }
        }

        private double CalculateFixedDistance()
        {
            var leftDistance = 0d;
            if (_toResizeColumns == null)
            {
                _toResizeColumns = new List<GridViewColumn>();
                foreach (var column in _columns)
                    if (column.Width >= 0)
                        leftDistance += column.ActualWidth;
                    else
                        _toResizeColumns.Add(column);
            }
            else
            {
                leftDistance = _columns.Where(column => !_toResizeColumns.Contains(column)).Sum(column => column.ActualWidth);
            }

            return leftDistance;
        }
    }
}