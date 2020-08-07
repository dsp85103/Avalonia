using System.Linq;
using Avalonia.Controls.Presenters;
using Avalonia.Controls.Templates;
using Avalonia.Input;
using Avalonia.Styling;
using Avalonia.UnitTests;
using Avalonia.VisualTree;
using Xunit;

namespace Avalonia.Controls.UnitTests
{
    public class ListBoxTests_Multiple
    {
        [Fact]
        public void Focusing_Item_With_Shift_And_Arrow_Key_Should_Add_To_Selection()
        {
            var target = new ListBox
            {
                Template = new FuncControlTemplate(CreateListBoxTemplate),
                Items = new[] { "Foo", "Bar", "Baz " },
                SelectionMode = SelectionMode.Multiple
            };

            ApplyTemplate(target);

            target.SelectedItem = "Foo";

            target.Presenter.RealizedElements.ElementAt(1).RaiseEvent(new GotFocusEventArgs
            {
                RoutedEvent = InputElement.GotFocusEvent,
                NavigationMethod = NavigationMethod.Directional,
                KeyModifiers = KeyModifiers.Shift
            });

            Assert.Equal(new[] { "Foo", "Bar" }, target.SelectedItems);
        }

        [Fact]
        public void Focusing_Item_With_Ctrl_And_Arrow_Key_Should_Add_To_Selection()
        {
            var target = new ListBox
            {
                Template = new FuncControlTemplate(CreateListBoxTemplate),
                Items = new[] { "Foo", "Bar", "Baz " },
                SelectionMode = SelectionMode.Multiple
            };

            ApplyTemplate(target);

            target.SelectedItem = "Foo";

            target.Presenter.RealizedElements.ElementAt(1).RaiseEvent(new GotFocusEventArgs
            {
                RoutedEvent = InputElement.GotFocusEvent,
                NavigationMethod = NavigationMethod.Directional,
                KeyModifiers = KeyModifiers.Control
            });

            Assert.Equal(new[] { "Foo", "Bar" }, target.SelectedItems);
        }

        [Fact]
        public void Focusing_Selected_Item_With_Ctrl_And_Arrow_Key_Should_Remove_From_Selection()
        {
            var target = new ListBox
            {
                Template = new FuncControlTemplate(CreateListBoxTemplate),
                Items = new[] { "Foo", "Bar", "Baz " },
                SelectionMode = SelectionMode.Multiple
            };

            ApplyTemplate(target);

            target.SelectedItems.Add("Foo");
            target.SelectedItems.Add("Bar");

            target.Presenter.RealizedElements.ElementAt(1).RaiseEvent(new GotFocusEventArgs
            {
                RoutedEvent = InputElement.GotFocusEvent,
                NavigationMethod = NavigationMethod.Directional,
                KeyModifiers = KeyModifiers.Control
            });

            Assert.Equal(new[] { "Bar" }, target.SelectedItems);
        }

        private Control CreateListBoxTemplate(ITemplatedControl parent, INameScope scope)
        {
            return new ScrollViewer
            {
                Template = new FuncControlTemplate(CreateScrollViewerTemplate),
                Content = new ItemsPresenter
                {
                    Name = "PART_ItemsPresenter",
                    [~ItemsPresenter.ItemsProperty] = parent.GetObservable(ItemsControl.ItemsProperty).ToBinding(),
                }.RegisterInNameScope(scope)
            };
        }

        private Control CreateScrollViewerTemplate(ITemplatedControl parent, INameScope scope)
        {
            return new ScrollContentPresenter
            {
                Name = "PART_ContentPresenter",
                [~ContentPresenter.ContentProperty] =
                    parent.GetObservable(ContentControl.ContentProperty).ToBinding(),
            }.RegisterInNameScope(scope);
        }

        private void ApplyTemplate(ListBox target)
        {
            // Apply the template to the ListBox itself.
            target.ApplyTemplate();

            // Then to its inner ScrollViewer.
            var scrollViewer = (ScrollViewer)target.GetVisualChildren().Single();
            scrollViewer.ApplyTemplate();

            // Then make the ScrollViewer create its child.
            ((ContentPresenter)scrollViewer.Presenter).UpdateChild();

            // Now the ItemsPresenter should be reigstered, so apply its template.
            target.Presenter.ApplyTemplate();
        }
    }
}
