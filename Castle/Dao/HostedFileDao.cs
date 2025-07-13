using Castle.Models.HostedFile;
using SQLite;

namespace Castle.Dao
{
    [Table("hostedfiles")]
    public sealed class HostedFileDao
    {
        [PrimaryKey, Column("id")]
        public string Id { get; set; } = string.Empty;

        [Column("commander")]
        public string Commander { get; set; } = string.Empty;

        [Column("uri")]
        public string Uri { get; set; } = string.Empty;

        [Column("filename")]
        public string Filename { get; set; } = string.Empty;

        [Column("size")]
        public long Size { get; set; }

        public static implicit operator HostedFileDao(HostedFile file)
        {
            return new HostedFileDao
            {
                Id = file.Id,
                Commander = file.Commander,
                Uri = file.Uri,
                Filename = file.Filename,
                Size = file.Size
            };
        }

        public static implicit operator HostedFile(HostedFileDao dao)
        {
            return dao is null ?
                null :
                new HostedFile
            {
                Id = dao.Id,
                Commander = dao.Commander,
                Uri = dao.Uri,
                Filename = dao.Filename,
                Size = dao.Size
            };

        }
    }
}
