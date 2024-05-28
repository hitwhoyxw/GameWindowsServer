using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace GameWindowsServer
{
    public class SocketConnectEvent
    {
        public static readonly string RECEIVE_DATA = "receiveData";
        public static readonly string SEND_DATA = "sendData";
    }
    internal class SocketConnect : EventDispacth, IDisposable
    {
        private readonly string TAG = "SocketConnect";
        private readonly int READ_BUFFER_SIZE = 1024;
        private readonly Socket _socket;
        private readonly SocketAsyncEventArgs _readEventArgs;
        /// <summary>
        ///  需要多个发送参数，用池管理
        /// </summary>
        private readonly SocketAsyncEventArgs _writeEventArgs;
        private byte[] readBuffer;
        private bool disposedValue;

        public SocketConnect(Socket socket)
        {
            _socket = socket;
            _readEventArgs.SetBuffer(readBuffer,0, READ_BUFFER_SIZE);
            _readEventArgs.Completed += HandleRevieve;
            _readEventArgs = new SocketAsyncEventArgs();
            _writeEventArgs = new SocketAsyncEventArgs();
            readBuffer = new byte[READ_BUFFER_SIZE];

        }
        public void Dispose()
        {
            // 不要更改此代码。请将清理代码放入“Dispose(bool disposing)”方法中
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
        public void SendTo(byte[] data)
        {
            try
            {
                _writeEventArgs.SetBuffer(data, 0, data.Length);
                _socket.SendAsync(_writeEventArgs);
            }
            catch (Exception e)
            {
                Console.WriteLine(TAG + e.Message);
            }
        }
        public void Close()
        {
            try
            {
                _socket.Shutdown(SocketShutdown.Both);
            }
            catch (Exception)
            {
                // ignored
            }
            finally
            {
                Dispose(true);
            }
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    readBuffer = null;
                }
                _socket.Dispose();
                _readEventArgs.Dispose();
                disposedValue = true;
            }
        }
        private void HandleRevieve(object sender, SocketAsyncEventArgs e)
        {
            if (e == null|| e.SocketError != SocketError.Success)
            {
                return;
            }
            int bytesRead = e.BytesTransferred;
            //发空包一般是退出或者断开连接的包
            if (bytesRead > 0)
            {
                //TODO:处理数据
                DispatchEvent(SocketConnectEvent.RECEIVE_DATA, new EventParam() { sender = this, param = null, type = string.Empty });
            }
        }
        // TODO: 仅当“Dispose(bool disposing)”拥有用于释放未托管资源的代码时才替代终结器
        ~SocketConnect()
        {
            // 不要更改此代码。请将清理代码放入“Dispose(bool disposing)”方法中
            Dispose(disposing: false);
        }


    }
}
