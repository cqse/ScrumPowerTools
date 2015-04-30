using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ScrumPowerTools.Framework.Presentation
{
    public class PropertyGroupDescriptor<T>
    {
        private Func<T, object> descriptor;

        public T Item { get; private set; }

        public PropertyGroupDescriptor(T item, Func<T, object> descriptor)
        {
            this.Item = item;
            this.descriptor = descriptor;
        }

        public override string ToString()
        {
            return this.descriptor(Item).ToString();
        }

        public override bool Equals(object o)
        {
            PropertyGroupDescriptor<T> other = o as PropertyGroupDescriptor<T>;
            if (o != null)
            {
                o = other.descriptor(other.Item);
            }

            return this.descriptor(Item).Equals(o);
        }

        public override int GetHashCode()
        {
            return this.descriptor(Item).GetHashCode();
        }
    }
}
