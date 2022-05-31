using Entities.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.DataTransferObjects
{
    public class EmployeeEntity
    {
        public EmployeeEntity()
        {
            Entity = new Entity();
        }
        public Guid Id { get; set; }
        public Entity Entity { get; set; }
    }
}
