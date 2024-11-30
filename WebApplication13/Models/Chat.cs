namespace WebApplication13.Models
{
    public class Chat
    {
        public int Id { get; set; }
        public int User1Id { get; set; } // ID первого пользователя
        public int User2Id { get; set; } // ID второго пользователя

        // Навигационные свойства
        public User User1 { get; set; } // Ссылка на первого пользователя
        public User User2 { get; set; } // Ссылка на второго пользователя
        public ICollection<Message> Messages { get; set; } // Связанные сообщения
    }


}
