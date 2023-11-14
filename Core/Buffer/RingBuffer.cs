using System;
using System.Collections.Generic;

using Core.Util;

namespace Core.Buffer
{
    public class RingBuffer
    {
        public int UseSize { get; private set; }
        public int FreeSize => _buffer.Length - UseSize;
        public int BufferEnd => _buffer.Length;

        private byte[] _buffer;
        private int _bufferFront;
        private int _bufferRear;

        public RingBuffer(int bufferSize)
        {
            if (bufferSize == 0)
            {
                Logger.Warnning("RingBuffer Size Is Zero");
                return;
            }

            UseSize = 0;

            _buffer = new byte[bufferSize];
            _bufferFront = 0;
            _bufferRear = 0;
        }

        public List<ArraySegment<byte>> GetWritable()
        {
            if (FreeSize == 0)
                return null;

            var segmentList = new List<ArraySegment<byte>>();
            segmentList.Add(new ArraySegment<byte>(_buffer, _bufferRear, GetDirectWritableSize()));

            if (_bufferRear < _bufferFront)
                return segmentList;

            segmentList.Add(new ArraySegment<byte>(_buffer, 0, _bufferFront));
            return segmentList;
        }

        public void FinishWrite(int size)
        {
            if (size <= (BufferEnd - _bufferRear - 1))
            {
                _bufferRear += size;
            }
            else
            {
                var tempSize = BufferEnd - _bufferRear;
                _bufferRear = size - tempSize;
            }

            UseSize += size;
        }

        public void FinishRead(int size)
        {
            if (size <= (BufferEnd - _bufferFront - 1))
            {
                _bufferFront += size;
            }
            else
            {
                var tempSize = BufferEnd - _bufferFront;
                _bufferFront = size - tempSize;
            }

            UseSize -= size;
        }

        public ArraySegment<byte> Peek(int size)
        {
            if (size > UseSize)
                return default;

            var segment = new ArraySegment<byte>(_buffer);
            if (size <= BufferEnd - _bufferFront)
            {
                return new ArraySegment<byte>(_buffer, _bufferFront, size);
            }
            else
            {
                var tempData = new byte[size];
                var frontDataSize = BufferEnd - _bufferFront;
                Array.Copy(_buffer, _bufferFront, tempData, 0, frontDataSize);

                var remainDataSize = size - frontDataSize;
                Array.Copy(_buffer, 0, tempData, frontDataSize, remainDataSize);

                return new ArraySegment<byte>(tempData, 0, size);
            }
        }

        //public bool Enqueue(byte[] srcData)
        //{
        //    var srcDataSize = srcData.Length;
        //    if (srcDataSize > FreeSize)
        //        return false;

        //    if (srcDataSize <= BufferEnd - _bufferRear)
        //    {
        //        Array.Copy(srcData, 0, _buffer, _bufferRear, srcDataSize);
        //        _bufferRear += srcDataSize;
        //    }
        //    else
        //    {
        //        int rearRemainSize = BufferEnd - _bufferRear;
        //        Array.Copy(srcData, 0, _buffer, _bufferRear, rearRemainSize);
        //        _bufferRear = 0;

        //        int remainDataSize = srcDataSize - rearRemainSize;
        //        Array.Copy(srcData, rearRemainSize, _buffer, _bufferRear, remainDataSize);
        //        _bufferRear += remainDataSize;
        //    }

        //    UseSize += srcDataSize;
        //    return true;
        //}

        //public byte[] Dequeue(int size)
        //{
        //    if (size > UseSize)
        //        return null;

        //    var data = new byte[size];
        //    if (size <= BufferEnd - _bufferFront)
        //    {
        //        Array.Copy(_buffer, _bufferFront, data, 0, size);
        //        _bufferFront += size;
        //    }
        //    else
        //    {
        //        int frontDataSize = BufferEnd - _bufferFront;
        //        Array.Copy(_buffer, _bufferFront, data, 0, frontDataSize);
        //        _bufferFront = 0;

        //        int remainDataSize = size - frontDataSize;
        //        Array.Copy(_buffer, _bufferFront, data, frontDataSize, remainDataSize);
        //        _bufferFront += remainDataSize;
        //    }

        //    UseSize -= size;
        //    return data;
        //}



        public bool IsEmpty()
        {
            return UseSize == 0;
        }

        public void Clear()
        {
            Array.Clear(_buffer);

            _bufferFront = 0;
            _bufferRear = 0;
        }

        private int GetDirectWritableSize()
        {
            if (_bufferFront <= _bufferRear)
                return BufferEnd - _bufferRear;
            else
                return _bufferFront - _bufferRear;
        }

        private int GetDirectReadableSize()
        {
            if (_bufferFront <= _bufferRear)
                return _bufferRear - _bufferFront;
            else
                return BufferEnd - _bufferFront;
        }
    }
}
