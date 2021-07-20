//
// Copyright (c) David Wendland. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
//

using System;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using SniffCore.Behaviors.Internal;

namespace SniffCore.Behaviors
{
    /// <summary>
    ///     Brings the feature to UI elements with a <see cref="System.Windows.Controls.GridViewColumnHeader" /> to have a
    ///     sorting by clicking the corresponding header.
    /// </summary>
    /// <example>
    ///     <code lang="XAML">
    /// <![CDATA[
    /// <DataTemplate x:Key="HeaderArrowUp">
    ///     <Grid>
    ///         <TextBlock Text="{Binding}" VerticalAlignment="Center" HorizontalAlignment="Center" />
    ///         <Path StrokeThickness="0" SnapsToDevicePixels="True" Data="M 0,4 L 4,0 L 8,4 L 0,4"
    ///               Margin="0,-2,0,0" VerticalAlignment="Top" HorizontalAlignment="Center">
    ///             <Path.Fill>
    ///                 <LinearGradientBrush StartPoint="0,0" EndPoint="1,1">
    ///                     <GradientStop Color="#FF3C5E72" Offset="0"/>
    ///                     <GradientStop Color="#FFC3E4F5" Offset="1"/>
    ///                 </LinearGradientBrush>
    ///             </Path.Fill>
    ///         </Path>
    ///     </Grid>
    /// </DataTemplate>
    /// <DataTemplate x:Key="HeaderArrowDown">
    ///     <Grid>
    ///         <TextBlock Text="{Binding}" VerticalAlignment="Center" HorizontalAlignment="Center" />
    ///         <Path StrokeThickness="0" SnapsToDevicePixels="True" Data="M 0,0 L 4,4 L 8,0 L 0,0"
    ///               Margin="0,-2,0,0" VerticalAlignment="Top" HorizontalAlignment="Center">
    ///             <Path.Fill>
    ///                 <LinearGradientBrush StartPoint="0,0" EndPoint="1,1">
    ///                     <GradientStop Color="#FF3C5E72" Offset="0"/>
    ///                     <GradientStop Color="#FFC4E3F4" Offset="1"/>
    ///                 </LinearGradientBrush>
    ///             </Path.Fill>
    ///         </Path>
    ///     </Grid>
    /// </DataTemplate>
    /// <DataTemplate x:Key="HeaderTransparent">
    ///     <Grid>
    ///         <TextBlock Text="{Binding}" VerticalAlignment="Center" HorizontalAlignment="Center" />
    ///     </Grid>
    /// </DataTemplate>
    /// 
    /// <ListView controls:ColumnSortBehavior.AllowColumnSortings="True"
    ///           controls:ColumnSortBehavior.AscendingSortHeaderTemplate="{StaticResource HeaderArrowUp}"
    ///           controls:ColumnSortBehavior.DescendingSortHeaderTemplate="{StaticResource HeaderArrowDown}"
    ///           controls:ColumnSortBehavior.NeutralHeaderTemplate="{StaticResource HeaderTransparent}">
    ///     <ListView.View>
    ///         <GridView>
    ///             <GridViewColumn Header="Name" controls:ColumnSortBehavior.IsDefaultSortColumn="True" controls:ColumnSortBehavior.SortPropertyName="Name" />
    ///             <GridViewColumn Header="Size" controls:ColumnSortBehavior.SortPropertyName="Size" />
    ///             <GridViewColumn Header="Date" controls:ColumnSortBehavior.SortPropertyName="Date" />
    ///         </GridView>
    ///     </ListView.View>
    /// </ListView>
    /// ]]>
    /// </code>
    /// </example>
    public class ColumnSortBehavior
    {
        /// <summary>
        ///     Identifies the <see cref="GetNeutralHeaderTemplate(DependencyObject)" />
        ///     <see cref="SetNeutralHeaderTemplate(DependencyObject, DataTemplate)" /> attached property.
        /// </summary>
        public static readonly DependencyProperty NeutralHeaderTemplateProperty =
            DependencyProperty.RegisterAttached("NeutralHeaderTemplate", typeof(DataTemplate), typeof(ColumnSortBehavior), new UIPropertyMetadata(null));

        /// <summary>
        ///     Identifies the <see cref="GetAscendingSortHeaderTemplate(DependencyObject)" />
        ///     <see cref="SetAscendingSortHeaderTemplate(DependencyObject, DataTemplate)" /> attached property.
        /// </summary>
        public static readonly DependencyProperty AscendingSortHeaderTemplateProperty =
            DependencyProperty.RegisterAttached("AscendingSortHeaderTemplate", typeof(DataTemplate), typeof(ColumnSortBehavior), new UIPropertyMetadata(null));

