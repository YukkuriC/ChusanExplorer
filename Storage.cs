﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChusanExplorer
{
    public class IDObject
    {
        public int id;
    }

    public class IDStorage<T> : Dictionary<int, T> where T : IDObject
    {
        public void Push(T obj)
        {
            if (ContainsKey(obj.id))
            {
                // TODO log override
            }
            this[obj.id] = obj;
        }
    }

    public static class Storage
    {
        public static IDStorage<Character> Characters = new IDStorage<Character>();
        public static IDStorage<CharaImageGroup> DDSChara = new IDStorage<CharaImageGroup>();
        public static IDStorage<Music> Music = new IDStorage<Music>();
    }
}