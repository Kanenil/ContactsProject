using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace BusinnesLogicLayer.Interfaces
{
    public interface IService<T>
    {
        IEnumerable<T> GetContacts();
        Task AddItemAsync(object item);
        Task<T> FindDTO(T id);
        Task UpdateDTO(T item);
        Task DeleteDTO(T id);
        Task AddList(IEnumerable<T> templist);
        int GetIdDTO(T item);
    }
}
