namespace WebApplication13.Models
{
    public class UploadedFile
    {
        public int Id { get; set; } // Уникальный идентификатор файла
        public int ChatId { get; set; } // Идентификатор чата
        public string FileName { get; set; } // Имя файла
        public string FilePath { get; set; } // Путь к файлу
        public long Size { get; set; } // Размер файла в байтах
        public DateTime UploadedAt { get; set; } // Время загрузки
    }


}
