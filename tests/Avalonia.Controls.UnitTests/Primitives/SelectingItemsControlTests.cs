using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using Avalonia.Collections;
using Avalonia.Controls.Presenters;
using Avalonia.Controls.Primitives;
using Avalonia.Controls.Templates;
using Avalonia.Data;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Layout;
using Avalonia.Markup.Data;
using Avalonia.Styling;
using Avalonia.UnitTests;
using Avalonia.VisualTree;
using Moq;
using Xunit;

namespace Avalonia.Controls.UnitTests.Primitives
{
    public partial class SelectingItemsControlTests
    {
        private MouseTestHelper _helper = new MouseTestHelper();

        [Fact]
        public void SelectedIndex_Should_Initially_Be_Minus_1()
        {
            using var app = Start();

            var items = new[]
            {
                new Item(),
                new Item(),
            };

            var target = new SelectingItemsControl
            {
                Items = items,
            };

            Prepare(target);

            Assert.Equal(-1, target.SelectedIndex);
        }

        [Fact]
        public void Item_IsSelected_Should_Initially_Be_False()
        {
            using var app = Start();

            var items = new[]
            {
                new Item(),
                new Item(),
            };

            var target = new SelectingItemsControl
            {
                Items = items,
            };

            Prepare(target);

            Assert.False(items[0].IsSelected);
            Assert.False(items[1].IsSelected);
        }

        [Fact]
        public void Setting_SelectedItem_Should_Set_Item_IsSelected_True()
        {
            using var app = Start();

            var items = new[]
            {
                new Item(),
                new Item(),
            };

            var target = new SelectingItemsControl
            {
                Items = items,
            };

            Prepare(target);
            target.SelectedItem = items[1];

            Assert.False(items[0].IsSelected);
            Assert.True(items[1].IsSelected);
        }

        [Fact]
        public void Setting_SelectedItem_Before_ApplyTemplate_Should_Set_Item_IsSelected_True()
        {
            using var app = Start();

            var items = new[]
            {
                new Item(),
                new Item(),
            };

            var target = new SelectingItemsControl
            {
                Items = items,
            };

            target.SelectedItem = items[1];
            Prepare(target);

            Assert.False(items[0].IsSelected);
            Assert.True(items[1].IsSelected);
        }

        [Fact]
        public void SelectedIndex_Should_Be_Minus_1_After_Initialize()
        {
            var items = new[]
            {
                new Item(),
                new Item(),
            };

            var target = new ListBox();
            target.BeginInit();
            target.Items = items;
            target.Template = Template();
            target.EndInit();

            Assert.Equal(-1, target.SelectedIndex);
        }

        [Fact]
        public void SelectedIndex_Should_Be_Minus_1_Without_Initialize()
        {
            var items = new[]
            {
                new Item(),
                new Item(),
            };

            var target = new ListBox();
            target.Items = items;
            target.Template = Template();
            target.DataContext = new object();

            Assert.Equal(-1, target.SelectedIndex);
        }

        [Fact]
        public void SelectedIndex_Should_Be_0_After_Initialize_With_AlwaysSelected()
        {
            var items = new[]
            {
                new Item(),
                new Item(),
            };

            var target = new ListBox();
            target.BeginInit();
            target.SelectionMode = SelectionMode.Single | SelectionMode.AlwaysSelected;
            target.Items = items;
            target.Template = Template();
            target.EndInit();

            Assert.Equal(0, target.SelectedIndex);
        }

        [Fact]
        public void Setting_SelectedIndex_During_Initialize_Should_Select_Item_When_AlwaysSelected_Is_Used()
        {
            var listBox = new ListBox
            {
                SelectionMode = SelectionMode.Single | SelectionMode.AlwaysSelected
            };

            listBox.BeginInit();

            listBox.SelectedIndex = 1;
            var items = new AvaloniaList<string>();
            listBox.Items = items;
            items.Add("A");
            items.Add("B");
            items.Add("C");

            listBox.EndInit();

            Assert.Equal("B", listBox.SelectedItem);
        }

        [Fact]
        public void Setting_SelectedIndex_Before_Initialize_Should_Retain()
        {
            var listBox = new ListBox
            {
                SelectionMode = SelectionMode.Single,
                Items = new[] { "foo", "bar", "baz" },
                SelectedIndex = 1
            };

            listBox.BeginInit();

            listBox.EndInit();

            Assert.Equal(1, listBox.SelectedIndex);
            Assert.Equal("bar", listBox.SelectedItem);
        }