        /// <summary>
        ///     Identifies the <see cref="GetDescendingSortHeaderTemplate(DependencyObject)" />
        ///     <see cref="SetDescendingSortHeaderTemplate(DependencyObject, DataTemplate)" /> attached property.
        /// </summary>
        public static readonly DependencyProperty DescendingSortHeaderTemplateProperty =
            DependencyProperty.RegisterAttached("DescendingSortHeaderTemplate", typeof(DataTemplate), typeof(ColumnSortBehavior), new UIPropertyMetadata(null));

        /// <summary>
        ///     Identifies the <see cref="GetAllowColumnSortings(DependencyObject)" />
        ///     <see cref="SetAllowColumnSortings(DependencyObject, bool)" /> attached property.
        /// </summary>
        public static readonly DependencyProperty AllowColumnSortingsProperty =
            DependencyProperty.RegisterAttached("AllowColumnSortings", typeof(bool), typeof(ColumnSortBehavior), new UIPropertyMetadata(OnAllowColumnSortingsChanged));

        /// <summary>
        ///     Identifies the <see cref="GetIsDefaultSortColumn(DependencyObject)" />
        ///     <see cref="SetIsDefaultSortColumn(DependencyObject, bool)" /> attached property.
        /// </summary>
        public static readonly DependencyProperty IsDefaultSortColumnProperty =
            DependencyProperty.RegisterAttached("IsDefaultSortColumn", typeof(bool), typeof(ColumnSortBehavior));

        /// <summary>
        ///     Identifies the <see cref="GetSortPropertyName(DependencyObject)" />
        ///     <see cref="SetSortPropertyName(DependencyObject, string)" /> attached property.
        /// </summary>
        public static readonly DependencyProperty SortPropertyNameProperty =
            DependencyProperty.RegisterAttached("SortPropertyName", typeof(string), typeof(ColumnSortBehavior), new UIPropertyMetadata(null));

        private static readonly DependencyProperty ColumnSortingBehaviorProperty =
            DependencyProperty.RegisterAttached("ColumnSortingBehavior", typeof(ColumnSortBehavior), typeof(ColumnSortBehavior), new UIPropertyMetadata(null));

        private GridViewColumnCollection _columns;
        private bool _isSorting;
        private ListSortDirection _lastDirection;
        private GridViewColumn _lastSortedColumn;
        private ItemsControl _owner;

        private ColumnSortBehavior()
        {
        }

        /// <summary>
        ///     Gets the header template to be used for sorting if the column is not used for sort actually.
        /// </summary>
        /// <param name="obj">The element from which the property value is read.</param>
        /// <returns>The DW.WPFToolkit.Interactivity.ColumnSortBehavior.NeutralHeaderTemplate property value for the element.</returns>
        public static DataTemplate GetNeutralHeaderTemplate(DependencyObject obj)
        {
            return (DataTemplate) obj.GetValue(NeutralHeaderTemplateProperty);
        }

        /// <summary>
        ///     Attaches the header template to be used for sorting if the column is not used for sort actually.
        /// </summary>
        /// <param name="obj">The element to which the attached property is written.</param>
        /// <param name="value">The needed DW.WPFToolkit.Interactivity.ColumnSortBehavior.NeutralHeaderTemplate value.</param>
        public static void SetNeutralHeaderTemplate(DependencyObject obj, DataTemplate value)
        {
            obj.SetValue(NeutralHeaderTemplateProperty, value);
        }

        /// <summary>
        ///     Gets the header template to be used for sorting if the column is used for sort ascending actually.
        /// </summary>
        /// <param name="obj">The element from which the property value is read.</param>
        /// <returns>The DW.WPFToolkit.Interactivity.ColumnSortBehavior.AscendingSortHeaderTemplate property value for the element.</returns>
        public static DataTemplate GetAscendingSortHeaderTemplate(DependencyObject obj)
        {
            return (DataTemplate) obj.GetValue(AscendingSortHeaderTemplateProperty);
        }

