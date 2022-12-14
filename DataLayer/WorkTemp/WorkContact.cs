using DataLayer.DataContext;
using DataLayer.Interfaces;
using DataLayer.Models;
using DataLayer.Repository;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace DataLayer.WorkTemp
{
    public class WorkContact : IWorkContact
    {
        private ContactContext _contactContext;
        private ContactRepository _contactRepository;
        public WorkContact(DbContextOptions<ContactContext> connectionString)
        {
            _contactContext = new ContactContext(connectionString);
        }
        public IRepository<Contact> Contacts { get { return _contactRepository = new ContactRepository(_contactContext); } }

        public void Dispose()
        {
            throw new NotImplementedException();
        }

        public async Task Save()
        {
            await _contactContext.SaveChangesAsync();
        }
    }
}