        [Fact]
        public void Setting_SelectedIndex_During_Initialize_Should_Take_Priority_Over_Previous_Value()
        {
            var listBox = new ListBox
            {
                SelectionMode = SelectionMode.Single,
                Items = new[] { "foo", "bar", "baz" },
                SelectedIndex = 2
            };

            listBox.BeginInit();

            listBox.SelectedIndex = 1;

            listBox.EndInit();

            Assert.Equal(1, listBox.SelectedIndex);
            Assert.Equal("bar", listBox.SelectedItem);
        }

        [Fact]
        public void Setting_SelectedItem_Before_Initialize_Should_Retain()
        {
            var listBox = new ListBox
            {
                SelectionMode = SelectionMode.Single,
                Items = new[] { "foo", "bar", "baz" },
                SelectedItem = "bar"
            };

            listBox.BeginInit();

            listBox.EndInit();

            Assert.Equal(1, listBox.SelectedIndex);
            Assert.Equal("bar", listBox.SelectedItem);
        }


        [Fact]
        public void Setting_SelectedItems_Before_Initialize_Should_Retain_Setting()
        {
            var listBox = new ListBox
            {
                SelectionMode = SelectionMode.Multiple,
                Items = new[] { "foo", "bar", "baz" },
            };

            var selected = new[] { "foo", "bar" };

            foreach (var v in selected)
            {
                listBox.SelectedItems.Add(v);
            }

            listBox.BeginInit();

            listBox.EndInit();

            Assert.Equal(selected, listBox.SelectedItems);
        }

        [Fact]
        public void Setting_SelectedItems_During_Initialize_Should_Take_Priority_Over_Previous_Value()
        {
            var listBox = new ListBox
            {
                SelectionMode = SelectionMode.Multiple,
                Items = new[] { "foo", "bar", "baz" },
            };

            var selected = new[] { "foo", "bar" };

            foreach (var v in new[] { "bar", "baz" })
            {
                listBox.SelectedItems.Add(v);
            }

            listBox.BeginInit();

            listBox.SelectedItems = new AvaloniaList<object>(selected);

            listBox.EndInit();

            Assert.Equal(selected, listBox.SelectedItems);
        }

        [Fact]
        public void Setting_SelectedIndex_Before_Initialize_With_AlwaysSelected_Should_Retain()
        {
            var listBox = new ListBox
            {
                SelectionMode = SelectionMode.Single | SelectionMode.AlwaysSelected,
                Items = new[] { "foo", "bar", "baz" },
                SelectedIndex = 1
            };

            listBox.BeginInit();

            listBox.EndInit();

            Assert.Equal(1, listBox.SelectedIndex);
            Assert.Equal("bar", listBox.SelectedItem);
        }

        [Fact]
        public void Setting_SelectedIndex_Before_ApplyTemplate_Should_Set_Item_IsSelected_True()
        {
            using var app = Start();

            var items = new[]
            {
                new Item(),
                new Item(),
            };

            var target = new SelectingItemsControl
            {
                Items = items,
            };

            target.SelectedIndex = 1;
            Prepare(target);

            Assert.False(items[0].IsSelected);
            Assert.True(items[1].IsSelected);
        }

        [Fact]
        public void Setting_SelectedItem_Should_Set_SelectedIndex()
        {
            using var app = Start();

            var items = new[]
            {
                new Item(),
                new Item(),
            };

            var target = new SelectingItemsControl
            {
                Items = items,
            };

            Prepare(target);
            target.SelectedItem = items[1];

            Assert.Equal(items[1], target.SelectedItem);
            Assert.Equal(1, target.SelectedIndex);
        }

        [Fact]
        public void SelectedIndex_Item_Is_Updated_As_Items_Removed_When_Last_Item_Is_Selected()
        {
            using var app = Start();

            var items = new ObservableCollection<string>
            {
               "Foo",
               "Bar",
               "FooBar"
            };

            var target = new SelectingItemsControl
            {
                Items = items,
            };

            Prepare(target);
            target.SelectedItem = items[2];

            Assert.Equal(items[2], target.SelectedItem);
            Assert.Equal(2, target.SelectedIndex);

            items.RemoveAt(0);

            Assert.Equal(items[1], target.SelectedItem);
            Assert.Equal(1, target.SelectedIndex);
        }