        /// <summary>
        ///     Attaches the header template to be used for sorting if the column is used for sort ascending actually.
        /// </summary>
        /// <param name="obj">The element to which the attached property is written.</param>
        /// <param name="value">The needed DW.WPFToolkit.Interactivity.ColumnSortBehavior.AscendingSortHeaderTemplate value.</param>
        public static void SetAscendingSortHeaderTemplate(DependencyObject obj, DataTemplate value)
        {
            obj.SetValue(AscendingSortHeaderTemplateProperty, value);
        }

        /// <summary>
        ///     Gets the header template to be used for sorting if the column is used for sort descending actually.
        /// </summary>
        /// <param name="obj">The element from which the property value is read.</param>
        /// <returns>
        ///     The DW.WPFToolkit.Interactivity.ColumnSortBehavior.DescendingSortHeaderTemplate property value for the
        ///     element.
        /// </returns>
        public static DataTemplate GetDescendingSortHeaderTemplate(DependencyObject obj)
        {
            return (DataTemplate) obj.GetValue(DescendingSortHeaderTemplateProperty);
        }

        /// <summary>
        ///     Attaches the header template to be used for sorting if the column is used for sort descending actually.
        /// </summary>
        /// <param name="obj">The element to which the attached property is written.</param>
        /// <param name="value">The needed DW.WPFToolkit.Interactivity.ColumnSortBehavior.DescendingSortHeaderTemplate value.</param>
        public static void SetDescendingSortHeaderTemplate(DependencyObject obj, DataTemplate value)
        {
            obj.SetValue(DescendingSortHeaderTemplateProperty, value);
        }

        /// <summary>
        ///     Gets the value that indicates if sorting is allowed or not.
        /// </summary>
        /// <param name="obj">The element from which the property value is read.</param>
        /// <returns>The DW.WPFToolkit.Interactivity.ColumnSortBehavior.AllowColumnSortings property value for the element.</returns>
        public static bool GetAllowColumnSortings(DependencyObject obj)
        {
            return (bool) obj.GetValue(AllowColumnSortingsProperty);
        }

        /// <summary>
        ///     Attaches the value if sorting is allowed or not.
        /// </summary>
        /// <param name="obj">The element to which the attached property is written.</param>
        /// <param name="value">The needed DW.WPFToolkit.Interactivity.ColumnSortBehavior.AllowColumnSortings value.</param>
        public static void SetAllowColumnSortings(DependencyObject obj, bool value)
        {
            obj.SetValue(AllowColumnSortingsProperty, value);
        }

        private static void OnAllowColumnSortingsChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            var element = (FrameworkElement) sender;
            element.Loaded += Eement_Loaded;
        }

        /// <summary>
        ///     Gets a value that indicates if a column is defined as default sort column or not.
        /// </summary>
        /// <param name="obj">The element from which the property value is read.</param>
        /// <returns>The DW.WPFToolkit.Interactivity.ColumnSortBehavior.IsDefaultSortColumn property value for the element.</returns>
        public static bool GetIsDefaultSortColumn(DependencyObject obj)
        {
            return (bool) obj.GetValue(IsDefaultSortColumnProperty);
        }

        /// <summary>
        ///     Attaches a value that indicates if a column is defined as default sort column or not.
        /// </summary>
        /// <param name="obj">The element to which the attached property is written.</param>
        /// <param name="value">The needed DW.WPFToolkit.Interactivity.ColumnSortBehavior.IsDefaultSortColumn value.</param>
        public static void SetIsDefaultSortColumn(DependencyObject obj, bool value)
        {
            obj.SetValue(IsDefaultSortColumnProperty, value);
        }

        /// <summary>
        ///     Gets the property name to be used for sorting.
        /// </summary>
        /// <param name="obj">The element from which the property value is read.</param>
        /// <returns>The DW.WPFToolkit.Interactivity.ColumnSortBehavior.SortPropertyName property value for the element.</returns>
        public static string GetSortPropertyName(DependencyObject obj)
        {
            return (string) obj.GetValue(SortPropertyNameProperty);
        }

        /// <summary>
        ///     Attaches the property name to be used for sorting.
        /// </summary>
        /// <param name="obj">The element to which the attached property is written.</param>
        /// <param name="value">The needed DW.WPFToolkit.Interactivity.ColumnSortBehavior.SortPropertyName value.</param>
        public static void SetSortPropertyName(DependencyObject obj, string value)
        {
            obj.SetValue(SortPropertyNameProperty, value);
        }

        private static ColumnSortBehavior GetColumnSortingBehavior(DependencyObject obj)
        {
            return (ColumnSortBehavior) obj.GetValue(ColumnSortingBehaviorProperty);
        }

