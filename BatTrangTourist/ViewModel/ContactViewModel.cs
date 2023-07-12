using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using System.Xml.Linq;
using BatTrangTourist.Models;

namespace BatTrangTourist.ViewModel
{
    public class ListContactViewModel
    {
        public PagedList.IPagedList<Contact> Contacts { get; set; }
        public string Name { get; set; }
    }

    public class ListSubcribeViewModel
    {
        public PagedList.IPagedList<Subcribe> Subcribes { get; set; }
        public string Name { get; set; }
    }

    public class ListFeedbackViewModel
    {
        public PagedList.IPagedList<Feedback> Feedbacks { get; set; }
        public string Name { get; set; }
    }
}