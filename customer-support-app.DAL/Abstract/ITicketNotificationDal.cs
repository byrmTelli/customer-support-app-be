﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace customer_support_app.DAL.Abstract
{
    public interface ITicketNotificationDal
    {
        Task CreateTicketNotificationAsync(int ticketId,string message);
    }
}
