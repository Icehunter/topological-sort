// topological-sort
// Rabbit.cs
//  
// Created by Ryan Wilson.
// Copyright (c) 2010-2012, Ryan Wilson. All rights reserved.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using topological_sort.Classes;

namespace topological_sort.Models
{
    /// <summary>
    /// </summary>
    public class ItemInfo
    {
        public string Key;
        public List<string> Requires = new List<string>();
    }

    /// <summary>
    /// </summary>
    public class Rabbit
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

        public static readonly Dictionary<string, ItemInfo> Unsorted = new Dictionary<string, ItemInfo>();
        private static List<string> _sorted = new List<string>();
        private static string _pkey = "";
        private static List<string> _seen = new List<string>();
        private static readonly Dictionary<string, List<string>> Cyclic = new Dictionary<string, List<string>>();

        #endregion

        #region " PUBLIC METHODS "

        /// <summary>
        /// </summary>
        public readonly Func<string, string, bool> TrySet = (parent, req) =>
        {
            if (!Exists(parent) || !Exists(req) || parent.Equals(req))
            {
                return false;
            }
            if (!Unsorted[parent].Requires.Contains(req))
            {
                Unsorted[parent].Requires.Add(req);
            }
            PostTitle(String.Format("Loaded :: {0}", Unsorted.Count));
            return true;
        };

        /// <summary>
        /// </summary>
        public readonly Func<IEnumerable<string>> Sort = () =>
        {
            PostNotice("NoticeEvent :: 'Checking Cyclical Dependencies' :\r");
            var message = new StringBuilder();
            foreach (var item in Unsorted.Values.AsParallel().Where(item => item.Requires.Count > 0))
            {
                Cyclic.Add(item.Key, new List<string>());
                var result = PathBuilder(item.Key, item);
                if (result)
                {
                    return null;
                }
            }
            _sorted = _seen = new List<string>();
            PostNotice("NoticeEvent :: 'No Cyclical Dependencies' :\r");
            PostNotice("NoticeEvent :: 'Isolating Non-Dependents' :\r");
            PostNotice("NoticeEvent :: 'Sorting' :\r");
            foreach (var item in Unsorted.AsParallel().Where(v => v.Value.Requires.Count == 0))
            {
                _seen.Add(item.Key);
            }
            _sorted = _seen = _seen.Distinct().ToList();
            foreach (var item in Unsorted.AsParallel().Where(item => item.Value.Requires.Any()))
            {
                message.Clear();
                message.Append(String.Format("Sorted :: {0}", _seen.Count));
                Helpers.Status.Content(message);
                TopologicalSort(Unsorted, item.Key);
            }
            return _sorted;
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
                    _seen.Clear();
                    _sorted.Clear();
                    Cyclic.Clear();
                    break;
            }
        }

        /// <summary>
        /// </summary>
        /// <param name="key"> </param>
        /// <returns> </returns>
        private static bool Exists(string key)
        {
            if (String.IsNullOrWhiteSpace(key))
            {
                return false;
            }
            if (!Unsorted.ContainsKey(key))
            {
                Unsorted.Add(key, new ItemInfo {Key = key, Requires = new List<string>()});
            }
            PostTitle(String.Format("Loaded :: {0}", Unsorted.Count));
            return true;
        }

        /// <summary>
        /// </summary>
        private static readonly Func<string, ItemInfo, bool> PathBuilder = (parent, item) =>
        {
            foreach (var s in item.Requires.AsParallel().Where(t => t.Any()))
            {
                Cyclic[parent].Add(s);
                if (Cyclic[parent].Contains(parent) || (Cyclic[parent].Distinct().Count() < Cyclic[parent].Count))
                {
                    _pkey = parent;
                    return true;
                }
                if (Unsorted.ContainsKey(s))
                {
                    if (Unsorted[s].Requires.Count > 0)
                    {
                        PathBuilder(parent, Unsorted[s]);
                    }
                }
            }
            return false;
        };

        /// <summary>
        /// </summary>
        private static readonly Func<IDictionary<string, ItemInfo>, string, bool> TopologicalSort = (list, key) =>
        {
            if (!_seen.Contains(key))
            {
                if (list.ContainsKey(key))
                {
                    foreach (var depItem in list[key].Requires)
                    {
                        TopologicalSort(list, depItem);
                    }
                }
                _seen.Add(key);
                if (!_sorted.Contains(key))
                {
                    _sorted.Add(key);
                }
            }
            return true;
        };

        #endregion
    }
}