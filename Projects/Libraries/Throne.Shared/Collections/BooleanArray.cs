using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;

namespace Throne.Shared.Collections
{
    public sealed class BooleanArray<TIndexer> : ICloneable, IEnumerable
        where TIndexer : IConvertible
    {
        private const UInt32 SINGLE = (UInt32) 1 << 31;
        private Int32 _length;
        private Object _syncRoot;
        private UInt32[] bits;

        public BooleanArray(BooleanArray<TIndexer> srcBools)
        {
            InitializeFrom(srcBools.ToArray());
        }

        public BooleanArray(IEnumerable<Boolean> srcBits)
        {
            InitializeFrom(srcBits.ToArray());
        }

        public BooleanArray(Int32 capacity)
        {
            bits = new UInt32[RequiredSize(capacity)];
            _length = capacity;
        }

        public Boolean this[Int32 index]
        {
            get
            {
                if (index >= _length) throw new IndexOutOfRangeException();

                int byteIndex = index >> 5;
                int bitIndex = index & 0x1f;
                return ((bits[byteIndex] << bitIndex) & SINGLE) != 0;
            }
            set
            {
                if (index >= _length) throw new IndexOutOfRangeException();

                int byteIndex = index >> 5;
                int bitIndex = index & 0x1f;
                if (value) bits[byteIndex] |= (SINGLE >> bitIndex);
                else bits[byteIndex] &= ~(SINGLE >> bitIndex);
            }
        }

        public Boolean this[TIndexer index]
        {
            get { return Get(index); }
            set { Set(index, value); }
        }

        public Int32 Count
        {
            get { return _length; }
        }

        public Object SyncRoot
        {
            get
            {
                if (_syncRoot == null) Interlocked.CompareExchange<Object>(ref _syncRoot, new Object(), null);
                return _syncRoot;
            }
        }

        #region IEnumerable

        public IEnumerator GetEnumerator()
        {
            return ToArray().GetEnumerator();
        }

        #endregion

        #region ICloneable

        public Object Clone()
        {
            return new BooleanArray<TIndexer>(this);
        }

        public BooleanArray<TIndexer> StrongClone()
        {
            return new BooleanArray<TIndexer>(this);
        }

        #endregion

        private void InitializeFrom(ICollection<Boolean> srcBits)
        {
            _length = srcBits.Count;
            bits = new UInt32[RequiredSize(_length)];

            int index = 0;
            foreach (bool b in srcBits) this[index++] = b;
        }

        private static Int32 RequiredSize(Int32 bitCapacity)
        {
            return (bitCapacity + 31) >> 5;
        }

        public Boolean[] ToArray()
        {
            var vbits = new bool[_length];
            for (int i = 0; i < _length; i++) vbits[i] = this[i];
            return vbits;
        }

        public Boolean Get(TIndexer index)
        {
            return this[index.ToInt32(CultureInfo.InvariantCulture)];
        }

        public BooleanArray<TIndexer> GetBits(Int32 startBit = 0, Int32 numBits = -1)
        {
            if (numBits == -1) numBits = bits.Length;
            return new BooleanArray<TIndexer>(ToArray().Skip(startBit).Take(numBits).ToArray());
        }

        public BooleanArray<TIndexer> Set(TIndexer index, Boolean value)
        {
            this[index.ToInt32(CultureInfo.InvariantCulture)] = value;
            return this;
        }

        public BooleanArray<TIndexer> SetAll(Boolean value)
        {
            for (int i = 0; i < Count; i++) this[i] = value;
            return this;
        }

        public Byte[] ToBytes(Int32 startBit = 0, Int32 numBits = -1)
        {
            if (numBits == -1) numBits = _length - startBit;
            var ba = GetBits(startBit, numBits);
            var nb = (numBits + 7)/8;
            var bb = new byte[nb];
            for (var i = 0; i < ba.Count; i++)
            {
                if (!ba[i]) continue;
                var bp = (i%8);
                bb[i/8] = (byte) (bb[i/8] | (1 << bp));
            }
            return bb;
        }

        public static implicit operator Byte[](BooleanArray<TIndexer> ba)
        {
            return ba.ToBytes();
        }
    }
}