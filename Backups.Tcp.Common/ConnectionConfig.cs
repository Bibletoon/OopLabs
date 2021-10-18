namespace Backups.TcpServer.Common
{
    public class ConnectionConfig
    {
        public ConnectionConfig(string ip, int port)
        {
            Ip = ip;
            Port = port;
        }

        public string Ip { get; }
        public int Port { get; }
    }
}