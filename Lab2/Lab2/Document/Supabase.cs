using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Postgrest.Attributes;
using Postgrest.Models;
using Postgrest;
using Supabase;
//using Supabase.Postgrest.Attributes;
//using Supabase.Postgrest.Models;

namespace Lab2.Document
{
    [Table("docs")]
    public class DocumentRecord : BaseModel
    {
        [PrimaryKey("id")]
        public Guid Id { get; set; } = Guid.NewGuid();

        [Column("content")]
        public string Content { get; set; }

        [Column("type")]
        public string Type { get; set; }

        [Column("editors")]
        public string Editors { get; set; }

        [Column("viewers")]
        public string Viewers { get; set; }

        [Column("file_name")]
        public string FileName { get; set; }

        [Column("created_at")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
    public class SupabaseStorageStrategy: IStorageStrategy
    {
        private readonly Supabase.Client _supabase;
        public SupabaseStorageStrategy(string supabaseUrl, string supabaseKey)
        {
            _supabase = new Supabase.Client(supabaseUrl, supabaseKey);
            _supabase.InitializeAsync().Wait();
        }

        public async Task SaveDocument(DocumentData data, string fileName)
        {
            var record = new DocumentRecord
            {
                Content = data.Content,
                Type = data.Type.ToString(),
                Editors = string.Join(", ", data.Editors),
                Viewers = string.Join(", ", data.Viewers),
                FileName = fileName,
                CreatedAt = DateTime.UtcNow
            };

            await _supabase.From<DocumentRecord>().Insert(record);
        }
        public async Task<DocumentData> LoadDocument(string fileName)
        {
            var response = await _supabase.From<DocumentRecord>()
                .Where(x => x.FileName == fileName)
                .Get();

            var record = response.Models.FirstOrDefault();
            if (record == null)
                throw new FileNotFoundException("Document not found in storage");

            return new DocumentData
            {
                Content = record.Content,
                Editors = Parser.ParseUsers(record.Editors),
                Viewers = Parser.ParseUsers(record.Viewers),
                Type = Enum.Parse<DocumentType>(record.Type)
            };
        }
    }
}
