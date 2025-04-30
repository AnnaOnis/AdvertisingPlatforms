using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdvertisingPlatforms.Domain.Interfaces
{
    /// <summary>
    /// Interface for all entities with unique identifier
    /// </summary>
    public interface IEntity
    {
        Guid Id { get; init; }
    }
}
