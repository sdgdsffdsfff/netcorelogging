using MongoDB.Driver;


namespace Logging.Server.DB
{
    //server=127.0.0.1:27017,127.0.0.1:27017;SafeMode=false
    public class MongoDataBase
    {
        public static string DatabaseName = Config.LogMongodbDBName ?? "Logs";
        //private static MongoServer _server;

        public static MongoClient _client;


        // private static string Mongo = Config.MongodbHost; //System.Configuration.ConfigurationManager.AppSettings["Mongo"];

        //internal static MongoServer GetServer()
        //{
        //    return _server ?? (_server = new MongoClient(Mongo).GetServer());
        //}

        public static MongoClient GetClient()
        {
            if (_client == null)
            {
                // var mongo = System.Configuration.ConfigurationManager.AppSettings["Mongo"];
                _client = new MongoClient(Config.MongodbHost);
                //_client.Settings.WriteConcern = WriteConcern.Acknowledged;
                //_client.Settings.Freeze();
            }
            return _client;
        }

        internal static IMongoDatabase GetDataBase()
        {
            return GetClient().GetDatabase(DatabaseName);
        }

        public static IMongoCollection<T> GetCollection<T>()
        {
            return GetDataBase().GetCollection<T>(typeof(T).Name);
        }

        public static void DropCollectionAsync<T>()
        {
            GetDataBase().DropCollectionAsync(typeof(T).Name);
        }


    }
}