        [Fact]
        public void Setting_SelectedItem_To_Not_Present_Item_Should_Clear_Selection()
        {
            using var app = Start();

            var items = new[]
            {
                new Item(),
                new Item(),
            };

            var target = new SelectingItemsControl
            {
                Items = items,
            };

            Prepare(target);
            target.SelectedItem = items[1];

            Assert.Equal(items[1], target.SelectedItem);
            Assert.Equal(1, target.SelectedIndex);

            target.SelectedItem = new Item();

            Assert.Null(target.SelectedItem);
            Assert.Equal(-1, target.SelectedIndex);
        }

        [Fact]
        public void Setting_SelectedIndex_Should_Set_SelectedItem()
        {
            using var app = Start();

            var items = new[]
            {
                new Item(),
                new Item(),
            };

            var target = new SelectingItemsControl
            {
                Items = items,
            };

            Prepare(target);
            target.SelectedIndex = 1;

            Assert.Equal(items[1], target.SelectedItem);
        }

        [Fact]
        public void Setting_SelectedIndex_Out_Of_Bounds_Should_Clear_Selection()
        {
            using var app = Start();

            var items = new[]
            {
                new Item(),
                new Item(),
            };

            var target = new SelectingItemsControl
            {
                Items = items,
            };

            Prepare(target);
            target.SelectedIndex = 2;

            Assert.Equal(-1, target.SelectedIndex);
        }

        [Fact]
        public void Setting_SelectedItem_To_Non_Existent_Item_Should_Clear_Selection()
        {
            using var app = Start();

            var target = new SelectingItemsControl();

            Prepare(target);
            target.SelectedItem = new Item();

            Assert.Equal(-1, target.SelectedIndex);
            Assert.Null(target.SelectedItem);
        }

        [Fact]
        public void Adding_Selected_Item_Should_Update_Selection()
        {
            using var app = Start();

            var items = new AvaloniaList<Item>(new[]
            {
                new Item(),
                new Item(),
            });

            var target = new SelectingItemsControl
            {
                Items = items,
                Template = Template(),
            };

            Prepare(target);
            items.Add(new Item { IsSelected = true });

            Assert.Equal(2, target.SelectedIndex);
            Assert.Equal(items[2], target.SelectedItem);
        }

        [Fact]
        public void Setting_Items_To_Null_Should_Clear_Selection()
        {
            var items = new AvaloniaList<Item>
            {
                new Item(),
                new Item(),
            };

            var target = new SelectingItemsControl
            {
                Items = items,
            };

            target.ApplyTemplate();
            target.SelectedIndex = 1;

            Assert.Equal(items[1], target.SelectedItem);
            Assert.Equal(1, target.SelectedIndex);

            target.Items = null;

            Assert.Null(target.SelectedItem);
            Assert.Equal(-1, target.SelectedIndex);
        }

        [Fact]
        public void Removing_Selected_Item_Should_Clear_Selection()
        {
            var items = new AvaloniaList<Item>
            {
                new Item(),
                new Item(),
            };

            var target = new SelectingItemsControl
            {
                Items = items,
            };

            target.ApplyTemplate();
            target.Presenter.ApplyTemplate();
            target.SelectedIndex = 1;

            Assert.Equal(items[1], target.SelectedItem);
            Assert.Equal(1, target.SelectedIndex);

            SelectionChangedEventArgs receivedArgs = null;

            target.SelectionChanged += (_, args) => receivedArgs = args;

            var removed = items[1];

            items.RemoveAt(1);

            Assert.Null(target.SelectedItem);
            Assert.Equal(-1, target.SelectedIndex);
            Assert.NotNull(receivedArgs);
            Assert.Empty(receivedArgs.AddedItems);
            Assert.Equal(new[] { removed }, receivedArgs.RemovedItems);
            Assert.False(items.Single().IsSelected);
        }

        [Fact]
        public void Removing_Selected_Item_Should_Clear_Selection_With_BeginInit()
        {
            var items = new AvaloniaList<Item>
            {
                new Item(),
                new Item(),
            };

            var target = new SelectingItemsControl();
            target.BeginInit();
            target.Items = items;
            target.Template = Template();
            target.EndInit();

            target.ApplyTemplate();
            target.Presenter.ApplyTemplate();
            target.SelectedIndex = 0;

            Assert.Equal(items[0], target.SelectedItem);
            Assert.Equal(0, target.SelectedIndex);

            SelectionChangedEventArgs receivedArgs = null;

            target.SelectionChanged += (_, args) => receivedArgs = args;

            var removed = items[0];

            items.RemoveAt(0);

            Assert.Null(target.SelectedItem);
            Assert.Equal(-1, target.SelectedIndex);
            Assert.NotNull(receivedArgs);
            Assert.Empty(receivedArgs.AddedItems);
            Assert.Equal(new[] { removed }, receivedArgs.RemovedItems);
            Assert.False(items.Single().IsSelected);
        }

