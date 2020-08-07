using System.Collections;
using Avalonia.Controls.Generators;
using Avalonia.Controls.Primitives;
using Avalonia.Input;
using Avalonia.Layout;
using Avalonia.VisualTree;

#nullable enable

namespace Avalonia.Controls
{
    /// <summary>
    /// An <see cref="ItemsControl"/> in which individual items can be selected.
    /// </summary>
    public class ListBox : SelectingItemsControl
    {
        /// <summary>
        /// Defines the <see cref="Scroll"/> property.
        /// </summary>
        public static readonly DirectProperty<ListBox, IScrollable?> ScrollProperty =
            AvaloniaProperty.RegisterDirect<ListBox, IScrollable?>(nameof(Scroll), o => o.Scroll);

        /// <summary>
        /// Defines the <see cref="SelectedItems"/> property.
        /// </summary>
        public static readonly new DirectProperty<SelectingItemsControl, IList> SelectedItemsProperty =
            SelectingItemsControl.SelectedItemsProperty;

        /// <summary>
        /// Defines the <see cref="Selection"/> property.
        /// </summary>
        public static readonly new DirectProperty<SelectingItemsControl, ISelectionModel> SelectionProperty =
            SelectingItemsControl.SelectionProperty;

        /// <summary>
        /// Defines the <see cref="SelectionMode"/> property.
        /// </summary>
        public static readonly new StyledProperty<SelectionMode> SelectionModeProperty = 
            SelectingItemsControl.SelectionModeProperty;

        private IScrollable? _scroll;

        static ListBox()
        {
            LayoutProperty.OverrideDefaultValue<ListBox>(new StackLayout());
        }

        /// <summary>
        /// Gets the scroll information for the <see cref="ListBox"/>.
        /// </summary>
        public IScrollable? Scroll
        {
            get { return _scroll; }
            private set { SetAndRaise(ScrollProperty, ref _scroll, value); }
        }

        /// <inheritdoc/>
        public new IList SelectedItems
        {
            get => base.SelectedItems;
            set => base.SelectedItems = value;
        }

        /// <summary>
        /// Gets or sets a model holding the current selection.
        /// </summary>
        public new ISelectionModel Selection
        {
            get => base.Selection;
            set => base.Selection = value;
        }

        /// <summary>
        /// Gets or sets the selection mode.
        /// </summary>
        /// <remarks>
        /// Note that the selection mode only applies to selections made via user interaction.
        /// Multiple selections can be made programatically regardless of the value of this property.
        /// </remarks>
        public new SelectionMode SelectionMode
        {
            get { return base.SelectionMode; }
            set { base.SelectionMode = value; }
        }

        /// <summary>
        /// Selects all items in the <see cref="ListBox"/>.
        /// </summary>
        public void SelectAll() => Selection.SelectAll();

        /// <summary>
        /// Deselects all items in the <see cref="ListBox"/>.
        /// </summary>
        public void UnselectAll() => Selection.ClearSelection();

        /// <inheritdoc/>
        protected override IItemContainerGenerator CreateItemContainerGenerator()
        {
            return new ItemContainerGenerator<ListBoxItem>(
                this,
                ListBoxItem.ContentProperty,
                ListBoxItem.ContentTemplateProperty);
        }

        /// <inheritdoc/>
        protected override void OnGotFocus(GotFocusEventArgs e)
        {
            base.OnGotFocus(e);

            if (e.NavigationMethod == NavigationMethod.Directional)
            {
                e.Handled = UpdateSelectionFromEventSource(
                    e.Source,
                    true,
                    (e.KeyModifiers & KeyModifiers.Shift) != 0,
                    (e.KeyModifiers & KeyModifiers.Control) != 0);
            }
        }

        /// <inheritdoc/>
        protected override void OnPointerPressed(PointerPressedEventArgs e)
        {
            base.OnPointerPressed(e);

            if (e.Source is IVisual source)
            {
                var point = e.GetCurrentPoint(source);

                if (point.Properties.IsLeftButtonPressed || point.Properties.IsRightButtonPressed)
                {
                    e.Handled = UpdateSelectionFromEventSource(
                        e.Source,
                        true,
                        (e.KeyModifiers & KeyModifiers.Shift) != 0,
                        (e.KeyModifiers & KeyModifiers.Control) != 0,
                        point.Properties.IsRightButtonPressed);
                }
            }
        }

        protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
        {
            Scroll = e.NameScope.Find<IScrollable>("PART_ScrollViewer");
        }
    }
}
