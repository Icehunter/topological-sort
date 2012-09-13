// topological-sort
// Topological.cs
//  
// Created by Ryan Wilson.
// Copyright (c) 2010-2012, Ryan Wilson. All rights reserved.

using System;
using System.Collections.Generic;
using System.Linq;

namespace topological_sort.Models
{
    /// <summary>
    /// </summary>
    /// <typeparam name="T"> </typeparam>
    public class ItemInfo<T>
    {
        public T Key { get; set; }
        public readonly List<T> Requires = new List<T>();
        public int Inverts;

        public void Clear()
        {
            Requires.Clear();
        }
    }

    /// <summary>
    /// </summary>
    /// <typeparam name="T"> </typeparam>
    internal class Topological<T> where T : IEquatable<T>
    {
        #region " EVENTS "

        public delegate void StringEventHandler(string line);

        public static event StringEventHandler Title;
        public static event StringEventHandler Status;
        public static event StringEventHandler Notice;

        /// <summary>
        /// </summary>
        /// <param name="msg"> </param>
        private static void PostTitle(string msg)
        {
            RaiseTitle(msg);
        }

        /// <summary>
        /// </summary>
        /// <param name="state"> </param>
        private static void RaiseTitle(object state)
        {
            if (Title != null)
            {
                Title((string) state);
            }
        }

        /// <summary>
        /// </summary>
        /// <param name="msg"> </param>
        private static void PostStatus(string msg)
        {
            RaiseStatus(msg);
        }

        /// <summary>
        /// </summary>
        /// <param name="state"> </param>
        private static void RaiseStatus(object state)
        {
            if (Status != null)
            {
                Status((string) state);
            }
        }

        /// <summary>
        /// </summary>
        /// <param name="msg"> </param>
        private static void PostNotice(string msg)
        {
            RaiseNotice(msg);
        }

        /// <summary>
        /// </summary>
        /// <param name="state"> </param>
        private static void RaiseNotice(object state)
        {
            if (Notice != null)
            {
                Notice((string) state);
            }
        }

        #endregion

        #region " VARIABLES "

        public static readonly Dictionary<T, ItemInfo<T>> Unsorted = new Dictionary<T, ItemInfo<T>>();
        private static string _pkey = "";
        private static List<T> _sorted = new List<T>();
        private static readonly Dictionary<T, List<T>> Cyclic = new Dictionary<T, List<T>>();

        #endregion

        #region " PUBLIC METHODS "

        /// <summary>
        /// </summary>
        public readonly Func<T, T, bool> TrySet = (parent, req) =>
        {
            if (!Exists(parent) || !Exists(req) || parent.Equals(req))
            {
                return false;
            }
            if (!Unsorted[parent].Requires.Contains(req))
            {
                Unsorted[parent].Requires.Add(req);
                ++Unsorted[parent].Inverts;
            }
            PostTitle(String.Format("Loaded :: {0}", Unsorted.Count));
            return true;
        };

        /// <summary>
        /// </summary>
        public readonly Func<IEnumerable<T>> Sort = () =>
        {
            _sorted = new List<T>();
            PostNotice("NoticeEvent :: 'Isolating Non-Dependents' :\r");
            PostNotice("NoticeEvent :: 'Sorting' :\r");
            var oust = Enumerable.ToList(Unsorted.Values.AsParallel().Where(s => s.Inverts == 0).Select(s => s.Key));
            while (oust.Any())
            {
                var key = oust[0];
                _sorted.Add(key);
                oust.RemoveAt(0);
                Unsorted.Remove(key);
                foreach (var s in Unsorted)
                {
                    foreach (var v in _sorted.Where(v => Unsorted[s.Key].Requires.Contains(v)))
                    {
                        Unsorted[s.Key].Requires.Remove(v);
                        --Unsorted[s.Key].Inverts;
                    }
                    if (Unsorted[s.Key].Inverts == 0)
                    {
                        if (!oust.Contains(s.Key))
                        {
                            oust.Add(s.Key);
                        }
                        Unsorted[s.Key].Clear();
                    }
                }
                PostStatus(String.Format("Sorted :: {0}", _sorted.Count));
            }
            PostNotice("NoticeEvent :: 'Checking Cyclical Dependencies' :\r");
            return (Unsorted.Count == 0) ? _sorted : null;
        };

        #endregion

        #region " PRIVATE METHODS "

        /// <summary>
        ///   Clear lists to resort if wanted
        /// </summary>
        public void Clear(string type = "")
        {
            switch (type)
            {
                case "":
                    Unsorted.Clear();
                    break;
                case "sort":
                    _sorted.Clear();
                    Cyclic.Clear();
                    break;
            }
        }

        /// <summary>
        /// </summary>
        /// <param name="key"> </param>
        /// <returns> </returns>
        private static bool Exists(T key)
        {
            if (key == null || String.IsNullOrWhiteSpace(key.ToString()))
            {
                return false;
            }
            if (!Unsorted.ContainsKey(key))
            {
                Unsorted.Add(key, new ItemInfo<T> {Key = key});
            }
            PostTitle(String.Format("Loaded :: {0}", Unsorted.Count));
            return true;
        }

        #endregion
    }
}