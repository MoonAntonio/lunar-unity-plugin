﻿using UnityEngine;

using System;
using System.Text;

using LunarPlugin;
using LunarEditor;
using LunarPluginInternal;

using LunarPlugin.Test;

using NUnit.Framework;

namespace ConsoleViewTests
{
    using Assert = NUnit.Framework.Assert;

    public abstract class ConsoleViewTest : TestFixtureBase
    {
        protected CTag tag = new CTag("TestTag");

        #region Helpers

        internal void AssertVisibleRows(TableView table, params string[] values)
        {
            Assert.AreEqual(values.Length, table.VisibleCellsCount,
                "expected: " + StringArrayToString(values) + "\n" +
                "actual: " + VisibleCellsToString(table));

            int index = 0;
            TableViewCell cell = table.FirstVisibleCell;
            TableViewCell lastCell = null;
            while (cell != null)
            {
                ConsoleTextEntryView textCell = cell as ConsoleTextEntryView;
                Assert.IsNotNull(textCell);

                Assert.AreEqual(values[index++], textCell.Value);
                lastCell = cell;
                cell = cell.NextCell;
            }
            Assert.AreEqual(values.Length, index);
            Assert.AreSame(lastCell, table.LastVisibleCell);
        }

        private string VisibleCellsToString(TableView table)
        {
            StringBuilder buffer = new StringBuilder("[");
            int index = 0;
            for (TableViewCell cell = table.FirstVisibleCell; cell != null; cell = cell.NextCell)
            {
                ConsoleTextEntryView textCell = cell as ConsoleTextEntryView;
                Assert.IsNotNull(textCell);

                buffer.Append(textCell.Value);
                if (++index < table.VisibleCellsCount)
                {
                    buffer.Append(", ");
                }
            }

            buffer.Append("]");
            return buffer.ToString();
        }

        private string StringArrayToString(string[] values)
        {
            StringBuilder buffer = new StringBuilder("[");
            int index = 0;
            foreach (string value in values)
            {
                buffer.Append(value);
                if (++index < values.Length)
                {
                    buffer.Append(", ");
                }
            }

            buffer.Append("]");
            return buffer.ToString();
        }

        #endregion
    }

    class MockConsoleView : ConsoleView, ICTextMeasure
    {
        public MockConsoleView(CAbstractConsole console, float width, float height)
            : base(console, width, height)
        {
        }

        protected override ICTextMeasure CreateTextMeasure()
        {
            return this;
        }

        public Vector2 CalcSize(string text)
        {
            return new Vector2(text.Length * 10, 10);
        }

        public float CalcHeight(string text, float width)
        {
            int height = 1;
            for (int i = 0; i < text.Length; ++i)
            {
                if (text [i] == '\n')
                {
                    ++height;
                }
            }

            return height;
        }

        public float LineHeight
        {
            get { return 1; }
        }
    }

    class MockConsole : CAbstractConsole
    {
        public MockConsole(int historySize = 100)
            : base(historySize)
        {
        }

        public void Add(CLogLevel level, CTag tag, string line)
        {
            ConsoleViewCellEntry entry = new ConsoleViewCellEntry(line);
            entry.level = level;
            entry.tag = tag;
            Add(entry);
        }
    }
}

