using System.Collections.Concurrent;

namespace GameWindowsServer
{
    internal class ServerManager: Singleton<ServerManager>
    {
        //还有消息队列和监听器队列
        private ConcurrentDictionary<string,SocketConnect> _dicConnectedClient = new ConcurrentDictionary<string, SocketConnect>();
    }
}
