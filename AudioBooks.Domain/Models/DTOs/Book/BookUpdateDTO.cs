using Microsoft.AspNetCore.Http;
using System;
using System.ComponentModel.DataAnnotations;

namespace AudioBooks.Domain.DTOs.Book;

    public class BookUpdateDTO
    {
        [Required(ErrorMessage = "Id maydoni majburiy.")]
        public Guid Id { get; set; }

        [Required(ErrorMessage = "Title maydoni majburiy.")]
        [StringLength(100, ErrorMessage = "Title 100 belgidan uzun bo'lishi mumkin emas.")]
        public string Title { get; set; }

        [Required(ErrorMessage = "Author maydoni majburiy.")]
        [StringLength(50, ErrorMessage = "Author 50 belgidan uzun bo'lishi mumkin emas.")]
        public string Author { get; set; }

        [StringLength(500, ErrorMessage = "Description 500 belgidan uzun bo'lishi mumkin emas.")]
        public string Description { get; set; }
        public IFormFile? ImageUrl { get; set; }

        [Range(0.01, double.MaxValue, ErrorMessage = "Narx 0 dan katta bo'lishi kerak.")]
        public double Price { get; set; }
        public IFormFile? DownloadUrl { get; set; }
        public IFormFile? AudioUrl { get; set; }

        [Required(ErrorMessage = "ReleaseDate maydoni majburiy.")]
        [DataType(DataType.Date, ErrorMessage = "Noto'g'ri sana formati.")]
        public DateTime ReleaseDate { get; set; }

        public List<Guid> CategoryIds { get; set; }
    }

