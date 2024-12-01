using customer_support_app.CORE.ViewModels.Category;
using customer_support_app.CORE.ViewModels.Ticket;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace customer_support_app.CORE.ViewModels.Admin.CategoriesPage
{
    public class CategoriesPageViewModel
    {
        public CategoryViewModel? Category { get; set; }
        public int CompletedTicketCount { get; set; }
        public int PendingTicketCount { get; set; }
        public int WaitingTicketCount { get; set; }
        public int CancelledTicketCount { get; set; }
        public List<TicketViewModel>? Tickets { get; set; }
    }
}
