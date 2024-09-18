using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebAPIwithMongoDB.Entities
{
    public interface IMongoEntity
    {
        string Id { get; set; }
    }
}