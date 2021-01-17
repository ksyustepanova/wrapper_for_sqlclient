using System.Data;

namespace wrapper_for_sqlclient.data.Sql
{
    public struct CmdSettings
    {
        public string ConnectionString { get; internal set; }
        public string Query { get; internal set; }
        public int Timeout { get; internal set; }
        public CommandType Type { get; internal set; }
    }
}