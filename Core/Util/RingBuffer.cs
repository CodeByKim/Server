using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;

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
            _buffer = new byte[bufferSize];
            _bufferFront = 0;
            _bufferRear = 0;
            UseSize = 0;
        }

        public void RegisterToReceive(SocketAsyncEventArgs args)
        {
            args.SetBuffer(_buffer, _bufferRear, GetDirectEnqueueSize());
        }

        public bool Enqueue(byte[] srcData)
        {
            var srcDataSize = srcData.Length;
            if (srcDataSize > FreeSize)
                return false;

            if (srcDataSize <= BufferEnd - _bufferRear)
            {
                Array.Copy(srcData, 0, _buffer, _bufferRear, srcDataSize);
                _bufferRear += srcDataSize;
            }
            else
            {
                int rearRemainSize = BufferEnd - _bufferRear;
                Array.Copy(srcData, 0, _buffer, _bufferRear, rearRemainSize);
                _bufferRear = 0;

                int remainDataSize = srcDataSize - rearRemainSize;
                Array.Copy(srcData, rearRemainSize, _buffer, _bufferRear, remainDataSize);
                _bufferRear += remainDataSize;
            }

            UseSize += srcDataSize;
            return true;
        }

        public byte[] Dequeue(int size)
        {
            if (size > UseSize)
                return null;

            var data = new byte[size];
            if (size <= BufferEnd - _bufferFront)
            {
                Array.Copy(_buffer, _bufferFront, data, 0, size);
                _bufferFront += size;
            }
            else
            {
                int frontDataSize = BufferEnd - _bufferFront;
                Array.Copy(_buffer, _bufferFront, data, 0, frontDataSize);
                _bufferFront = 0;

                int remainDataSize = size - frontDataSize;
                Array.Copy(_buffer, _bufferFront, data, frontDataSize, remainDataSize);
                _bufferFront += remainDataSize;
            }

            UseSize -= size;
            return data;
        }

        public byte[] Peek(int size)
        {
            if (size > UseSize)
                return null;

            var data = new byte[size];
            if (size <= BufferEnd - _bufferFront)
            {
                Array.Copy(_buffer, _bufferFront, data, 0, size);
            }
            else
            {
                int frontDataSize = BufferEnd - _bufferFront;
                Array.Copy(_buffer, _bufferFront, data, 0, frontDataSize);

                int remainDataSize = size - frontDataSize;
                Array.Copy(_buffer, 0, data, frontDataSize, remainDataSize);
            }

            return data;
        }

        public void OnReceiveCompleted(int bytesTransferred)
        {
            UseSize += bytesTransferred;
            _bufferRear += bytesTransferred;

            if (_bufferRear == BufferEnd)
                _bufferRear = 0;
        }

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

        private int GetDirectEnqueueSize()
        {
            if (_bufferFront <= _bufferRear)
                return BufferEnd - _bufferRear;
            else
                return _bufferFront - _bufferRear;
        }

        private int GetDirectDequeueSize()
        {
            if (_bufferFront <= _bufferRear)
                return _bufferRear - _bufferFront;
            else
                return BufferEnd - _bufferFront;
        }
    }
}
