namespace WebApplication13.Models
{
    public class Message
    {
        public int Id { get; set; }
        public int ChatId { get; set; } // Чат, к которому относится сообщение
        public int SenderId { get; set; } // Идентификатор отправителя
        public int ReceiverId { get; set; } // Идентификатор получателя
        public string Content { get; set; } // Содержимое сообщения
        public DateTime Timestamp { get; set; } // Дата и время отправки

        public User Sender { get; set; } // Ссылка на пользователя-отправителя
        public User Receiver { get; set; } // Ссылка на пользователя-получателя
        public Chat Chat { get; set; } // Связь с чатом
    }



}
