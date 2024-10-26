using MongoDB.Bson;

namespace Zealot.server
{
    public sealed class DB
    {
        /// <summary>
        /// Имя базы данных сервера.
        /// </summary> <summary>
        private readonly string _dbName;

        /// <summary>
        /// Имя коллекции в базе данных где хранятся клиенты.
        /// </summary> 
        private readonly string _collectionName;

        private readonly IServer _server;

        public DB(IServer server, string dbName, string collectionName)
        {
            this._server = server;

            _dbName = dbName;
            _collectionName = collectionName;
        }

        /// <summary>
        /// Инициализирует уже имеющихся клиентов.
        /// </summary>
        public bool Initilize()
        {
            // Проверяем наличие базы данных, если что создаем ее.
            if (MongoDB.ContainsDatabase(_dbName, out string containsDBerror))
            {
                Logger.I.To(_server, $"База данныx {_dbName} уже создана.");
            }
            else
            {
                if (containsDBerror != "")
                {
                    Logger.I.To(_server, $"База данныx {_dbName} уже создана.");
                }
                else
                {
                    Logger.I.To(_server, $"Создаем базу данных {_dbName}.");

                    if (MongoDB.TryCreatingDatabase(_dbName, out string infoR))
                    {
                        Logger.I.To(_server, infoR);
                    }
                    else
                    {
                        Logger.S_E.To(_server, infoR);

                        _server.Destroy();

                        return false;
                    }
                }
            }

            // Проверяем наличие коллекции, если что создаем ее.
            if (MongoDB.ContainsCollection<BsonDocument>(_dbName, _collectionName, out string error))
            {
                Logger.I.To(_server, $"Коллекция [{_collectionName}] в базе данных [{_dbName}] уже создана.");
            }
            else
            {
                // Коллекции нету, создадим ее.
                if (error == "")
                {
                    if (MongoDB.TryCreatingCollection(_dbName, _collectionName, out string infoI))
                    {
                        Logger.I.To(_server, infoI);
                    }
                    else
                    {
                        Logger.S_E.To(_server, infoI);

                        _server.Destroy();

                        return false;
                    }
                }
                else
                {
                    Logger.S_E.To(_server, error);

                    _server.Destroy();

                    return false;
                }
            }
            // Проверяем наличие документа хранящего адреса и диопазоны адресов.
            if (MongoDB.TryFind(_dbName, _collectionName, out string findInfo, out List<BsonDocument> clients))
            {
                Logger.I.To(_server, findInfo);

                if (clients != null)
                {
                    try
                    {
                        foreach (BsonDocument doc in clients)
                        {
                            _server.AddClientData(new ClientData()
                            {
                                Login = doc["Login"].ToString(),
                                Password = doc["Password"].ToString()
                            });
                        }

                        return true;
                    }
                    catch (Exception ex)
                    {
                        Logger.S_E.To(_server, ex.ToString());

                        _server.Destroy();

                        return false;
                    }
                }
                else
                {
                    Logger.S_E.To(_server, findInfo);

                    _server.Destroy();

                    return false;
                }
            }
            else
            {
                Logger.S_E.To(_server, findInfo);

                _server.Destroy();

                return false;
            }
        }
    }
}