        [Fact]
        public void Resetting_Items_Collection_Should_Clear_Selection()
        {
            // Need to use ObservableCollection here as AvaloniaList signals a Clear as an
            // add + remove.
            var items = new ObservableCollection<Item>
            {
                new Item(),
                new Item(),
            };

            var target = new SelectingItemsControl
            {
                Items = items,
            };

            target.ApplyTemplate();
            target.SelectedIndex = 1;

            Assert.Equal(items[1], target.SelectedItem);
            Assert.Equal(1, target.SelectedIndex);

            items.Clear();

            Assert.Null(target.SelectedItem);
            Assert.Equal(-1, target.SelectedIndex);
        }

        [Fact]
        public void Raising_IsSelectedChanged_On_Item_Should_Update_Selection()
        {
            using var app = Start();

            var items = new[]
            {
                new Item(),
                new Item(),
            };

            var target = new SelectingItemsControl
            {
                Items = items,
            };

            Prepare(target);
            target.SelectedItem = items[1];

            Assert.False(items[0].IsSelected);
            Assert.True(items[1].IsSelected);

            items[0].IsSelected = true;
            items[0].RaiseEvent(new RoutedEventArgs(SelectingItemsControl.IsSelectedChangedEvent));

            Assert.Equal(0, target.SelectedIndex);
            Assert.Equal(items[0], target.SelectedItem);
            Assert.True(items[0].IsSelected);
            Assert.False(items[1].IsSelected);
        }

        [Fact]
        public void Clearing_IsSelected_And_Raising_IsSelectedChanged_On_Item_Should_Update_Selection()
        {
            using var app = Start();

            var items = new[]
            {
                new Item(),
                new Item(),
            };

            var target = new SelectingItemsControl
            {
                Items = items,
            };

            Prepare(target);
            target.SelectedItem = items[1];

            Assert.False(items[0].IsSelected);
            Assert.True(items[1].IsSelected);

            items[1].IsSelected = false;
            items[1].RaiseEvent(new RoutedEventArgs(SelectingItemsControl.IsSelectedChangedEvent));

            Assert.Equal(-1, target.SelectedIndex);
            Assert.Null(target.SelectedItem);
        }

        [Fact]
        public void Raising_IsSelectedChanged_On_Someone_Elses_Item_Should_Not_Update_Selection()
        {
            using var app = Start();

            var items = new[]
            {
                new Item(),
                new Item(),
            };

            var target = new SelectingItemsControl
            {
                Items = items,
            };

            Prepare(target);
            target.SelectedItem = items[1];

            var notChild = new Item
            {
                IsSelected = true,
            };

            target.RaiseEvent(new RoutedEventArgs
            {
                RoutedEvent = SelectingItemsControl.IsSelectedChangedEvent,
                Source = notChild,
            });

            Assert.Equal(target.SelectedItem, items[1]);
        }

        [Fact]
        public void Setting_SelectedIndex_Should_Raise_SelectionChanged_Event()
        {
            var items = new[]
            {
                new Item(),
                new Item(),
            };

            var target = new SelectingItemsControl
            {
                Items = items,
            };

            var called = false;

            target.SelectionChanged += (s, e) =>
            {
                Assert.Same(items[1], e.AddedItems.Cast<object>().Single());
                Assert.Empty(e.RemovedItems);
                called = true;
            };

            target.SelectedIndex = 1;

            Assert.True(called);
        }

        [Fact]
        public void Clearing_SelectedIndex_Should_Raise_SelectionChanged_Event()
        {
            using var app = Start();

            var items = new[]
            {
                new Item(),
                new Item(),
            };

            var target = new SelectingItemsControl
            {
                Items = items,
                SelectedIndex = 1,
            };

            Prepare(target);

            var called = false;

            target.SelectionChanged += (s, e) =>
            {
                Assert.Same(items[1], e.RemovedItems.Cast<object>().Single());
                Assert.Empty(e.AddedItems);
                called = true;
            };

            target.SelectedIndex = -1;

            Assert.True(called);
        }

