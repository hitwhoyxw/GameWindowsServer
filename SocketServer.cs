using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace GameWindowsServer
{
    internal class SocketServerEvent
    {
        public static readonly string CLIENT_CONNECT = "clientConnected";
        public static readonly string CLIENT_DISCONNECT = "clientDisconnected";
    }
    /// <summary>
    /// 服务器套接字
    /// </summary>
    internal class SocketServer : EventDispacth, IDisposable
    {

        private int Port
        {
            set
            {
                if (value < 0 || value > 65535)
                {
                    throw new ArgumentOutOfRangeException("Port", "端口号必须在0-65535之间");
                }
                Port = value;
            }
            get
            {
                return Port;
            }
        }
        private long Ip
        {
            set
            {
                if (value < 0 || value > 4294967295)
                {
                    throw new ArgumentOutOfRangeException("Ip", "IP地址必须在0-4294967295之间");
                }
                Ip = value;
            }
            get
            {
                return Ip;
            }
        }
        private Socket _socket;
        private SocketAsyncEventArgs _socketAsyncEventArgs;
        private bool disposedValue;

        public SocketServer(long ip, int port)
        {
            Ip = ip;
            Port = port;
            _socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            _socketAsyncEventArgs = new SocketAsyncEventArgs();
        }
        public void Start()
        {
            IPEndPoint iPEndPoint = new IPEndPoint(Ip, Port);
            _socket.Bind(iPEndPoint);
            _socket.Listen(10);
            _socketAsyncEventArgs.Completed += new EventHandler<SocketAsyncEventArgs>(AcceptAsyncCallback);
            if (!_socket.AcceptAsync(_socketAsyncEventArgs))
            {
                HandleAccept(_socketAsyncEventArgs);
            }
        }
        public void Stop()
        {
            //确保先调用shutdown，会发送完数据，再调用close，释放资源
            try
            {
                _socket.Shutdown(SocketShutdown.Both);
            }
            finally
            {
                _socket.Close();
            }
        }

        private void AcceptAsyncCallback(object sender, SocketAsyncEventArgs e)
        {
            HandleAccept(e);
        }
        private void HandleAccept(SocketAsyncEventArgs e)
        {
            var connectSocket = e.AcceptSocket;
            Console.WriteLine($"客户端连接成功 {connectSocket?.RemoteEndPoint}");
            if (connectSocket != null)
            {
                SocketConnect socketConnect = new SocketConnect(connectSocket);
                DispatchEvent(SocketServerEvent.CLIENT_CONNECT, new EventParam() { sender = this, param = socketConnect, type = string.Empty });
            }
            e.AcceptSocket = null;
            if (!_socket.AcceptAsync(e))
            {
                HandleAccept(e);
            }
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: 释放托管状态(托管对象)
                }
                _socket.Dispose();
                _socketAsyncEventArgs.Dispose();
                disposedValue = true;
            }
        }

        // TODO: 仅当“Dispose(bool disposing)”拥有用于释放未托管资源的代码时才替代终结器
        ~SocketServer()
        {
            // 不要更改此代码。请将清理代码放入“Dispose(bool disposing)”方法中
            Dispose(disposing: false);
        }

        void IDisposable.Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}
