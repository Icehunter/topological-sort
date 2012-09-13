// topological-sort
// Helpers.cs
//  
// Created by Ryan Wilson.
// Copyright (c) 2010-2012, Ryan Wilson. All rights reserved.

using System;
using System.Text;

namespace topological_sort.Classes
{
    internal static class Helpers
    {
        public static class GUI
        {
            public static void Update(string type)
            {
                MainWindow.View.Dispatcher.Invoke((Action) (() =>
                {
                    switch (type)
                    {
                        case "default":
                            MainWindow.View.btn_g.IsEnabled = false;
                            MainWindow.View.btn_d.IsEnabled = true;
                            break;
                        case "processing":
                            MainWindow.View.btn_g.IsEnabled = false;
                            MainWindow.View.btn_d.IsEnabled = false;
                            break;
                        case "sorted":
                            MainWindow.View.btn_g.IsEnabled = true;
                            MainWindow.View.btn_d.IsEnabled = false;
                            break;
                    }
                }));
            }
        }

        public static class RTB
        {
            public static void Clear()
            {
                MainWindow.View.Dispatcher.Invoke((Action) (() => MainWindow.View.Final.Document.Blocks.Clear()));
            }

            public static void Append(StringBuilder sb)
            {
                MainWindow.View.Dispatcher.Invoke((Action) (() => MainWindow.View.Final.AppendText(sb.ToString())));
            }
        }

        public static class Status
        {
            public static void Content(StringBuilder sb)
            {
                MainWindow.View.Dispatcher.Invoke((Action) (() => MainWindow.View.sbi_Status.Content = sb.ToString()));
            }
        }

        public static class Title
        {
            public static void Text(StringBuilder sb)
            {
                MainWindow.View.Dispatcher.Invoke((Action) (() => MainWindow.View.Title = sb.ToString()));
            }
        }
    }
}