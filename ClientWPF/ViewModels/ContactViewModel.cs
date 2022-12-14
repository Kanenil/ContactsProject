using BusinnesLogicLayer.Interfaces;
using BusinnesLogicLayer.ModelsDTO;
using ClientWPF.ViewModels.Base;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace ClientWPF.ViewModels
{
    public class ContactViewModel : ViewModel
    {
        public readonly IService<ContactDTO> _service;
        public ObservableCollection<ContactDTO> Contacts { get; set; }
        private List<ContactDTO> AllContacts { get; set; }
        public ContactViewModel(IService<ContactDTO> service)
        {
            _service = service;
            AllContacts = new List<ContactDTO>(service.GetContacts());
            Contacts = new ObservableCollection<ContactDTO>();
            CopyContacts();
        }
        private void CopyContacts()
        {
            Contacts.Clear();
            foreach (var item in AllContacts)
            {
                Contacts.Add(item);
            }
        }
    }
}
