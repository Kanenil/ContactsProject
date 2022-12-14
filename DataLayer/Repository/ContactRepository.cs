using DataLayer.DataContext;
using DataLayer.Interfaces;
using DataLayer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataLayer.Repository
{
    public class ContactRepository : IRepository<Contact>
    {
        ContactContext _contactContext;
        public ContactRepository(ContactContext context)
        {
            _contactContext = context;
        }
        public async Task Create(Contact item)
        {
            if (item != null)
            {
                await _contactContext.Contacts.AddAsync(item);
            }
        }

        public void Delete(Contact item)
        {
            _contactContext.Contacts.Remove(_contactContext.Contacts.First(e => e.Name == item.Name && e.Surname == item.Surname && e.Patronymic == item.Patronymic && e.Phone == item.Phone && e.Email == item.Email));
        }

        public async Task<Contact> Find(Contact item)
        {
            var employee = await _contactContext.Contacts.FindAsync(item);
            if (employee != null)
                return employee;
            throw new InvalidOperationException();
        }

        public IEnumerable<Contact> GetAll()
        {
            return _contactContext.Contacts;
        }

        public void Update(Contact item)
        {
            if (item != null)
            {
                var newItem = _contactContext.Contacts.Where(x => x.Id == item.Id).First();
                newItem.Surname = item.Surname;
                newItem.Name = item.Name;
                newItem.Patronymic = item.Patronymic;
                newItem.Phone = item.Phone;
                newItem.Email = item.Email;
            }
        }
        public int GetId(Contact item)
        {
            var tempItem = _contactContext.Contacts.Where(e => e.Name == item.Name && e.Surname == item.Surname && e.Patronymic == item.Patronymic && e.Phone == item.Phone && e.Email == item.Email).First();
            return tempItem.Id;
        }
    }
}
