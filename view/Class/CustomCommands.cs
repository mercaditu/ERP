using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;

namespace Cognitivo.Class
{
    public static class CustomCommands
    {
        static Cognitivo.GestureSettings propGesture = new Cognitivo.GestureSettings();

        //Used For Delete Operation in Child DataGrid.
        public static readonly RoutedUICommand Delete =
            new RoutedUICommand("Delete", "Delete", typeof(CustomCommands)); //, new InputGestureCollection() { new KeyGesture(Key.Delete, ModifierKeys.Alt) }

        //Used In All Window For CRUD Operation.
        //Save
        static Key SaveKey = (Key)Enum.Parse(typeof(Key), propGesture.SaveKey);
        static ModifierKeys SaveModifier = (ModifierKeys)Enum.Parse(typeof(ModifierKeys), propGesture.SaveModifier);
        public static RoutedUICommand SaveAll =
            new RoutedUICommand("Save", "Save", typeof(CustomCommands), new InputGestureCollection() { new KeyGesture(SaveKey, SaveModifier)});
        //New
        static Key NewKey = (Key)Enum.Parse(typeof(Key), propGesture.NewKey);
        static ModifierKeys NewModifier = (ModifierKeys)Enum.Parse(typeof(ModifierKeys), propGesture.NewModifier);
        public static RoutedUICommand NewAll =
            new RoutedUICommand("New", "New", typeof(CustomCommands), new InputGestureCollection() { new KeyGesture(NewKey, NewModifier) });
        //Delete
        static Key DeleteKey = (Key)Enum.Parse(typeof(Key), propGesture.DeleteKey);
        static ModifierKeys DeleteModifier = (ModifierKeys)Enum.Parse(typeof(ModifierKeys), propGesture.DeleteModifier);
        public static RoutedUICommand DeleteAll =
            new RoutedUICommand("DeleteMain", "DeleteMain", typeof(CustomCommands), new InputGestureCollection() { new KeyGesture(DeleteKey, DeleteModifier) });
        //Cancel
        static Key CancelKey = (Key)Enum.Parse(typeof(Key), propGesture.CancelKey);
        static ModifierKeys CancelModifier = (ModifierKeys)Enum.Parse(typeof(ModifierKeys), propGesture.CancelModifier);
        public static RoutedUICommand CancelAll =
            new RoutedUICommand("Cancel", "Cancel", typeof(CustomCommands), new InputGestureCollection() { new KeyGesture(CancelKey, CancelModifier) });
        //Edit
        static Key EditKey = (Key)Enum.Parse(typeof(Key), propGesture.EditKey);
        static ModifierKeys EditModifier = (ModifierKeys)Enum.Parse(typeof(ModifierKeys), propGesture.EditModifier);
        public static RoutedUICommand EditAll =
            new RoutedUICommand("Edit", "Edit", typeof(CustomCommands), new InputGestureCollection() { new KeyGesture(EditKey, EditModifier) });
        //Approve
        static Key ApproveKey = (Key)Enum.Parse(typeof(Key), propGesture.EditKey);
        static ModifierKeys ApproveModifier = (ModifierKeys)Enum.Parse(typeof(ModifierKeys), propGesture.EditModifier);
        public static RoutedUICommand Approve =
            new RoutedUICommand("Approve", "Approve", typeof(CustomCommands), new InputGestureCollection() { new KeyGesture(EditKey, ApproveModifier) });
        //anull
        static Key AnullKey = (Key)Enum.Parse(typeof(Key), propGesture.EditKey);
        static ModifierKeys AnullModifier = (ModifierKeys)Enum.Parse(typeof(ModifierKeys), propGesture.EditModifier);
        public static RoutedUICommand Anull =
            new RoutedUICommand("Anull", "Anull", typeof(CustomCommands), new InputGestureCollection() { new KeyGesture(EditKey, AnullModifier) });
        //Pending
        static Key PendingKey = (Key)Enum.Parse(typeof(Key), propGesture.EditKey);
        static ModifierKeys PendingModifier = (ModifierKeys)Enum.Parse(typeof(ModifierKeys), propGesture.EditModifier);
        public static RoutedUICommand Pending =
            new RoutedUICommand("Pending", "Pending", typeof(CustomCommands), new InputGestureCollection() { new KeyGesture(EditKey, PendingModifier) });

    }
}
