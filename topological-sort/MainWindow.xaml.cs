// topological-sort
// MainWindow.xaml.cs
//  
// Created by Ryan Wilson.
// Copyright (c) 2010-2012, Ryan Wilson. All rights reserved.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using topological_sort.Classes;
using topological_sort.Models;

namespace topological_sort
{
    /// <summary>
    ///   Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        #region " VARIABLES "

        public static MainWindow View;
        private const String Filename = "sample.txt";
        private const string Alpha = "abcdefghijklmnopqrstuvwxyz";
        private static readonly Random Rand = new Random();
        public const string Titles = "Data Loaded :: {0} Parents, {1} Children";
        private static Rabbit _rabbit = new Rabbit();
        private static Topological<string> _ts = new Topological<string>();

        #endregion

        public MainWindow()
        {
            InitializeComponent();
            //listen to events
            Topological<string>.Notice += e_Notice;
            Topological<string>.Status += e_Status;
            Topological<string>.Title += e_Title;
            Rabbit.Notice += e_Notice;
            Rabbit.Status += e_Status;
            Rabbit.Title += e_Title;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            View = this;
        }

        private static void e_Title(string line)
        {
            Helpers.Title.Text(new StringBuilder().Append(line));
        }

        private static void e_Notice(string line)
        {
            Helpers.RTB.Append(new StringBuilder().Append(line));
        }

        private static void e_Status(string line)
        {
            Helpers.Status.Content(new StringBuilder().Append(line));
        }

        #region " DATA LOADING FUNCTIONS "

        /// <summary>
        ///   The following function will generate upto 20,000 records for speed testing.
        /// </summary>
        private static void LoadSampleData()
        {
            ////small topological sample
            //_ts.TrySet("a", "b");
            //_ts.TrySet("a", "c");
            //_ts.TrySet("c", "d");
            //_ts.TrySet("c", "e");
            ////small rabbit sample
            //_rabbit.TrySet("a", "b");
            //_rabbit.TrySet("a", "c");
            //_rabbit.TrySet("c", "d");
            //_rabbit.TrySet("c", "e");
            const int n = 10;
            const int limit = 10000;
            var olimit = Rand.Next(limit);
            //var limit = (Math.Pow(26, n) > int.MaxValue) ? 100000 : (int)Math.Pow(26, n);
            for (var o = 0; o < olimit; o++)
            {
                var ilimit = Rand.Next(1, 5);
                for (var i = 0; i < ilimit; i++)
                {
                    _ts.TrySet(NewName(n), NewName(n));
                    _rabbit.TrySet(NewName(n), NewName(n));
                }
            }
        }

        /// <summary>
        ///   Load Samples From File: sample.txt
        ///   Correct Format: parent:item1,item2,item3,item4
        ///   parent:listitem1
        ///   parent:
        ///   parent
        ///   :
        ///   :,
        ///   :,list1
        ///   :listitem1,
        /// </summary>
        private static void LoadSampleData(IEnumerable<string> lines)
        {
            foreach (var str in lines)
            {
                var f = str.Split(':');
                var pos = f.Count() == 1 ? 0 : 1;
                var t = f[pos].Split(',').Where(s => !String.IsNullOrWhiteSpace(s)).Aggregate("", (current, s) => current + (s + ","));
                t = t.Contains(',') ? t.Substring(0, t.Length - 1) : "";
                if (f.Count() <= 1)
                {
                    continue;
                }
                var name = (String.IsNullOrWhiteSpace(f[0])) ? "" : f[0];
                var dep = (String.IsNullOrWhiteSpace(t)) ? new List<string> {name} : t.Split(',').ToList();
                foreach (var s in dep)
                {
                    _ts.TrySet(name, s);
                    _rabbit.TrySet(name, s);
                }
            }
            Helpers.GUI.Update("default");
        }

        #endregion

        #region " PRIVATE FUNCTIONS "

        /// <summary>
        ///   Random name
        /// </summary>
        /// <returns> </returns>
        private static string NewName(int length)
        {
            return new string(Enumerable.Repeat(Alpha, Rand.Next((int) Math.Floor((decimal) length/2), length)).Select(s => s[Rand.Next(s.Length)]).ToArray());
        }

        /// <summary>
        ///   Random bool
        /// </summary>
        /// <returns> </returns>
        private static bool NewBool()
        {
            return Rand.Next(0, 10) > 5;
        }