        private static void SetColumnSortingBehavior(DependencyObject obj, ColumnSortBehavior value)
        {
            obj.SetValue(ColumnSortingBehaviorProperty, value);
        }

        private static ColumnSortBehavior GetOrSetBehavior(DependencyObject sender)
        {
            var behavior = GetColumnSortingBehavior(sender);
            if (behavior != null)
                return behavior;

            behavior = new ColumnSortBehavior();
            SetColumnSortingBehavior(sender, behavior);
            return behavior;
        }

        private static void Eement_Loaded(object sender, RoutedEventArgs e)
        {
            var element = (FrameworkElement) sender;
            if (!GetAllowColumnSortings(element))
                return;

            if (GetColumnSortingBehavior(element) != null)
                return;

            var behavior = GetOrSetBehavior(element);
            element.AddHandler(ButtonBase.ClickEvent, new RoutedEventHandler(behavior.GridViewColumnHeaderClickedHandler));
            behavior._owner = (ItemsControl) element;
            behavior.GetColumns(element);
            behavior.RunDefaultSort();
        }

        private void GetColumns(DependencyObject sender)
        {
            var presenter = VisualTreeAssist.FindChild<GridViewHeaderRowPresenter>(_owner);
            if (presenter != null)
                _columns = presenter.Columns;

            if (_columns == null)
                return;

            foreach (var column in _columns)
                column.HeaderTemplate = GetNeutralHeaderTemplate(sender);
        }

        private void RunDefaultSort()
        {
            if (_columns == null)
                return;

            var sortableGridViewColumn = _columns.FirstOrDefault(GetIsDefaultSortColumn);
            if (sortableGridViewColumn == null)
                return;

            _lastSortedColumn = sortableGridViewColumn;
            Sort(GetSortPropertyName(sortableGridViewColumn), ListSortDirection.Ascending);
            sortableGridViewColumn.HeaderTemplate = GetAscendingSortHeaderTemplate(_owner);
        }

        private void GridViewColumnHeaderClickedHandler(object sender, RoutedEventArgs e)
        {
            if (e.OriginalSource is GridViewColumnHeader headerClicked && headerClicked.Role != GridViewColumnHeaderRole.Padding)
            {
                var clickedColumn = headerClicked.Column;
                var toSortPropertyName = GetSortPropertyName(clickedColumn);
                if (clickedColumn != null && !string.IsNullOrEmpty(toSortPropertyName))
                {
                    var direction = ListSortDirection.Ascending;
                    var isNewSortColumn = IsNewSortColumn(toSortPropertyName);
                    if (!isNewSortColumn)
                        direction = _lastDirection == ListSortDirection.Ascending ? ListSortDirection.Descending : ListSortDirection.Ascending;

                    Sort(toSortPropertyName, direction);

                    clickedColumn.HeaderTemplate = direction == ListSortDirection.Ascending ? GetAscendingSortHeaderTemplate(_owner) : GetDescendingSortHeaderTemplate(_owner);

                    ResetLastTemplate(isNewSortColumn);
                    _lastSortedColumn = clickedColumn;
                }
            }
        }

        private bool IsNewSortColumn(string toSortPropertyName)
        {
            if (_lastSortedColumn == null)
                return true;

            var lastSortPropertyName = GetSortPropertyName(_lastSortedColumn);
            return string.IsNullOrEmpty(toSortPropertyName) || !string.Equals(lastSortPropertyName, toSortPropertyName, StringComparison.InvariantCultureIgnoreCase);
        }

        private void ResetLastTemplate(bool isNewSortColumn)
        {
            if (isNewSortColumn && _lastSortedColumn != null)
                _lastSortedColumn.HeaderTemplate = GetNeutralHeaderTemplate(_owner);
        }

        private void Resort()
        {
            if (!_isSorting)
            {
                _isSorting = true;
                if (_lastSortedColumn != null && !string.IsNullOrEmpty(GetSortPropertyName(_lastSortedColumn)))
                    Sort(GetSortPropertyName(_lastSortedColumn), _lastDirection);
                _isSorting = false;
            }
        }

        private void Sort(string sortBy, ListSortDirection direction)
        {
            _lastDirection = direction;
            var dataView = CollectionViewSource.GetDefaultView(_owner.Items);
            if (dataView != null)
            {
                dataView.SortDescriptions.Clear();
                dataView.SortDescriptions.Add(new SortDescription(sortBy, direction));
                dataView.Refresh();
            }
        }
    }
}