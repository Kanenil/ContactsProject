using AutoMapper;
using BusinnesLogicLayer.Interfaces;
using BusinnesLogicLayer.ModelsDTO;
using DataLayer.Interfaces;
using DataLayer.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace BusinnesLogicLayer.Services
{
    public class ContactService : IService<ContactDTO>
    {
        private readonly IWorkContact _contactDB;
        public ContactService(IWorkContact ContactDB)
        {
            this._contactDB = ContactDB;
        }
        public async Task AddItemAsync(object item)
        {
            if (item is ContactDTO)
                await AddContact((ContactDTO)item);
        }
        private async Task AddContact(ContactDTO item)
        {
            var contact = new Contact
            {
                Surname = item.Surname,
                Name = item.Name,
                Patronymic = item.Patronymic,
                Phone = item.Phone,
                Email = item.Email
            };

            await _contactDB.Contacts.Create(contact);
            await _contactDB.Save();
        }
        public IEnumerable<ContactDTO> GetContacts()
        {
            var firstobj = new MapperConfiguration(map => map.CreateMap<Contact, ContactDTO>()).CreateMapper();
            IEnumerable<ContactDTO> list = firstobj.Map<IEnumerable<Contact>, IEnumerable<ContactDTO>>(_contactDB.Contacts.GetAll());
            return list;
        }

        public async Task<ContactDTO> FindDTO(ContactDTO item)
        {
            var secondtobj = new MapperConfiguration(map => map.CreateMap<Contact, ContactDTO>()).CreateMapper();
            ContactDTO employeeDTO = secondtobj.Map<Contact, ContactDTO>(await _contactDB.Contacts.Find(MappingModels(item)));
            return employeeDTO;
        }

        public async Task UpdateDTO(ContactDTO item)
        {
            _contactDB.Contacts.Update(MappingModels(item));
            await _contactDB.Save();
        }

        public async Task DeleteDTO(ContactDTO item)
        {
            _contactDB.Contacts.Delete(MappingModels(item));
            await _contactDB.Save();
        }

        public async Task AddList(IEnumerable<ContactDTO> templist)
        {
            if (templist != null)
            {
                foreach (var item in templist)
                {
                    await AddContact(item);
                }
            }
        }
        public int GetIdDTO(ContactDTO item)
        {
            return _contactDB.Contacts.GetId(MappingModels(item));
        }
        private Contact MappingModels(ContactDTO contactDTO)
        {
            var firstobj = new MapperConfiguration(map => map.CreateMap<ContactDTO, Contact>()).CreateMapper();
            Contact contact = firstobj.Map<ContactDTO, Contact>(contactDTO);
            return contact;
        }
    }
}