        [Fact]
        public void Order_Of_Setting_Items_And_SelectedIndex_During_Initialization_Should_Not_Matter()
        {
            var items = new[] { "Foo", "Bar" };
            var target = new SelectingItemsControl();

            ((ISupportInitialize)target).BeginInit();
            target.SelectedIndex = 1;
            target.Items = items;
            ((ISupportInitialize)target).EndInit();

            Assert.Equal(1, target.SelectedIndex);
            Assert.Equal("Bar", target.SelectedItem);
        }

        [Fact]
        public void Order_Of_Setting_Items_And_SelectedItem_During_Initialization_Should_Not_Matter()
        {
            var items = new[] { "Foo", "Bar" };
            var target = new SelectingItemsControl();

            ((ISupportInitialize)target).BeginInit();
            target.SelectedItem = "Bar";
            target.Items = items;
            ((ISupportInitialize)target).EndInit();

            Assert.Equal(1, target.SelectedIndex);
            Assert.Equal("Bar", target.SelectedItem);
        }

        [Fact]
        public void Changing_DataContext_Should_Not_Clear_Nested_ViewModel_SelectedItem()
        {
            var items = new[]
            {
                new Item(),
                new Item(),
            };

            var vm = new MasterViewModel
            {
                Child = new ChildViewModel
                {
                    Items = items,
                    SelectedItem = items[1],
                }
            };

            var target = new SelectingItemsControl { DataContext = vm };
            var itemsBinding = new Binding("Child.Items");
            var selectedBinding = new Binding("Child.SelectedItem");

            target.Bind(SelectingItemsControl.ItemsProperty, itemsBinding);
            target.Bind(SelectingItemsControl.SelectedItemProperty, selectedBinding);

            Assert.Equal(1, target.SelectedIndex);
            Assert.Same(vm.Child.SelectedItem, target.SelectedItem);

            items = new[]
            {
                new Item { Value = "Item1" },
                new Item { Value = "Item2" },
                new Item { Value = "Item3" },
            };

            vm = new MasterViewModel
            {
                Child = new ChildViewModel
                {
                    Items = items,
                    SelectedItem = items[2],
                }
            };

            target.DataContext = vm;

            Assert.Equal(2, target.SelectedIndex);
            Assert.Same(vm.Child.SelectedItem, target.SelectedItem);
        }

        [Fact]
        public void Nested_ListBox_Does_Not_Change_Parent_SelectedIndex()
        {
            using var app = Start();

            SelectingItemsControl nested;

            var root = new SelectingItemsControl
            {
                Items = new IControl[]
                {
                    new Border(),
                    nested = new ListBox
                    {
                        Template = Template(),
                        Items = new[] { "foo", "bar" },
                        SelectedIndex = 1,
                    }
                },
                SelectedIndex = 0,
            };

            Prepare(root);

            Assert.Equal(0, root.SelectedIndex);
            Assert.Equal(1, nested.SelectedIndex);

            nested.SelectedIndex = 0;

            Assert.Equal(0, root.SelectedIndex);
        }

        [Fact]
        public void Setting_SelectedItem_With_Pointer_Should_Set_TabOnceActiveElement()
        {
            using var app = Start();

            var target = new ListBox
            {
                Items = new[] { "Foo", "Bar", "Baz " },
            };

            Prepare(target);

            var selected = target.Presenter.RealizedElements.ElementAt(1);
            _helper.Down(selected);

            var presenter = (InputElement)target.Presenter;

            Assert.Equal(
                KeyboardNavigation.GetTabOnceActiveElement(presenter),
                selected);
        }

        [Fact]
        public void Removing_SelectedItem_Should_Clear_TabOnceActiveElement()
        {
            var items = new ObservableCollection<string>(new[] { "Foo", "Bar", "Baz " });

            var target = new ListBox
            {
                Template = Template(),
                Items = items,
            };

            Prepare(target);

            var selected = target.Presenter.RealizedElements.ElementAt(1);
            _helper.Down(selected);

            items.RemoveAt(1);

            var presenter = (InputElement)target.Presenter;

            Assert.Null(KeyboardNavigation.GetTabOnceActiveElement(presenter));
        }

        [Fact]
        public void Resetting_Items_Collection_Should_Retain_Selection()
        {
            var itemsMock = new Mock<List<string>>();
            var itemsMockAsINCC = itemsMock.As<INotifyCollectionChanged>();

            itemsMock.Object.AddRange(new[] { "Foo", "Bar", "Baz" });
            var target = new SelectingItemsControl
            {
                Items = itemsMock.Object
            };

            target.SelectedIndex = 1;

            itemsMockAsINCC.Raise(e => e.CollectionChanged += null, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));

            Assert.True(target.SelectedIndex == 1);
        }

