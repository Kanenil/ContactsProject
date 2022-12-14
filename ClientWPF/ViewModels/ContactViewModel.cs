using BusinnesLogicLayer.Interfaces;
using BusinnesLogicLayer.ModelsDTO;
using ClientWPF.Commands;
using ClientWPF.ViewModels.Base;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Windows.Input;
using System.Windows;
using ClientWPF.Views;
using System.Windows.Controls;
using System.Windows.Media;
using System.Text.RegularExpressions;
using System.Linq;
using Microsoft.Win32;
using ClientWPF.Models;
using SendEmailAndSMSConsole;
using MimeKit;
using System.IO;
using System.Threading;
using System.Windows.Threading;
using System.Threading.Tasks;

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
        ///////////////////////////Open Window
        static AddContactWindow _window;
        private ICommand _OpenWindow;
        public ICommand CreateOpenWindow => _OpenWindow ??= new LambdaCommand(OnOpenWindowExecude, CanOpenWindowExecude);
        private static bool CanOpenWindowExecude(object p) => _window == null;
        public void OnOpenWindowExecude(object p)
        {
            WindowParametrs(p);
            var window = new AddContactWindow(this)
            {
                Owner = Application.Current.MainWindow
            };
            _window = window;
            window.Closed += OnWindowClosed;
            window.ShowDialog();
        }
        private void OnWindowClosed(object sender, EventArgs e)
        {
            ((Window)sender).Closed -= OnWindowClosed;
            _window = null;
        }
        ///////////////////////////////Window parametr
        private void WindowParametrs(object p)
        {
            if (p != null)
            {
                ContactView = (ContactDTO)p;
                ButtonContent = "Edit";
                SurnameView = ContactView.Surname;
                NameView = ContactView.Name;
                PatronymicView = ContactView.Patronymic;
                PhoneView = ContactView.Phone;
                EmailView = ContactView.Email;
            }
            else
            {
                ButtonContent = "Add";
            }
        }
        ///////////////////////////////Close Window
        private ICommand _CloseApplication;
        public ICommand CloseApplicationCommand => _CloseApplication ??= new LambdaCommand(OnCloseApplicationCommandExecuted, CanCloseApplicationCommandExecute);
        private bool CanCloseApplicationCommandExecute(object p) => true;
        private void OnCloseApplicationCommandExecuted(object p)
        {
            _window.Close();
            ItemNull();
        }
        ///////////////////////////////Add new contact and Edit item
        public string ButtonContent { get; set; }
        public ContactDTO ContactView;
        public string SurnameView { get; set; }
        public string NameView { get; set; }
        public string PatronymicView { get; set; }
        public string PhoneView { get; set; }
        public string EmailView { get; set; }

        private RelayCommand _AddContact;
        public RelayCommand CreateNewContactCommand
        {
            get
            {
                return _AddContact ?? new RelayCommand(obj =>
                {
                    Window wnd = obj as Window;
                    if (!ErrorItem(wnd))
                    {
                        if (ButtonContent == "Edit")
                            SaveEditContact();
                        else
                            SaveAddContact();
                        ItemNull();
                        wnd.Close();
                    }
                }
                );
            }
        }
        private void SetRedBlockControll(Window wnd, string blockName)
        {
            Control block = wnd.FindName(blockName) as Control;
            block.BorderBrush = Brushes.Red;
        }
        private bool ErrorItem(Window wnd)
        {
            bool errorItem = false;
            if (SurnameView == null || SurnameView.Replace(" ", "").Length == 0)
            {
                SetRedBlockControll(wnd, "Surname");
                errorItem = true;
            }
            if (NameView == null || NameView.Replace(" ", "").Length == 0)
            {
                SetRedBlockControll(wnd, "Name");
                errorItem = true;
            }
            if (PatronymicView == null || PatronymicView.Replace(" ", "").Length == 0)
            {
                SetRedBlockControll(wnd, "Patronymic");
                errorItem = true;
            }
            if (PhoneView == null || PhoneView.Replace(" ", "").Length == 0 || !Regex.IsMatch(PhoneView, "^\\+?\\d{1,4}?[-.\\s]?\\(?\\d{1,3}?\\)?[-.\\s]?\\d{1,4}[-.\\s]?\\d{1,4}[-.\\s]?\\d{1,9}$"))
            {
                SetRedBlockControll(wnd, "Phone");
                errorItem = true;
            }
            if (EmailView == null || EmailView.Replace(" ", "").Length == 0 || !Regex.IsMatch(EmailView, @"^([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)$"))
            {
                SetRedBlockControll(wnd, "Email");
                errorItem = true;
            }
            return errorItem;
        }
        private bool ErrorMessage(Window wnd)
        {
            bool errorItem = false;
            if (From == null || From.Replace(" ", "").Length == 0)
            {
                SetRedBlockControll(wnd, "From");
                errorItem = true;
            }
            if (To == null || To.Replace(" ", "").Length == 0)
            {
                SetRedBlockControll(wnd, "To");
                errorItem = true;
            }
            if (Subject == null || Subject.Replace(" ", "").Length == 0)
            {
                SetRedBlockControll(wnd, "Subject");
                errorItem = true;
            }
            if (Body == null || Body.Replace(" ", "").Length == 0)
            {
                SetRedBlockControll(wnd, "Phone");
                errorItem = true;
            }
            return errorItem;
        }
        private void SaveAddContact()
        {
            ContactView = new ContactDTO();
            SaveContactView();
            _service.AddItemAsync(ContactView);
            AllContacts.Add(ContactView);
            CopyContacts();
        }
        private void SaveEditContact()
        {
            if (ContactView.Id == 0)
            {
                var temp = AllContacts.First(e => e.Surname == ContactView.Surname && e.Name == ContactView.Name && e.Patronymic == ContactView.Patronymic && e.Phone == ContactView.Phone && e.Email == ContactView.Email);
                int i = AllContacts.IndexOf(temp);
                ContactView.Id = _service.GetIdDTO(AllContacts[i]);
                AllContacts[i] = ContactView;
                SaveContactView();
            }
            else
            {
                SaveContactView();
                var temp = AllContacts.First(e => e.Id == ContactView.Id);
                int i = AllContacts.IndexOf(temp);
                AllContacts[i] = ContactView;
            }
            _service.UpdateDTO(ContactView);
            CopyContacts();
        }
        private void SaveContactView()
        {
            ContactView.Surname = SurnameView;
            ContactView.Name = NameView;
            ContactView.Patronymic = PatronymicView;
            ContactView.Phone = PhoneView;
            ContactView.Email = EmailView;
        }
        ///////////////////////////////refresh item
        private void ItemNull()
        {
            ContactView = null;
            SurnameView = null;
            NameView = null;
            PatronymicView = null;
            PhoneView = null;
            EmailView = null;
        }
        private void MessageNull()
        {
            ContactView = null;
            From = null;
            To = null;
            Subject = null;
            Body = null;
            Attachments = null;
        }
        ///////////////////////////////Find item
        private string searchString = string.Empty;
        public string SearchString
        {
            get
            {
                return searchString;
            }
            set
            {
                if (searchString == value)
                    return;
                
                searchString = value;
                CopyContacts();
                foreach (var item in AllContacts)
                {
                    if (!item.Surname.ToString().ToLower().Contains(value.ToLower()) &&
                        !item.Name.ToString().ToLower().Contains(value.ToLower()) &&
                        !item.Patronymic.ToString().ToLower().Contains(value.ToLower()) &&
                        !item.Phone.ToString().ToLower().Contains(value.ToLower()) &&
                        !item.Email.ToString().ToLower().Contains(value.ToLower()))
                    {
                        Contacts.Remove(item);
                    }
                }
            }
        }
        //////////////////////////////Delete item
        private RelayCommand _DeleteContact;
        public RelayCommand DeleteContact
        {
            get
            {
                return _DeleteContact ?? (_DeleteContact =
                    new RelayCommand(obj =>
                    {
                        if (MessageBoxQuestion("Delete this contact?", "Delete Form Closing") == MessageBoxResult.Yes)
                        {
                            _service.DeleteDTO((ContactDTO)obj);
                            AllContacts.Remove((ContactDTO)obj);
                            CopyContacts();
                        }
                    }));
            }
        }
        private MessageBoxResult MessageBoxQuestion(string message, string caption)
        {
            var result = MessageBox.Show(message, caption, MessageBoxButton.YesNo);
            return result;
        }
        //////////////////////////////Send message window
        static MessageWindow _windowMessage;
        private ICommand _OpenMessageWindow;
        public ICommand MessageOpenWindow => _OpenMessageWindow ??= new LambdaCommand(OnOpenMessageWindowExecude, CanOpenMessageWindowExecude);
        private static bool CanOpenMessageWindowExecude(object p) => _windowMessage == null;
        public void OnOpenMessageWindowExecude(object p)
        {
            MessageWindowParametrs(p);
            var window = new MessageWindow(this)
            {
                Owner = Application.Current.MainWindow
            };
            _windowMessage = window;
            window.Closed += OnMessageWindowClosed;
            window.ShowDialog();
        }
        private void OnMessageWindowClosed(object sender, EventArgs e)
        {
            ((Window)sender).Closed -= OnMessageWindowClosed;
            _windowMessage = null;
        }
        ///////////////////////////////Message Window parametr
        private void MessageWindowParametrs(object p)
        {
            ContactView = (ContactDTO)p;
            To = ContactView.Email;
            From = "oleksandr.burda@ukr.net";
        }
        ///////////////////////////////Close Message Window
        private ICommand _CloseMessage;
        public ICommand CloseMessage => _CloseMessage ??= new LambdaCommand(OnCloseMessageCommandExecuted, CanCloseMessageCommandExecute);
        private bool CanCloseMessageCommandExecute(object p) => true;
        private void OnCloseMessageCommandExecuted(object p)
        {
            _windowMessage.Close();
            MessageNull();
        }
        ///////////////////////////////message item
        public string Subject { get; set; }
        public string Body { get; set; }
        private List<AttachmentText> _attachments;
        public List<AttachmentText> Attachments { get => _attachments; set { _attachments = value; OnPropertyChanged(); } }
        public string To { get; set; }
        public string From { get; set; }
        ///////////////////////////////add Attachment
        private RelayCommand _addAttachment;
        public RelayCommand AddAttachment
        {
            get
            {
                return _addAttachment ?? (_addAttachment =
                    new RelayCommand(obj =>
                    {
                        OpenFileDialog openFile = new OpenFileDialog();
                        openFile.Filter = "All files (*.*) | *.*";
                        openFile.Multiselect = true;
                        if(openFile.ShowDialog(_windowMessage) != false)
                        {
                            var attachments = new List<AttachmentText>();
                            foreach (var item in openFile.FileNames)
                                attachments.Add(new AttachmentText() { Text = item});
                            Attachments = attachments;
                        }
                    }));
            }
        }
        ///////////////////////////////send message
        private async void SendMessage(EmailLogicLayer.Message message, List<string> attachments)
        {
            SmtpEmailService emailService = new SmtpEmailService();
            await emailService.Send(message, attachments);
        }
        private RelayCommand _sendMessage;
        public RelayCommand SendMessageCommand
        {
            get
            {
                return _sendMessage ?? new RelayCommand(obj =>
                {
                    Window wnd = obj as Window;
                    if (!ErrorMessage(wnd))
                    {
                        var message = new EmailLogicLayer.Message()
                        {
                            Body = Body,
                            To = To,
                           Subject = Subject
                        };
                        var attachments = new List<string>();
                        if (Attachments != null)
                            foreach (var item in Attachments)
                                attachments.Add(item.Text);


                        Task.WaitAll(Task.Run(()=>SendMessage(message,attachments)));

                        MessageNull();
                        wnd.Close();
                    }
                }
                );
            }
        }
        //////////////////////////////Delete attachment
        private RelayCommand _DeleteAttachment;
        public RelayCommand DeleteAttachment
        {
            get
            {
                return _DeleteAttachment ?? (_DeleteAttachment =
                    new RelayCommand(obj =>
                    {
                        if (MessageBoxQuestion("Delete this attachment?", "Delete Form Closing") == MessageBoxResult.Yes)
                        {
                            _attachments.Remove((AttachmentText)obj);
                            Attachments = _attachments;
                        }
                    }));
            }
        }
    }
}
