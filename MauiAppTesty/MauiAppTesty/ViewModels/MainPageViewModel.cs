using MauiAppTesty.BaseView;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MauiAppTesty.ViewModels
{
    internal sealed class MailPageViewModel : BaseViewModel
    {
        public string Sender { get; set; }
        public string Receiver { get; set; }
        public string Subject { get; set; }
    }
}
