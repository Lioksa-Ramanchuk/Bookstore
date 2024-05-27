using LibApp.Models;
using System;
using System.Net;
using System.Xml.Linq;

namespace LibApp.ViewModels.SubViewModels
{
    public class OrderVM : BaseVM
    {
        readonly Order _order;

        public long Id
        {
            get => _order.Id;
            set { _order.Id = value; OnPropertyChanged(nameof(Id)); }
        }
        public long IdClient
        {
            get => _order.IdClient;
            set { _order.IdClient = value; OnPropertyChanged(nameof(IdClient)); }
        }
        public DateTime Date
        {
            get => _order.Date;
            set { _order.Date = value; OnPropertyChanged(nameof(Date)); }
        }
        public string Name
        {
            get => _order.Name;
            set { _order.Name = value; OnPropertyChanged(nameof(Name)); }
        }
        public string Email
        {
            get => _order.Email;
            set { _order.Email = value; OnPropertyChanged(nameof(Email)); }
        }
        public string Address
        {
            get => _order.Address;
            set { _order.Address = value; OnPropertyChanged(nameof(Address)); }
        }
        public string Comment
        {
            get => _order.Comment;
            set { _order.Comment = value; OnPropertyChanged(nameof(Comment)); }
        }
        public short IdStatus
        {
            get => _order.IdStatus;
        }
        public OrderStatus IdStatusNavigation
        {
            get => _order.IdStatusNavigation;
            set
            {
                _order.IdStatusNavigation = value;
                _order.IdStatus = value?.Id ?? 0;
                OnPropertyChanged(nameof(IdStatusNavigation));
            }
        }

        public OrderVM(Order o)
        {
            _order = o;
        }
    }
}
