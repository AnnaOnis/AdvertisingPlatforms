using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdvertisingPlatforms.Domain.Interfaces
{
    /// <summary>
    /// Интерфейс для всех сущностей с уникальным идентификатором.
    /// </summary>
    public interface IEntity
    {
        Guid Id { get; init; }
    }
}
