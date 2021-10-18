using Backups.Server;
using Backups.TcpServer.Common;

var config =  new ConnectionConfig("127.0.0.1", 8080);

new TcpServer(config).Run();