        #endregion

        #region " GUI FUNCTIONS "

        /// <summary>
        ///   Get Data (Random/sample.txt)
        ///   reverse the comments below to test random data (item.sorted...)
        /// </summary>
        /// <param name="sender"> </param>
        /// <param name="e"> </param>
        private void btn_g_Click(object sender, RoutedEventArgs e)
        {
            Func<bool> c = delegate
            {
                Helpers.RTB.Clear();
                Helpers.GUI.Update("processing");
                _rabbit.Clear();
                _ts.Clear();
                LoadSampleData();
                //LoadSampleData(File.ReadAllLines(Filename));
                Helpers.GUI.Update("default");
                return true;
            };
            c.BeginInvoke(null, null);
        }

        /// <summary>
        ///   Dependency results
        /// </summary>
        /// <param name="sender"> </param>
        /// <param name="e"> </param>
        private void btn_d_Click(object sender, RoutedEventArgs e)
        {
            Func<bool> c = delegate
            {
                _ts.Clear("sort");
                _rabbit.Clear("sort");
                Helpers.GUI.Update("processing");
                Helpers.RTB.Clear();
                Helpers.RTB.Append(new StringBuilder().Append("NoticeEvent :: 'Inspecting Data For Sort' :\r"));
                var sw = new Stopwatch();
                sw.Start();
                //var results = _ts.Sort();
                var results = _rabbit.Sort();
                var memtime = sw.ElapsedMilliseconds/1000.00;
                sw.Restart();
                var message = new StringBuilder();
                if (results != null)
                {
                    SaveUnsorted();
                    Helpers.RTB.Append(new StringBuilder().Append("NoticeEvent :: 'Completed' Building Results, Please Wait :\r"));
                    foreach (var s in results)
                    {
                        message.Append(String.Format("{0}\r", s));
                    }
                    View.Dispatcher.Invoke((Action) (() =>
                    {
                        try
                        {
                            Clipboard.Clear();
                            Clipboard.SetText(message.ToString());
                            Helpers.RTB.Append(new StringBuilder().Append("DataEvent :: 'Clipboard' Copied To Clipboard :\r"));
                        }
                        catch
                        {
                            Helpers.RTB.Append(new StringBuilder().Append("DataEvent :: 'Clipboard' Failed To Copy :\r"));
                        }
                    }));
                    var filename = string.Format("sort-{0:yyyy-MM-dd_hh-mm-ss-tt}.csv", DateTime.Now);
                    File.AppendAllText(filename, message.ToString());
                    Helpers.RTB.Append(new StringBuilder().Append("DataEvent :: 'Saved' File Saved To Launch Directory :\r"));
                    Helpers.RTB.Append(new StringBuilder().Append(String.Format("NoticeEvent :: 'Filename' {0} :\r", filename)));
                }
                else
                {
                    Helpers.RTB.Append((new StringBuilder().Append("ErrorEvent :: 'Cyclic Dependencies' : Aborted Sort :\r")));
                }
                var dsptime = sw.ElapsedMilliseconds/1000.00;
                sw.Stop();
                Helpers.Status.Content(new StringBuilder().Append(String.Format("Processing Time (Seconds) :: ~ {0:F4} (Memory) : ~ {1:F4} (Display)", memtime, dsptime)));
                Helpers.GUI.Update("sorted");
                return true;
            };
            c.BeginInvoke(null, null);
        }

        /// <summary>
        ///   Hierarchical results
        /// </summary>
        private void SaveUnsorted()
        {
            Func<bool> c = delegate
            {
                Helpers.GUI.Update("processing");
                var results = Rabbit.Unsorted;
                var message = new StringBuilder().Append("Parent,Child\r");
                foreach (var s in results)
                {
                    if (s.Value.Requires.Count > 0)
                    {
                        foreach (var item in s.Value.Requires)
                        {
                            message.Append(String.Format("{0},{1}\r", s.Key, item));
                        }
                        continue;
                    }
                    message.Append(String.Format("{0}\r", s.Key));
                }
                var filename = string.Format("original-{0:yyyy-MM-dd_hh-mm-ss-tt}.csv", DateTime.Now);
                File.AppendAllText(filename, message.ToString());
                Helpers.GUI.Update("default");
                return true;
            };
            c.BeginInvoke(null, null);
        }

        #endregion
    }
}