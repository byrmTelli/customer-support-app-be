namespace customer_support_app.CORE.RequestModels.Notification
{
    public class CreateCommentNotificationRM
    {
        public int TicketId { get; set; }
        public string Message { get; set; }
    }
}