        [Fact]
        public void Binding_With_DelayedBinding_And_Initialization_Where_DataContext_Is_Root_Works()
        {
            // Test for #1932.
            var root = new RootWithItems();

            root.BeginInit();
            root.DataContext = root;

            var target = new ListBox();
            target.BeginInit();
            root.Child = target;

            DelayedBinding.Add(target, ItemsControl.ItemsProperty, new Binding(nameof(RootWithItems.Items)));
            DelayedBinding.Add(target, ListBox.SelectedItemProperty, new Binding(nameof(RootWithItems.Selected)));
            target.EndInit();
            root.EndInit();

            Assert.Equal("b", target.SelectedItem);
        }

        [Fact]
        public void Mode_For_SelectedIndex_Is_TwoWay_By_Default()
        {
            var items = new[]
            {
                new Item(),
                new Item(),
                new Item(),
            };

            var vm = new MasterViewModel
            {
                Child = new ChildViewModel
                {
                    Items = items,
                    SelectedIndex = 1,
                }
            };

            var target = new SelectingItemsControl { DataContext = vm };
            var itemsBinding = new Binding("Child.Items");
            var selectedIndBinding = new Binding("Child.SelectedIndex");

            target.Bind(SelectingItemsControl.ItemsProperty, itemsBinding);
            target.Bind(SelectingItemsControl.SelectedIndexProperty, selectedIndBinding);

            Assert.Equal(1, target.SelectedIndex);

            target.SelectedIndex = 2;

            Assert.Equal(2, target.SelectedIndex);
            Assert.Equal(2, vm.Child.SelectedIndex);
        }

        [Fact]
        public void Should_Select_Correct_Item_When_Duplicate_Items_Are_Present()
        {
            using var app = Start();

            var target = new ListBox
            {
                Items = new[] { "Foo", "Bar", "Baz", "Foo", "Bar", "Baz" },
            };

            Prepare(target);

            var selected = (Interactive)target.Presenter.RealizedElements.ElementAt(3);
            _helper.Down(selected);

            Assert.Equal(3, target.SelectedIndex);
        }

        [Fact]
        public void Should_Apply_Selected_Pseudoclass_To_Correct_Item_When_Duplicate_Items_Are_Present()
        {
            using var app = Start();

            var target = new ListBox
            {
                Items = new[] { "Foo", "Bar", "Baz", "Foo", "Bar", "Baz" },
            };

            Prepare(target);

            var selected = (Interactive)target.Presenter.RealizedElements.ElementAt(3);
            _helper.Down(selected);

            Assert.Equal(new[] { ":pressed", ":selected" }, selected.Classes);
        }

        [Fact]
        public void Adding_Item_Before_SelectedItem_Should_Update_SelectedIndex()
        {
            using var app = Start();

            var items = new ObservableCollection<string>
            {
               "Foo",
               "Bar",
               "Baz"
            };

            var target = new ListBox
            {
                Items = items,
                SelectedIndex = 1,
            };

            Prepare(target);

            items.Insert(0, "Qux");

            Assert.Equal(2, target.SelectedIndex);
            Assert.Equal("Bar", target.SelectedItem);
        }

        [Fact]
        public void Removing_Item_Before_SelectedItem_Should_Update_SelectedIndex()
        {
            using var app = Start();

            var items = new ObservableCollection<string>
            {
               "Foo",
               "Bar",
               "Baz"
            };

            var target = new ListBox
            {
                Items = items,
                SelectedIndex = 1,
            };

            Prepare(target);

            items.RemoveAt(0);

            Assert.Equal(0, target.SelectedIndex);
            Assert.Equal("Bar", target.SelectedItem);
        }

        [Fact]
        public void Replacing_Selected_Item_Should_Update_SelectedItem()
        {
            using var app = Start();

            var items = new ObservableCollection<string>
            {
               "Foo",
               "Bar",
               "Baz"
            };

            var target = new ListBox
            {
                Items = items,
                SelectedIndex = 1,
            };

            Prepare(target);

            items[1] = "Qux";

            Assert.Equal(-1, target.SelectedIndex);
            Assert.Null(target.SelectedItem);
        }

        [Fact]
        public void AutoScrollToSelectedItem_Causes_Scroll_To_SelectedItem()
        {
            using var app = Start();

            var items = new ObservableCollection<string>
            {
               "Foo",
               "Bar",
               "Baz"
            };

            var target = new ListBox
            {
                Items = items,
            };

            Prepare(target);

            var raised = false;
            target.AddHandler(Control.RequestBringIntoViewEvent, (s, e) => raised = true);

            target.SelectedIndex = 2;

            Assert.True(raised);
        }

