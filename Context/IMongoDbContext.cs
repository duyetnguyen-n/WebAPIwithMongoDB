using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MongoDB.Driver;

namespace WebAPIwithMongoDB.Context
{
    public interface IMongoDbContext
    {
        IMongoCollection<T> GetCollection<T>(string name);
    }
}