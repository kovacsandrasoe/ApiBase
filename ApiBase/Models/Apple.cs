using ApiBase.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ApiBase.Models
{
    public class Apple : IEntity<Apple>
    {
        public string Id { get; set; }
        public string OwnerId { get; set; }

        public string AppleName { get; set; }

        public void CopyFrom(Apple another)
        {
            OwnerId = another.OwnerId;
            AppleName = another.AppleName;
            Id = Guid.NewGuid().ToString();
        }

        public Apple()
        {
            Id = Guid.NewGuid().ToString();
        }
    }
}
