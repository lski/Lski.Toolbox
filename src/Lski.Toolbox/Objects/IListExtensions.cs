﻿using System;
using System.Collections;

namespace Lski.Toolbox.Objects {

    /// <summary>
    /// Provides extension methods for moving items up/down the list
    /// </summary>
    public static class IListExtensions {

        /// <summary>
        /// Moves the item, in the selected List, at the selected index, one index down E.g. 2 -> 1
        /// </summary>
        public static IList MoveUp(this IList list, Int32 index) {

            // Cant be moved anymore so exit
            if (index == 0) {
                return list;
            }

            var count = list.Count;

            if (index < 0 || (index > count - 1)) {
                throw new IndexOutOfRangeException(string.Format("Index {0} does not exist, list has only {1} items", index, count));
            }

            // Add the item to a position before the current one
            list.Insert(index - 1, list[index]);

            // Remove the reference to the item, which is now a position above where it was
            list.RemoveAt(index + 1);

            return list;
        }

        /// <summary>
        /// Moves the item, in the selected list, at the selected index, one index up E.g. 2 -> 3
        /// </summary>
        public static IList MoveDown(this IList list, Int32 index) {

            var count = list.Count;

            // Cant be moved further to just exit
            if (index == count - 1) {
                return list;
            }

            if (index < 0 || (index > count - 1)) {
                throw new IndexOutOfRangeException(string.Format("Index {0} does not exist, list has only {1} items", index, count));
            }

            // Add the item to a position before the current one
            list.Insert(index + 1, list[index]);

            // Remove the reference to the item, which is now still in the same position as it was
            list.RemoveAt(index);

            return list;
        }

        /// <summary>
        /// Moves the item, in the selected list, at the selected index, to the the start (index = 0) of the list
        /// </summary>
        public static IList MoveFirst(this IList list, Int32 index) {

            // Cant be moved anymore so exit
            if (index == 0) {
                return list;
            }

            var count = list.Count;

            if (index < 0 || (index > count - 1)) {
                throw new IndexOutOfRangeException(string.Format("Index {0} does not exist, list has only {1} items", index, count));
            }

            // Add the item to a position before the current one
            list.Insert(0, list[index]);

            // Remove the reference to the item, which is now a position above where it was
            list.RemoveAt(index + 1);

            return list;
        }

        /// <summary>
        /// Move the item, in the selcted list, at the selected index, to the end (index = list.count - 1) of the list
        /// </summary>
        public static IList MoveEnd(this IList list, Int32 index) {

            var count = list.Count;

            // Cant be moved further to just exit
            if (index == count - 1) {
                return list;
            }

            if (index < 0 || (index > count - 1)) {
                throw new IndexOutOfRangeException(string.Format("Index {0} does not exist, list has only {1} items", index, list.Count));
            }

            // Add the item to a position before the current one
            list.Add(list[index]);

            // Remove the reference to the item, which is now still in the same position as it was
            list.RemoveAt(index);

            return list;
        }
    }
}