        [Fact]
        public void AutoScrollToSelectedItem_On_Reset_Works()
        {
            using var app = Start();

            // Issue #3148
            var items = new ResettingCollection(100);

            var target = new ListBox
            {
                Items = items,
                ItemTemplate = new FuncDataTemplate<string>((x, _) =>
                    new TextBlock
                    {
                        Text = x,
                        Width = 100,
                        Height = 10
                    }),
                AutoScrollToSelectedItem = true,
                Layout = new NonVirtualizingStackLayout(),
            };

            Prepare(target);

            Assert.Equal(100, target.Presenter.RealizedElements.Count());

            target.SelectedItem = "Item99";

            // #3148 triggered here.
            items.Reset(new[] { "Item99" });
            Layout(target);

            Assert.Equal(0, target.SelectedIndex);
            Assert.Equal(1, target.Presenter.RealizedElements.Count());
        }

        [Fact]
        public void Can_Set_Both_SelectedItem_And_SelectedItems_During_Initialization()
        {
            // Issue #2969.
            var target = new ListBox();
            var selectedItems = new List<object>();

            target.BeginInit();
            target.Template = Template();
            target.Items = new[] { "Foo", "Bar", "Baz" };
            target.SelectedItems = selectedItems;
            target.SelectedItem = "Bar";
            target.EndInit();

            Assert.Equal("Bar", target.SelectedItem);
            Assert.Equal(1, target.SelectedIndex);
            Assert.Same(selectedItems, target.SelectedItems);
            Assert.Equal(new[] { "Bar" }, selectedItems);
        }

        [Fact]
        public void MoveSelection_Wrap_Does_Not_Hang_With_No_Focusable_Controls()
        {
            // Issue #3094.
            var target = new TestSelector
            {
                Template = Template(),
                Items = new[]
                {
                    new ListBoxItem { Focusable = false },
                    new ListBoxItem { Focusable = false },
                },
                SelectedIndex = 0,
            };

            target.Measure(new Size(100, 100));
            target.Arrange(new Rect(0, 0, 100, 100));
            target.MoveSelection(NavigationDirection.Next, true);
        }

        [Fact]
        public void MoveSelection_Does_Select_Disabled_Controls()
        {
            // Issue #3426.
            var target = new TestSelector
            {
                Template = Template(),
                Items = new[]
                {
                    new ListBoxItem(),
                    new ListBoxItem { IsEnabled = false },
                },
                SelectedIndex = 0,
            };

            target.Measure(new Size(100, 100));
            target.Arrange(new Rect(0, 0, 100, 100));
            target.MoveSelection(NavigationDirection.Next, true);

            Assert.Equal(0, target.SelectedIndex);
        }

        [Fact]
        public void Pre_Selecting_Item_Should_Set_Selection_After_It_Was_Added_When_AlwaysSelected()
        {
            var target = new TestSelector(SelectionMode.AlwaysSelected)
            {
                Template = Template()
            };

            var second = new Item { IsSelected = true };

            var items = new AvaloniaList<object>
            {
                new Item(),
                second
            };

            target.Items = items;

            target.ApplyTemplate();

            target.Presenter.ApplyTemplate();

            Assert.Equal(second, target.SelectedItem);

            Assert.Equal(1, target.SelectedIndex);
        }

        private static IDisposable Start()
        {
            var services = TestServices.MockPlatformRenderInterface.With(
                styler: new Styler(),
                windowingPlatform: new MockWindowingPlatform());
            return UnitTestApplication.Start(services);
        }

        private static void Prepare(SelectingItemsControl target)
        {
            var root = new TestRoot
            {
                Child = target,
                Width = 100,
                Height = 100,
                Styles =
                {
                    new Style(x => x.Is<SelectingItemsControl>())
                    {
                        Setters =
                        {
                            new Setter(ListBox.TemplateProperty, Template()),
                        },
                    },
                },
            };

            root.LayoutManager.ExecuteInitialLayoutPass();
        }

        private void Layout(SelectingItemsControl target)
        {
            var root = (TestRoot)target.GetVisualRoot();
            root.LayoutManager.ExecuteLayoutPass();
        }

