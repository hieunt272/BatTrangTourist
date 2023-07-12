using BatTrangTourist.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BatTrangTourist.ViewModel
{
    public class UserOrderViewModel
    {
        public IEnumerable<Order> Orders { get; set; }

    }
}