using DataLayer.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace DataLayer.DataContext
{
    public class ContactContext : DbContext
    {
        private ContactContext _contactContext;

        public DbSet<Contact> Contacts { get; set; }
        public ContactContext(DbContextOptions<ContactContext> connectionString) : base(connectionString)
        {
            Database.EnsureCreated();
        }

        public ContactContext(ContactContext employeeContext)
        {
            this._contactContext = employeeContext;
        }
    }
}