        private static FuncControlTemplate Template()
        {
            return new FuncControlTemplate<SelectingItemsControl>((parent, scope) =>
                new ScrollViewer
                {
                    Name = "PART_ScrollViewer",
                    Template = ScrollViewerTemplate(),
                    Content = new ItemsPresenter
                    {
                        Name = "PART_ItemsPresenter",
                        HorizontalCacheLength = 0,
                        VerticalCacheLength = 0,
                        [~ItemsPresenter.ItemsProperty] = parent.GetObservable(ItemsControl.ItemsProperty).ToBinding(),
                        [~ItemsPresenter.LayoutProperty] = parent.GetObservable(ItemsControl.LayoutProperty).ToBinding(),
                    }.RegisterInNameScope(scope)
                }.RegisterInNameScope(scope));
        }

        private static FuncControlTemplate ScrollViewerTemplate()
        {
            return new FuncControlTemplate<ScrollViewer>((parent, scope) =>
                new Panel
                {
                    Children =
                    {
                        new ScrollContentPresenter
                        {
                            Name = "PART_ContentPresenter",
                            [~ScrollContentPresenter.CanHorizontallyScrollProperty] = parent.GetObservable(ScrollViewer.CanHorizontallyScrollProperty).ToBinding(),
                            [~ScrollContentPresenter.CanVerticallyScrollProperty] = parent.GetObservable(ScrollViewer.CanVerticallyScrollProperty).ToBinding(),
                            [~ScrollContentPresenter.ContentProperty] = parent.GetObservable(ScrollViewer.ContentProperty).ToBinding(),
                            [~~ScrollContentPresenter.ExtentProperty] = parent[~~ScrollViewer.ExtentProperty],
                            [~~ScrollContentPresenter.OffsetProperty] = parent[~~ScrollViewer.OffsetProperty],
                            [~~ScrollContentPresenter.ViewportProperty] = parent[~~ScrollViewer.ViewportProperty],
                        }.RegisterInNameScope(scope),
                        new ScrollBar
                        {
                            Name = "verticalScrollBar",
                            [~ScrollBar.MaximumProperty] = parent[~ScrollViewer.VerticalScrollBarMaximumProperty],
                            [~~ScrollBar.ValueProperty] = parent[~~ScrollViewer.VerticalScrollBarValueProperty],
                        }
                    }
                });
        }

        private class Item : Control, ISelectable
        {
            public string Value { get; set; }
            public bool IsSelected { get; set; }
        }

        private class MasterViewModel : NotifyingBase
        {
            private ChildViewModel _child;

            public ChildViewModel Child
            {
                get { return _child; }
                set
                {
                    _child = value;
                    RaisePropertyChanged();
                }
            }
        }

        private class ChildViewModel : NotifyingBase
        {
            public IList<Item> Items { get; set; }
            public Item SelectedItem { get; set; }
            public int SelectedIndex { get; set; }
        }

        private class RootWithItems : TestRoot
        {
            public List<string> Items { get; set; } = new List<string>() { "a", "b", "c", "d", "e" };
            public string Selected { get; set; } = "b";
        }

        private class TestSelector : SelectingItemsControl
        {
            public static readonly new AvaloniaProperty<IList> SelectedItemsProperty =
                SelectingItemsControl.SelectedItemsProperty;

            public TestSelector()
            {
            }

            public TestSelector(SelectionMode selectionMode)
            {
                SelectionMode = selectionMode;
            }

            public new IList SelectedItems
            {
                get { return base.SelectedItems; }
                set { base.SelectedItems = value; }
            }

            public new ISelectionModel Selection
            {
                get => base.Selection;
                set => base.Selection = value;
            }

            public new SelectionMode SelectionMode
            {
                get { return base.SelectionMode; }
                set { base.SelectionMode = value; }
            }

            public void SelectAll() => Selection.SelectAll();
            public void UnselectAll() => Selection.ClearSelection();
            public void SelectRange(int index) => UpdateSelection(index, true, true);
            public void Toggle(int index) => UpdateSelection(index, true, false, true);

            public new bool MoveSelection(NavigationDirection direction, bool wrap)
            {
                return base.MoveSelection(direction, wrap);
            }
        }

        private class ResettingCollection : List<string>, INotifyCollectionChanged
        {
            public ResettingCollection(int itemCount)
            {
                AddRange(Enumerable.Range(0, itemCount).Select(x => $"Item{x}"));
            }

            public void Reset(IEnumerable<string> items)
            {
                Clear();
                AddRange(items);
                CollectionChanged?.Invoke(
                    this,
                    new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
            }

            public event NotifyCollectionChangedEventHandler CollectionChanged;
        }
    }
}
