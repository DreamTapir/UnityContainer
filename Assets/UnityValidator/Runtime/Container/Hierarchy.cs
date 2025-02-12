using System;
using System.Collections.Generic;

namespace UnityValidator
{
    public class Hierarchy : IDisposable
    {
        /// <summary>
        /// 対応するオブジェクト
        /// </summary>
        public object Self { get; }

        public bool IsActive { get; }

        /// <summary>
        /// 階層の深さ
        /// </summary>
        public int Depth
        {
            get
            {
                var depth = 0;
                var parent = Parent;
                while (parent != null)
                {
                    depth++;
                    parent = parent.Parent;
                }
                return depth;
            }
        }

        /// <summary>
        /// 一番上の階層のオブジェクトを返す
        /// </summary>
        public Hierarchy Root => Parent == null ? this : Parent.Root;

        /// <summary>
        /// 一つ上の階層のオブジェクトを返す
        /// </summary>
        public Hierarchy Parent { get; }

        /// <summary>
        /// 一つ下の階層のオブジェクトを返す
        /// </summary>
        public IEnumerable<Hierarchy> Children => _children;
        private List<Hierarchy> _children;
        
        public Hierarchy(object self, Hierarchy parent = null)
        {
            Self = self;
            Parent = parent;
            _children = new();
        }

        public void AddChild(Hierarchy child)
        {
            _children.Add(child);
        }

        public void RemoveChild(Hierarchy child)
        {
            _children.Remove(child);
        }

        public void Dispose()
        {
            _children.ForEach(c => c.Dispose());
            _children.Clear();
            _children = null;
        }
    }
}