using DataLayer.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace DataLayer.Interfaces
{
    public interface IWorkContact : IWork
    {
        public IRepository<Contact> Contacts { get; }
    }
}
